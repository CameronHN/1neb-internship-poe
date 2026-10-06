using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Portfolio.WebApi.Extensions;

namespace Portfolio.WebApi.RateLimiting
{
    public static class RateLimitingSetup
    {
        /// <summary>
        /// Registers the rate limiter. Call app.UseRateLimiter() after UseAuthentication so the
        /// per-user "pdf" policy can see who is logged in.
        /// </summary>
        public static IServiceCollection AddApiRateLimiting(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.Configure<RateLimitingSettings>(
                configuration.GetSection(RateLimitingSettings.SectionName)
            );

            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.OnRejected = WriteRejectionAsync;
            });

            // Configured through IOptions so the settings are read at first use, after tests
            // have added their own configuration.
            services
                .AddOptions<RateLimiterOptions>()
                .Configure<IOptions<RateLimitingSettings>>(
                    (options, settingsAccessor) =>
                    {
                        var settings = settingsAccessor.Value;

                        options.AddPolicy(
                            RateLimitPolicies.Auth,
                            context =>
                                RateLimitPartition.GetFixedWindowLimiter(
                                    ClientKey(context),
                                    _ => FixedWindow(settings.Auth.PermitLimit, settings.Auth.WindowSeconds)
                                )
                        );

                        options.AddPolicy(
                            RateLimitPolicies.Pdf,
                            context =>
                                RateLimitPartition.GetFixedWindowLimiter(
                                    UserOrClientKey(context),
                                    _ => FixedWindow(settings.Pdf.PermitLimit, settings.Pdf.WindowSeconds)
                                )
                        );

                        // Runs for every request, on top of any [EnableRateLimiting] policy.
                        // Requests to endpoints without the matching marker get no limiter.
                        options.GlobalLimiter = PartitionedRateLimiter.CreateChained(
                            DemoWindow(
                                "demo-burst",
                                settings.Demo.BurstLimit,
                                settings.Demo.BurstWindowSeconds
                            ),
                            DemoWindow(
                                "demo-daily",
                                settings.Demo.DailyLimit,
                                (int)TimeSpan.FromDays(1).TotalSeconds
                            ),
                            PdfConcurrency(settings.PdfConcurrency)
                        );
                    }
                );

            return services;
        }

        /// <summary>
        /// The client's IP address. IPv6 addresses are grouped by their /64 prefix, because one
        /// host can usually use every address in its /64.
        /// </summary>
        public static string ClientKey(HttpContext context)
        {
            var address = context.Connection.RemoteIpAddress;
            if (address is null)
                return "unknown";

            if (address.IsIPv4MappedToIPv6)
                address = address.MapToIPv4();

            if (address.AddressFamily == AddressFamily.InterNetworkV6)
            {
                var bytes = address.GetAddressBytes();
                Array.Clear(bytes, 8, 8);
                return $"{new IPAddress(bytes)}/64";
            }

            return address.ToString();
        }

        private static string UserOrClientKey(HttpContext context)
        {
            var userId = context.User.GetUserId();
            return userId is null ? $"ip:{ClientKey(context)}" : $"user:{userId}";
        }

        private static FixedWindowRateLimiterOptions FixedWindow(int permitLimit, int windowSeconds)
        {
            return new FixedWindowRateLimiterOptions
            {
                PermitLimit = permitLimit,
                Window = TimeSpan.FromSeconds(windowSeconds),
                QueueLimit = 0,
            };
        }

        private static PartitionedRateLimiter<HttpContext> DemoWindow(
            string name,
            int permitLimit,
            int windowSeconds
        )
        {
            return PartitionedRateLimiter.Create<HttpContext, string>(context =>
                IsMarked<DemoQuotaAttribute>(context)
                    ? RateLimitPartition.GetFixedWindowLimiter(
                        $"{name}:{ClientKey(context)}",
                        _ => FixedWindow(permitLimit, windowSeconds)
                    )
                    : RateLimitPartition.GetNoLimiter(string.Empty)
            );
        }

        private static PartitionedRateLimiter<HttpContext> PdfConcurrency(ConcurrencyLimit limit)
        {
            return PartitionedRateLimiter.Create<HttpContext, string>(context =>
                IsMarked<PdfGenerationAttribute>(context)
                    ? RateLimitPartition.GetConcurrencyLimiter(
                        "pdf-generation",
                        _ => new ConcurrencyLimiterOptions
                        {
                            PermitLimit = limit.PermitLimit,
                            QueueLimit = limit.QueueLimit,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        }
                    )
                    : RateLimitPartition.GetNoLimiter(string.Empty)
            );
        }

        private static bool IsMarked<TAttribute>(HttpContext context)
            where TAttribute : Attribute
        {
            return context.GetEndpoint()?.Metadata.GetMetadata<TAttribute>() is not null;
        }

        /// <summary>
        /// Answers in the same { error: { message, statusCode } } shape as
        /// ExceptionHandlingMiddleware, with Retry-After when the limiter knows it.
        /// </summary>
        private static async ValueTask WriteRejectionAsync(
            OnRejectedContext context,
            CancellationToken cancellationToken
        )
        {
            var response = context.HttpContext.Response;
            var hasRetryAfter = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter);

            if (hasRetryAfter)
            {
                response.Headers.RetryAfter = Math.Ceiling(retryAfter.TotalSeconds)
                    .ToString(CultureInfo.InvariantCulture);
            }

            // Only the time-window limiters give a Retry-After. Without one, the shared
            // concurrency cap was full.
            string message;
            if (!hasRetryAfter)
                message = "The server is busy. Try again shortly.";
            else if (IsMarked<DemoQuotaAttribute>(context.HttpContext))
                message = "Demo limit reached. Try again later, or register for full access.";
            else
                message = "Too many requests. Try again later.";

            var body = new { error = new { message, statusCode = response.StatusCode } };
            response.ContentType = "application/json";
            await response.WriteAsync(JsonSerializer.Serialize(body), cancellationToken);
        }
    }
}

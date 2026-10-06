using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Portfolio.Tests.Common
{
    public static class TestHosts
    {
        /// <summary>
        /// Starts a separate in-memory host with extra configuration on top of
        /// PortfolioApiFactory's (same test database, its own rate-limiter state).
        /// Lets a test choose the caller's IP with <see cref="ClientIpStartupFilter.HeaderName"/>.
        /// Dispose it at the end of the test.
        /// </summary>
        public static WebApplicationFactory<Program> WithSettings(
            this PortfolioApiFactory factory,
            Dictionary<string, string?> settings,
            Action<IServiceCollection>? configureServices = null
        )
        {
            return factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration(
                    (_, config) => config.AddInMemoryCollection(settings)
                );
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton<IStartupFilter, ClientIpStartupFilter>();
                    configureServices?.Invoke(services);
                });
            });
        }
    }

    /// <summary>
    /// TestServer leaves RemoteIpAddress empty, so every request would share one rate-limit
    /// partition. This sets it from a test-only header, before the rest of the pipeline runs.
    /// </summary>
    public sealed class ClientIpStartupFilter : IStartupFilter
    {
        public const string HeaderName = "X-Test-Client-IP";

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                app.Use(
                    async (context, nextMiddleware) =>
                    {
                        if (
                            context.Request.Headers.TryGetValue(HeaderName, out var value)
                            && IPAddress.TryParse(value, out var address)
                        )
                        {
                            context.Connection.RemoteIpAddress = address;
                        }

                        await nextMiddleware();
                    }
                );
                next(app);
            };
        }
    }
}

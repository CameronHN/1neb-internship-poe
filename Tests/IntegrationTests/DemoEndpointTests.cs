using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs.Resume;
using Portfolio.Tests.Common;
using Xunit;

namespace Portfolio.Tests.IntegrationTests
{
    /// <summary>
    /// The anonymous demo, POST /api/Resume/create-pdf. The frontend's Demo page keeps a
    /// count in localStorage, which anyone can clear, so the server enforces the real limits:
    /// the Demo page's shape, a 32 KB body, a per-IP burst and daily quota, and a concurrency
    /// cap shared by every PDF endpoint.
    /// </summary>
    [Collection(ApiCollection.Name)]
    public class DemoEndpointTests
    {
        private const string CreatePdf = "/api/Resume/create-pdf";

        // Matches [RequestSizeLimit] on create-pdf.
        private const int CreatePdfSizeLimit = 32 * 1024;

        private readonly PortfolioApiFactory _factory;

        public DemoEndpointTests(PortfolioApiFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// The biggest request the real Demo page can send (frontend DemoPage.tsx @ 5d12f45):
        /// its fixed number of items, every input at its maxLength, plus the "socials" and
        /// "certifications" keys it sends that the API ignores. The limits must not reject it.
        /// </summary>
        [Fact]
        public async Task CreatePdf_LargestDemoPagePayload_Returns200()
        {
            using var client = TestClients.CreateHttpsClient(_factory);
            var payload = LargestDemoPagePayload();

            var bytes = Encoding.UTF8.GetByteCount(JsonSerializer.Serialize(payload));
            Assert.True(bytes < CreatePdfSizeLimit, $"{bytes} bytes from the Demo page");

            using var response = await client.PostAsJsonAsync(CreatePdf, payload);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var pdf = await response.Content.ReadAsByteArrayAsync();
            Assert.Equal("%PDF", Encoding.ASCII.GetString(pdf, 0, 4));
        }

        [Fact]
        public async Task CreatePdf_MoreThanTheDemoPageAllows_Returns400()
        {
            using var client = TestClients.CreateHttpsClient(_factory);

            using var response = await client.PostAsJsonAsync(
                CreatePdf,
                new
                {
                    name = "Too Many",
                    experience = new[] { new { jobTitle = "a" }, new { jobTitle = "b" }, new { jobTitle = "c" } },
                }
            );

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal(
                "The demo allows at most 2 experiences.",
                await TestClients.ReadErrorMessageAsync(response)
            );
        }

        [Fact]
        public async Task CreatePdf_OverTheDailyQuota_Returns429WithRetryAfter()
        {
            await using var host = _factory.WithSettings(DemoLimits(burst: 100, daily: 2));
            using var client = TestClients.CreateHttpsClient(host);

            using var first = await PostDemoAsync(client);
            using var second = await PostDemoAsync(client);
            using var third = await PostDemoAsync(client);

            Assert.Equal(HttpStatusCode.OK, first.StatusCode);
            Assert.Equal(HttpStatusCode.OK, second.StatusCode);
            Assert.Equal(HttpStatusCode.TooManyRequests, third.StatusCode);
            Assert.True(third.Headers.RetryAfter?.Delta > TimeSpan.Zero, "Retry-After should be set");

            using var json = JsonDocument.Parse(await third.Content.ReadAsStringAsync());
            var error = json.RootElement.GetProperty("error");
            Assert.Equal(429, error.GetProperty("statusCode").GetInt32());
            Assert.StartsWith("Demo limit reached", error.GetProperty("message").GetString());
        }

        [Fact]
        public async Task CreatePdf_OverTheBurstLimit_Returns429()
        {
            await using var host = _factory.WithSettings(DemoLimits(burst: 1, daily: 100));
            using var client = TestClients.CreateHttpsClient(host);

            using var first = await PostDemoAsync(client);
            using var second = await PostDemoAsync(client);

            Assert.Equal(HttpStatusCode.OK, first.StatusCode);
            Assert.Equal(HttpStatusCode.TooManyRequests, second.StatusCode);
            Assert.True(second.Headers.RetryAfter?.Delta <= TimeSpan.FromSeconds(60));
        }

        [Fact]
        public async Task CreatePdf_QuotaIsCountedPerClientIp()
        {
            await using var host = _factory.WithSettings(DemoLimits(burst: 100, daily: 1));
            using var client = TestClients.CreateHttpsClient(host);

            Assert.Equal(HttpStatusCode.OK, await PostDemoStatusAsync(client, "203.0.113.10"));
            Assert.Equal(HttpStatusCode.TooManyRequests, await PostDemoStatusAsync(client, "203.0.113.10"));

            // A different IPv4 address has its own quota.
            Assert.Equal(HttpStatusCode.OK, await PostDemoStatusAsync(client, "203.0.113.11"));

            // IPv6 addresses in the same /64 count as one client; another /64 does not.
            Assert.Equal(HttpStatusCode.OK, await PostDemoStatusAsync(client, "2001:db8::1"));
            Assert.Equal(HttpStatusCode.TooManyRequests, await PostDemoStatusAsync(client, "2001:db8::2"));
            Assert.Equal(HttpStatusCode.OK, await PostDemoStatusAsync(client, "2001:db8:0:1::1"));
        }

        /// <summary>
        /// Clearing localStorage or cookies, or using a private window, is the same as a brand
        /// new client. The quota must still apply.
        /// </summary>
        [Fact]
        public async Task CreatePdf_FreshClientFromTheSameIp_StillGets429()
        {
            await using var host = _factory.WithSettings(DemoLimits(burst: 100, daily: 1));

            using var firstBrowser = TestClients.CreateHttpsClient(host);
            Assert.Equal(HttpStatusCode.OK, await PostDemoStatusAsync(firstBrowser, "198.51.100.7"));

            using var clearedBrowser = TestClients.CreateHttpsClient(host);
            Assert.Equal(
                HttpStatusCode.TooManyRequests,
                await PostDemoStatusAsync(clearedBrowser, "198.51.100.7")
            );
        }

        /// <summary>
        /// The concurrency cap is shared by every PDF endpoint and does not care who calls, so
        /// it also stops abuse spread across many IPs. A gated fake generator holds the one
        /// permit open while a second request arrives.
        /// </summary>
        [Fact]
        public async Task PdfEndpoints_WhenTheConcurrencyCapIsFull_Return429()
        {
            var generator = new GatedResumeGenerationService();
            var settings = DemoLimits(burst: 100, daily: 100);
            settings["RateLimiting:PdfConcurrency:PermitLimit"] = "1";
            settings["RateLimiting:PdfConcurrency:QueueLimit"] = "0";
            await using var host = _factory.WithSettings(
                settings,
                services =>
                {
                    services.RemoveAll<IResumeGenerationService>();
                    services.AddSingleton<IResumeGenerationService>(generator);
                }
            );

            using var holder = TestClients.CreateHttpsClient(host);
            using var other = TestClients.CreateHttpsClient(host);
            await TestClients.RegisterAsync(other, "Busy");

            var heldRequest = PostDemoAsync(holder, "192.0.2.1");
            await generator.Entered.Task.WaitAsync(TimeSpan.FromSeconds(30));

            // Another IP on the demo, and a logged-in user on get-resume, are both turned away.
            using var demo = await PostDemoAsync(other, "192.0.2.2");
            using var getResume = await other.PostAsJsonAsync("/api/Resume/get-resume", new { });

            generator.Release.SetResult();
            using var held = await heldRequest.WaitAsync(TimeSpan.FromSeconds(30));

            Assert.Equal(HttpStatusCode.TooManyRequests, demo.StatusCode);
            Assert.Equal(HttpStatusCode.TooManyRequests, getResume.StatusCode);
            Assert.Equal(
                "The server is busy. Try again shortly.",
                await TestClients.ReadErrorMessageAsync(demo)
            );
            Assert.Equal(HttpStatusCode.OK, held.StatusCode);
        }

        private static Dictionary<string, string?> DemoLimits(int burst, int daily)
        {
            return new Dictionary<string, string?>
            {
                ["RateLimiting:Demo:BurstLimit"] = burst.ToString(),
                ["RateLimiting:Demo:DailyLimit"] = daily.ToString(),
            };
        }

        private static async Task<HttpResponseMessage> PostDemoAsync(
            HttpClient client,
            string? clientIp = null
        )
        {
            var request = new HttpRequestMessage(HttpMethod.Post, CreatePdf)
            {
                Content = JsonContent.Create(new { name = "Demo User" }),
            };
            if (clientIp is not null)
            {
                request.Headers.Add(ClientIpStartupFilter.HeaderName, clientIp);
            }

            return await client.SendAsync(request);
        }

        private static async Task<HttpStatusCode> PostDemoStatusAsync(HttpClient client, string clientIp)
        {
            using var response = await PostDemoAsync(client, clientIp);
            return response.StatusCode;
        }

        private static object LargestDemoPagePayload()
        {
            static string Text(int length) => new('a', length);
            const string date = "September 2026";

            var skills = Enumerable.Range(0, 4).Select(_ => new { skill = Text(100), skillLevel = Text(100) }).ToArray();
            var links = Enumerable.Range(0, 2).Select(_ => new { link = Text(100), linkType = Text(100) }).ToArray();
            var certifications = Enumerable
                .Range(0, 2)
                .Select(_ => new
                {
                    name = Text(100),
                    organisation = Text(100),
                    credentialUrl = Text(100),
                    issuedDate = date,
                    expirationDate = date,
                })
                .ToArray();

            return new
            {
                name = Text(60),
                title = Text(100),
                email = Text(256),
                phoneNumber = Text(15),
                summary = Text(200),
                skills,
                professionalLinks = links,
                experience = Enumerable
                    .Range(0, 2)
                    .Select(_ => new
                    {
                        company = Text(100),
                        jobTitle = Text(100),
                        startDate = date,
                        endDate = date,
                        responsibilities = new[] { Text(255), Text(255) },
                    })
                    .ToArray(),
                education = Enumerable
                    .Range(0, 2)
                    .Select(_ => new
                    {
                        institution = Text(100),
                        qualification = Text(100),
                        startDate = date,
                        endDate = date,
                        major = Text(100),
                        achievement = Text(100),
                    })
                    .ToArray(),
                certification = certifications,
                // Sent by the Demo page under names the API does not bind.
                socials = links,
                certifications,
            };
        }

        private sealed class GatedResumeGenerationService : IResumeGenerationService
        {
            public TaskCompletionSource Entered { get; } =
                new(TaskCreationOptions.RunContinuationsAsynchronously);

            public TaskCompletionSource Release { get; } =
                new(TaskCreationOptions.RunContinuationsAsynchronously);

            public async Task<byte[]> GenerateResumePdfAsync(ResumeDTO dto)
            {
                Entered.TrySetResult();
                await Release.Task;
                return "%PDF-1.4"u8.ToArray();
            }
        }
    }
}

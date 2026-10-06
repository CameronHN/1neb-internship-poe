using System.Net;
using System.Net.Http.Json;
using Portfolio.Tests.Common;
using Xunit;

namespace Portfolio.Tests.IntegrationTests
{
    /// <summary>
    /// The "auth" policy (login, register, change-password: per client IP) and the "pdf"
    /// policy (authenticated PDF downloads: per user). Each test starts its own host with a
    /// limit of 2, so it does not share limiter state with any other test.
    /// </summary>
    [Collection(ApiCollection.Name)]
    public class RateLimitingTests
    {
        private readonly PortfolioApiFactory _factory;

        public RateLimitingTests(PortfolioApiFactory factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/api/Auth/login")]
        [InlineData("/api/Auth/register")]
        [InlineData("/api/Auth/change-password")]
        public async Task AuthEndpoint_ThirdRequestInTheWindow_Returns429(string url)
        {
            await using var host = _factory.WithSettings(
                new() { ["RateLimiting:Auth:PermitLimit"] = "2" }
            );
            using var client = TestClients.CreateRawClient(host);

            var statuses = new List<HttpStatusCode>();
            for (var i = 0; i < 3; i++)
            {
                using var response = await client.PostAsJsonAsync(url, AuthBody(url));
                statuses.Add(response.StatusCode);

                if (i == 2)
                {
                    Assert.True(response.Headers.RetryAfter?.Delta > TimeSpan.Zero);
                    Assert.Equal(
                        "Too many requests. Try again later.",
                        await TestClients.ReadErrorMessageAsync(response)
                    );
                }
            }

            Assert.DoesNotContain(HttpStatusCode.TooManyRequests, statuses.Take(2));
            Assert.Equal(HttpStatusCode.TooManyRequests, statuses[2]);
        }

        [Fact]
        public async Task AuthLimit_IsCountedPerClientIp()
        {
            await using var host = _factory.WithSettings(
                new() { ["RateLimiting:Auth:PermitLimit"] = "1" }
            );
            using var client = TestClients.CreateRawClient(host);

            Assert.NotEqual(HttpStatusCode.TooManyRequests, await LoginStatusAsync(client, "203.0.113.20"));
            Assert.Equal(HttpStatusCode.TooManyRequests, await LoginStatusAsync(client, "203.0.113.20"));
            Assert.NotEqual(HttpStatusCode.TooManyRequests, await LoginStatusAsync(client, "203.0.113.21"));
        }

        [Fact]
        public async Task GetResume_ThirdRequestInTheWindow_Returns429()
        {
            await using var host = _factory.WithSettings(
                new() { ["RateLimiting:Pdf:PermitLimit"] = "2" }
            );
            using var client = TestClients.CreateHttpsClient(host);
            await TestClients.RegisterAsync(client, "PdfLimit");

            using var first = await client.PostAsJsonAsync("/api/Resume/get-resume", new { });
            using var second = await client.PostAsJsonAsync("/api/Resume/get-resume", new { });
            using var third = await client.PostAsJsonAsync("/api/Resume/get-resume", new { });

            Assert.Equal(HttpStatusCode.OK, first.StatusCode);
            Assert.Equal(HttpStatusCode.OK, second.StatusCode);
            Assert.Equal(HttpStatusCode.TooManyRequests, third.StatusCode);
        }

        /// <summary>
        /// PDF downloads need a login, so they are counted per user. Two users behind the same
        /// IP (for example an office) do not share a limit.
        /// </summary>
        [Fact]
        public async Task PdfLimit_IsCountedPerUser()
        {
            await using var host = _factory.WithSettings(
                new() { ["RateLimiting:Pdf:PermitLimit"] = "1" }
            );
            using var alice = TestClients.CreateHttpsClient(host);
            await TestClients.RegisterAsync(alice, "Alice");
            using var bob = TestClients.CreateHttpsClient(host);
            await TestClients.RegisterAsync(bob, "Bob");

            using var aliceFirst = await alice.PostAsJsonAsync("/api/Resume/get-resume", new { });
            using var aliceSecond = await alice.PostAsJsonAsync("/api/Resume/get-resume", new { });
            using var bobFirst = await bob.PostAsJsonAsync("/api/Resume/get-resume", new { });

            Assert.Equal(HttpStatusCode.OK, aliceFirst.StatusCode);
            Assert.Equal(HttpStatusCode.TooManyRequests, aliceSecond.StatusCode);
            Assert.Equal(HttpStatusCode.OK, bobFirst.StatusCode);
        }

        private static object AuthBody(string url)
        {
            return url switch
            {
                "/api/Auth/register" => new
                {
                    firstName = "Rate",
                    lastName = "Limit",
                    email = TestClients.UniqueEmail("rate"),
                    phone = "0820000001",
                    password = TestClients.Password,
                    confirmPassword = TestClients.Password,
                },
                "/api/Auth/login" => new
                {
                    email = TestClients.UniqueEmail("nobody"),
                    password = "Wrong-pass1!",
                },
                _ => new { currentPassword = "x", newPassword = "y" },
            };
        }

        private static async Task<HttpStatusCode> LoginStatusAsync(HttpClient client, string clientIp)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, "/api/Auth/login")
            {
                Content = JsonContent.Create(
                    new { email = TestClients.UniqueEmail("ip"), password = "Wrong-pass1!" }
                ),
            };
            request.Headers.Add(ClientIpStartupFilter.HeaderName, clientIp);
            using var response = await client.SendAsync(request);
            return response.StatusCode;
        }
    }
}

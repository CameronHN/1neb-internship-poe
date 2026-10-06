using System.Net;
using System.Net.Http.Json;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Portfolio.Tests.Common;
using Xunit;

namespace Portfolio.Tests.IntegrationTests
{
    /// <summary>
    /// The API uses cookie auth but must answer 401 instead of redirecting to a login page
    /// (see ConfigureApplicationCookie in Program.cs).
    /// </summary>
    [Collection(ApiCollection.Name)]
    public class AuthorizationTests
    {
        private readonly PortfolioApiFactory _factory;

        public AuthorizationTests(PortfolioApiFactory factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("GET", "/api/Auth/me")]
        [InlineData("GET", "/api/User")]
        [InlineData("GET", "/api/SavedResume/list")]
        [InlineData("GET", "/api/Resume/get-user-resume-details")]
        [InlineData("POST", "/api/Resume/get-resume")]
        public async Task AnonymousRequestToProtectedEndpoint_Returns401(string method, string url)
        {
            using var client = TestClients.CreateHttpsClient(_factory);
            using var request = new HttpRequestMessage(new HttpMethod(method), url);
            if (method == "POST")
            {
                request.Content = JsonContent.Create(new { });
            }

            using var response = await client.SendAsync(request);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Null(response.Headers.Location);
        }

        [Fact]
        public async Task AnonymousCreatePdf_ReturnsPdf()
        {
            using var client = TestClients.CreateHttpsClient(_factory);

            using var response = await client.PostAsJsonAsync(
                "/api/Resume/create-pdf",
                new { name = "Test User" }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/pdf", response.Content.Headers.ContentType?.MediaType);
            var bytes = await response.Content.ReadAsByteArrayAsync();
            Assert.Equal("%PDF", Encoding.ASCII.GetString(bytes, 0, 4));
        }

        [Fact]
        public async Task AfterLogout_ProtectedEndpointReturns401()
        {
            using var client = TestClients.CreateHttpsClient(_factory);
            await TestClients.RegisterAsync(client, "Logout");

            using var before = await client.GetAsync("/api/Auth/me");
            // Logout now needs a JSON body (see Logout_WithoutAJsonBody_IsRejected).
            using var logout = await TestClients.LogoutAsync(client);
            using var after = await client.GetAsync("/api/Auth/me");

            Assert.Equal(HttpStatusCode.OK, before.StatusCode);
            Assert.Equal(HttpStatusCode.OK, logout.StatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, after.StatusCode);
        }

        /// <summary>
        /// The auth cookie is sent on cross-site requests in Development (SameSite=None), so
        /// logout must not accept what a cross-site page can send without a CORS preflight:
        /// no body, a text/plain body or a form. Such a request must not log the user out.
        /// </summary>
        [Theory]
        [InlineData("none")]
        [InlineData("text/plain")]
        [InlineData("form")]
        public async Task Logout_WithoutAJsonBody_IsRejected(string body)
        {
            using var client = TestClients.CreateHttpsClient(_factory);
            await TestClients.RegisterAsync(client, "Csrf");

            using var request = new HttpRequestMessage(HttpMethod.Post, "/api/Auth/logout")
            {
                Content = body switch
                {
                    "text/plain" => new StringContent("{}", Encoding.UTF8, "text/plain"),
                    "form" => new FormUrlEncodedContent(new Dictionary<string, string>()),
                    _ => null,
                },
            };
            request.Headers.Add("Origin", "https://evil.example");
            using var logout = await client.SendAsync(request);

            Assert.False(logout.IsSuccessStatusCode, $"logout answered {(int)logout.StatusCode}");
            Assert.Equal(HttpStatusCode.OK, await TestClients.GetMeStatusAsync(client));
        }

        [Theory]
        [InlineData(null, "samesite=none")] // appsettings.Development.json
        [InlineData("Lax", "samesite=lax")]
        [InlineData("Strict", "samesite=strict")]
        public async Task AuthCookie_SameSite_FollowsConfiguration(string? setting, string expected)
        {
            var settings = new Dictionary<string, string?>();
            if (setting is not null)
            {
                settings["Auth:CookieSameSite"] = setting;
            }
            await using var host = _factory.WithSettings(settings);
            using var client = TestClients.CreateRawClient(host);

            using var register = await TestClients.PostRegisterAsync(
                client,
                TestClients.UniqueEmail("samesite")
            );

            var cookie = TestClients.GetAuthCookieHeader(register);
            Assert.Contains(expected, cookie, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("secure", cookie, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("httponly", cookie, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task AuthCookie_InProduction_DefaultsToLax()
        {
            await using var production = _factory.WithWebHostBuilder(builder =>
                builder.UseEnvironment("Production")
            );
            using var client = TestClients.CreateRawClient(production);

            using var register = await TestClients.PostRegisterAsync(
                client,
                TestClients.UniqueEmail("prodcookie")
            );

            Assert.Contains(
                "samesite=lax",
                TestClients.GetAuthCookieHeader(register),
                StringComparison.OrdinalIgnoreCase
            );
        }
    }
}

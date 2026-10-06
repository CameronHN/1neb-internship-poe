using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Portfolio.Tests.Common;
using Xunit;

namespace Portfolio.Tests.IntegrationTests
{
    /// <summary>
    /// Security headers on every response, and HSTS outside Development.
    /// </summary>
    [Collection(ApiCollection.Name)]
    public class SecurityHeadersTests
    {
        private readonly PortfolioApiFactory _factory;

        public SecurityHeadersTests(PortfolioApiFactory factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("GET", "/api/Auth/me")] // 401
        [InlineData("GET", "/api/does-not-exist")] // 404
        [InlineData("POST", "/api/Resume/create-pdf")] // 200, a PDF
        public async Task ApiResponses_HaveSecurityHeaders(string method, string url)
        {
            using var client = TestClients.CreateHttpsClient(_factory);
            using var request = new HttpRequestMessage(new HttpMethod(method), url);
            if (method == "POST")
            {
                request.Content = JsonContent.Create(new { name = "Headers" });
            }

            using var response = await client.SendAsync(request);

            Assert.Equal("nosniff", Header(response, "X-Content-Type-Options"));
            Assert.Equal("DENY", Header(response, "X-Frame-Options"));
            Assert.Equal("no-referrer", Header(response, "Referrer-Policy"));
            Assert.Equal(
                "default-src 'none'; frame-ancestors 'none'",
                Header(response, "Content-Security-Policy")
            );
        }

        /// <summary>
        /// Swagger UI runs scripts, so it is left out of the strict Content-Security-Policy.
        /// </summary>
        [Fact]
        public async Task SwaggerUi_InDevelopment_LoadsWithoutTheStrictCsp()
        {
            using var client = TestClients.CreateHttpsClient(_factory);

            using var response = await client.GetAsync("/swagger/index.html");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Null(Header(response, "Content-Security-Policy"));
            Assert.Equal("nosniff", Header(response, "X-Content-Type-Options"));
        }

        /// <summary>
        /// UseHsts skips "localhost", so these requests use another host name.
        /// </summary>
        [Fact]
        public async Task Hsts_IsSentInProduction()
        {
            await using var production = _factory.WithWebHostBuilder(builder =>
                builder.UseEnvironment("Production")
            );
            using var client = CreateClientForHost(production);

            using var response = await client.GetAsync("/api/Auth/me");

            Assert.NotNull(Header(response, "Strict-Transport-Security"));
        }

        [Fact]
        public async Task Hsts_IsNotSentInDevelopment()
        {
            using var client = CreateClientForHost(_factory);

            using var response = await client.GetAsync("/api/Auth/me");

            Assert.Null(Header(response, "Strict-Transport-Security"));
        }

        private static HttpClient CreateClientForHost(WebApplicationFactory<Program> factory)
        {
            return factory.CreateClient(
                new WebApplicationFactoryClientOptions
                {
                    BaseAddress = new Uri("https://portfolio.test"),
                    AllowAutoRedirect = false,
                }
            );
        }

        private static string? Header(HttpResponseMessage response, string name)
        {
            if (response.Headers.TryGetValues(name, out var values))
                return string.Join(",", values);
            if (response.Content.Headers.TryGetValues(name, out var contentValues))
                return string.Join(",", contentValues);
            return null;
        }
    }
}

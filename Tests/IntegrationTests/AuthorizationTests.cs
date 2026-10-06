using System.Net;
using System.Net.Http.Json;
using System.Text;
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
            using var logout = await client.PostAsync("/api/Auth/logout", content: null);
            using var after = await client.GetAsync("/api/Auth/me");

            Assert.Equal(HttpStatusCode.OK, before.StatusCode);
            Assert.Equal(HttpStatusCode.OK, logout.StatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, after.StatusCode);
        }
    }
}

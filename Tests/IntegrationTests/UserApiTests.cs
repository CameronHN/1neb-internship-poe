using System.Net;
using System.Net.Http.Json;
using Portfolio.Tests.Common;
using Xunit;

namespace Portfolio.Tests.IntegrationTests
{
    /// <summary>
    /// Regression tests for issue #25: GET /api/User must only ever return the logged-in
    /// user's own details, whatever id is put in the query string.
    /// </summary>
    [Collection(ApiCollection.Name)]
    public class UserApiTests
    {
        private readonly PortfolioApiFactory _factory;

        public UserApiTests(PortfolioApiFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetUser_WithAnotherUsersId_ReturnsCallersOwnDetailsNotTheirs()
        {
            using var alice = TestClients.CreateHttpsClient(_factory);
            var aliceUser = await TestClients.RegisterAsync(alice, "Alice");

            using var bob = TestClients.CreateHttpsClient(_factory);
            var bobUser = await TestClients.RegisterAsync(bob, "Bobby");

            // Alice is logged in (her own cookie jar) and asks for Bobby's id.
            using var response = await alice.GetAsync($"/api/User?id={bobUser.Id}");
            var body = await response.Content.ReadAsStringAsync();

            // Whatever the status code, none of Bobby's details may come back.
            Assert.DoesNotContain(bobUser.Email, body, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("Bobby", body, StringComparison.Ordinal);

            // The endpoint ignores the id and answers with Alice's own details.
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var details = System.Text.Json.JsonSerializer.Deserialize<UserDetails>(
                body,
                new System.Text.Json.JsonSerializerOptions(
                    System.Text.Json.JsonSerializerDefaults.Web
                )
            );
            Assert.NotNull(details);
            Assert.Equal(aliceUser.Email, details.Email, ignoreCase: true);
            Assert.Equal("Alice", details.FirstName);
        }

        [Fact]
        public async Task GetUser_WithoutAnId_ReturnsCallersOwnDetails()
        {
            using var alice = TestClients.CreateHttpsClient(_factory);
            var aliceUser = await TestClients.RegisterAsync(alice, "Alice");

            using var response = await alice.GetAsync("/api/User");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var details = await response.Content.ReadFromJsonAsync<UserDetails>();
            Assert.NotNull(details);
            Assert.Equal(aliceUser.Email, details.Email, ignoreCase: true);
            Assert.Equal("Alice", details.FirstName);
            Assert.Equal("Test", details.LastName);
        }

        private sealed record UserDetails(
            string FirstName,
            string LastName,
            string Email,
            string PhoneNumber
        );
    }
}

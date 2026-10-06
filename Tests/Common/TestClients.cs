using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Portfolio.Tests.Common
{
    /// <summary>
    /// A user created by <see cref="TestClients.RegisterAsync"/>.
    /// </summary>
    public sealed record RegisteredUser(Guid Id, string Email);

    /// <summary>
    /// Helpers for calling the in-memory API as a real client would.
    /// </summary>
    public static class TestClients
    {
        public const string Password = "Passw0rd!";

        /// <summary>
        /// Creates a client with its own cookie jar. The base address must be https because the
        /// auth cookie is marked Secure, and the client would not send it back over http.
        /// </summary>
        public static HttpClient CreateHttpsClient(PortfolioApiFactory factory)
        {
            return factory.CreateClient(
                new WebApplicationFactoryClientOptions
                {
                    BaseAddress = new Uri("https://localhost"),
                    HandleCookies = true,
                    AllowAutoRedirect = false,
                }
            );
        }

        /// <summary>
        /// Registers a new user with a unique email. Registration also logs the client in.
        /// Returns the new user's id and email so a test can address that specific user.
        /// </summary>
        public static async Task<RegisteredUser> RegisterAsync(HttpClient client, string firstName)
        {
            var email = $"{firstName.ToLowerInvariant()}-{Guid.NewGuid():N}@test.local";

            var response = await client.PostAsJsonAsync(
                "/api/Auth/register",
                new
                {
                    firstName,
                    lastName = "Test",
                    email,
                    phone = "0820000001",
                    password = Password,
                    confirmPassword = Password,
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var body = await response.Content.ReadFromJsonAsync<RegisterResponse>();
            Assert.NotNull(body);
            return new RegisteredUser(body.UserId, email);
        }

        private sealed record RegisterResponse(Guid UserId);

        /// <summary>
        /// Adds one title for the logged-in user and returns its id.
        /// </summary>
        public static async Task<Guid> AddTitleAsync(HttpClient client, string title)
        {
            var response = await client.PostAsJsonAsync("/api/Title/add", new[] { new { title } });

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var ids = await response.Content.ReadFromJsonAsync<List<Guid>>();
            Assert.NotNull(ids);
            return Assert.Single(ids);
        }
    }
}

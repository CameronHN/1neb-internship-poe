using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Portfolio.Tests.Common
{
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
        /// </summary>
        public static async Task RegisterAsync(HttpClient client, string firstName)
        {
            var response = await client.PostAsJsonAsync(
                "/api/Auth/register",
                new
                {
                    firstName,
                    lastName = "Test",
                    email = $"{firstName.ToLowerInvariant()}-{Guid.NewGuid():N}@test.local",
                    phone = "0820000001",
                    password = Password,
                    confirmPassword = Password,
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

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

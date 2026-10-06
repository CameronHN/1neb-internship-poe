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
        public static HttpClient CreateHttpsClient(WebApplicationFactory<Program> factory)
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
        /// Creates a client with no cookie jar, for tests that send a cookie by hand
        /// (for example to replay an old cookie after logout).
        /// </summary>
        public static HttpClient CreateRawClient(WebApplicationFactory<Program> factory)
        {
            return factory.CreateClient(
                new WebApplicationFactoryClientOptions
                {
                    BaseAddress = new Uri("https://localhost"),
                    HandleCookies = false,
                    AllowAutoRedirect = false,
                }
            );
        }

        /// <summary>
        /// Registers a new user from any client and returns the raw response.
        /// </summary>
        public static Task<HttpResponseMessage> PostRegisterAsync(HttpClient client, string email)
        {
            return client.PostAsJsonAsync(
                "/api/Auth/register",
                new
                {
                    firstName = "Raw",
                    lastName = "Test",
                    email,
                    phone = "0820000001",
                    password = Password,
                    confirmPassword = Password,
                }
            );
        }

        /// <summary>
        /// A unique email address for a test user.
        /// </summary>
        public static string UniqueEmail(string label)
        {
            return $"{label.ToLowerInvariant()}-{Guid.NewGuid():N}@test.local";
        }

        /// <summary>
        /// The auth cookie from a response, as "name=value", ready for a Cookie header.
        /// </summary>
        public static string GetAuthCookie(HttpResponseMessage response)
        {
            return GetAuthCookieHeader(response).Split(';')[0];
        }

        /// <summary>
        /// The whole Set-Cookie header for the auth cookie, including its attributes.
        /// </summary>
        public static string GetAuthCookieHeader(HttpResponseMessage response)
        {
            return response
                .Headers.GetValues("Set-Cookie")
                .Single(cookie => cookie.StartsWith(".AspNetCore.Identity.Application="));
        }

        /// <summary>
        /// Logs the client out. Logout only accepts a JSON body, so a cross-site form or a
        /// bodyless request cannot trigger it.
        /// </summary>
        public static async Task<HttpResponseMessage> LogoutAsync(
            HttpClient client,
            string? cookie = null
        )
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, "/api/Auth/logout")
            {
                Content = JsonContent.Create(new { }),
            };
            if (cookie is not null)
            {
                request.Headers.Add("Cookie", cookie);
            }

            return await client.SendAsync(request);
        }

        /// <summary>
        /// GET /api/Auth/me, optionally with a cookie sent by hand. Returns the status code.
        /// </summary>
        public static async Task<HttpStatusCode> GetMeStatusAsync(
            HttpClient client,
            string? cookie = null
        )
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, "/api/Auth/me");
            if (cookie is not null)
            {
                request.Headers.Add("Cookie", cookie);
            }

            using var response = await client.SendAsync(request);
            return response.StatusCode;
        }

        /// <summary>
        /// The message from an { error: { message, statusCode } } body.
        /// </summary>
        public static async Task<string> ReadErrorMessageAsync(HttpResponseMessage response)
        {
            using var json = System.Text.Json.JsonDocument.Parse(
                await response.Content.ReadAsStringAsync()
            );
            return json.RootElement.GetProperty("error").GetProperty("message").GetString()!;
        }

        /// <summary>
        /// Registers a new user with a unique email. Registration also logs the client in.
        /// Returns the new user's id and email so a test can address that specific user.
        /// </summary>
        public static async Task<RegisteredUser> RegisterAsync(HttpClient client, string firstName)
        {
            var email = UniqueEmail(firstName);

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

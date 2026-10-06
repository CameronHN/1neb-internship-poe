using System.Net;
using System.Net.Http.Json;
using Portfolio.Tests.Common;
using Xunit;

namespace Portfolio.Tests.IntegrationTests
{
    /// <summary>
    /// Auth cookies are checked against the user's security stamp on every request. Logout and
    /// password changes rotate the stamp, so old cookies stop working straight away.
    /// Old cookies are replayed by hand, the way an attacker with a stolen cookie would.
    /// </summary>
    [Collection(ApiCollection.Name)]
    public class SessionRevocationTests
    {
        private readonly PortfolioApiFactory _factory;

        public SessionRevocationTests(PortfolioApiFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task OldCookie_AfterLogout_Returns401()
        {
            using var client = TestClients.CreateRawClient(_factory);
            using var register = await TestClients.PostRegisterAsync(
                client,
                TestClients.UniqueEmail("replay")
            );
            var cookie = TestClients.GetAuthCookie(register);

            var before = await TestClients.GetMeStatusAsync(client, cookie);
            using var logout = await TestClients.LogoutAsync(client, cookie);
            var after = await TestClients.GetMeStatusAsync(client, cookie);

            Assert.Equal(HttpStatusCode.OK, before);
            Assert.Equal(HttpStatusCode.OK, logout.StatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, after);
        }

        [Fact]
        public async Task Logout_EndsTheUsersSessionsOnOtherDevices()
        {
            using var laptop = TestClients.CreateHttpsClient(_factory);
            var user = await TestClients.RegisterAsync(laptop, "Devices");
            using var phone = TestClients.CreateRawClient(_factory);
            var phoneCookie = await LoginCookieAsync(phone, user.Email, TestClients.Password);

            using var logout = await TestClients.LogoutAsync(laptop);

            Assert.Equal(HttpStatusCode.OK, logout.StatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, await TestClients.GetMeStatusAsync(phone, phoneCookie));
        }

        [Fact]
        public async Task OtherSession_AfterPasswordChange_Returns401()
        {
            using var owner = TestClients.CreateHttpsClient(_factory);
            var user = await TestClients.RegisterAsync(owner, "Pwd");
            using var attacker = TestClients.CreateRawClient(_factory);
            var attackerCookie = await LoginCookieAsync(attacker, user.Email, TestClients.Password);

            using var change = await ChangePasswordAsync(owner, "N3w-Passw0rd!");

            Assert.Equal(HttpStatusCode.OK, change.StatusCode);
            Assert.Equal(
                HttpStatusCode.Unauthorized,
                await TestClients.GetMeStatusAsync(attacker, attackerCookie)
            );
        }

        [Fact]
        public async Task CurrentSession_AfterPasswordChange_StillWorks()
        {
            using var owner = TestClients.CreateHttpsClient(_factory);
            var user = await TestClients.RegisterAsync(owner, "Keep");

            using var change = await ChangePasswordAsync(owner, "N3w-Passw0rd!");

            Assert.Equal(HttpStatusCode.OK, change.StatusCode);
            Assert.Equal(HttpStatusCode.OK, await TestClients.GetMeStatusAsync(owner));

            // And the new password is the one that works.
            using var fresh = TestClients.CreateRawClient(_factory);
            using var oldLogin = await fresh.PostAsJsonAsync(
                "/api/Auth/login",
                new { email = user.Email, password = TestClients.Password }
            );
            using var newLogin = await fresh.PostAsJsonAsync(
                "/api/Auth/login",
                new { email = user.Email, password = "N3w-Passw0rd!" }
            );
            Assert.Equal(HttpStatusCode.BadRequest, oldLogin.StatusCode);
            Assert.Equal(HttpStatusCode.OK, newLogin.StatusCode);
        }

        private static async Task<string> LoginCookieAsync(HttpClient client, string email, string password)
        {
            using var login = await client.PostAsJsonAsync(
                "/api/Auth/login",
                new { email, password }
            );
            Assert.Equal(HttpStatusCode.OK, login.StatusCode);
            return TestClients.GetAuthCookie(login);
        }

        private static Task<HttpResponseMessage> ChangePasswordAsync(HttpClient client, string newPassword)
        {
            return client.PostAsJsonAsync(
                "/api/Auth/change-password",
                new { currentPassword = TestClients.Password, newPassword }
            );
        }
    }
}

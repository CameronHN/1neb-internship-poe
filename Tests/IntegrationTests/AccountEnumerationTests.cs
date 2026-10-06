using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using Portfolio.Tests.Common;
using Xunit;

namespace Portfolio.Tests.IntegrationTests
{
    /// <summary>
    /// Login and register must not reveal which emails have accounts: not through the
    /// message, not through a lockout message, and not through how long login takes.
    /// </summary>
    [Collection(ApiCollection.Name)]
    public class AccountEnumerationTests
    {
        private const string WrongPassword = "Wrong-pass1!";

        private readonly PortfolioApiFactory _factory;

        public AccountEnumerationTests(PortfolioApiFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Six wrong passwords lock a real account after the fifth. Every answer, for the real
        /// account, for an unknown email, and for the owner's correct password while locked,
        /// must be identical.
        /// </summary>
        [Fact]
        public async Task Login_LockedWrongPasswordAndUnknownEmail_AnswerIdentically()
        {
            using var registration = TestClients.CreateHttpsClient(_factory);
            var user = await TestClients.RegisterAsync(registration, "Locked");
            using var client = TestClients.CreateRawClient(_factory);

            var answers = new List<string>();
            for (var i = 0; i < 6; i++)
            {
                answers.Add(await LoginAnswerAsync(client, user.Email, WrongPassword));
                answers.Add(await LoginAnswerAsync(client, TestClients.UniqueEmail("unknown"), WrongPassword));
            }
            answers.Add(await LoginAnswerAsync(client, user.Email, TestClients.Password));

            Assert.All(answers, answer => Assert.Equal("400 Invalid login attempt", answer));
        }

        [Fact]
        public async Task Register_ExistingEmail_GetsAGenericMessage()
        {
            using var first = TestClients.CreateHttpsClient(_factory);
            var user = await TestClients.RegisterAsync(first, "Taken");
            using var second = TestClients.CreateRawClient(_factory);

            using var response = await TestClients.PostRegisterAsync(second, user.Email);
            var body = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains("Registration could not be completed with these details.", body);
            Assert.DoesNotContain("already taken", body, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain(user.Email, body, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Before the fix an unknown email answered in about 1 ms and a real one in about 53 ms
        /// (the password hash). Now every path hashes a password. Each real account gets 4
        /// wrong passwords, one short of the lockout, and a separate account is measured
        /// while locked.
        /// </summary>
        [Fact]
        public async Task Login_UnknownAndLockedAccounts_TakeAsLongAsARealAccount()
        {
            using var client = TestClients.CreateRawClient(_factory);
            await LoginAnswerAsync(client, TestClients.UniqueEmail("warmup"), WrongPassword);

            var existing = new List<long>();
            var unknown = new List<long>();
            for (var u = 0; u < 3; u++)
            {
                using var registration = TestClients.CreateHttpsClient(_factory);
                var user = await TestClients.RegisterAsync(registration, "Timing");
                for (var i = 0; i < 4; i++)
                {
                    existing.Add(await TimeLoginAsync(client, user.Email));
                    unknown.Add(await TimeLoginAsync(client, TestClients.UniqueEmail("unknown")));
                }
            }

            using var lockedRegistration = TestClients.CreateHttpsClient(_factory);
            var lockedUser = await TestClients.RegisterAsync(lockedRegistration, "TimingLocked");
            for (var i = 0; i < 5; i++)
            {
                await LoginAnswerAsync(client, lockedUser.Email, WrongPassword);
            }
            var locked = new List<long>();
            for (var i = 0; i < 4; i++)
            {
                locked.Add(await TimeLoginAsync(client, lockedUser.Email));
            }

            var existingMedian = Median(existing);
            Assert.True(
                Median(unknown) * 2 >= existingMedian,
                $"unknown [{string.Join(",", unknown)}] vs existing [{string.Join(",", existing)}]"
            );
            Assert.True(
                Median(locked) * 2 >= existingMedian,
                $"locked [{string.Join(",", locked)}] vs existing [{string.Join(",", existing)}]"
            );
        }

        private static async Task<string> LoginAnswerAsync(HttpClient client, string email, string password)
        {
            using var response = await client.PostAsJsonAsync(
                "/api/Auth/login",
                new { email, password }
            );
            return $"{(int)response.StatusCode} {await response.Content.ReadAsStringAsync()}";
        }

        private static async Task<long> TimeLoginAsync(HttpClient client, string email)
        {
            var stopwatch = Stopwatch.StartNew();
            await LoginAnswerAsync(client, email, WrongPassword);
            return stopwatch.ElapsedMilliseconds;
        }

        private static long Median(List<long> values)
        {
            return values.Order().ElementAt(values.Count / 2);
        }
    }
}

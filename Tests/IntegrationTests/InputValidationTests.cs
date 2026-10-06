using System.Net;
using System.Net.Http.Json;
using Portfolio.Tests.Common;
using Xunit;

namespace Portfolio.Tests.IntegrationTests
{
    /// <summary>
    /// Bad input is the client's mistake, so it must get 400, not 500. Before the fix, seven
    /// controllers threw System.ComponentModel.DataAnnotations.ValidationException, which
    /// ExceptionHandlingMiddleware does not map, and dates went through DateOnly.Parse.
    /// </summary>
    [Collection(ApiCollection.Name)]
    public class InputValidationTests
    {
        private readonly PortfolioApiFactory _factory;

        public InputValidationTests(PortfolioApiFactory factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("POST", "/api/Experience/add")]
        [InlineData("DELETE", "/api/Experience/delete")]
        [InlineData("DELETE", "/api/Skill/delete")]
        [InlineData("DELETE", "/api/Title/delete")]
        [InlineData("DELETE", "/api/ProfessionalSummary/delete")]
        [InlineData("DELETE", "/api/ProfessionalLink/delete")]
        public async Task EmptyList_Returns400(string method, string url)
        {
            using var client = TestClients.CreateHttpsClient(_factory);
            await TestClients.RegisterAsync(client, "Empty");

            using var request = new HttpRequestMessage(new HttpMethod(method), url)
            {
                Content = JsonContent.Create(Array.Empty<object>()),
            };
            using var response = await client.SendAsync(request);

            // The controller's own message comes back through ExceptionHandlingMiddleware.
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var message = await TestClients.ReadErrorMessageAsync(response);
            Assert.True(
                message.EndsWith("cannot be null or empty.")
                    || message == "At least one experience is required.",
                message
            );
        }

        [Fact]
        public async Task EducationAdd_BadDate_Returns400()
        {
            using var client = TestClients.CreateHttpsClient(_factory);
            await TestClients.RegisterAsync(client, "EduDate");

            using var response = await client.PostAsJsonAsync(
                "/api/Education/add",
                new[]
                {
                    new
                    {
                        institutionName = "Uni",
                        qualification = "BSc",
                        startDate = "not-a-date",
                        endDate = "2020-01-01",
                    },
                }
            );

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("Start date is not a valid date.", await TestClients.ReadErrorMessageAsync(response));
        }

        [Fact]
        public async Task CertificationAdd_BadDate_Returns400()
        {
            using var client = TestClients.CreateHttpsClient(_factory);
            await TestClients.RegisterAsync(client, "CertDate");

            using var response = await client.PostAsJsonAsync(
                "/api/Certification/add",
                new[] { new { certificationName = "Cert", issuedDate = "31/31/2020" } }
            );

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("Issued date is not a valid date.", await TestClients.ReadErrorMessageAsync(response));
        }

        [Fact]
        public async Task ExperiencePatch_BadDate_Returns400()
        {
            using var client = TestClients.CreateHttpsClient(_factory);
            await TestClients.RegisterAsync(client, "ExpDate");
            using var add = await client.PostAsJsonAsync(
                "/api/Experience/add",
                new[]
                {
                    new
                    {
                        jobTitle = "Developer",
                        startDate = "2020-01-01",
                        endDate = "2021-01-01",
                        responsibilities = new[] { new { responsibility = "Code" } },
                    },
                }
            );
            Assert.Equal(HttpStatusCode.Created, add.StatusCode);
            var id = Assert.Single((await add.Content.ReadFromJsonAsync<List<Guid>>())!);

            using var patch = await client.PatchAsJsonAsync(
                "/api/Experience/patch",
                new { id, endDate = "not-a-date" }
            );

            Assert.Equal(HttpStatusCode.BadRequest, patch.StatusCode);
            Assert.Equal("End date is not a valid date.", await TestClients.ReadErrorMessageAsync(patch));
        }

        [Theory]
        [InlineData("overlong first name")]
        [InlineData("blank names")]
        [InlineData("mismatched confirm password")]
        [InlineData("overlong phone")]
        public async Task Register_InvalidDetails_Returns400(string problem)
        {
            using var client = TestClients.CreateRawClient(_factory);
            var firstName = problem switch
            {
                "overlong first name" => new string('a', 101),
                "blank names" => " ",
                _ => "Valid",
            };

            using var response = await client.PostAsJsonAsync(
                "/api/Auth/register",
                new
                {
                    firstName,
                    lastName = problem == "blank names" ? "" : "Valid",
                    email = TestClients.UniqueEmail("invalid"),
                    phone = problem == "overlong phone" ? new string('1', 21) : "0820000001",
                    password = TestClients.Password,
                    confirmPassword = problem == "mismatched confirm password"
                        ? "Different1!"
                        : TestClients.Password,
                }
            );

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}

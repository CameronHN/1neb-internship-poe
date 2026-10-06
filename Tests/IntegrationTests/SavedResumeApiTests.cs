using System.Net;
using System.Net.Http.Json;
using System.Text;
using Portfolio.Tests.Common;
using Xunit;

namespace Portfolio.Tests.IntegrationTests
{
    /// <summary>
    /// End-to-end tests for issue #8 through HTTP:
    /// GET /api/SavedResume/{id}/pdf returns the owner's saved resume, and 404 for anyone else.
    /// </summary>
    [Collection(ApiCollection.Name)]
    public class SavedResumeApiTests
    {
        private readonly PortfolioApiFactory _factory;

        public SavedResumeApiTests(PortfolioApiFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetSavedResumePdf_ForOwner_ReturnsPdf()
        {
            using var alice = TestClients.CreateHttpsClient(_factory);
            await TestClients.RegisterAsync(alice, "Alice");
            var savedResumeId = await SaveResumeAsync(alice);

            using var response = await alice.GetAsync($"/api/SavedResume/{savedResumeId}/pdf");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/pdf", response.Content.Headers.ContentType?.MediaType);

            var pdf = await response.Content.ReadAsByteArrayAsync();
            Assert.True(pdf.Length > 4, "The PDF should not be empty.");
            Assert.Equal("%PDF", Encoding.ASCII.GetString(pdf, 0, 4));
        }

        [Fact]
        public async Task GetSavedResumePdf_ForAnotherUsersId_Returns404()
        {
            using var alice = TestClients.CreateHttpsClient(_factory);
            await TestClients.RegisterAsync(alice, "Alice");
            var aliceResumeId = await SaveResumeAsync(alice);

            using var bob = TestClients.CreateHttpsClient(_factory);
            await TestClients.RegisterAsync(bob, "Bob");

            using var response = await bob.GetAsync($"/api/SavedResume/{aliceResumeId}/pdf");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        /// <summary>
        /// A user can keep at most 50 saved resumes. The 51st is refused with 422 until they
        /// delete one.
        /// </summary>
        [Fact]
        public async Task SaveResume_51st_Returns422UntilOneIsDeleted()
        {
            using var client = TestClients.CreateHttpsClient(_factory);
            await TestClients.RegisterAsync(client, "Fifty");
            var ids = new List<Guid>();
            for (var i = 0; i < 50; i++)
            {
                ids.Add(await SaveResumeAsync(client));
            }

            using var fiftyFirst = await PostSaveAsync(client);

            Assert.Equal(HttpStatusCode.UnprocessableEntity, fiftyFirst.StatusCode);
            Assert.Equal(
                "You can save up to 50 resumes. Delete one to save another.",
                await TestClients.ReadErrorMessageAsync(fiftyFirst)
            );

            using var delete = await client.DeleteAsync($"/api/SavedResume/{ids[0]}");
            Assert.Equal(HttpStatusCode.OK, delete.StatusCode);

            using var afterDelete = await PostSaveAsync(client);
            Assert.Equal(HttpStatusCode.Created, afterDelete.StatusCode);
        }

        private static Task<HttpResponseMessage> PostSaveAsync(HttpClient client)
        {
            return client.PostAsJsonAsync(
                "/api/SavedResume/save",
                new
                {
                    savedResumeName = "API test resume",
                    resumeData = new { name = "Test User" },
                    templateType = "classic",
                }
            );
        }

        /// <summary>
        /// Saves a classic resume for the logged-in user and returns its id.
        /// </summary>
        private static async Task<Guid> SaveResumeAsync(HttpClient client)
        {
            using var response = await PostSaveAsync(client);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            return await response.Content.ReadFromJsonAsync<Guid>();
        }
    }
}

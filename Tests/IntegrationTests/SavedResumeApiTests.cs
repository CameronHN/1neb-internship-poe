using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Portfolio.Core.DTOs.Resume;
using Portfolio.Core.Entities;
using Portfolio.Infrastructure.Persistence;
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

        /// <summary>
        /// Issue #32: the order is stored inside SavedResume.Data (lower-cased, no migration)
        /// and the saved PDF follows it.
        /// </summary>
        [Theory]
        [InlineData("classic", "certifications,skills,experience,education")]
        [InlineData("simplified", "certifications,skills,experience,education")]
        public async Task SaveResume_WithSectionOrder_StoresItAndTheSavedPdfFollowsIt(
            string templateType,
            string expectedOrder
        )
        {
            using var client = TestClients.CreateHttpsClient(_factory);
            await TestClients.RegisterAsync(client, "Ordered");

            // Mixed case is accepted; the stored value is lower-case. Education and Experience are
            // left out, so they follow in the template's default order.
            using var save = await client.PostAsJsonAsync(
                "/api/SavedResume/save",
                new
                {
                    savedResumeName = "Ordered",
                    resumeData = SectionedResumeData(new[] { "Certifications", "skills" }),
                    templateType,
                }
            );
            Assert.Equal(HttpStatusCode.Created, save.StatusCode);
            var id = await save.Content.ReadFromJsonAsync<Guid>();

            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var stored = await db.SavedResume.AsNoTracking().SingleAsync(r => r.Id == id);
                using var json = JsonDocument.Parse(stored.Data);
                var order = json.RootElement.GetProperty("SectionOrder").EnumerateArray();
                Assert.Equal(["certifications", "skills"], order.Select(e => e.GetString()));
            }

            using var response = await client.GetAsync($"/api/SavedResume/{id}/pdf");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var headings = PdfText.HeadingsInOrder(
                await response.Content.ReadAsByteArrayAsync(),
                ["SKILLS", "EXPERIENCE", "EDUCATION", "CERTIFICATIONS"]
            );
            var expected = expectedOrder.Split(',').Select(key => key.ToUpperInvariant());
            // Simplified calls Experience "PROFESSIONAL EXPERIENCE"; the stripped text still contains this.
            Assert.Equal(expected, headings);
        }

        /// <summary>
        /// A resume saved before this feature has no SectionOrder in its JSON. It must still load
        /// and render in each template's default order.
        /// </summary>
        [Theory]
        [InlineData("classic", "experience,skills,education,certifications")]
        [InlineData("simplified", "skills,experience,education,certifications")]
        public async Task GetSavedResumePdf_SnapshotWithoutSectionOrder_UsesTheDefaultOrder(
            string templateType,
            string expectedOrder
        )
        {
            using var client = TestClients.CreateHttpsClient(_factory);
            var user = await TestClients.RegisterAsync(client, "LegacyOrder");
            var data = JsonSerializer.Serialize(
                new
                {
                    Name = "Legacy Order",
                    Skills = new[] { new { Skill = "CSharp", SkillLevel = (string?)null } },
                    Experience = new[] { new { JobTitle = "Developer", Company = "Acme" } },
                    Education = new[] { new { Institution = "Uni", Qualification = "BSc" } },
                    Certification = new[] { new { Name = "AZ-900" } },
                }
            );
            Assert.DoesNotContain("SectionOrder", data);
            var id = await InsertSnapshotAsync(user.Id, data, templateType);

            using var response = await client.GetAsync($"/api/SavedResume/{id}/pdf");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var headings = PdfText.HeadingsInOrder(
                await response.Content.ReadAsByteArrayAsync(),
                ["SKILLS", "EXPERIENCE", "EDUCATION", "CERTIFICATIONS"]
            );
            Assert.Equal(expectedOrder.Split(',').Select(key => key.ToUpperInvariant()), headings);
        }

        /// <summary>
        /// Stored snapshots are never validated, so a bad order (unknown keys, duplicates, the
        /// fixed "summary") has to be ignored rather than break the download.
        /// </summary>
        [Theory]
        [InlineData("classic")]
        [InlineData("simplified")]
        public async Task GetSavedResumePdf_SnapshotWithUnknownKeysAndDuplicates_RendersIgnoringThem(
            string templateType
        )
        {
            using var client = TestClients.CreateHttpsClient(_factory);
            var user = await TestClients.RegisterAsync(client, "BadOrder");
            var data = JsonSerializer.Serialize(
                new ResumeDTO
                {
                    Name = "Bad Order",
                    SectionOrder = ["summary", "Education", "bogus", "education", "links"],
                    Skills = [new SkillsItem { Skill = "CSharp" }],
                    Experience = [new ExperienceItem { JobTitle = "Developer", Company = "Acme" }],
                    Education = [new EducationItem { Institution = "Uni", Qualification = "BSc" }],
                    Certification = [new CertificationItem { Name = "AZ-900" }],
                }
            );
            var id = await InsertSnapshotAsync(user.Id, data, templateType);

            using var response = await client.GetAsync($"/api/SavedResume/{id}/pdf");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var headings = PdfText.HeadingsInOrder(
                await response.Content.ReadAsByteArrayAsync(),
                ["SKILLS", "EXPERIENCE", "EDUCATION", "CERTIFICATIONS"]
            );
            Assert.Equal(4, headings.Count);
            Assert.Equal("EDUCATION", headings[0]);
        }

        private async Task<Guid> InsertSnapshotAsync(Guid userId, string data, string templateType)
        {
            var snapshot = new SavedResume
            {
                Name = "Snapshot",
                Data = data,
                TemplateType = templateType,
                UserId = userId,
            };
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.SavedResume.Add(snapshot);
            await db.SaveChangesAsync();
            return snapshot.Id;
        }

        /// <summary>
        /// A resume with all four movable sections filled in, as the request body of save
        /// (camelCase, like the API).
        /// </summary>
        private static object SectionedResumeData(string[]? sectionOrder)
        {
            return new
            {
                name = "Sectioned User",
                sectionOrder,
                skills = new[] { new { skill = "CSharp" } },
                experience = new[] { new { jobTitle = "Developer", company = "Acme" } },
                education = new[] { new { institution = "Uni", qualification = "BSc" } },
                certification = new[] { new { name = "AZ-900" } },
            };
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

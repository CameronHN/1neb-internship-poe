using System.Net;
using System.Net.Http.Json;
using Portfolio.Tests.Common;
using Xunit;

namespace Portfolio.Tests.IntegrationTests
{
    /// <summary>
    /// Issue #32 through HTTP for POST /api/Resume/get-resume: the optional sectionOrder decides
    /// where the four movable sections land, below the fixed header. get-resume always draws the
    /// classic template; the saved-resume tests in SavedResumeApiTests cover both templates.
    /// </summary>
    [Collection(ApiCollection.Name)]
    public class SectionOrderApiTests
    {
        private static readonly string[] Headings = ["SKILLS", "EXPERIENCE", "EDUCATION", "CERTIFICATIONS"];

        private readonly PortfolioApiFactory _factory;

        public SectionOrderApiTests(PortfolioApiFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetResume_PartialSectionOrder_DrawsListedSectionsFirstThenTheRestInClassicDefaultOrder()
        {
            using var client = TestClients.CreateHttpsClient(_factory);
            await TestClients.RegisterAsync(client, "Ordering");
            var items = await AddOneOfEachSectionAsync(client);

            // Mixed case is accepted. Classic's default is experience, skills, education, certifications.
            var pdf = await GetResumeAsync(client, items, ["Certifications", "skills"]);

            var text = PdfText.ExtractWithoutWhitespace(pdf);
            Assert.Equal(
                ["CERTIFICATIONS", "SKILLS", "EXPERIENCE", "EDUCATION"],
                PdfText.HeadingsInOrder(pdf, Headings)
            );
            // The header (name) is always above the first section.
            Assert.True(
                text.IndexOf("OrderingTest", StringComparison.Ordinal)
                    < text.IndexOf("CERTIFICATIONS", StringComparison.Ordinal)
            );
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task GetResume_NoSectionOrder_UsesTheClassicDefaultOrder(bool emptyArray)
        {
            using var client = TestClients.CreateHttpsClient(_factory);
            await TestClients.RegisterAsync(client, "Default");
            var items = await AddOneOfEachSectionAsync(client);

            var pdf = await GetResumeAsync(client, items, emptyArray ? [] : null);

            Assert.Equal(
                ["EXPERIENCE", "SKILLS", "EDUCATION", "CERTIFICATIONS"],
                PdfText.HeadingsInOrder(pdf, Headings)
            );
        }

        [Fact]
        public async Task GetResume_FullSectionOrder_IsUsedAsGiven()
        {
            using var client = TestClients.CreateHttpsClient(_factory);
            await TestClients.RegisterAsync(client, "Full");
            var items = await AddOneOfEachSectionAsync(client);

            var pdf = await GetResumeAsync(
                client,
                items,
                ["education", "certifications", "experience", "skills"]
            );

            Assert.Equal(
                ["EDUCATION", "CERTIFICATIONS", "EXPERIENCE", "SKILLS"],
                PdfText.HeadingsInOrder(pdf, Headings)
            );
        }

        /// <summary>
        /// A section with no data is skipped, wherever the order puts it.
        /// </summary>
        [Fact]
        public async Task GetResume_SectionWithoutItems_IsSkipped()
        {
            using var client = TestClients.CreateHttpsClient(_factory);
            await TestClients.RegisterAsync(client, "Sparse");
            var items = await AddOneOfEachSectionAsync(client);
            items = items with { Education = [] };

            var pdf = await GetResumeAsync(client, items, ["education", "certifications"]);

            Assert.Equal(
                ["CERTIFICATIONS", "EXPERIENCE", "SKILLS"],
                PdfText.HeadingsInOrder(pdf, Headings)
            );
        }

        private sealed record Items(List<Guid> Skills, List<Guid> Experience, List<Guid> Education, List<Guid> Certifications);

        private static async Task<Items> AddOneOfEachSectionAsync(HttpClient client)
        {
            var skills = await AddAsync(client, "/api/Skill/add", new[] { new { skill = "CSharp" } });
            var experience = await AddAsync(
                client,
                "/api/Experience/add",
                new[]
                {
                    new
                    {
                        jobTitle = "Developer",
                        companyName = "Acme",
                        startDate = "2020-01-01",
                        endDate = "2021-01-01",
                        responsibilities = new[] { new { responsibility = "Code" } },
                    },
                }
            );
            var education = await AddAsync(
                client,
                "/api/Education/add",
                new[]
                {
                    new
                    {
                        institutionName = "Uni",
                        qualification = "BSc",
                        startDate = "2015-01-01",
                        endDate = "2018-01-01",
                    },
                }
            );
            var certifications = await AddAsync(
                client,
                "/api/Certification/add",
                new[] { new { certificationName = "AZ-900", issuingOrganisation = "Microsoft" } }
            );
            return new Items(skills, experience, education, certifications);
        }

        private static async Task<List<Guid>> AddAsync(HttpClient client, string url, object body)
        {
            using var response = await client.PostAsJsonAsync(url, body);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            return (await response.Content.ReadFromJsonAsync<List<Guid>>())!;
        }

        private static async Task<byte[]> GetResumeAsync(HttpClient client, Items items, string[]? sectionOrder)
        {
            using var response = await client.PostAsJsonAsync(
                "/api/Resume/get-resume",
                new
                {
                    skillsIds = new { ids = items.Skills },
                    experienceIds = new { ids = items.Experience },
                    educationIds = new { ids = items.Education },
                    certificationIds = new { ids = items.Certifications },
                    sectionOrder,
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/pdf", response.Content.Headers.ContentType?.MediaType);
            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}

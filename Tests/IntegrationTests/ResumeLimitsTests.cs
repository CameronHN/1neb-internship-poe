using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Portfolio.Core.Constants;
using Portfolio.Tests.Common;
using Xunit;

namespace Portfolio.Tests.IntegrationTests
{
    /// <summary>
    /// Caps on a ResumeDTO in a request body (Core/Constants ResumeLimits). Checked through
    /// POST /api/SavedResume/save, because the anonymous demo has tighter caps of its own.
    /// </summary>
    [Collection(ApiCollection.Name)]
    public class ResumeLimitsTests
    {
        // Matches [RequestSizeLimit] on SavedResume/save.
        private const int SaveRequestSizeLimit = 256 * 1024;

        private readonly PortfolioApiFactory _factory;

        public ResumeLimitsTests(PortfolioApiFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task SaveResume_WithEveryFieldAtItsCap_Returns201AndFitsTheSizeLimit()
        {
            using var client = TestClients.CreateHttpsClient(_factory);
            await TestClients.RegisterAsync(client, "Caps");
            var request = SaveRequest(ResumeAtCaps());

            // The caps must allow nothing bigger than the request size limit lets through.
            var bytes = Encoding.UTF8.GetByteCount(JsonSerializer.Serialize(request));
            Assert.True(bytes < SaveRequestSizeLimit, $"{bytes} bytes at the caps");

            using var response = await client.PostAsJsonAsync("/api/SavedResume/save", request);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Theory]
        [InlineData("skills")]
        [InlineData("professionalLinks")]
        [InlineData("experience")]
        [InlineData("responsibilities")]
        [InlineData("education")]
        [InlineData("certification")]
        public async Task SaveResume_OneItemOverACap_Returns400(string list)
        {
            using var client = TestClients.CreateHttpsClient(_factory);
            await TestClients.RegisterAsync(client, "Over");
            var resume = ResumeAtCaps();
            switch (list)
            {
                case "skills":
                    resume["skills"] = Repeat(ResumeLimits.MaxSkills + 1, () => new { skill = "x" });
                    break;
                case "professionalLinks":
                    resume["professionalLinks"] = Repeat(
                        ResumeLimits.MaxProfessionalLinks + 1,
                        () => new { link = "https://example.com" }
                    );
                    break;
                case "experience":
                    resume["experience"] = Repeat(
                        ResumeLimits.MaxExperiences + 1,
                        () => new { jobTitle = "x" }
                    );
                    break;
                case "responsibilities":
                    resume["experience"] = new[]
                    {
                        new
                        {
                            responsibilities = Repeat(
                                ResumeLimits.MaxResponsibilitiesPerExperience + 1,
                                () => "x"
                            ),
                        },
                    };
                    break;
                case "education":
                    resume["education"] = Repeat(
                        ResumeLimits.MaxEducation + 1,
                        () => new { institution = "x" }
                    );
                    break;
                case "certification":
                    resume["certification"] = Repeat(
                        ResumeLimits.MaxCertifications + 1,
                        () => new { name = "x" }
                    );
                    break;
            }

            using var response = await client.PostAsJsonAsync(
                "/api/SavedResume/save",
                SaveRequest(resume)
            );

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains(Constants.MaxItemCountErrorMessage, await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task SaveResume_OverlongSummary_Returns400()
        {
            using var client = TestClients.CreateHttpsClient(_factory);
            await TestClients.RegisterAsync(client, "Long");
            var resume = ResumeAtCaps();
            resume["summary"] = new string('s', ResumeLimits.SummaryLength + 1);

            using var response = await client.PostAsJsonAsync(
                "/api/SavedResume/save",
                SaveRequest(resume)
            );

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        /// <summary>
        /// [MaxLength] on a List&lt;string&gt; only counts items, so ExperienceItem checks each
        /// responsibility's length itself.
        /// </summary>
        [Fact]
        public async Task SaveResume_OverlongResponsibility_Returns400()
        {
            using var client = TestClients.CreateHttpsClient(_factory);
            await TestClients.RegisterAsync(client, "Resp");
            var resume = new Dictionary<string, object?>
            {
                ["experience"] = new[]
                {
                    new
                    {
                        responsibilities = new[]
                        {
                            new string('r', ResumeLimits.ResponsibilityLength + 1),
                        },
                    },
                },
            };

            using var response = await client.PostAsJsonAsync(
                "/api/SavedResume/save",
                SaveRequest(resume)
            );

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains("Responsibilities", await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task CreatePdf_OverlongName_Returns400()
        {
            using var client = TestClients.CreateHttpsClient(_factory);

            using var response = await client.PostAsJsonAsync(
                "/api/Resume/create-pdf",
                new { name = new string('n', ResumeLimits.NameLength + 1) }
            );

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        private static object SaveRequest(Dictionary<string, object?> resume)
        {
            return new
            {
                savedResumeName = "Limits test",
                resumeData = resume,
                templateType = "classic",
            };
        }

        private static List<T> Repeat<T>(int count, Func<T> item)
        {
            return Enumerable.Range(0, count).Select(_ => item()).ToList();
        }

        /// <summary>
        /// A resume with every list at its maximum size and every string at its maximum length.
        /// </summary>
        private static Dictionary<string, object?> ResumeAtCaps()
        {
            static string Text(int length) => new('a', length);
            var date = Text(ResumeLimits.DateLength);

            return new Dictionary<string, object?>
            {
                ["name"] = Text(ResumeLimits.NameLength),
                ["title"] = Text(ResumeLimits.TitleLength),
                ["email"] = Text(ResumeLimits.EmailLength),
                ["phoneNumber"] = Text(ResumeLimits.PhoneNumberLength),
                ["summary"] = Text(ResumeLimits.SummaryLength),
                ["skills"] = Repeat(
                    ResumeLimits.MaxSkills,
                    () => new
                    {
                        skill = Text(ResumeLimits.TextLength),
                        skillLevel = Text(ResumeLimits.TextLength),
                    }
                ),
                ["professionalLinks"] = Repeat(
                    ResumeLimits.MaxProfessionalLinks,
                    () => new
                    {
                        link = Text(ResumeLimits.TextLength),
                        linkType = Text(ResumeLimits.TextLength),
                    }
                ),
                ["experience"] = Repeat(
                    ResumeLimits.MaxExperiences,
                    () => new
                    {
                        company = Text(ResumeLimits.TextLength),
                        jobTitle = Text(ResumeLimits.TextLength),
                        startDate = date,
                        endDate = date,
                        responsibilities = Repeat(
                            ResumeLimits.MaxResponsibilitiesPerExperience,
                            () => Text(ResumeLimits.ResponsibilityLength)
                        ),
                    }
                ),
                ["education"] = Repeat(
                    ResumeLimits.MaxEducation,
                    () => new
                    {
                        institution = Text(ResumeLimits.TextLength),
                        qualification = Text(ResumeLimits.TextLength),
                        startDate = date,
                        endDate = date,
                        major = Text(ResumeLimits.TextLength),
                        achievement = Text(ResumeLimits.TextLength),
                    }
                ),
                ["certification"] = Repeat(
                    ResumeLimits.MaxCertifications,
                    () => new
                    {
                        name = Text(ResumeLimits.TextLength),
                        organisation = Text(ResumeLimits.TextLength),
                        credentialUrl = Text(ResumeLimits.TextLength),
                        issuedDate = date,
                        expirationDate = date,
                    }
                ),
            };
        }
    }
}

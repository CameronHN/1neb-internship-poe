using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Portfolio.Core.DTOs.Resume;
using Portfolio.Core.Entities;
using Portfolio.Infrastructure.Persistence;
using Portfolio.Tests.Common;
using Xunit;

namespace Portfolio.Tests.IntegrationTests
{
    /// <summary>
    /// Only http(s) links may become clickable in a generated PDF. A javascript:, file:, UNC
    /// or smb: link keeps its label as plain text, because clicking a file or network-share
    /// link on Windows can leak the reader's credentials.
    /// </summary>
    [Collection(ApiCollection.Name)]
    public class PdfLinkSafetyTests
    {
        private const string SafeUrl = "https://example.com/safe";

        private readonly PortfolioApiFactory _factory;

        public PdfLinkSafetyTests(PortfolioApiFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task CreatePdf_UnsafeLinks_AreNotClickable()
        {
            using var client = TestClients.CreateHttpsClient(_factory);

            // The demo allows 2 links and 2 certifications.
            using var response = await client.PostAsJsonAsync(
                "/api/Resume/create-pdf",
                new
                {
                    name = "Link Test",
                    professionalLinks = new[]
                    {
                        new { link = "javascript:app.alert(1)", linkType = "JSLABEL" },
                        new { link = SafeUrl, linkType = "SAFELABEL" },
                    },
                    certification = new[]
                    {
                        new { name = "FileCert", credentialUrl = "file:///C:/Windows/win.ini" },
                        new { name = "SmbCert", credentialUrl = "smb://attacker.example/x" },
                    },
                }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            await AssertOnlySafeLinkIsClickableAsync(response);
        }

        [Theory]
        [InlineData("classic")]
        [InlineData("simplified")]
        public async Task SavedResume_UnsafeLinks_AreNotClickable(string templateType)
        {
            using var client = TestClients.CreateHttpsClient(_factory);
            await TestClients.RegisterAsync(client, "Links");

            using var save = await client.PostAsJsonAsync(
                "/api/SavedResume/save",
                new
                {
                    savedResumeName = "Links",
                    resumeData = new
                    {
                        name = "Link Test",
                        professionalLinks = new[]
                        {
                            new { link = "javascript:app.alert(1)", linkType = "JSLABEL" },
                            new { link = @"\\attacker.example\share\x", linkType = "UNCLABEL" },
                            new { link = SafeUrl, linkType = "SAFELABEL" },
                        },
                        certification = new[]
                        {
                            new { name = "FileCert", credentialUrl = "file:///C:/Windows/win.ini" },
                            new { name = "SmbCert", credentialUrl = "smb://attacker.example/x" },
                        },
                    },
                    templateType,
                }
            );
            Assert.Equal(HttpStatusCode.Created, save.StatusCode);
            var id = await save.Content.ReadFromJsonAsync<Guid>();

            using var response = await client.GetAsync($"/api/SavedResume/{id}/pdf");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            await AssertOnlySafeLinkIsClickableAsync(response);
            Assert.Contains("UNCLABEL", PdfText.ExtractWithoutWhitespace(await response.Content.ReadAsByteArrayAsync()));
        }

        /// <summary>
        /// Resumes saved before this check are JSON snapshots that were never validated. The
        /// check runs when the PDF is drawn, so they are covered too.
        /// </summary>
        [Fact]
        public async Task SnapshotSavedBeforeTheFix_WithAnUnsafeLink_IsNotClickable()
        {
            using var client = TestClients.CreateHttpsClient(_factory);
            var user = await TestClients.RegisterAsync(client, "Legacy");
            var snapshot = new SavedResume
            {
                Name = "Legacy snapshot",
                Data = JsonSerializer.Serialize(
                    new ResumeDTO
                    {
                        Name = "Legacy User",
                        ProfessionalLinks =
                        [
                            new ProfessionalLinkItem { Link = "javascript:app.alert(1)", LinkType = "JSLABEL" },
                            new ProfessionalLinkItem { Link = SafeUrl, LinkType = "SAFELABEL" },
                        ],
                    }
                ),
                TemplateType = "classic",
                UserId = user.Id,
            };
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.SavedResume.Add(snapshot);
                await db.SaveChangesAsync();
            }

            using var response = await client.GetAsync($"/api/SavedResume/{snapshot.Id}/pdf");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            await AssertOnlySafeLinkIsClickableAsync(response);
        }

        private static async Task AssertOnlySafeLinkIsClickableAsync(HttpResponseMessage response)
        {
            var pdf = await response.Content.ReadAsByteArrayAsync();

            Assert.Equal([SafeUrl], PdfText.GetLinkUris(pdf));

            // The unsafe link's label is still shown, as plain text.
            Assert.Contains("JSLABEL", PdfText.ExtractWithoutWhitespace(pdf));
        }
    }
}

using System.Net;
using System.Net.Http.Json;
using Portfolio.Tests.Common;
using Xunit;

namespace Portfolio.Tests.IntegrationTests
{
    /// <summary>
    /// End-to-end regression test for issue #5 through HTTP:
    /// POST /api/Resume/get-resume must not put another user's data into the caller's PDF.
    /// </summary>
    [Collection(ApiCollection.Name)]
    public class ResumeOwnershipApiTests
    {
        private readonly PortfolioApiFactory _factory;

        public ResumeOwnershipApiTests(PortfolioApiFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetResume_WithAnotherUsersTitleId_DoesNotIncludeTheirTitle()
        {
            // A single word with no spaces, so it survives PDF text extraction intact.
            var aliceTitle = "ALICETITLE" + Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();

            // Alice creates a title. Her own PDF must contain it (proves the check below can see text).
            using var alice = TestClients.CreateHttpsClient(_factory);
            await TestClients.RegisterAsync(alice, "Alice");
            var aliceTitleId = await TestClients.AddTitleAsync(alice, aliceTitle);
            var alicePdfText = await GetResumeTextAsync(alice, aliceTitleId);

            Assert.Contains(aliceTitle, alicePdfText, StringComparison.OrdinalIgnoreCase);

            // Bob asks for a resume using Alice's title id. He gets a PDF, but without her title.
            using var bob = TestClients.CreateHttpsClient(_factory);
            await TestClients.RegisterAsync(bob, "Bob");
            var bobPdfText = await GetResumeTextAsync(bob, aliceTitleId);

            Assert.Contains("Bob", bobPdfText, StringComparison.Ordinal);
            Assert.DoesNotContain(aliceTitle, bobPdfText, StringComparison.OrdinalIgnoreCase);
        }

        private static async Task<string> GetResumeTextAsync(HttpClient client, Guid titleId)
        {
            using var response = await client.PostAsJsonAsync(
                "/api/Resume/get-resume",
                new { titleId }
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/pdf", response.Content.Headers.ContentType?.MediaType);

            var pdf = await response.Content.ReadAsByteArrayAsync();
            return PdfText.ExtractWithoutWhitespace(pdf);
        }
    }
}

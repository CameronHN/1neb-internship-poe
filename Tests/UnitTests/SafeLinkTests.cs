using Portfolio.Application.Documents;
using Xunit;

namespace Portfolio.Tests.UnitTests
{
    /// <summary>
    /// Only absolute http and https links may become clickable in a generated PDF.
    /// </summary>
    public class SafeLinkTests
    {
        [Theory]
        [InlineData("https://example.com/me", "https://example.com/me")]
        [InlineData("http://example.com", "http://example.com/")]
        [InlineData("  https://example.com/me  ", "https://example.com/me")]
        [InlineData("HTTPS://Example.com/Path", "https://example.com/Path")]
        public void HttpAndHttps_AreAllowed(string raw, string expected)
        {
            Assert.True(SafeLink.TryGetSafeUri(raw, out var uri));
            Assert.Equal(expected, uri);
        }

        [Theory]
        [InlineData("javascript:app.alert(1)")]
        [InlineData("file:///C:/Windows/win.ini")]
        [InlineData(@"\\attacker.example\share\x")]
        [InlineData("smb://attacker.example/x")]
        [InlineData("ftp://example.com/file")]
        [InlineData("mailto:someone@example.com")]
        [InlineData("data:text/html,<script>alert(1)</script>")]
        [InlineData("linkedin.com/in/me")]
        [InlineData("/relative/path")]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void EverythingElse_IsRejected(string? raw)
        {
            Assert.False(SafeLink.TryGetSafeUri(raw, out var uri));
            Assert.Equal(string.Empty, uri);
        }
    }
}

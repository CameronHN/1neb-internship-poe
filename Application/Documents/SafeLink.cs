namespace Portfolio.Application.Documents
{
    /// <summary>
    /// Decides which links may become clickable in a generated PDF. Only absolute http and
    /// https URLs are allowed. Anything else (javascript:, file:, UNC paths, smb:, relative
    /// paths) is rendered as plain text, because clicking a file or network-share link on
    /// Windows can leak the reader's credentials.
    /// This runs when the PDF is drawn, so it also covers resumes saved before the check existed.
    /// </summary>
    public static class SafeLink
    {
        public static bool TryGetSafeUri(string? raw, out string uri)
        {
            uri = string.Empty;

            if (string.IsNullOrWhiteSpace(raw))
                return false;

            if (!Uri.TryCreate(raw.Trim(), UriKind.Absolute, out var parsed))
                return false;

            if (parsed.Scheme != Uri.UriSchemeHttp && parsed.Scheme != Uri.UriSchemeHttps)
                return false;

            uri = parsed.AbsoluteUri;
            return true;
        }
    }
}

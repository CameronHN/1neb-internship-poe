namespace Portfolio.WebApi.RateLimiting
{
    /// <summary>
    /// The "RateLimiting" section of appsettings.json. Read when the rate limiter is first
    /// used, so tests can override any value with in-memory configuration.
    /// </summary>
    public sealed class RateLimitingSettings
    {
        public const string SectionName = "RateLimiting";

        /// <summary>Login, register and change-password, per client IP.</summary>
        public WindowLimit Auth { get; set; } = new();

        /// <summary>Authenticated PDF downloads, per user.</summary>
        public WindowLimit Pdf { get; set; } = new();

        /// <summary>The anonymous demo (POST /api/Resume/create-pdf), per client IP.</summary>
        public DemoLimit Demo { get; set; } = new();

        /// <summary>PDFs being generated at the same time, across every caller.</summary>
        public ConcurrencyLimit PdfConcurrency { get; set; } = new();
    }

    public sealed class WindowLimit
    {
        public int PermitLimit { get; set; } = 10;
        public int WindowSeconds { get; set; } = 60;
    }

    public sealed class DemoLimit
    {
        public int BurstLimit { get; set; } = 2;
        public int BurstWindowSeconds { get; set; } = 60;

        /// <summary>
        /// One more than the Demo page's own limit of 2, because offices and campuses share
        /// one IP address. Set it to 2 to match the page exactly.
        /// </summary>
        public int DailyLimit { get; set; } = 3;
    }

    public sealed class ConcurrencyLimit
    {
        public int PermitLimit { get; set; } = 4;
        public int QueueLimit { get; set; } = 2;
    }
}

namespace Portfolio.WebApi.RateLimiting
{
    /// <summary>
    /// Policy names for [EnableRateLimiting].
    /// </summary>
    public static class RateLimitPolicies
    {
        public const string Auth = "auth";
        public const string Pdf = "pdf";
    }

    /// <summary>
    /// Marks an action that renders a PDF. All of them share one concurrency cap, so PDF
    /// generation cannot use up the server's CPU however many clients call at once.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class PdfGenerationAttribute : Attribute { }

    /// <summary>
    /// Marks the anonymous demo endpoint. It gets a per-client burst limit and daily quota,
    /// which the server enforces. The Demo page's own count in localStorage is only a display.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class DemoQuotaAttribute : Attribute { }
}

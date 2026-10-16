namespace Portfolio.Core.Constants
{
    public class Constants
    {
        public const string MaxCharacterLengthErrorMessage = "Max. character limit reached.";

        public const string MaxItemCountErrorMessage = "Too many items.";

        public const int MaxSavedResumesPerUser = 50;
    }

    /// <summary>
    /// Caps on a ResumeDTO sent in a request body (POST /api/Resume/create-pdf and
    /// POST /api/SavedResume/save). String lengths match the entity columns the data
    /// normally comes from. List sizes sit above the frontend builder's own limits
    /// (at most 20 selected items in total).
    /// </summary>
    public static class ResumeLimits
    {
        // FirstName (100) + space + LastName (100).
        public const int NameLength = 201;
        public const int TitleLength = 100;
        public const int EmailLength = 256;
        public const int PhoneNumberLength = 50;
        public const int SummaryLength = 200;
        public const int TextLength = 100;
        public const int ResponsibilityLength = 255;

        // Dates arrive pre-formatted, e.g. "January 2024".
        public const int DateLength = 50;

        public const int MaxSkills = 20;
        public const int MaxProfessionalLinks = 20;
        public const int MaxExperiences = 20;
        public const int MaxResponsibilitiesPerExperience = 20;
        public const int MaxEducation = 20;
        public const int MaxCertifications = 20;
    }

    /// <summary>
    /// The four resume sections a user can reorder with <c>SectionOrder</c>. The header and the
    /// Summary are fixed at the top, so "summary", "title" and "links" are not valid keys.
    /// </summary>
    public static class ResumeSections
    {
        public const string Skills = "skills";
        public const string Experience = "experience";
        public const string Education = "education";
        public const string Certifications = "certifications";

        public const int MaxCount = 4;

        public static readonly IReadOnlyList<string> All = [Skills, Experience, Education, Certifications];

        public static bool IsKnown(string? key)
        {
            return Normalise(key) is { } normalised && All.Contains(normalised);
        }

        /// <summary>
        /// Trims, lower-cases, removes duplicates and drops unknown keys. Sections that were not
        /// listed are not added; see <see cref="Resolve"/>.
        /// </summary>
        public static List<string> Clean(IEnumerable<string?>? requested)
        {
            var cleaned = new List<string>();
            foreach (var key in requested ?? [])
            {
                var normalised = Normalise(key);
                if (normalised != null && All.Contains(normalised) && !cleaned.Contains(normalised))
                {
                    cleaned.Add(normalised);
                }
            }

            return cleaned;
        }

        /// <summary>
        /// The final order to draw: the cleaned requested keys, then every section that was left
        /// out, in the template's default order. Never throws, so bad stored data still renders.
        /// </summary>
        public static List<string> Resolve(
            IEnumerable<string?>? requested,
            IEnumerable<string> defaultOrder
        )
        {
            var order = Clean(requested);
            order.AddRange(defaultOrder.Where(key => !order.Contains(key)));
            return order;
        }

        private static string? Normalise(string? key)
        {
            return key?.Trim().ToLowerInvariant();
        }
    }

    /// <summary>
    /// Caps on the anonymous demo (POST /api/Resume/create-pdf). They match the fixed shape of
    /// the frontend's Demo page, which has no controls for adding more items.
    /// </summary>
    public static class DemoResumeLimits
    {
        public const int MaxSkills = 4;
        public const int MaxProfessionalLinks = 2;
        public const int MaxExperiences = 2;
        public const int MaxResponsibilitiesPerExperience = 2;
        public const int MaxEducation = 2;
        public const int MaxCertifications = 2;
    }
}

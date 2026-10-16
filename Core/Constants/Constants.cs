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
    /// Summary are fixed at the top, so they are not members. Used inside the code only: the
    /// API and the stored JSON carry the lower-case names as strings (see <see cref="ResumeSections"/>).
    /// </summary>
    public enum ResumeSection
    {
        Skills,
        Experience,
        Education,
        Certifications,
    }

    /// <summary>
    /// Turns the lower-case string keys from a request or a stored snapshot ("skills", ...) into
    /// <see cref="ResumeSection"/> values. The DTOs keep strings so stored snapshots can be read
    /// leniently and stay lower-case; everything after this class works with the enum.
    /// </summary>
    public static class ResumeSections
    {
        public const int MaxCount = 4;

        public static readonly IReadOnlyList<ResumeSection> All = Enum.GetValues<ResumeSection>();

        // Keyed by the lower-case name. Looked up by hand, because Enum.TryParse would also
        // accept numbers ("1"), comma lists and values that are not members.
        private static readonly Dictionary<string, ResumeSection> ByKey = All.ToDictionary(section =>
            section.ToString().ToLowerInvariant()
        );

        public static string ToKey(ResumeSection section)
        {
            return section.ToString().ToLowerInvariant();
        }

        public static List<string> ToKeys(IEnumerable<ResumeSection> sections)
        {
            return sections.Select(ToKey).ToList();
        }

        /// <summary>
        /// Case-insensitive and ignores surrounding spaces. False for anything that is not one of
        /// the four sections, including the fixed "summary", "title" and "links".
        /// </summary>
        public static bool TryParse(string? key, out ResumeSection section)
        {
            section = default;
            return key != null && ByKey.TryGetValue(key.Trim().ToLowerInvariant(), out section);
        }

        public static bool IsKnown(string? key)
        {
            return TryParse(key, out _);
        }

        /// <summary>
        /// Parses, removes duplicates and drops unknown keys. Sections that were not listed are
        /// not added; see <see cref="Resolve"/>.
        /// </summary>
        public static List<ResumeSection> Clean(IEnumerable<string?>? requested)
        {
            var cleaned = new List<ResumeSection>();
            foreach (var key in requested ?? [])
            {
                if (TryParse(key, out var section) && !cleaned.Contains(section))
                {
                    cleaned.Add(section);
                }
            }

            return cleaned;
        }

        /// <summary>
        /// The final order to draw: the cleaned requested sections, then every section that was
        /// left out, in the template's default order. Never throws, so bad stored data still renders.
        /// </summary>
        public static List<ResumeSection> Resolve(
            IEnumerable<string?>? requested,
            IEnumerable<ResumeSection> defaultOrder
        )
        {
            var order = Clean(requested);
            order.AddRange(defaultOrder.Where(section => !order.Contains(section)));
            return order;
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

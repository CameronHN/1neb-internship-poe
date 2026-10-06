namespace Portfolio.Core.Constants
{
    public class Constants
    {
        public const string MaxCharacterLengthErrorMessage = "Max. character limit reached.";

        public const string MaxItemCountErrorMessage = "Too many items.";
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

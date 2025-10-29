namespace Portfolio.Core.DTOs
{
    public class ResumeRequest
    {
        //Used to get a single title
        public Guid? TitleId { get; set; }

        //Used to get a single professional summary
        public Guid? ProfessionalSummaryId { get; set; }

        //Used to get multiple social media links
        public ItemListRequest? SocialMediaIds { get; set; } = new();

        //Used to get multiple experiences
        public ItemListRequest? ExperienceIds { get; set; } = new();

        //Used to get multiple educations
        public ItemListRequest? EducationIds { get; set; } = new();

        //Used to get multiple certifications
        public ItemListRequest? CertificationIds { get; set; } = new();

        //Used to get multiple skills
        public ItemListRequest? SkillsIds { get; set; } = new();
    }
}

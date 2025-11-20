namespace Portfolio.Core.DTOs.Resume
{
    public class GetAllResumeDetails
    {
        public string? Name { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public List<TitleItems> Title { get; set; } = new();

        public List<SummaryItems> Summaries { get; set; } = new();

        public List<SkillsItems> Skills { get; set; } = new();

        public List<SocialMediaItems> Socials { get; set; } = new();

        public List<ExperienceItems> Experience { get; set; } = new();

        public List<EducationItems> Education { get; set; } = new();

        public List<CertificationItems> Certification { get; set; } = new();
    }

    public class TitleItems
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
    }

    public class SummaryItems
    {
        public Guid Id { get; set; }
        public string? Summary { get; set; }
    }

    public class SocialMediaItems
    {
        public Guid Id { get; set; }
        public string? SocialMediaType { get; set; }
        public string? SocialMediaUrl { get; set; }
    }

    public class SkillsItems
    {
        public Guid Id { get; set; }
        public string? Skill { get; set; }
        public string? SkillLevel { get; set; }
    }

    public class ExperienceItems
    {
        public Guid Id { get; set; }
        public string? Company { get; set; }
        public string? JobTitle { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public List<string>? Responsibilities { get; set; }
    }

    public class EducationItems
    {
        public Guid Id { get; set; }
        public string? Institution { get; set; }
        public string? Qualification { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string? Major { get; set; }
        public string? Achievement { get; set; }
    }

    public class CertificationItems
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Organisation { get; set; }
        public string? CredentialUrl { get; set; }
        public string? IssuedDate { get; set; }
        public string? ExpirationDate { get; set; }
    }
}

namespace Portfolio.Core.DTOs.Resume
{
    public class ResumeDTO
    {
        public string? Name { get; set; }

        public string? Title { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Summary { get; set; }

        public List<SkillsItem>? Skills { get; set; }

        public List<ProfessionalLinkItem>? ProfessionalLinks { get; set; }

        public List<ExperienceItem>? Experience { get; set; }

        public List<EducationItem>? Education { get; set; }

        public List<CertificationItem>? Certification { get; set; }
    }

    public class ProfessionalLinkItem
    {
        public string? Link { get; set; }
        public string? LinkType { get; set; }
    }

    public class SkillsItem
    {
        public string? Skill { get; set; }
        public string? SkillLevel { get; set; }
    }

    public class ExperienceItem
    {
        public string? Company { get; set; }
        public string? JobTitle { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public List<string>? Responsibilities { get; set; }
    }

    public class EducationItem
    {
        public string? Institution { get; set; }
        public string? Qualification { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string? Major { get; set; }
        public string? Achievement { get; set; }
    }

    public class CertificationItem
    {
        public string? Name { get; set; }
        public string? Organisation { get; set; }
        public string? CredentialUrl { get; set; }
        public string? IssuedDate { get; set; }
        public string? ExpirationDate { get; set; }
    }
}

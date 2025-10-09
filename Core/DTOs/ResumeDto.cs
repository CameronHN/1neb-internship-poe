namespace Portfolio.Core.DTOs
{
    public class ResumeDto
    {
        public string? Name { get; set; }
        public string? Title { get; set; }
        public ContactInfo? Contact { get; set; }
        public string? Summary { get; set; }
        public List<string>? Skills { get; set; }
        public List<ExperienceItem>? Experience { get; set; }
        public List<EducationItem>? Education { get; set; }
        public List<CertificationItem>? Certification { get; set; }
    }

    public class ContactInfo
    {
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Location { get; set; }
        public string? Website { get; set; }
        public string? LinkedIn { get; set; }
        public string? Github { get; set; }
    }

    public class ExperienceItem
    {
        public string? Company { get; set; }
        public string? Role { get; set; }
        public string? Start { get; set; }
        public string? End { get; set; }
        public List<string>? Responsibilities { get; set; }
    }

    public class EducationItem
    {
        public string? Institution { get; set; }
        public string? Degree { get; set; }
        public string? Start { get; set; }
        public string? End { get; set; }
        public string? Major { get; set; }
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

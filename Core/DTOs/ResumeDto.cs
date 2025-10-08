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
        public string? Location { get; set; }
        public List<string>? Bullets { get; set; }
    }

    public class EducationItem
    {
        public string? Institution { get; set; }
        public string? Degree { get; set; }
        public string? Start { get; set; }
        public string? End { get; set; }
        public string? Additional { get; set; }
    }

    public class AwardItem
    {
        public string? Name;
        public string? Organisation;
        public string? CredentialUrl;
        public string? IssuedDate;
        public string? ExpirationDate;
    }


}

namespace Portfolio.Core.Models
{
    public class ExperienceModel
    {
        public Guid Id { get; set; }

        public required string JobTitle { get; set; }

        public string? CompanyName { get; set; }

        public required string StartDate { get; set; }

        public required string EndDate { get; set; }
    }
}

namespace Portfolio.Core.Models
{
    public class ExperienceWithResponsibilitiesModel
    {
        public required string JobTitle { get; set; }

        public string? CompanyName { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public List<string>? Responsibilities { get; set; }
    }
}

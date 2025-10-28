namespace Portfolio.Core.Models
{
    public class ExperienceModel
    {
        public required string JobTitle { get; set; }

        public string? CompanyName { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }
    }
}

namespace Portfolio.Core.Models
{
    public class EducationModel
    {
        public required string InstitutionName { get; set; }

        public required string Qualification { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public string? Major { get; set; }

        public string? Achievement { get; set; }
    }
}

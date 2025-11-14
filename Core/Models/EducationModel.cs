namespace Portfolio.Core.Models
{
    public class EducationModel
    {
        public Guid Id { get; set; }

        public required string InstitutionName { get; set; }

        public required string Qualification { get; set; }

        public required string StartDate { get; set; }

        public required string EndDate { get; set; }

        public string? Major { get; set; }

        public string? Achievement { get; set; }
    }
}

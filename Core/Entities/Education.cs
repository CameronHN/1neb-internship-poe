using System.ComponentModel.DataAnnotations;

namespace Portfolio.Core.Entities
{
    public class Education
    {
        [Key]
        public Guid Id { get; set; }

        public string InstitutionName { get; set; }

        public string Qualification { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public string Major { get; set; }

        public string Achievement { get; set; }

        public Guid UserId { get; set; }

        public required User User { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Portfolio.Core.Entities
{
    public class Experience
    {
        [Key]
        public Guid Id { get; set; }

        public string JobTitle { get; set; }

        public string CompanyName { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public Guid UserId { get; set; }

        public List<ExperienceResponsibility> Responsibilities { get; set; } = [];

        public required User User { get; set; }
    }
}

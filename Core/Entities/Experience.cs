using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portfolio.Core.Entities
{
    public class Experience
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column(TypeName = "varchar(100)")]
        public required string JobTitle { get; set; }

        public string? CompanyName { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public Guid UserId { get; set; }

        public User User { get; set; }
        public List<ExperienceResponsibility> Responsibilities { get; set; }
    }
}

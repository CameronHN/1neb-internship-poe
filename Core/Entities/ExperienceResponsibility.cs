using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portfolio.Core.Entities
{
    public class ExperienceResponsibility
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ExperienceId { get; set; }

        [Column(TypeName = "varchar(255)")]
        public required string Responsibility { get; set; }

        public Experience? Experience { get; set; }
    }
}

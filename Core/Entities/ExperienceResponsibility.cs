using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portfolio.Core.Entities
{
    public class ExperienceResponsibility
    {
        [Key]
        public Guid Id { get; set; }

        public Guid ExperienceId { get; set; }

        [Column(TypeName = "varchar(255)")]
        public string Responsibility { get; set; }

        public Experience? Experience { get; set; }
    }
}

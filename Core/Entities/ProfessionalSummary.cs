using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portfolio.Core.Entities
{
    public class ProfessionalSummary
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column(TypeName = "varchar(200)")]
        public required string Summary { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}

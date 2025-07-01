using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portfolio.Core.Entities
{
    public class ProfessionalSummary
    {
        [Key]
        public Guid Id { get; set; }

        [Column(TypeName = "varchar(200)")]
        public string Summary { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}

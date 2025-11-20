using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portfolio.Core.Entities
{
    public class ProfessionalLink
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column(TypeName = "varchar(100)")]
        public required string LinkType { get; set; }

        [Column(TypeName = "varchar(100)")]
        public required string Link { get; set; }

        public ApplicationUser User { get; set; }

        public Guid UserId { get; set; }
    }
}

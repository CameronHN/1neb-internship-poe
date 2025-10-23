using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portfolio.Core.Entities
{
    public class Contact
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column(TypeName = "varchar(100)")]
        public string? ContactUrl { get; set; }

        public ApplicationUser User { get; set; }

        public Guid UserId { get; set; }
    }
}

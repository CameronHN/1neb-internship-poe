using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portfolio.Core.Entities
{
    public class Contact
    {
        [Key]
        public Guid Id { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string LinkedIn { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string GitHub { get; set; }
        public User User { get; set; }

        public Guid UserId { get; set; }

    }
}

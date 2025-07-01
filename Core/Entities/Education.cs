using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portfolio.Core.Entities
{
    public class Education
    {
        [Key]
        public Guid Id { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string InstitutionName { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string Qualification { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string Major { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string Achievement { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portfolio.Core.Entities
{
    public class Certification
    {
        [Key]
        public Guid Id { get; set; }

        [Column(TypeName = "varchar(100)")]
        public required string CertificationName { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string IssuingOrganisation { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string? CredentialUrl { get; set; }

        public DateOnly? IssuedDate { get; set; }

        public DateOnly? ExpiryDate { get; set; }

        public User User { get; set; }

        public Guid UserId { get; set; }

    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portfolio.Core.Entities
{
    public class Certification
    {
        [Key]
        public Guid Id { get; set; }

        [Column(TypeName ="varchar(100)")]
        public required string CertificationName { get; set; }

        public required Guid UserId { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string IssuingOrganisation { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string? CredentialUrl { get; set; }

        public required User User { get; set; }

        public DateTime? IssuedDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public required DateTime UpdatedAt { get; set; }

        public required Guid UpdaterId { get; set; }
    }
}

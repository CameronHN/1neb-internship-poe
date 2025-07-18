using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portfolio.Core.Entities
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column(TypeName = "varchar(100)")]
        public required string FirstName { get; set; }

        [Column(TypeName = "varchar(100)")]
        public required string LastName { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string? Gender { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string? PhoneNumber { get; set; }

        [Column(TypeName = "varchar(100)")]
        public required string Email { get; set; }

        public List<Certification> Certifications { get; set; }
        public List<Education> Educations { get; set; }

        public List<Experience> Experiences { get; set; }

        public List<ProfessionalSummary> ProfessionalSummaries { get; set; }

        public List<Skill> Skills { get; set; }

        public List<Contact> Contacts { get; set; }

        public List<Resume> Resumes { get; set; }

    }
}

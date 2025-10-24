using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portfolio.Core.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        [Column(TypeName = "varchar(100)")]
        public required string FirstName { get; set; }

        [Column(TypeName = "varchar(100)")]
        public required string LastName { get; set; }

        public List<Certification> Certifications { get; set; }
        public List<Education> Educations { get; set; }

        public List<Experience> Experiences { get; set; }

        public List<ProfessionalSummary> ProfessionalSummaries { get; set; }

        public List<Skill> Skills { get; set; }

        public List<Contact> Contacts { get; set; }

        public List<Resume> Resumes { get; set; }

        public List<Title> Titles { get; set; }
    }
}

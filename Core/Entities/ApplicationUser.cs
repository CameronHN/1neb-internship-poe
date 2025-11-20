using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

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

        public List<ProfessionalLink> ProfessionalLinks { get; set; }

        public List<SavedResume> SavedResumes { get; set; }

        public List<Title> Titles { get; set; }
    }
}

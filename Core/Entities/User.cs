using System.ComponentModel.DataAnnotations;

namespace Portfolio.Core.Entities
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Gender { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public List<Education> Educations { get; set; }

        public List<Experience> Experiences { get; set; }

        public List<ProfessionalSummary> ProfessionalSummaries { get; set; }

        public List<Skill> Skills { get; set; }

        public List<Contact> Contacts { get; set; }

        public List<Resume> Resumes { get; set; }

        public List<Certification> Certifications { get; set; }
    }
}

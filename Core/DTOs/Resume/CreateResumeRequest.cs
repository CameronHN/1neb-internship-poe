using System.ComponentModel.DataAnnotations;
using Portfolio.Core.DTOs.Certification;
using Portfolio.Core.DTOs.Contact;
using Portfolio.Core.DTOs.Education;
using Portfolio.Core.DTOs.Experience;
using Portfolio.Core.DTOs.Skill;

namespace Portfolio.Core.DTOs.Resume
{
    public class CreateResumeRequest
    {
        [MaxLength(100, ErrorMessage = Constants.Constants.MaxCharacterLengthErrorMessage)]
        public string? Title { get; set; }

        [MaxLength(200, ErrorMessage = Constants.Constants.MaxCharacterLengthErrorMessage)]
        public string? Summary { get; set; }

        public List<AddContact>? Contacts { get; set; }

        public List<AddSkill>? Skills { get; set; }

        public List<AddExperience>? Experiences { get; set; }

        public List<AddEducation>? Education { get; set; }

        public List<AddCertification>? Certifications { get; set; }
    }
}

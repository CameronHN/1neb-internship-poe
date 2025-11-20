using System.ComponentModel.DataAnnotations;
using Portfolio.Core.DTOs.Resume;

namespace Portfolio.Core.DTOs.SavedResume
{
    public class SaveResumeDataRequest
    {
        [Required(ErrorMessage = "Resume name is required.")]
        [MaxLength(200, ErrorMessage = Constants.Constants.MaxCharacterLengthErrorMessage)]
        public required string SavedResumeName { get; set; }

        [Required(ErrorMessage = "Resume data is required.")]
        public required ResumeDTO ResumeData { get; set; }

        [Required(ErrorMessage = "Template type is required.")]
        [MaxLength(100, ErrorMessage = Constants.Constants.MaxCharacterLengthErrorMessage)]
        public required string TemplateType { get; set; }
    }
}

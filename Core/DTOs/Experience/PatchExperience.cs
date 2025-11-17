using System.ComponentModel.DataAnnotations;
using Portfolio.Core.DTOs.Experience.ExperienceResponsibility;

namespace Portfolio.Core.DTOs.Experience
{
    public class PatchExperience
    {
        [Required]
        public Guid Id { get; set; }

        [MaxLength(100, ErrorMessage = Constants.Constants.MaxCharacterLengthErrorMessage)]
        [RegularExpression(@"\S.*", ErrorMessage = "Job title cannot be whitespace.")]
        public string? JobTitle { get; set; }

        [MaxLength(100, ErrorMessage = Constants.Constants.MaxCharacterLengthErrorMessage)]
        public string? CompanyName { get; set; }

        public string? StartDate { get; set; }
        public string? EndDate { get; set; }

        public List<PatchResponsibilities>? Responsibilities { get; set; }
    }
}

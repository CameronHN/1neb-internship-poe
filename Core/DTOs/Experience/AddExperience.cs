using Portfolio.Core.DTOs.Experience.ExperienceResponsibility;
using System.ComponentModel.DataAnnotations;

namespace Portfolio.Core.DTOs.Experience
{
    public class AddExperience
    {
        [Required(ErrorMessage = "Job title is required.")]
        [MaxLength(100, ErrorMessage = Constants.Constants.MaxCharacterLengthErrorMessage)]
        [RegularExpression(@"\S.*", ErrorMessage = "Job title cannot be whitespace.")]
        public required string JobTitle { get; set; }

        [MaxLength(100, ErrorMessage = Constants.Constants.MaxCharacterLengthErrorMessage)]
        public string? CompanyName { get; set; }

        [Required(ErrorMessage = "Start date is required.")]
        public required string StartDate { get; set; }

        [Required(ErrorMessage = "End date is required.")]
        public required string EndDate { get; set; }

        [MaxLength(100, ErrorMessage = Constants.Constants.MaxCharacterLengthErrorMessage)]
        public string? Major { get; set; }

        [MaxLength(100, ErrorMessage = Constants.Constants.MaxCharacterLengthErrorMessage)]
        public string? Achievement { get; set; }

        [MinLength(1, ErrorMessage = "At least one responsibility is required.")]
        public required List<AddResponsibility> Responsibilities { get; set; }

        [Required]
        public Guid UserId { get; set; }
    }
}

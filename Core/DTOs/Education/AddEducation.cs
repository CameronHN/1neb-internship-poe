using System.ComponentModel.DataAnnotations;

namespace Portfolio.Core.DTOs.Education
{
    public class AddEducation
    {
        [Required(ErrorMessage = "Institution name is required.")]
        [MaxLength(100, ErrorMessage = Constants.Constants.MaxCharacterLengthErrorMessage)]
        [RegularExpression(@"\S.*", ErrorMessage = "Institution name cannot be whitespace.")]
        public required string InstitutionName { get; set; }

        [Required(ErrorMessage = "Qualification is required.")]
        [MaxLength(100, ErrorMessage = Constants.Constants.MaxCharacterLengthErrorMessage)]
        [RegularExpression(@"\S.*", ErrorMessage = "Qualification cannot be whitespace.")]
        public required string Qualification { get; set; }

        public string? StartDate { get; set; }

        public string? EndDate { get; set; }

        [MaxLength(100, ErrorMessage = Constants.Constants.MaxCharacterLengthErrorMessage)]
        public string? Major { get; set; }

        [MaxLength(100, ErrorMessage = Constants.Constants.MaxCharacterLengthErrorMessage)]
        public string? Achievement { get; set; }

        [Required]
        public Guid UserId { get; set; }
    }
}

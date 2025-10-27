using System.ComponentModel.DataAnnotations;

namespace Portfolio.Core.DTOs.Education
{
    public class PatchEducation
    {
        [Required]
        public Guid Id { get; set; }

        [MaxLength(100, ErrorMessage = Constants.Constants.MaxCharacterLengthErrorMessage)]
        [RegularExpression(@"\S.*", ErrorMessage = "Institution name cannot be whitespace.")]
        public string? InstitutionName { get; set; }

        [MaxLength(100, ErrorMessage = Constants.Constants.MaxCharacterLengthErrorMessage)]
        [RegularExpression(@"\S.*", ErrorMessage = "Qualification cannot be whitespace.")]
        public string? Qualification { get; set; }

        public string? StartDate { get; set; }

        public string? EndDate { get; set; }

        [MaxLength(100, ErrorMessage = Constants.Constants.MaxCharacterLengthErrorMessage)]
        public string? Major { get; set; }

        [MaxLength(100, ErrorMessage = Constants.Constants.MaxCharacterLengthErrorMessage)]
        public string? Achievement { get; set; }
    }
}
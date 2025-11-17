using System.ComponentModel.DataAnnotations;

namespace Portfolio.Core.DTOs.SavedResume
{
    public class AddSavedResume
    {
        [Required(ErrorMessage = "Saved resume name is required.")]
        [MaxLength(200, ErrorMessage = Constants.Constants.MaxCharacterLengthErrorMessage)]
        [RegularExpression(@"\S.*", ErrorMessage = "Saved resume name cannot be whitespace.")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Data is required.")]
        [RegularExpression(@"\S.*", ErrorMessage = "Data cannot be whitespace.")]
        public required string Data { get; set; }

        [Required(ErrorMessage = "Template type is required.")]
        [MaxLength(100, ErrorMessage = Constants.Constants.MaxCharacterLengthErrorMessage)]
        [RegularExpression(@"\S.*", ErrorMessage = "Template type cannot be whitespace.")]
        public required string TemplateType { get; set; }
    }
}

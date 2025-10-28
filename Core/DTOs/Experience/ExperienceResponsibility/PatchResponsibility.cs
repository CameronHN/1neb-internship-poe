using System.ComponentModel.DataAnnotations;

namespace Portfolio.Core.DTOs.Experience.ExperienceResponsibility
{
    public class PatchResponsibility
    {
        [MaxLength(255, ErrorMessage = Constants.Constants.MaxCharacterLengthErrorMessage)]
        [RegularExpression(@"\S.*", ErrorMessage = "Responsibility cannot be whitespace.")]
        public string? Responsibility { get; set; }

        [Required]
        public Guid Id { get; set; }
    }
}

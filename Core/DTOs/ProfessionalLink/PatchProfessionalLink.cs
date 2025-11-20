using System.ComponentModel.DataAnnotations;

namespace Portfolio.Core.DTOs.Contact
{
    public class PatchProfessionalLink
    {
        [Required]
        public Guid Id { get; set; }

        [MaxLength(100, ErrorMessage = Constants.Constants.MaxCharacterLengthErrorMessage)]
        [Url(ErrorMessage = "Invalid URL format.")]
        [RegularExpression(@"\S.*", ErrorMessage = "Social link cannot be whitespace.")]
        public string? Link { get; set; }

        [MaxLength(100, ErrorMessage = Constants.Constants.MaxCharacterLengthErrorMessage)]
        [RegularExpression(@"\S.*", ErrorMessage = "Social link type cannot be whitespace.")]
        public string? LinkType { get; set; }
    }
}

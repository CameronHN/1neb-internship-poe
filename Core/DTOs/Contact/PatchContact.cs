using System.ComponentModel.DataAnnotations;

namespace Portfolio.Core.DTOs.Contact
{
    public class PatchContact
    {
        [Required]
        public Guid Id { get; set; }

        [MaxLength(100, ErrorMessage = Constants.Constants.MaxCharacterLengthErrorMessage)]
        [Url(ErrorMessage = "Invalid URL format.")]
        [RegularExpression(@"\S.*", ErrorMessage = "Social link cannot be whitespace.")]
        public string? Social { get; set; }
    }
}

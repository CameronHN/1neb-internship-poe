using System.ComponentModel.DataAnnotations;

namespace Portfolio.Core.DTOs.Contact
{
    public class AddProfessionalLink
    {
        [Required(ErrorMessage = "Social link is required.")]
        [MaxLength(100, ErrorMessage = Constants.Constants.MaxCharacterLengthErrorMessage)]
        [Url(ErrorMessage = "Invalid URL format.")]
        public required string Link { get; set; }

        [Required(ErrorMessage = "Social link type is required.")]
        [MaxLength(100, ErrorMessage = Constants.Constants.MaxCharacterLengthErrorMessage)]
        public required string LinkType { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Portfolio.Core.DTOs.Contact
{
    public class AddContact
    {
        [MaxLength(100, ErrorMessage = Constants.Constants.MaxCharacterLengthErrorMessage)]
        [Url(ErrorMessage = "Invalid URL format.")]
        public string? SocialMediaUrl { get; set; }

        [Required]
        public Guid UserId { get; set; }
    }
}

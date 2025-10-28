using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Portfolio.Core.DTOs.Contact
{
    public class AddContact
    {
        [Required(ErrorMessage = "Social link is required.")]
        [MaxLength(100, ErrorMessage = Constants.Constants.MaxCharacterLengthErrorMessage)]
        [Url(ErrorMessage = "Invalid URL format.")]
        public required string Social { get; set; }

        [JsonIgnore]
        public Guid UserId { get; set; }
    }
}

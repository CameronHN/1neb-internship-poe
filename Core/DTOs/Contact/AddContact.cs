using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Portfolio.Core.DTOs.Contact
{
    public class AddContact
    {
        [MaxLength(100, ErrorMessage = Constants.Constants.MaxCharacterLengthErrorMessage)]
        [Url(ErrorMessage = "Invalid URL format.")]
        public string? Social { get; set; }

        [JsonIgnore]
        public Guid UserId { get; set; }
    }
}

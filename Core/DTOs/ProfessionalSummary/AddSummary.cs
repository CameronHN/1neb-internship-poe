using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Portfolio.Core.DTOs.ProfessionalSummary
{
    public class AddSummary
    {
        [Required(ErrorMessage = "Summary is required.")]
        [MaxLength(200, ErrorMessage = Constants.Constants.MaxCharacterLengthErrorMessage)]
        [RegularExpression(@"\S.*", ErrorMessage = "Summary cannot be whitespace.")]
        public required string Summary { get; set; }

        [JsonIgnore]
        public Guid UserId { get; set; }
    }
}

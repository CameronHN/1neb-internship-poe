using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Portfolio.Core.DTOs.Certification
{
    public class AddCertification
    {
        [Required(ErrorMessage = "Certification name is required.")]
        [MaxLength(100, ErrorMessage = Constants.Constants.MaxCharacterLengthErrorMessage)]
        [RegularExpression(@"\S.*", ErrorMessage = "Certification name cannot be whitespace.")]
        public required string CertificationName { get; set; }

        [MaxLength(100, ErrorMessage = Constants.Constants.MaxCharacterLengthErrorMessage)]
        public string? IssuingOrganisation { get; set; }

        [Url(ErrorMessage = "Invalid URL format.")]
        [MaxLength(100, ErrorMessage = Constants.Constants.MaxCharacterLengthErrorMessage)]
        public string? CredentialUrl { get; set; }

        public string? IssuedDate { get; set; }

        public string? ExpiryDate { get; set; }

        [JsonIgnore]
        public Guid UserId { get; set; }
    }
}

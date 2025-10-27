using System.ComponentModel.DataAnnotations;

namespace Portfolio.Core.DTOs.Certification
{
    public class PatchCertification
    {
        [Required]
        public Guid Id { get; set; }

        [MaxLength(100, ErrorMessage = Constants.Constants.MaxCharacterLengthErrorMessage)]
        [RegularExpression(@"\S.*", ErrorMessage = "Certification name cannot be whitespace.")]
        public string? CertificationName { get; set; }

        [MaxLength(100, ErrorMessage = Constants.Constants.MaxCharacterLengthErrorMessage)]
        public string? IssuingOrganisation { get; set; }

        [Url(ErrorMessage = "Invalid URL format.")]
        [MaxLength(100, ErrorMessage = Constants.Constants.MaxCharacterLengthErrorMessage)]
        public string? CredentialUrl { get; set; }

        public string? IssuedDate { get; set; }
        public string? ExpiryDate { get; set; }
    }
}

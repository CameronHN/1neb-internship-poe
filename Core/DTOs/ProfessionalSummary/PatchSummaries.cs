using System.ComponentModel.DataAnnotations;

namespace Portfolio.Core.DTOs.ProfessionalSummary
{
    public class PatchSummaries
    {
        [Required]
        public Guid Id { get; set; }

        [MaxLength(200, ErrorMessage = Constants.Constants.MaxCharacterLengthErrorMessage)]
        [RegularExpression(@"\S.*", ErrorMessage = "Summary cannot be whitespace.")]
        public string? Summary { get; set; }
    }
}

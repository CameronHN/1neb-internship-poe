using System.ComponentModel.DataAnnotations;

namespace Portfolio.Core.DTOs.ResumeTitle
{
    public class PatchResumeTitle
    {
        [Required]
        public Guid Id { get; set; }

        [MaxLength(100, ErrorMessage = Constants.Constants.MaxCharacterLengthErrorMessage)]
        [RegularExpression(@"\S.*", ErrorMessage = "Title cannot be whitespace.")]
        public string? Title { get; set; }
    }
}

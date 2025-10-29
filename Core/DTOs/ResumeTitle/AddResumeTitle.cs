using System.ComponentModel.DataAnnotations;

namespace Portfolio.Core.DTOs.ResumeTitle
{
    public class AddResumeTitle
    {
        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(100, ErrorMessage = Constants.Constants.MaxCharacterLengthErrorMessage)]
        [RegularExpression(@"\S.*", ErrorMessage = "Title cannot be whitespace.")]
        public required string Title { get; set; }
    }
}

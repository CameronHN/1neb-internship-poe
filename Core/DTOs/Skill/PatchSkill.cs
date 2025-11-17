using System.ComponentModel.DataAnnotations;

namespace Portfolio.Core.DTOs.Skill
{
    public class PatchSkill
    {
        [Required]
        public Guid Id { get; set; }

        [MaxLength(200, ErrorMessage = Constants.Constants.MaxCharacterLengthErrorMessage)]
        [RegularExpression(@"\S.*", ErrorMessage = "Skill cannot be whitespace.")]
        public string? Skill { get; set; }

        [MaxLength(200, ErrorMessage = Constants.Constants.MaxCharacterLengthErrorMessage)]
        public string? ProficiencyLevel { get; set; }
    }
}

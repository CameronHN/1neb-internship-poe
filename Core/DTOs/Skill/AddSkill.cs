using System.ComponentModel.DataAnnotations;

namespace Portfolio.Core.DTOs.Skill
{
    public class AddSkill
    {
        [Required(ErrorMessage = "Skill is required.")]
        [MaxLength(200, ErrorMessage = Constants.Constants.MaxCharacterLengthErrorMessage)]
        [RegularExpression(@"\S.*", ErrorMessage = "Skill cannot be whitespace.")]
        public required string Skill { get; set; }

        [MaxLength(200, ErrorMessage = Constants.Constants.MaxCharacterLengthErrorMessage)]
        public string? ProficiencyLevel { get; set; }
    }
}

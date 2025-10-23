using System.ComponentModel.DataAnnotations;

namespace Portfolio.Core.DTOs.Skill
{
    public class UpdateSkill : AddSkill
    {
        [Required]
        public Guid Id { get; set; }
    }
}

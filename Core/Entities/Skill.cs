using System.ComponentModel.DataAnnotations;

namespace Portfolio.Core.Entities
{
    public class Skill
    {
        [Key]
        public Guid Id { get; set; }

        public string SkillName { get; set; }

        public string ProficiencyLevel { get; set; }

        public Guid UserId { get; set; }

        public required User User { get; set; }
    }
}

namespace Portfolio.Core.Models
{
    public class SkillModel
    {
        public Guid Id { get; set; }

        public required string SkillName { get; set; }

        public string? ProficiencyLevel { get; set; }
    }
}

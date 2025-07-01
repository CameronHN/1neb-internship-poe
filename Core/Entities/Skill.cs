using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portfolio.Core.Entities
{
    public class Skill
    {
        [Key]
        public Guid Id { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string SkillName { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string ProficiencyLevel { get; set; }

        public Guid UserId { get; set; }

        public User User { get; set; }
    }
}

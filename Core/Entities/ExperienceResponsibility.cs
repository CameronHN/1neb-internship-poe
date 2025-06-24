using System.ComponentModel.DataAnnotations;

namespace Portfolio.Core.Entities
{
    public class ExperienceResponsibility
    {
        [Key]
        public Guid Id { get; set; }

        public Guid ExperienceId { get; set; }

        public string Resonsibility { get; set; }

        public required Experience Experience { get; set; }
    }
}

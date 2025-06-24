using System.ComponentModel.DataAnnotations;

namespace Portfolio.Core.Entities
{
    public class Resume
    {
        [Key]
        public Guid Id { get; set; }

        public string GeneratedPath { get; set; }

        public Guid TemplateId { get; set; }

        public Guid UserId { get; set; }

        public required User User { get; set; }
    }
}

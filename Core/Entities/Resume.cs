using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portfolio.Core.Entities
{
    public class Resume
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column(TypeName = "varchar(100)")]
        public required string PdfPath { get; set; }

        public Guid TemplateId { get; set; }

        public Guid UserId { get; set; }

        public User User { get; set; }

        public List<ResumeTemplate> ResumeTemplates { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portfolio.Core.Entities
{
    public class ResumeTemplate
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column(TypeName = "varchar(100)")]
        public required string TemplateName { get; set; }

        [Column(TypeName = "varchar(100)")]
        public required string TemplateFilePath { get; set; }

        public string? TemplateThumbnailPath { get; set; }

        public Resume Resume { get; set; }
    }
}

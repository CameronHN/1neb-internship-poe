using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portfolio.Core.Entities
{
    public class ResumeTemplate
    {
        [Key]
        public Guid Id { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string TemplateName { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string TemplateFilePath { get; set; }

        public string? TemplateThumbnailPath { get; set; }

        public Resume Resume { get; set; }
    }
}

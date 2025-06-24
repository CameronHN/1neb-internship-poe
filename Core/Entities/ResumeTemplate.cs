using System.ComponentModel.DataAnnotations;

namespace Portfolio.Core.Entities
{
    public class ResumeTemplate
    {
        [Key]
        public Guid Id { get; set; }

        public string TemplateName { get; set; }

        public string TemplateFilePath { get; set; }

        public string TemplateThumbnailPath { get; set; }

        public required Resume Resume { get; set; }
    }
}

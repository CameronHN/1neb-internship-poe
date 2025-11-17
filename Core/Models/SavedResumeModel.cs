namespace Portfolio.Core.Models
{
    public class SavedResumeModel
    {
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public required string Data { get; set; }

        public required string TemplateType { get; set; }

        public required string CreatedAt { get; set; }
    }
}

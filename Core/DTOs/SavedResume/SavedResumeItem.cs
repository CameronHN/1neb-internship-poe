namespace Portfolio.Core.DTOs.SavedResume
{
    public class SavedResumeItem
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string TemplateType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

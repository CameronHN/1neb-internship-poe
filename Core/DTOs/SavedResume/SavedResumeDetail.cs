using Portfolio.Core.DTOs.Resume;

namespace Portfolio.Core.DTOs.SavedResume
{
    public class SavedResumeDetail
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required ResumeDTO ResumeData { get; set; }
        public required string TemplateType { get; set; }
        public required string CreatedAt { get; set; }
    }
}

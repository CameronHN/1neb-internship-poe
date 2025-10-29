using Portfolio.Core.DTOs.Resume;

namespace Portfolio.Core.DTOs.SavedResume
{
    public class SavedResumeDetail
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required ResumeDto ResumeData { get; set; }
        public required string TemplateType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

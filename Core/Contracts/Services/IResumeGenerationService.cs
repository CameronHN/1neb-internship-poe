using Portfolio.Core.DTOs.Resume;

namespace Portfolio.Core.Contracts.Services
{
    public interface IResumeGenerationService
    {
        Task<byte[]> GenerateResumePdfAsync(ResumeDto dto);
    }
}

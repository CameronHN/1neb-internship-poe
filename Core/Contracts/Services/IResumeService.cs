using Portfolio.Core.DTOs;

namespace Portfolio.Core.Contracts.Services
{
    public interface IResumeService
    {
        byte[] RenderPdf(ResumeDto dto);

        Task<ResumeDto> GetResumeByUserId(Guid userId);

        Task<ResumeDto> GetResume(ResumeRequest resumeRequest);
    }
}

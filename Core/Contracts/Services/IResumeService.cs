using Portfolio.Core.DTOs.Resume;

namespace Portfolio.Core.Contracts.Services
{
    public interface IResumeService
    {
        byte[] RenderPdf(ResumeDto dto);

        Task<GetAllResumeDetails?> GetResumeByUserId(Guid userId);

        Task<ResumeDto> GetResumeDetailsAsync(Guid userId, ResumeRequest resumeRequest);
    }
}

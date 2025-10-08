using Portfolio.Core.DTOs;

namespace Portfolio.Core.Contracts.Services
{
    public interface IResumeService
    {
        Task<byte[]> RenderPdfAsync(ResumeDto dto);

        Task<ResumeDto> GetResumeByUserId(Guid userId);
    }
}

using Portfolio.Core.DTOs.Resume;

namespace Portfolio.Core.Contracts.Services
{
    public interface IResumeDataService
    {
        Task<GetAllResumeDetails?> GetResumeByUserIdAsync(Guid userId);

        Task<ResumeDTO> GetResumeDetailsAsync(Guid userId, ResumeRequest resumeRequest);
    }
}

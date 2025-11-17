using Portfolio.Core.DTOs.Resume;

namespace Portfolio.Core.Contracts.Services
{
    public interface IResumeDataService
    {
        Task<GetAllResumeDetails?> GetResumeByUserId(Guid userId);

        Task<ResumeDto> GetResumeDetailsAsync(Guid userId, ResumeRequest resumeRequest);
    }
}

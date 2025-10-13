using Portfolio.Core.DTOs;

namespace Portfolio.Core.Contracts.Services
{
    public interface IExperienceService
    {
        Task<ExperienceItem> GetExperienceById(Guid id);
    }
}

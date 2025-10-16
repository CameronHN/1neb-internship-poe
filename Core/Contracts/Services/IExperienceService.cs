using Portfolio.Core.DTOs;

namespace Portfolio.Core.Contracts.Services
{
    public interface IExperienceService
    {
        Task<ExperienceItem> GetExperienceById(Guid id);

        Task<List<ExperienceItem>> GetExperienceItemsByUserId(Guid id);

        Task<List<ExperienceItem>> GetAllExperiencesByIds(List<Guid> ids);
    }
}

using Portfolio.Core.DTOs;

namespace Portfolio.Core.Contracts.Repositories
{
    public interface IExperienceRepository
    {
        Task<ExperienceItem?> GetExperienceById(Guid id);

        Task<List<ExperienceItem>> GetAllExperiencesByUserId(Guid id);

        Task<List<ExperienceItem>> GetAllExperiencesByIds(List<Guid> ids);
    }
}

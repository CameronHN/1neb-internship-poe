using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Experience;

namespace Portfolio.Core.Contracts.Services
{
    public interface IExperienceService
    {
        Task<List<Guid>> AddExperiencesAsync(List<AddExperience> experiences);

        Task<ExperienceItem> GetExperienceById(Guid id);

        Task<List<ExperienceItem>> GetExperienceItemsByUserId(Guid id);

        //Task<List<ExperienceItem>> GetAllExperiencesByIds(List<Guid> ids);

        Task<List<ExperienceItem>> GetAllExperiencesByIds(ItemListRequest request);
    }
}

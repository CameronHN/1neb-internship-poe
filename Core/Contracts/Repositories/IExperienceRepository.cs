using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Experience;

namespace Portfolio.Core.Contracts.Repositories
{
    public interface IExperienceRepository
    {
        Task<List<Guid>> AddExperiencesAsync(List<AddExperience> experiences);

        Task<ExperienceItem?> GetExperienceById(Guid id);

        Task<List<ExperienceItem>> GetAllExperiencesByUserId(Guid id);

        //Task<List<ExperienceItem>> GetAllExperiencesByIds(List<Guid> ids);

        Task<List<ExperienceItem>> GetAllExperiencesByIds(ItemListRequest request);

        Task<bool> PatchExperiencesAsync(Guid userId, List<PatchExperience> patches);

        Task<bool> DeleteExperiencesAsync(Guid userId, List<Guid> experienceIds);
    }
}

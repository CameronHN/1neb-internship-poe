using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Experience;
using Portfolio.Core.DTOs.Resume;
using Portfolio.Core.Models;

namespace Portfolio.Core.Contracts.Repositories
{
    public interface IExperienceRepository
    {
        Task<List<Guid>> AddExperiencesAsync(Guid userId, List<AddExperience> experiences);

        Task<ExperienceWithResponsibilitiesModel> GetExperienceByIdAsync(Guid id, Guid userId);

        Task<List<ExperienceItem>> GetAllExperiencesByIdsAsync(ItemListRequest request);

        Task<bool> PatchExperienceAsync(Guid userId, PatchExperience patch);

        Task<bool> DeleteExperiencesAsync(Guid userId, List<Guid> experienceIds);
    }
}

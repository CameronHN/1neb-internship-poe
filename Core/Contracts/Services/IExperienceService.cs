using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Experience;
using Portfolio.Core.Models;

namespace Portfolio.Core.Contracts.Services
{
    public interface IExperienceService
    {
        Task<List<Guid>> AddExperiencesAsync(Guid userId, List<AddExperience> experiences);
        Task<ExperienceItem> GetExperienceById(Guid id);
        Task<
            List<ExperienceWithResponsibilitiesModel>
        > GetAllExperiencesIncludingResponsibilitiesByUserIdAsync(Guid id);
        Task<List<ExperienceModel>> GetAllExperiencesByIds(List<Guid> ids);
        Task<List<ExperienceItem>> GetAllExperiencesByIds(ItemListRequest request);
        Task<bool> PatchExperiencesAsync(Guid userId, List<PatchExperience> patches);
        Task<bool> DeleteExperiencesAsync(Guid userId, List<Guid> experienceIds);
    }
}

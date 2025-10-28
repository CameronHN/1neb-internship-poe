using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Experience;
using Portfolio.Core.Models;

namespace Portfolio.Application.Services
{
    public class ExperienceService : IExperienceService
    {
        private readonly IExperienceRepository _experienceRepository;

        public ExperienceService(IExperienceRepository experienceRepository)
        {
            _experienceRepository = experienceRepository;
        }

        public Task<List<ExperienceModel>> GetAllExperiencesByIds(List<Guid> ids)
        {
            return _experienceRepository.GetAllExperiencesByIds(ids);
        }

        public async Task<ExperienceItem> GetExperienceById(Guid id)
        {
            var experience = await _experienceRepository.GetExperienceById(id);
            if (experience == null)
                throw new ArgumentException($"Experience with id {id} not found.");
            return experience;
        }

        public Task<List<ExperienceWithResponsibilitiesModel>> GetExperienceItemsByUserId(Guid id)
        {
            return _experienceRepository.GetAllExperiencesIncludingResponsibilitiesByUserIdAsync(
                id
            );
        }

        public Task<List<ExperienceItem>> GetAllExperiencesByIds(ItemListRequest request)
        {
            return _experienceRepository.GetAllExperiencesByIds(request);
        }

        public async Task<List<Guid>> AddExperiencesAsync(List<AddExperience> experiences)
        {
            return await _experienceRepository.AddExperiencesAsync(experiences);
        }

        public async Task<bool> PatchExperiencesAsync(Guid userId, List<PatchExperience> patches)
        {
            return await _experienceRepository.PatchExperiencesAsync(userId, patches);
        }

        public async Task<bool> DeleteExperiencesAsync(Guid userId, List<Guid> experienceIds)
        {
            return await _experienceRepository.DeleteExperiencesAsync(userId, experienceIds);
        }
    }
}

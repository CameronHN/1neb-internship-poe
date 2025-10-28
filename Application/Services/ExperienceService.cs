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

        public Task<List<ExperienceWithResponsibilitiesModel>> GetAllExperiencesIncludingResponsibilitiesByUserIdAsync(Guid id)
        {
            return _experienceRepository.GetAllExperiencesIncludingResponsibilitiesByUserIdAsync(
                id
            );
        }

        public Task<List<ExperienceItem>> GetAllExperiencesByIds(ItemListRequest request)
        {
            return _experienceRepository.GetAllExperiencesByIds(request);
        }

        public Task<List<Guid>> AddExperiencesAsync(Guid userId, List<AddExperience> experiences)
        {
            return _experienceRepository.AddExperiencesAsync(userId, experiences);
        }

        public Task<bool> PatchExperiencesAsync(Guid userId, List<PatchExperience> patches)
        {
            return _experienceRepository.PatchExperiencesAsync(userId, patches);
        }

        public Task<bool> DeleteExperiencesAsync(Guid userId, List<Guid> experienceIds)
        {
            return _experienceRepository.DeleteExperiencesAsync(userId, experienceIds);
        }
    }
}

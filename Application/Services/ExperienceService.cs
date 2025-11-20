using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Experience;
using Portfolio.Core.DTOs.Resume;
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

        public async Task<ExperienceWithResponsibilitiesModel> GetExperienceByIdAsync(
            Guid id,
            Guid userId
        )
        {
            return await _experienceRepository.GetExperienceByIdAsync(id, userId);
        }

        public async Task<List<Guid>> AddExperiencesAsync(
            Guid userId,
            List<AddExperience> experiences
        )
        {
            return await _experienceRepository.AddExperiencesAsync(userId, experiences);
        }

        public async Task<bool> PatchExperienceAsync(Guid userId, PatchExperience patch)
        {
            return await _experienceRepository.PatchExperienceAsync(userId, patch);
        }

        public async Task<bool> DeleteExperiencesAsync(Guid userId, List<Guid> experienceIds)
        {
            return await _experienceRepository.DeleteExperiencesAsync(userId, experienceIds);
        }
    }
}

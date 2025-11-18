using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Education;
using Portfolio.Core.DTOs.Resume;
using Portfolio.Core.Models;

namespace Portfolio.Application.Services
{
    public class EducationService : IEducationService
    {
        private readonly IEducationRepository _educationRepository;

        public EducationService(IEducationRepository educationRepository)
        {
            _educationRepository = educationRepository;
        }

        public async Task<List<Guid>> AddEducationsAsync(Guid userId, List<AddEducation> educations)
        {
            return await _educationRepository.AddEducationsAsync(userId, educations);
        }

        public async Task<bool> DeleteEducationsAsync(Guid userId, List<Guid> educationIds)
        {
            return await _educationRepository.DeleteEducationsAsync(userId, educationIds);
        }

        public async Task<List<EducationItem>> GetAllEducationsByIdsAsync(ItemListRequest request)
        {
            return await _educationRepository.GetAllEducationsByIdsAsync(request);
        }

        public async Task<EducationModel> GetEducationByIdAsync(Guid id, Guid userId)
        {
            return await _educationRepository.GetEducationByIdAsync(id, userId);
        }

        public async Task<List<EducationModel>> GetEducationsByUserIdAsync(Guid userId)
        {
            return await _educationRepository.GetEducationsByUserIdAsync(userId);
        }

        public async Task<bool> PatchEducationAsync(Guid userId, PatchEducation patch)
        {
            return await _educationRepository.PatchEducationAsync(userId, patch);
        }
    }
}

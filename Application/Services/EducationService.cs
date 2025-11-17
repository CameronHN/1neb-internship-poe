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

        public Task<bool> DeleteEducationsAsync(Guid userId, List<Guid> educationIds)
        {
            return _educationRepository.DeleteEducationsAsync(userId, educationIds);
        }

        public Task<List<EducationItem>> GetAllEducationsByIds(ItemListRequest request)
        {
            return _educationRepository.GetAllEducationsByIds(request);
        }

        public async Task<EducationModel> GetEducationByIdAsync(Guid id, Guid userId)
        {
            return await _educationRepository.GetEducationById(id, userId);
        }

        public async Task<List<EducationModel>> GetEducationsByUserIdAsync(Guid userId)
        {
            return await _educationRepository.GetEducationsByUserIdAsync(userId);
        }

        public Task<bool> PatchEducationAsync(Guid userId, PatchEducation patch)
        {
            return _educationRepository.PatchEducationAsync(userId, patch);
        }
    }
}

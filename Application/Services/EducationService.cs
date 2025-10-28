using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Education;

namespace Portfolio.Application.Services
{
    public class EducationService : IEducationService
    {
        private readonly IEducationRepository _educationRepository;

        public EducationService(IEducationRepository educationRepository)
        {
            _educationRepository = educationRepository;
        }

        public async Task<List<Guid>> AddEducationsAsync(List<AddEducation> educations)
        {
            return await _educationRepository.AddEducationsAsync(educations);
        }

        public Task<bool> DeleteEducationsAsync(Guid userId, List<Guid> educationIds)
        {
            return _educationRepository.DeleteEducationsAsync(userId, educationIds);
        }

        public Task<List<EducationItem>> GetAllEducationsByIds(ItemListRequest request)
        {
            return _educationRepository.GetAllEducationsByIds(request);
        }

        public async Task<List<EducationItem>> GetEducationsByUserIdAsync(Guid userId)
        {
            return await _educationRepository.GetEducationsByUserIdAsync(userId);
        }

        public Task<bool> PatchEducationsAsync(Guid userId, List<PatchEducation> patches)
        {
            return _educationRepository.PatchEducationsAsync(userId, patches);
        }
    }
}

using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs;

namespace Portfolio.Application.Services
{
    public class ExperienceService : IExperienceService
    {
        private readonly IExperienceRepository _experienceRepository;

        public ExperienceService(IExperienceRepository experienceRepository)
        {
            _experienceRepository = experienceRepository;
        }

        //public Task<List<ExperienceItem>> GetAllExperiencesByIds(List<Guid> ids)
        //{
        //    return _experienceRepository.GetAllExperiencesByIds(ids);
        //}

        public async Task<ExperienceItem> GetExperienceById(Guid id)
        {
            var experience = await _experienceRepository.GetExperienceById(id);
            if (experience == null)
                throw new ArgumentException($"Experience with id {id} not found.");
            return experience;
        }

        public Task<List<ExperienceItem>> GetExperienceItemsByUserId(Guid id)
        {
            return _experienceRepository.GetAllExperiencesByUserId(id);
        }

        public Task<List<ExperienceItem>> GetAllExperiencesByIds(ItemListRequest request)
        {
            return _experienceRepository.GetAllExperiencesByIds(request);
        }
    }
}

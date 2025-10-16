using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs;

namespace Portfolio.Application.Services
{
    public class EducationService : IEducationService
    {
        private readonly IEducationRepository _educationRepository;

        public EducationService(IEducationRepository educationRepository)
        {
            _educationRepository = educationRepository;
        }

        public Task<List<EducationItem>> GetAllEducationsByIds(ItemListRequest request)
        {
            return _educationRepository.GetAllEducationsByIds(request);
        }
    }
}

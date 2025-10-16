using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs;

namespace Portfolio.Application.Services
{
    public class EducationService : IEducationService
    {
        private readonly IEducationService _educationService;

        public EducationService(IEducationService educationService)
        {
            _educationService = educationService;
        }
        public Task<List<EducationItem>> GetAllEducationsByIds(ItemListRequest request)
        {
            return _educationService.GetAllEducationsByIds(request);
        }
    }
}

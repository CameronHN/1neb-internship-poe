using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Education;

namespace Portfolio.Core.Contracts.Services
{
    public interface IEducationService
    {
        Task<List<Guid>> AddEducationsAsync(List<AddEducation> educations);

        Task<List<EducationItem>> GetAllEducationsByIds(ItemListRequest request);
    }
}

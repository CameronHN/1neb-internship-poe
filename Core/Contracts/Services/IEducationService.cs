using Portfolio.Core.DTOs;

namespace Portfolio.Core.Contracts.Services
{
    public interface IEducationService
    {
        Task<List<EducationItem>> GetAllEducationsByIds(ItemListRequest request);
    }
}

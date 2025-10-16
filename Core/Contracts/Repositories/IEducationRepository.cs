using Portfolio.Core.DTOs;

namespace Portfolio.Core.Contracts.Repositories
{
    public interface IEducationRepository
    {
        Task<List<EducationItem>> GetAllEducationsByIds(List<Guid> ids);
    }
}

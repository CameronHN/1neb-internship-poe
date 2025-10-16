using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.DTOs;

namespace Portfolio.Infrastructure.Repositories
{
    public class EducationRepository : IEducationRepository
    {
        public Task<List<EducationItem>> GetAllEducationsByIds(List<Guid> ids)
        {
            throw new NotImplementedException();
        }
    }
}

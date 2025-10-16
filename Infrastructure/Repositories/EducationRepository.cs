using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.DTOs;

namespace Portfolio.Infrastructure.Repositories
{
    public class SkillRepository : ISkillRepository
    {
        public Task<List<SkillsItem>> GetAllSkillsByIds(List<Guid> ids)
        {
            throw new NotImplementedException();
        }
    }
}

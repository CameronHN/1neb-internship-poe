using Portfolio.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portfolio.Core.Contracts.Repositories
{
    public interface ISkillRepository
    {
        Task<List<SkillsItem>> GetAllSkillsByIds(List<Guid> ids);
    }
}

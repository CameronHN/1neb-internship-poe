using Portfolio.Core.DTOs;
using Portfolio.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portfolio.Core.Contracts.Repositories
{
    public interface IExperienceRepository
    {
        Task<ExperienceItem?> GetExperienceById(Guid id);

        Task<List<ExperienceItem>> GetAllExperiencesByUserId(Guid id);
    }
}

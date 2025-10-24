using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Portfolio.Core.DTOs.ResumeTitle;

namespace Portfolio.Core.Contracts.Repositories
{
    public interface ITitleRepository
    {
        Task<Guid> AddTitleAsync(AddResumeTitle title);
        Task<string?> GetTitleById(Guid id);
    }
}

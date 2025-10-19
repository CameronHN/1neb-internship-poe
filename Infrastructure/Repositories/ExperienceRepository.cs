using Microsoft.EntityFrameworkCore;
using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.DTOs;
using Portfolio.Core.Exceptions;
using Portfolio.Infrastructure.Persistence;

namespace Portfolio.Infrastructure.Repositories
{
    public class ExperienceRepository : IExperienceRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ExperienceRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //public async Task<List<ExperienceItem>> GetAllExperiencesByIds(List<Guid> ids)
        //{
        //    return await _dbContext.Experience
        //        .Where(e => ids.Contains(e.Id))
        //        .Include(e => e.Responsibilities)
        //        .OrderBy(e => e.EndDate)
        //        .Select(e => new ExperienceItem
        //        {
        //            Company = e.CompanyName,
        //            Role = e.JobTitle,
        //            Start = e.StartDate.ToString("MMMM yyyy"),
        //            End = e.EndDate == default ? "Present" : e.EndDate.ToString("MMMM yyyy"),
        //            Responsibilities = e.Responsibilities.Select(r => r.Responsibility).ToList()
        //        })
        //        .ToListAsync();
        //}

        public async Task<List<ExperienceItem>> GetAllExperiencesByUserId(Guid id)
        {
            return await _dbContext.Experience
                .Where(e => e.UserId == id)
                .Include(e => e.Responsibilities)
                .OrderBy(e => e.EndDate)
                .Select(e => new ExperienceItem
                {
                    Company = e.CompanyName,
                    Role = e.JobTitle,
                    Start = e.StartDate.ToString("MMMM yyyy"),
                    End = e.EndDate == default ? "Present" : e.EndDate.ToString("MMMM yyyy"),
                    Responsibilities = e.Responsibilities.Select(r => r.Responsibility).ToList()
                })
                .ToListAsync();
        }

        public async Task<ExperienceItem?> GetExperienceById(Guid id)
        {
            var experience = await _dbContext.Experience
                .Include(e => e.Responsibilities)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (experience == null)
            {
                throw new NotFoundException("Experience not found.");
            }

            return new ExperienceItem
            {
                Company = experience.CompanyName,
                Role = experience.JobTitle,
                Start = experience.StartDate.ToString("MMMM yyyy"),
                End = experience.EndDate == default ? "Present" : experience.EndDate.ToString("MMMM yyyy"),
                Responsibilities = experience.Responsibilities.Select(r => r.Responsibility).ToList()
            };
        }

        public async Task<List<ExperienceItem>> GetAllExperiencesByIds(ItemListRequest request)
        {
            var ids = request.Ids;
            if (ids.Count == 0) return [];

            var experiences = await _dbContext.Experience
                .Where(exp => ids.Contains(exp.Id))
                .Select(ex => new
                {
                    ex.Id,
                    ex.CompanyName,
                    ex.JobTitle,
                    ex.StartDate,
                    ex.EndDate,
                    ex.Responsibilities
                })
                .ToListAsync();

            switch (request.Order)
            {
                case SortOrder.Descending:
                    experiences = experiences.OrderByDescending(e => e.EndDate).ToList();
                    break;
                case SortOrder.Ascending:
                    experiences = experiences.OrderBy(e => e.EndDate).ToList();
                    break;
                case SortOrder.None:
                default:
                    var order = ids.Select((id, idx) => new { id, idx }).ToDictionary(x => x.id, x => x.idx);
                    experiences = experiences.OrderBy(e => order.TryGetValue(e.Id, out var idx) ? idx : int.MaxValue).ToList();
                    break;
            }

            return experiences.Select(ex => new ExperienceItem
            {
                Company = ex.CompanyName,
                Role = ex.JobTitle,
                Start = ex.StartDate.ToString("MMMM yyyy"),
                End = ex.EndDate.ToString("MMMM yyyy"),
                Responsibilities = ex.Responsibilities.Select(r => r.Responsibility).ToList()
            }).ToList();
        }
    }
}

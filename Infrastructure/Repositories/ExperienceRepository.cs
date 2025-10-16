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
            if (ids == null || ids.Count == 0) return [];

            var experiences = await _dbContext.Experience
                .Where(e => ids.Contains(e.Id))
                .Include(e => e.Responsibilities)
                .Select(e => new
                {
                    e.Id,
                    e.CompanyName,
                    e.JobTitle,
                    e.StartDate,
                    e.EndDate,
                    Responsibilities = e.Responsibilities.Select(r => r.Responsibility).ToList()
                })
                .ToListAsync();

            if (request.IsDescending)
            {
                experiences = experiences
                    .OrderByDescending(e => e.EndDate)
                    .ToList();
            }
            else
            {
                // Preserve the exact order of the incoming Ids
                var order = ids
                    .Select((id, index) => new { id, index })
                    .ToDictionary(x => x.id, x => x.index);

                experiences = experiences
                    .OrderBy(e => order.TryGetValue(e.Id, out var idx) ? idx : int.MaxValue)
                    .ToList();
            }

            return experiences.Select(e => new ExperienceItem
            {
                Company = e.CompanyName,
                Role = e.JobTitle,
                Start = e.StartDate.ToString("MMMM yyyy"),
                End = e.EndDate == default ? "Present" : e.EndDate.ToString("MMMM yyyy"),
                Responsibilities = e.Responsibilities
            }).ToList();
        }
    }
}

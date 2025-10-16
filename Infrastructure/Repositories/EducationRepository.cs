using Microsoft.EntityFrameworkCore;
using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.DTOs;
using Portfolio.Infrastructure.Persistence;

namespace Portfolio.Infrastructure.Repositories
{
    public class EducationRepository : IEducationRepository
    {

        private readonly ApplicationDbContext _dbContext;

        public EducationRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<EducationItem>> GetAllEducationsByIds(ItemListRequest request)
        {
            var ids = request.Ids;
            if (ids.Count == 0) return [];

            var educations = await _dbContext.Education
                .Where(edu => ids.Contains(edu.Id))
                .Select(ed => new
                {
                    ed.Id,
                    ed.InstitutionName,
                    ed.Qualification,
                    ed.Major,
                    ed.StartDate,
                    ed.EndDate
                })
                .ToListAsync();

            if (request.IsDescending)
            {
                educations = educations
                    .OrderByDescending(e => e.EndDate)
                    .ToList();
            }
            else
            {
                // Preserve the exact order of the incoming Ids
                var order = ids
                    .Select((id, index) => new { id, index })
                    .ToDictionary(x => x.id, x => x.index);

                educations = educations
                    .OrderBy(e => order.TryGetValue(e.Id, out var idx) ? idx : int.MaxValue)
                    .ToList();
            }

            return educations.Select(ed => new EducationItem
            {
                Institution = ed.InstitutionName,
                Degree = ed.Qualification,
                Major = ed.Major,
                Start = ed.StartDate.ToString("MMMM yyyy"),
                End = ed.EndDate.ToString("MMMM yyyy")
            }).ToList();
        }
    }
}

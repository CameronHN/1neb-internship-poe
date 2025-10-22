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

            switch (request.Order)
            {
                case SortOrder.Descending:
                    educations = educations.OrderByDescending(e => e.EndDate).ToList();
                    break;
                case SortOrder.Ascending:
                    educations = educations.OrderBy(e => e.EndDate).ToList();
                    break;
                case SortOrder.None:
                default:
                    var order = ids.Select((id, idx) => new { id, idx }).ToDictionary(x => x.id, x => x.idx);
                    educations = educations.OrderBy(e => order.TryGetValue(e.Id, out var idx) ? idx : int.MaxValue).ToList();
                    break;
            }

            return educations.Select(ed => new EducationItem
            {
                Institution = ed.InstitutionName,
                Qualification = ed.Qualification,
                Major = ed.Major,
                StartDate = ed.StartDate.ToString("MMMM yyyy"),
                EndDate = ed.EndDate.ToString("MMMM yyyy")
            }).ToList();
        }
    }
}

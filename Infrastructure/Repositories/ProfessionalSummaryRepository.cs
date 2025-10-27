using Microsoft.EntityFrameworkCore;
using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.DTOs.ProfessionalSummary;
using Portfolio.Core.Entities;
using Portfolio.Infrastructure.Persistence;

namespace Portfolio.Infrastructure.Repositories
{
    public class ProfessionalSummaryRepository : IProfessionalSummaryRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ProfessionalSummaryRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid> AddSummaryAsync(AddSummary summary)
        {
            var entity = new ProfessionalSummary
            {
                Id = Guid.NewGuid(),
                Summary = summary.Summary,
                UserId = summary.UserId,
            };

            await _dbContext.ProfessionalSummary.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<string?> GetSummaryById(Guid id)
        {
            return await _dbContext
                .ProfessionalSummary.Where(s => s.Id == id)
                .Select(s => s.Summary)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> PatchSummariesAsync(Guid userId, List<PatchSummary> patches)
        {
            if (patches == null || patches.Count == 0)
                return false;

            var ids = patches.Select(p => p.Id).Distinct().ToList();

            var summaries = await _dbContext
                .ProfessionalSummary.Where(s => s.UserId == userId && ids.Contains(s.Id))
                .ToListAsync();

            if (summaries.Count == 0)
                return false;

            var patchMap = patches.ToDictionary(p => p.Id, p => p);
            var anyChange = false;

            foreach (var summary in summaries)
            {
                var patch = patchMap[summary.Id];

                if (patch.Summary != null && patch.Summary != summary.Summary)
                {
                    summary.Summary = patch.Summary;
                    anyChange = true;
                }
            }

            if (!anyChange)
                return false;

            var saved = await _dbContext.SaveChangesAsync();
            return saved > 0;
        }
    }
}

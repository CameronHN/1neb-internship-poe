using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.DTOs.ProfessionalSummary;
using Portfolio.Core.Entities;
using Portfolio.Core.Exceptions;
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

        public async Task<List<Guid>> AddSummariesAsync(Guid userId, List<AddSummary> summaries)
        {
            var entities = summaries
                .Select(summary => new ProfessionalSummary
                {
                    Summary = summary.Summary,
                    UserId = userId,
                })
                .ToList();

            await _dbContext.ProfessionalSummary.AddRangeAsync(entities);
            await _dbContext.SaveChangesAsync();

            return entities.Select(e => e.Id).ToList();
        }

        public async Task<bool> DeleteProfessionalSummariesAsync(Guid userId, List<Guid> summaryIds)
        {
            if (summaryIds.IsNullOrEmpty())
                return false;

            var summariesToDelete = await _dbContext
                .ProfessionalSummary.Where(summary =>
                    summary.UserId == userId && summaryIds.Contains(summary.Id)
                )
                .ToListAsync();

            if (summariesToDelete.Count != summaryIds.Count)
                return false;

            _dbContext.ProfessionalSummary.RemoveRange(summariesToDelete);

            var saved = await _dbContext.SaveChangesAsync();
            return saved > 0;
        }

        public async Task<string> GetProfessionalSummaryByIdAsync(Guid id, Guid userId)
        {
            var summary =
                await _dbContext
                    .ProfessionalSummary.Where(s => s.Id == id && s.UserId == userId)
                    .Select(s => s.Summary)
                    .FirstOrDefaultAsync() ?? throw new NotFoundException("Summary does not exist");
            return summary;
        }

        public async Task<List<string>> GetSummariesByUserIdAsync(Guid userId)
        {
            return await _dbContext
                .ProfessionalSummary.Where(s => s.UserId == userId)
                .Select(s => s.Summary)
                .ToListAsync();
        }

        public async Task<string?> GetSummaryByIdAsync(Guid id)
        {
            return await _dbContext
                .ProfessionalSummary.Where(s => s.Id == id)
                .Select(s => s.Summary)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> PatchSummaryAsync(Guid userId, PatchSummary patch)
        {
            if (patch == null)
                return false;

            var summary = await _dbContext.ProfessionalSummary.FirstOrDefaultAsync(s =>
                s.UserId == userId && s.Id == patch.Id
            );

            if (summary == null)
                return false;

            var anyChange = false;

            if (patch.Summary != null && patch.Summary != summary.Summary)
            {
                summary.Summary = patch.Summary;
                anyChange = true;
            }

            if (!anyChange)
                return false;

            var saved = await _dbContext.SaveChangesAsync();
            return saved > 0;
        }
    }
}

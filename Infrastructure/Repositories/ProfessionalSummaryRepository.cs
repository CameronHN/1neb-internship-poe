using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.DTOs.ProfessionalSummary;
using Portfolio.Core.Entities;
using Portfolio.Infrastructure.Persistence;

namespace Portfolio.Infrastructure.Repositories
{
    public class ProfessionalSummaryRepository:IProfessionalSummaryRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ProfessionalSummaryRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddSummariesAsync(List<AddSummary> summaries)
        {
            var entities = summaries.Select(summary => new ProfessionalSummary
            {
                Id = Guid.NewGuid(),
                Summary = summary.Summary,
                UserId = summary.UserId
            }).ToList();

            await _dbContext.ProfessionalSummary.AddRangeAsync(entities);
            await _dbContext.SaveChangesAsync();
        }
    }
}

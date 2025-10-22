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

        public async Task<Guid> AddSummariesAsync(AddSummary summary)
        {
            var entity = new ProfessionalSummary
            {
                Id = Guid.NewGuid(),
                Summary = summary.Summary,
                UserId = summary.UserId
            };

            await _dbContext.ProfessionalSummary.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }
    }
}

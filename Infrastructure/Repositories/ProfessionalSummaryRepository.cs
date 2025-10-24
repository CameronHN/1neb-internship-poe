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
                UserId = summary.UserId
            };

            await _dbContext.ProfessionalSummary.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<string?> GetSummaryById(Guid id)
        {
            return await _dbContext.ProfessionalSummary
                .Where(s => s.Id == id)
                .Select(s => s.Summary)
                .FirstOrDefaultAsync();
        }
    }
}

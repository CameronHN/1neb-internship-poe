using Microsoft.EntityFrameworkCore;
using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.Entities;
using Portfolio.Infrastructure.Persistence;

namespace Portfolio.Infrastructure.Repositories
{
    public class SavedResumeRepository : ISavedResumeRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public SavedResumeRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid> CreateAsync(SavedResume savedResume)
        {
            await _dbContext.SavedResume.AddAsync(savedResume);
            await _dbContext.SaveChangesAsync();
            return savedResume.Id;
        }

        public async Task<SavedResume?> GetByIdAsync(Guid id, Guid userId)
        {
            return await _dbContext.SavedResume.FirstOrDefaultAsync(sr =>
                sr.Id == id && sr.UserId == userId
            );
        }

        public async Task<List<SavedResume>> GetAllByUserIdAsync(Guid userId)
        {
            return await _dbContext
                .SavedResume.Where(sr => sr.UserId == userId)
                .OrderByDescending(sr => sr.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> DeleteAsync(Guid id, Guid userId)
        {
            var savedResume = await _dbContext.SavedResume.FirstOrDefaultAsync(sr =>
                sr.Id == id && sr.UserId == userId
            );

            if (savedResume == null)
                return false;

            _dbContext.SavedResume.Remove(savedResume);
            var saved = await _dbContext.SaveChangesAsync();
            return saved > 0;
        }
    }
}

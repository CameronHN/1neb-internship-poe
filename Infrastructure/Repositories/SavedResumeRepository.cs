using Microsoft.EntityFrameworkCore;
using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.DTOs.SavedResume;
using Portfolio.Core.Entities;
using Portfolio.Core.Exceptions;
using Portfolio.Core.Models;
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

        public async Task<Guid> CreateAsync(Guid userId, AddSavedResume savedResume)
        {
            var entity = new SavedResume
            {
                Name = savedResume.Name,
                Data = savedResume.Data,
                TemplateType = savedResume.TemplateType,
                CreatedAt = DateTime.UtcNow,
                UserId = userId,
            };

            await _dbContext.SavedResume.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<SavedResumeModel?> GetByIdAsync(Guid id, Guid userId)
        {
            return await _dbContext
                    .SavedResume.Where(sr => sr.Id == id && sr.UserId == userId)
                    .Select(sr => new SavedResumeModel
                    {
                        Id = sr.Id,
                        Name = sr.Name,
                        Data = sr.Data,
                        TemplateType = sr.TemplateType,
                        CreatedAt = sr.CreatedAt.ToString("MMMM yyyy"),
                    })
                    .FirstOrDefaultAsync()
                ?? throw new NotFoundException("Saved resume does not exist");
        }

        public async Task<List<SavedResumeItem>> GetAllByUserIdAsync(Guid userId)
        {
            return await _dbContext
                .SavedResume.Where(sr => sr.UserId == userId)
                .OrderByDescending(sr => sr.CreatedAt)
                .Select(sr => new SavedResumeItem
                {
                    Id = sr.Id,
                    Name = sr.Name,
                    TemplateType = sr.TemplateType,
                    CreatedAt = sr.CreatedAt,
                })
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

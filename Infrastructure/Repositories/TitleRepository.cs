using Microsoft.EntityFrameworkCore;
using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.DTOs.ResumeTitle;
using Portfolio.Core.Entities;
using Portfolio.Infrastructure.Persistence;

namespace Portfolio.Infrastructure.Repositories
{
    public class TitleRepository : ITitleRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public TitleRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid> AddTitleAsync(AddResumeTitle title)
        {
            var entity = new Title
            {
                Id = Guid.NewGuid(),
                ResumeTitle = title.Title,
                UserId = title.UserId,
            };

            await _dbContext.Title.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<string?> GetTitleById(Guid id)
        {
            return await _dbContext
                .Title.Where(t => t.Id == id)
                .Select(t => t.ResumeTitle)
                .FirstOrDefaultAsync();
        }
    }
}

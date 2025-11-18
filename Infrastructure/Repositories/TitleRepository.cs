using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.DTOs.ResumeTitle;
using Portfolio.Core.Entities;
using Portfolio.Core.Exceptions;
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

        public async Task<List<string>> GetTitlesByUserIdAsync(Guid userId)
        {
            return await _dbContext
                .Title.Where(s => s.UserId == userId)
                .Select(s => s.ResumeTitle)
                .ToListAsync();
        }

        public async Task<List<Guid>> AddTitlesAsync(Guid userId, List<AddResumeTitle> titles)
        {
            var entities = titles
                .Select(title => new Title { ResumeTitle = title.Title, UserId = userId })
                .ToList();

            await _dbContext.Title.AddRangeAsync(entities);
            await _dbContext.SaveChangesAsync();

            return entities.Select(e => e.Id).ToList();
        }

        public async Task<bool> DeleteTitlesAsync(Guid userId, List<Guid> titleIds)
        {
            if (titleIds.IsNullOrEmpty())
                return false;

            var titlesToDelete = await _dbContext
                .Title.Where(title => title.UserId == userId && titleIds.Contains(title.Id))
                .ToListAsync();

            if (titlesToDelete.Count != titleIds.Count)
                return false;

            _dbContext.Title.RemoveRange(titlesToDelete);

            var saved = await _dbContext.SaveChangesAsync();
            return saved > 0;
        }

        public async Task<string?> GetTitleByIdAsync(Guid id)
        {
            return await _dbContext
                .Title.Where(t => t.Id == id)
                .Select(t => t.ResumeTitle)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> PatchTitleAsync(Guid userId, PatchResumeTitle patch)
        {
            if (patch == null)
                return false;

            var title = await _dbContext.Title.FirstOrDefaultAsync(t =>
                t.UserId == userId && t.Id == patch.Id
            );

            if (title == null)
                return false;

            var anyChange = false;

            if (patch.Title != null && patch.Title != title.ResumeTitle)
            {
                title.ResumeTitle = patch.Title;
                anyChange = true;
            }

            if (!anyChange)
                return false;

            var saved = await _dbContext.SaveChangesAsync();
            return saved > 0;
        }

        public async Task<string> GetResumeTitleByIdAsync(Guid id, Guid userId)
        {
            var title =
                await _dbContext
                    .Title.Where(t => t.Id == id && t.UserId == userId)
                    .Select(t => t.ResumeTitle)
                    .FirstOrDefaultAsync()
                ?? throw new NotFoundException("Resume title does not exist.");
            return title;
        }
    }
}

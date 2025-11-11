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

        public async Task<Guid> AddTitleAsync(Guid userId, AddResumeTitle title)
        {
            var entity = new Title { ResumeTitle = title.Title, UserId = userId };

            await _dbContext.Title.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
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

        public async Task<string?> GetTitleById(Guid id)
        {
            return await _dbContext
                .Title.Where(t => t.Id == id)
                .Select(t => t.ResumeTitle)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> PatchTitlesAsync(Guid userId, List<PatchResumeTitle> patches)
        {
            if (patches == null || patches.Count == 0)
                return false;

            var ids = patches.Select(p => p.Id).Distinct().ToList();

            var titles = await _dbContext
                .Title.Where(t => t.UserId == userId && ids.Contains(t.Id))
                .ToListAsync();

            if (titles.Count == 0)
                return false;

            var patchMap = patches.ToDictionary(p => p.Id, p => p);
            var anyChange = false;

            foreach (var title in titles)
            {
                var patch = patchMap[title.Id];

                if (patch.Title != null && patch.Title != title.ResumeTitle)
                {
                    title.ResumeTitle = patch.Title;
                    anyChange = true;
                }
            }

            if (!anyChange)
                return false;

            var saved = await _dbContext.SaveChangesAsync();
            return saved > 0;
        }

        public async Task<string> GetResumeTitleById(Guid id)
        {
            var title =
                await _dbContext
                    .Title.Where(t => t.Id == id)
                    .Select(t => t.ResumeTitle)
                    .FirstOrDefaultAsync()
                ?? throw new NotFoundException("Resume title does not exist.");
            return title;
        }
    }
}

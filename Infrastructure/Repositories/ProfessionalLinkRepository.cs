using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.ProfessionalLink;
using Portfolio.Core.DTOs.Resume;
using Portfolio.Core.Entities;
using Portfolio.Core.Models;
using Portfolio.Infrastructure.Persistence;

namespace Portfolio.Infrastructure.Repositories
{
    public class ProfessionalLinkRepository : IProfessionalLinkRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ProfessionalLinkRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Guid>> AddProfessionalLinksAsync(
            Guid userId,
            List<AddProfessionalLink> links
        )
        {
            var entities = links
                .Select(link => new ProfessionalLink
                {
                    LinkType = link.LinkType,
                    Link = link.Link,
                    UserId = userId,
                })
                .ToList();

            await _dbContext.ProfessionalLink.AddRangeAsync(entities);
            await _dbContext.SaveChangesAsync();

            return entities.Select(e => e.Id).ToList();
        }

        public async Task<ProfessionalLinkModel?> GetProfessionalLinkAsync(Guid id, Guid userId)
        {
            var link = await _dbContext
                .ProfessionalLink.Where(c => c.Id == id && c.UserId == userId)
                .Select(c => new ProfessionalLinkModel
                {
                    Id = c.Id,
                    Link = c.Link,
                    LinkType = c.LinkType,
                })
                .FirstOrDefaultAsync();

            return link;
        }

        public async Task<List<ProfessionalLinkModel>> GetProfessionalLinksByUserIdAsync(
            Guid userId
        )
        {
            return await _dbContext
                .ProfessionalLink.Where(c => c.UserId == userId)
                .Select(c => new ProfessionalLinkModel { Link = c.Link, LinkType = c.LinkType })
                .ToListAsync();
        }

        public async Task<List<ProfessionalLinkItem>> GetProfessionalLinksByIdsAsync(
            ItemListRequest request
        )
        {
            List<Guid> ids = request.Ids;
            if (request == null || ids == null || ids.Count == 0)
                return [];

            var links = await _dbContext
                .ProfessionalLink.Where(c => ids.Contains(c.Id))
                .Select(c => new
                {
                    c.Id,
                    c.Link,
                    c.LinkType,
                })
                .ToListAsync();

            // Preserve input order
            var order = ids.Select((id, idx) => new { id, idx })
                .ToDictionary(x => x.id, x => x.idx);

            links = links
                .OrderBy(c => order.TryGetValue(c.Id, out var idx) ? idx : int.MaxValue)
                .ToList();

            return links
                .Select(c => new ProfessionalLinkItem { Link = c.Link, LinkType = c.LinkType })
                .ToList();
        }

        public async Task<bool> PatchProfessionalLinkAsync(Guid userId, PatchProfessionalLink patch)
        {
            if (patch == null)
                return false;

            var link = await _dbContext.ProfessionalLink.FirstOrDefaultAsync(c =>
                c.UserId == userId && c.Id == patch.Id
            );

            if (link == null)
                return false;

            var anyChange = false;

            if (patch.Link != null && patch.Link != link.Link)
            {
                link.Link = patch.Link;
                anyChange = true;
            }

            if (!anyChange)
                return false;

            var saved = await _dbContext.SaveChangesAsync();
            return saved > 0;
        }

        public async Task<bool> DeleteProfessionalLinksAsync(Guid userId, List<Guid> linkIds)
        {
            if (linkIds.IsNullOrEmpty())
                return false;

            var linksToDelete = await _dbContext
                .ProfessionalLink.Where(c => c.UserId == userId && linkIds.Contains(c.Id))
                .ToListAsync();

            if (linksToDelete.Count != linkIds.Count)
                return false;

            _dbContext.ProfessionalLink.RemoveRange(linksToDelete);

            var saved = await _dbContext.SaveChangesAsync();
            return saved > 0;
        }
    }
}

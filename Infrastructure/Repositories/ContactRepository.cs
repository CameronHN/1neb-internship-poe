using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Contact;
using Portfolio.Core.DTOs.Resume;
using Portfolio.Core.Entities;
using Portfolio.Core.Models;
using Portfolio.Infrastructure.Persistence;

namespace Portfolio.Infrastructure.Repositories
{
    public class ContactRepository : IContactRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ContactRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Guid>> AddContactsAsync(Guid userId, List<AddProfessionalLink> contacts)
        {
            var entities = contacts
                .Select(contact => new ProfessionalLink { LinkType = contact.LinkType, Link = contact.Link, UserId = userId })
                .ToList();

            await _dbContext.ProfessionalLink.AddRangeAsync(entities);
            await _dbContext.SaveChangesAsync();

            return entities.Select(e => e.Id).ToList();
        }

        public async Task<ProfessionalLinkModel?> GetContactByIdAsync(Guid id, Guid userId)
        {
            var contact = await _dbContext
                .ProfessionalLink.Where(c => c.Id == id && c.UserId == userId)
                .Select(c => new ProfessionalLinkModel { Id = c.Id, Link = c.Link, LinkType = c.LinkType })
                .FirstOrDefaultAsync();

            return contact;
        }

        public async Task<List<ProfessionalLinkModel>> GetContactsByUserIdAsync(Guid userId)
        {
            return await _dbContext
                .ProfessionalLink.Where(c => c.UserId == userId)
                .Select(c => new ProfessionalLinkModel { Link = c.Link, LinkType = c.LinkType })
                .ToListAsync();
        }

        public async Task<List<ProfessionalLinkItem>> GetContactsByIdsAsync(ItemListRequest request)
        {
            List<Guid> ids = request.Ids;
            if (request == null || ids == null || ids.Count == 0)
                return [];

            var contacts = await _dbContext
                .ProfessionalLink.Where(c => ids.Contains(c.Id))
                .Select(c => new { c.Id, c.Link, c.LinkType })
                .ToListAsync();

            // Preserve input order
            var order = ids.Select((id, idx) => new { id, idx })
                .ToDictionary(x => x.id, x => x.idx);

            contacts = contacts
                .OrderBy(c => order.TryGetValue(c.Id, out var idx) ? idx : int.MaxValue)
                .ToList();

            return contacts
                .Select(c => new ProfessionalLinkItem { Link = c.Link, LinkType = c.LinkType })
                .ToList();
        }

        public async Task<bool> PatchContactAsync(Guid userId, PatchProfessionalLink patch)
        {
            if (patch == null)
                return false;

            var contact = await _dbContext.ProfessionalLink.FirstOrDefaultAsync(c =>
                c.UserId == userId && c.Id == patch.Id
            );

            if (contact == null)
                return false;

            var anyChange = false;

            if (patch.Link != null && patch.Link != contact.Link)
            {
                contact.Link = patch.Link;
                anyChange = true;
            }

            if (!anyChange)
                return false;

            var saved = await _dbContext.SaveChangesAsync();
            return saved > 0;
        }

        public async Task<bool> DeleteContactsAsync(Guid userId, List<Guid> contactIds)
        {
            if (contactIds.IsNullOrEmpty())
                return false;

            var contactsToDelete = await _dbContext
                .ProfessionalLink.Where(c => c.UserId == userId && contactIds.Contains(c.Id))
                .ToListAsync();

            if (contactsToDelete.Count != contactIds.Count)
                return false;

            _dbContext.ProfessionalLink.RemoveRange(contactsToDelete);

            var saved = await _dbContext.SaveChangesAsync();
            return saved > 0;
        }
    }
}

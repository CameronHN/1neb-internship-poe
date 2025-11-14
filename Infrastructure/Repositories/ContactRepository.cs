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

        public async Task<List<Guid>> AddContactsAsync(Guid userId, List<AddContact> contacts)
        {
            var entities = contacts
                .Select(contact => new Contact { ContactUrl = contact.Social, UserId = userId })
                .ToList();

            await _dbContext.Contact.AddRangeAsync(entities);
            await _dbContext.SaveChangesAsync();

            return entities.Select(e => e.Id).ToList();
        }

        public async Task<ContactModel?> GetContactByIdAsync(Guid id)
        {
            var contact = await _dbContext
                .Contact.Where(c => c.Id == id)
                .Select(c => new ContactModel { Id = c.Id, ContactUrl = c.ContactUrl })
                .FirstOrDefaultAsync();

            return contact;
        }

        public async Task<List<ContactModel>> GetContactsByUserIdAsync(Guid userId)
        {
            return await _dbContext
                .Contact.Where(c => c.UserId == userId)
                .Select(c => new ContactModel { ContactUrl = c.ContactUrl })
                .ToListAsync();
        }

        public async Task<List<SocialMediaItem>> GetContactsByIdsAsync(ItemListRequest request)
        {
            List<Guid> ids = request.Ids;
            if (request == null || ids == null || ids.Count == 0)
                return [];

            var contacts = await _dbContext
                .Contact.Where(c => ids.Contains(c.Id))
                .Select(c => new { c.Id, c.ContactUrl })
                .ToListAsync();

            // Preserve input order
            var order = ids.Select((id, idx) => new { id, idx })
                .ToDictionary(x => x.id, x => x.idx);

            contacts = contacts
                .OrderBy(c => order.TryGetValue(c.Id, out var idx) ? idx : int.MaxValue)
                .ToList();

            return contacts
                .Select(c => new SocialMediaItem { SocialMediaUrl = c.ContactUrl })
                .ToList();
        }

        public async Task<bool> PatchContactsAsync(Guid userId, List<PatchContact> patches)
        {
            if (patches == null || patches.Count == 0)
                return false;

            var ids = patches.Select(p => p.Id).Distinct().ToList();

            var contacts = await _dbContext
                .Contact.Where(c => c.UserId == userId && ids.Contains(c.Id))
                .ToListAsync();

            if (contacts.Count == 0)
                return false;

            var patchMap = patches.ToDictionary(p => p.Id, p => p);
            var anyChange = false;

            foreach (var contact in contacts)
            {
                var patch = patchMap[contact.Id];

                if (patch.Social != null && patch.Social != contact.ContactUrl)
                {
                    contact.ContactUrl = patch.Social;
                    anyChange = true;
                }
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
                .Contact.Where(c => c.UserId == userId && contactIds.Contains(c.Id))
                .ToListAsync();

            if (contactsToDelete.Count != contactIds.Count)
                return false;

            _dbContext.Contact.RemoveRange(contactsToDelete);

            var saved = await _dbContext.SaveChangesAsync();
            return saved > 0;
        }
    }
}

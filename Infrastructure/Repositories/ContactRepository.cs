using Microsoft.EntityFrameworkCore;
using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Contact;
using Portfolio.Core.Entities;
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

        public async Task<List<Guid>> AddContactsAsync(List<AddContact> contacts)
        {
            var entities = contacts.Select(contact => new Contact
            {
                Id = Guid.NewGuid(),
                ContactUrl = contact.Social,
                UserId = contact.UserId
            }).ToList();

            await _dbContext.Contact.AddRangeAsync(entities);
            await _dbContext.SaveChangesAsync();

            return entities.Select(e => e.Id).ToList();
        }

        public async Task<List<SocialMediaItem>> GetContactsByIdsAsync(ItemListRequest request)
        {
            List<Guid> ids = request.Ids;
            if (request == null || ids == null || ids.Count == 0)
                return [];

            var contacts = await _dbContext.Contact
                .Where(c => ids.Contains(c.Id))
                .Select(c => new
                {
                    c.Id,
                    c.ContactUrl
                })
                .ToListAsync();

            // Preserve input order
            var order = ids.Select((id, idx) => new { id, idx })
                .ToDictionary(x => x.id, x => x.idx);

            contacts = contacts
                .OrderBy(c => order.TryGetValue(c.Id, out var idx) ? idx : int.MaxValue)
                .ToList();

            return contacts
                .Select(c => new SocialMediaItem
                {
                    SocialMediaUrl = c.ContactUrl
                })
                .ToList();
        }
    }
}

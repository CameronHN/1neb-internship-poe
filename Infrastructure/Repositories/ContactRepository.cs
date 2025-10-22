using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.DTOs.Contact;
using Portfolio.Core.Entities;
using Portfolio.Infrastructure.Persistence;

namespace Portfolio.Infrastructure.Repositories
{
    public class ContactRepository: IContactRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ContactRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddContactsAsync(List<AddContact> contacts)
        {
            var entities = contacts.Select(contact => new Contact
            {
                Id = Guid.NewGuid(),
                LinkedIn = contact.LinkedIn,
                GitHub = contact.GitHub,
                UserId = contact.UserId
            }).ToList();

            await _dbContext.Contact.AddRangeAsync(entities);
            await _dbContext.SaveChangesAsync();
        }
    }
}

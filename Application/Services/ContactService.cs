using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Contact;

namespace Portfolio.Core.Services
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository _contactRepository;

        public ContactService(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        public async Task<List<Guid>> AddContactsAsync(Guid userId, List<AddContact> contacts)
        {
            return await _contactRepository.AddContactsAsync(userId, contacts);
        }

        public Task<List<SocialMediaItem>> GetContactsByIdsAsync(ItemListRequest request)
        {
            return _contactRepository.GetContactsByIdsAsync(request);
        }
    }
}
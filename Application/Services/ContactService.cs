using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Contact;
using Portfolio.Core.DTOs.Resume;
using Portfolio.Core.Models;

namespace Portfolio.Application.Services
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

        public async Task<ContactModel?> GetContactByIdAsync(Guid id)
        {
            return await _contactRepository.GetContactByIdAsync(id);
        }

        public async Task<List<ContactModel>> GetContactsByUserIdAsync(Guid userId)
        {
            return await _contactRepository.GetContactsByUserIdAsync(userId);
        }

        public Task<List<SocialMediaItem>> GetContactsByIdsAsync(ItemListRequest request)
        {
            return _contactRepository.GetContactsByIdsAsync(request);
        }

        public async Task<bool> PatchContactsAsync(Guid userId, List<PatchContact> patches)
        {
            return await _contactRepository.PatchContactsAsync(userId, patches);
        }

        public async Task<bool> DeleteContactsAsync(Guid userId, List<Guid> contactIds)
        {
            return await _contactRepository.DeleteContactsAsync(userId, contactIds);
        }
    }
}

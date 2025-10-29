using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Contact;
using Portfolio.Core.DTOs.Resume;
using Portfolio.Core.Models;

namespace Portfolio.Core.Contracts.Repositories
{
    public interface IContactRepository
    {
        // Create
        Task<List<Guid>> AddContactsAsync(Guid userId, List<AddContact> contacts);

        // Read
        Task<SocialMediaItem?> GetContactByIdAsync(Guid id);
        Task<List<ContactModel>> GetContactsByUserIdAsync(Guid userId);
        Task<List<SocialMediaItem>> GetContactsByIdsAsync(ItemListRequest request);

        // Update
        Task<bool> PatchContactsAsync(Guid userId, List<PatchContact> patches);

        // Delete
        Task<bool> DeleteContactsAsync(Guid userId, List<Guid> contactIds);
    }
}

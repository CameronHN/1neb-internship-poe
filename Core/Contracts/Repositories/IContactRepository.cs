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
        Task<ContactModel?> GetContactByIdAsync(Guid id, Guid userId);
        Task<List<ContactModel>> GetContactsByUserIdAsync(Guid userId);
        Task<List<SocialMediaItem>> GetContactsByIdsAsync(ItemListRequest request);

        // Update
        Task<bool> PatchContactAsync(Guid userId, PatchContact patch);

        // Delete
        Task<bool> DeleteContactsAsync(Guid userId, List<Guid> contactIds);
    }
}

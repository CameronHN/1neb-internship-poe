using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Contact;
using Portfolio.Core.DTOs.Resume;
using Portfolio.Core.Models;

namespace Portfolio.Core.Contracts.Repositories
{
    public interface IContactRepository
    {
        // Create
        Task<List<Guid>> AddContactsAsync(Guid userId, List<AddProfessionalLink> contacts);

        // Read
        Task<ProfessionalLinkModel?> GetContactByIdAsync(Guid id, Guid userId);
        Task<List<ProfessionalLinkModel>> GetContactsByUserIdAsync(Guid userId);
        Task<List<ProfessionalLinkItem>> GetContactsByIdsAsync(ItemListRequest request);

        // Update
        Task<bool> PatchContactAsync(Guid userId, PatchProfessionalLink patch);

        // Delete
        Task<bool> DeleteContactsAsync(Guid userId, List<Guid> contactIds);
    }
}

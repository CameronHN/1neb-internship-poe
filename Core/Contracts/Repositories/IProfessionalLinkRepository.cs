using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Contact;
using Portfolio.Core.DTOs.Resume;
using Portfolio.Core.Models;

namespace Portfolio.Core.Contracts.Repositories
{
    public interface IProfessionalLinkRepository
    {
        // Create
        Task<List<Guid>> AddProfessionalLinksAsync(Guid userId, List<AddProfessionalLink> contacts);

        // Read
        Task<ProfessionalLinkModel?> GetProfessionalLinkAsync(Guid id, Guid userId);
        Task<List<ProfessionalLinkModel>> GetProfessionalLinksByUserIdAsync(Guid userId);
        Task<List<ProfessionalLinkItem>> GetProfessionalLinksByIdsAsync(ItemListRequest request);

        // Update
        Task<bool> PatchProfessionalLinkAsync(Guid userId, PatchProfessionalLink patch);

        // Delete
        Task<bool> DeleteProfessionalLinksAsync(Guid userId, List<Guid> contactIds);
    }
}

using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Certification;

namespace Portfolio.Core.Contracts.Repositories
{
    public interface ICertificationRepository
    {
        // Create
        Task<List<Guid>> AddCertificationsAsync(List<AddCertification> certification);

        // Read
        Task<CertificationItem?> GetCertificationByIdAsync(Guid id);

        // Update
        Task<bool> PatchCertificationsAsync(Guid userId, List<PatchCertification> patches);

        // Delete
        Task DeleteCertificationAsync(Guid id);

        Task<List<CertificationItem>> GetAllCertificationsByTheirIdsAsync(ItemListRequest request);
    }
}

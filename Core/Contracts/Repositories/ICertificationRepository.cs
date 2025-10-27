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
        Task<List<CertificationItem>> GetCertificationsByUserIdAsync(Guid userId);

        // Update
        Task<bool> PatchCertificationsAsync(Guid userId, List<PatchCertification> patches);

        // Delete
        Task<bool> DeleteCertificationsAsync(Guid userId, List<Guid> certificationIds);

        Task<List<CertificationItem>> GetAllCertificationsByTheirIdsAsync(ItemListRequest request);
    }
}

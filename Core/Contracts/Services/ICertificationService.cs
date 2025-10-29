using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Certification;
using Portfolio.Core.Models;

namespace Portfolio.Core.Contracts.Services
{
    public interface ICertificationService
    {
        // Create
        Task<List<Guid>> AddCertificationAsync(Guid userId, List<AddCertification> certification);

        // Read
        Task<CertificationItem?> GetCertificationByIdAsync(Guid id);
        Task<List<CertificationModel>> GetCertificationsByUserIdAsync(Guid userId);
        Task<List<CertificationItem>> GetAllCertsByIds(ItemListRequest certificationRequest);

        // Update
        Task<bool> PatchCertificationsAsync(Guid userId, List<PatchCertification> patches);

        // Delete
        Task<bool> DeleteCertificationsAsync(Guid userId, List<Guid> certificationIds);

    }
}

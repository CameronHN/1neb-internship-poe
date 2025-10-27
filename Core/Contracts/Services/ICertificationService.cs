using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Certification;

namespace Portfolio.Core.Contracts.Services
{
    public interface ICertificationService
    {
        // Create
        Task<List<Guid>> AddCertificationAsync(List<AddCertification> certification);

        // Read
        Task<CertificationItem?> GetCertificationByIdAsync(Guid id);

        // Update
        Task<bool> PatchCertificationsAsync(Guid userId, List<PatchCertification> patches);

        // Delete
        Task DeleteCertificationAsync(Guid id);

        Task<List<CertificationItem>> GetAllCertsByIds(ItemListRequest certificationRequest);
    }
}

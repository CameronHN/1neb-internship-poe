using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Certification;

namespace Portfolio.Core.Contracts.Services
{
    public interface ICertificationService
    {
        // Create
        Task AddCertificationAsync(List<AddCertification> certification);

        // Read
        Task<CertificationItem?> GetCertificationByIdAsync(Guid id);

        // Update
        Task UpdateCertificationAsync(Guid userId, UpdateCertification certification);

        // Delete
        Task DeleteCertificationAsync(Guid id);

        Task<List<CertificationItem>> GetAllCertsByIds(ItemListRequest certificationRequest);
    }
}

using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Certification;
using Portfolio.Core.DTOs.Resume;
using Portfolio.Core.Models;

namespace Portfolio.Core.Contracts.Services
{
    public interface ICertificationService
    {
        // Create
        Task<List<Guid>> AddCertificationAsync(Guid userId, List<AddCertification> certification);

        // Read
        Task<CertificationModel> GetCertificationByIdAsync(Guid id, Guid userId);
        Task<List<CertificationModel>> GetCertificationsByUserIdAsync(Guid userId);
        Task<List<CertificationItem>> GetAllCertsByIds(ItemListRequest certificationRequest);

        // Update
        Task<bool> PatchCertificationAsync(Guid userId, PatchCertification patch);

        // Delete
        Task<bool> DeleteCertificationsAsync(Guid userId, List<Guid> certificationIds);
    }
}

using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Certification;

namespace Portfolio.Core.Contracts.Repositories
{
    public interface ICertificationRepository
    {
        // Create
        Task AddCertificationAsync(Guid userId, AddCertification certification);

        // Read
        Task<CertificationItem?> GetCertificationByIdAsync(Guid id);

        // Update
        Task UpdateCertificationAsync(Guid userId, UpdateCertification certification);

        // Delete
        Task DeleteCertificationAsync(Guid id);

        Task<List<CertificationItem>> GetAllCertificationsByTheirIdsAsync(ItemListRequest request);
    }
}

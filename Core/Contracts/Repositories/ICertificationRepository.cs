using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Certification;
using Portfolio.Core.Models;

namespace Portfolio.Core.Contracts.Repositories
{
    public interface ICertificationRepository
    {
        // -------------------- Create --------------------
        /// <summary>
        /// Adds multiple certifications for a user.
        /// Returns a list of the IDs for the newly created certifications.
        /// </summary>
        Task<List<Guid>> AddCertificationsAsync(List<AddCertification> certification);

        // -------------------- Read --------------------
        /// <summary>
        /// Retrieves a single certification by its unique identifier.
        /// </summary>
        Task<CertificationItem?> GetCertificationByIdAsync(Guid id);

        /// <summary>
        /// Gets all certifications associated with a specific user.
        /// </summary>
        Task<List<CertificationModel>> GetCertificationsByUserIdAsync(Guid userId);

        /// <summary>
        /// Retrieves certifications matching a list of IDs, with optional sorting.
        /// </summary>
        Task<List<CertificationItem>> GetAllCertificationsByTheirIdsAsync(ItemListRequest request);

        // -------------------- Update --------------------
        /// <summary>
        /// Applies partial updates to multiple certifications for a user.
        /// </summary>
        Task<bool> PatchCertificationsAsync(Guid userId, List<PatchCertification> patches);

        // -------------------- Delete --------------------
        /// <summary>
        /// Deletes multiple certifications for a user by their IDs.
        /// </summary>
        Task<bool> DeleteCertificationsAsync(Guid userId, List<Guid> certificationIds);
    }
}

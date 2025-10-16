using Portfolio.Core.DTOs;

namespace Portfolio.Core.Contracts.Repositories
{
    public interface ICertificationRepository
    {
        Task<List<CertificationItem>> GetAllCertsByIds(ItemListRequest request);
    }
}

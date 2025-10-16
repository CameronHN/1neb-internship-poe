using Portfolio.Core.DTOs;

namespace Portfolio.Core.Contracts.Services
{
    public interface ICertificationService
    {
        Task<List<CertificationItem>> GetAllCertsByIds(ItemListRequest certificationRequest);
    }
}

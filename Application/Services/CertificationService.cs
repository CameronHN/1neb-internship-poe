using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs;

namespace Portfolio.Application.Services
{
    public class CertificationService : ICertificationService
    {
        private readonly ICertificationRepository _certificationRepository;

        public CertificationService(ICertificationRepository certificationRepository)
        {
            _certificationRepository = certificationRepository;
        }

        public Task<List<CertificationItem>> GetAllCertsByIds(ItemListRequest request)
        {
            return _certificationRepository.GetAllCertsByIds(request);
        }
    }
}

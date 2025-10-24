using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Certification;

namespace Portfolio.Application.Services
{
    public class CertificationService : ICertificationService
    {
        private readonly ICertificationRepository _certificationRepository;

        public CertificationService(ICertificationRepository certificationRepository)
        {
            _certificationRepository = certificationRepository;
        }

        public async Task<List<Guid>> AddCertificationAsync(List<AddCertification> certification)
        {
            return await _certificationRepository.AddCertificationsAsync(certification);
        }

        public Task DeleteCertificationAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<List<CertificationItem>> GetAllCertsByIds(ItemListRequest request)
        {
            return _certificationRepository.GetAllCertificationsByTheirIdsAsync(request);
        }

        public Task<CertificationItem?> GetCertificationByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateCertificationAsync(Guid userId, UpdateCertification certification)
        {
            throw new NotImplementedException();
        }
    }
}

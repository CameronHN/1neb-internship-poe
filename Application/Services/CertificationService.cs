using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Certification;
using Portfolio.Core.DTOs.Resume;
using Portfolio.Core.Models;

namespace Portfolio.Application.Services
{
    public class CertificationService : ICertificationService
    {
        private readonly ICertificationRepository _certificationRepository;

        public CertificationService(ICertificationRepository certificationRepository)
        {
            _certificationRepository = certificationRepository;
        }

        public async Task<List<Guid>> AddCertificationAsync(
            Guid userId,
            List<AddCertification> certification
        )
        {
            return await _certificationRepository.AddCertificationsAsync(userId, certification);
        }

        public async Task<bool> DeleteCertificationsAsync(Guid userId, List<Guid> certificationIds)
        {
            return await _certificationRepository.DeleteCertificationsAsync(
                userId,
                certificationIds
            );
        }

        public async Task<List<CertificationItem>> GetAllCertsByIds(ItemListRequest request)
        {
            return await _certificationRepository.GetAllCertificationsByTheirIdsAsync(request);
        }

        public async Task<CertificationItem> GetCertificationByIdAsync(Guid id)
        {
            return await _certificationRepository.GetCertificationByIdAsync(id);
        }

        public async Task<List<CertificationModel>> GetCertificationsByUserIdAsync(Guid userId)
        {
            return await _certificationRepository.GetCertificationsByUserIdAsync(userId);
        }

        public Task<bool> PatchCertificationsAsync(Guid userId, List<PatchCertification> patches)
        {
            return _certificationRepository.PatchCertificationsAsync(userId, patches);
        }
    }
}

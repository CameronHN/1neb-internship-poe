using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.ProfessionalLink;
using Portfolio.Core.DTOs.Resume;
using Portfolio.Core.Models;

namespace Portfolio.Application.Services
{
    public class ProfessionalLinkService : IProfessionalLinkService
    {
        private readonly IProfessionalLinkRepository _professionalLinkRepository;

        public ProfessionalLinkService(IProfessionalLinkRepository professionalLinkRepository)
        {
            _professionalLinkRepository = professionalLinkRepository;
        }

        public async Task<List<Guid>> AddProfessionalLinksAsync(
            Guid userId,
            List<AddProfessionalLink> links
        )
        {
            return await _professionalLinkRepository.AddProfessionalLinksAsync(userId, links);
        }

        public async Task<ProfessionalLinkModel?> GetProfessionalLinkByIdAsync(Guid id, Guid userId)
        {
            return await _professionalLinkRepository.GetProfessionalLinkAsync(id, userId);
        }

        public async Task<List<ProfessionalLinkModel>> GetProfessionalLinksByUserIdAsync(
            Guid userId
        )
        {
            return await _professionalLinkRepository.GetProfessionalLinksByUserIdAsync(userId);
        }

        public Task<List<ProfessionalLinkItem>> GetProfessionalLinksByIdsAsync(
            ItemListRequest request
        )
        {
            return _professionalLinkRepository.GetProfessionalLinksByIdsAsync(request);
        }

        public async Task<bool> PatchProfessionalLinkAsync(Guid userId, PatchProfessionalLink patch)
        {
            return await _professionalLinkRepository.PatchProfessionalLinkAsync(userId, patch);
        }

        public async Task<bool> DeleteProfessionalLinksAsync(Guid userId, List<Guid> linkIds)
        {
            return await _professionalLinkRepository.DeleteProfessionalLinksAsync(userId, linkIds);
        }
    }
}

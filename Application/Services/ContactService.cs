using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Contact;
using Portfolio.Core.DTOs.Resume;
using Portfolio.Core.Models;

namespace Portfolio.Application.Services
{
    public class ContactService : IProfessionalLinkService
    {
        private readonly IProfessionalLinkRepository _contactRepository;

        public ContactService(IProfessionalLinkRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        public async Task<List<Guid>> AddProfessionalLinksAsync(Guid userId, List<AddProfessionalLink> contacts)
        {
            return await _contactRepository.AddProfessionalLinksAsync(userId, contacts);
        }

        public async Task<ProfessionalLinkModel?> GetProfessionalLinkByIdAsync(Guid id, Guid userId)
        {
            return await _contactRepository.GetProfessionalLinkAsync(id, userId);
        }

        public async Task<List<ProfessionalLinkModel>> GetProfessionalLinksByUserIdAsync(Guid userId)
        {
            return await _contactRepository.GetProfessionalLinksByUserIdAsync(userId);
        }

        public Task<List<ProfessionalLinkItem>> GetProfessionalLinksByIdsAsync(ItemListRequest request)
        {
            return _contactRepository.GetProfessionalLinksByIdsAsync(request);
        }

        public async Task<bool> PatchProfessionalLinkAsync(Guid userId, PatchProfessionalLink patch)
        {
            return await _contactRepository.PatchProfessionalLinkAsync(userId, patch);
        }

        public async Task<bool> DeleteProfessionalLinksAsync(Guid userId, List<Guid> contactIds)
        {
            return await _contactRepository.DeleteProfessionalLinksAsync(userId, contactIds);
        }
    }
}

using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Resume;
using Portfolio.Core.DTOs.Skill;
using Portfolio.Core.Models;

namespace Portfolio.Application.Services
{
    public class SkillService : ISkillService
    {
        private readonly ISkillRepository _skillRepository;

        public SkillService(ISkillRepository skillRepository)
        {
            _skillRepository = skillRepository;
        }

        public async Task<List<Guid>> AddSkillsAsync(Guid userId, List<AddSkill> skills)
        {
            return await _skillRepository.AddSkillsAsync(userId, skills);
        }

        public async Task<bool> DeleteSkillsAsync(Guid userId, List<Guid> skillIds)
        {
            return await _skillRepository.DeleteSkillsAsync(userId, skillIds);
        }

        public async Task<List<SkillsItem>> GetAllSkillsByIdsAsync(ItemListRequest request)
        {
            return await _skillRepository.GetAllSkillsByIdsAsync(request);
        }

        public async Task<SkillModel> GetSkillByIdAsync(Guid id, Guid userId)
        {
            return await _skillRepository.GetSkillByIdAsync(id, userId);
        }

        public async Task<List<SkillModel>> GetSkillsByUserIdAsync(Guid userId)
        {
            return await _skillRepository.GetSkillsByUserIdAsync(userId);
        }

        public async Task<bool> PatchSkillAsync(Guid userId, PatchSkill patch)
        {
            return await _skillRepository.PatchSkillAsync(userId, patch);
        }
    }
}

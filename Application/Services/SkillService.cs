using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Skill;

namespace Portfolio.Application.Services
{
    public class SkillService : ISkillService
    {
        private readonly ISkillRepository _skillRepository;

        public SkillService(ISkillRepository skillRepository)
        {
            _skillRepository = skillRepository;
        }

        public async Task<List<Guid>> AddSkillsAsync(List<AddSkill> skills)
        {
            return await _skillRepository.AddSkillsAsync(skills);
        }

        public async Task<bool> DeleteSkillsAsync(Guid userId, List<Guid> skillIds)
        {
            return await _skillRepository.DeleteSkillsAsync(userId, skillIds);
        }

        public Task<List<SkillsItem>> GetAllSkillsByIds(ItemListRequest request)
        {
            return _skillRepository.GetAllSkillsByIds(request);
        }

        public async Task<bool> PatchSkillsAsync(Guid userId, List<PatchSkill> patches)
        {
            return await _skillRepository.PatchSkillsAsync(userId, patches);
        }
    }
}

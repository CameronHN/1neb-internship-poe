using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Skill;

namespace Portfolio.Core.Contracts.Services
{
    public interface ISkillService
    {
        Task<List<Guid>> AddSkillsAsync(List<AddSkill> skills);

        Task<List<SkillsItem>> GetAllSkillsByIds(ItemListRequest request);

        Task<bool> PatchSkillsAsync(Guid userId, List<PatchSkill> patches);
    }
}

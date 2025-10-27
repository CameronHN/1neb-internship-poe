using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Skill;

namespace Portfolio.Core.Contracts.Repositories
{
    public interface ISkillRepository
    {
        Task<List<Guid>> AddSkillsAsync(List<AddSkill> skills);

        Task<List<SkillsItem>> GetAllSkillsByIds(ItemListRequest request);
        Task<List<SkillsItem>> GetSkillsByUserIdAsync(Guid userId);


        Task<bool> PatchSkillsAsync(Guid userId, List<PatchSkill> patches);

        Task<bool> DeleteSkillsAsync(Guid userId, List<Guid> skillIds);
    }
}

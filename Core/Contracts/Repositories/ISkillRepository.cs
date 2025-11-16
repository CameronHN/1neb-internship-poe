using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Resume;
using Portfolio.Core.DTOs.Skill;
using Portfolio.Core.Models;

namespace Portfolio.Core.Contracts.Repositories
{
    public interface ISkillRepository
    {
        Task<List<Guid>> AddSkillsAsync(Guid userId, List<AddSkill> skills);

        Task<List<SkillsItem>> GetAllSkillsByIds(ItemListRequest request);
        Task<List<SkillModel>> GetSkillsByUserIdAsync(Guid userId);
        Task<SkillModel> GetSkillById(Guid id);

        Task<bool> PatchSkillAsync(Guid userId, PatchSkill patch);

        Task<bool> DeleteSkillsAsync(Guid userId, List<Guid> skillIds);
    }
}

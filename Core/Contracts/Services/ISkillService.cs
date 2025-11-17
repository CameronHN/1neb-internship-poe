using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Resume;
using Portfolio.Core.DTOs.Skill;
using Portfolio.Core.Models;

namespace Portfolio.Core.Contracts.Services
{
    public interface ISkillService
    {
        Task<List<Guid>> AddSkillsAsync(Guid userId, List<AddSkill> skills);

        Task<List<SkillsItem>> GetAllSkillsByIds(ItemListRequest request);
        Task<List<SkillModel>> GetSkillsByUserIdAsync(Guid userId);
        Task<SkillModel> GetSkillByIdAsync(Guid id, Guid userId);

        // Update
        Task<bool> PatchSkillAsync(Guid userId, PatchSkill patch);

        Task<bool> DeleteSkillsAsync(Guid userId, List<Guid> skillIds);
    }
}

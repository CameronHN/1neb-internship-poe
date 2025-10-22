using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Skill;

namespace Portfolio.Core.Contracts.Repositories
{
    public interface ISkillRepository
    {
        Task AddSkillsAsync(List<AddSkill> skills);
        Task<List<SkillsItem>> GetAllSkillsByIds(ItemListRequest request);
    }
}

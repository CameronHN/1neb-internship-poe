using Portfolio.Core.DTOs;

namespace Portfolio.Core.Contracts.Repositories
{
    public interface ISkillRepository
    {
        Task<List<SkillsItem>> GetAllSkillsByIds(ItemListRequest request);
    }
}

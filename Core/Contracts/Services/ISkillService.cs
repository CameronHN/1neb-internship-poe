using Portfolio.Core.DTOs;

namespace Portfolio.Core.Contracts.Services
{
    public interface ISkillService
    {
        Task<List<SkillsItem>> GetAllSkillsByIds(ItemListRequest request);
    }
}

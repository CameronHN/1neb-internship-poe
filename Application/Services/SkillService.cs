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

        public Task<List<Guid>> AddSkillsAsync(List<AddSkill> skills)
        {
            throw new NotImplementedException();
        }

        public Task<List<SkillsItem>> GetAllSkillsByIds(ItemListRequest request)
        {
            return _skillRepository.GetAllSkillsByIds(request);
        }
    }
}

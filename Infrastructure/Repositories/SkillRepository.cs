using Microsoft.EntityFrameworkCore;
using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.DTOs;
using Portfolio.Infrastructure.Persistence;

namespace Portfolio.Infrastructure.Repositories
{
    public class SkillRepository : ISkillRepository

    {
        private readonly ApplicationDbContext _dbContext;

        public SkillRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<SkillsItem>> GetAllSkillsByIds(ItemListRequest request)
        {
            var ids = request.Ids;
            if (ids.Count == 0) return [];

            var skills = await _dbContext.Skill
                .Where(skill => ids.Contains(skill.Id))
                .Select(sk => new
                {
                    sk.Id,
                    sk.SkillName,
                    sk.ProficiencyLevel
                })
                .ToListAsync();

            switch (request.Order)
            {
                case SortOrder.Descending:
                    skills = skills.OrderByDescending(s => s.SkillName).ToList();
                    break;
                case SortOrder.Ascending:
                    skills = skills.OrderBy(s => s.SkillName).ToList();
                    break;
                case SortOrder.None:
                default:
                    var order = ids.Select((id, idx) => new { id, idx }).ToDictionary(x => x.id, x => x.idx);
                    skills = skills.OrderBy(s => order.TryGetValue(s.Id, out var idx) ? idx : int.MaxValue).ToList();
                    break;
            }

            return skills.Select(sk => new SkillsItem
            {
                Skill = sk.SkillName,
                SkillLevel = sk.ProficiencyLevel
            }).ToList();
        }
    }
}

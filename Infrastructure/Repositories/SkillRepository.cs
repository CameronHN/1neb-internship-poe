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
                }).ToListAsync();

            if (request.IsDescending)
            {
                // Order by Name descending
                skills = skills
                    .OrderByDescending(c => c.SkillName)
                    .ToList();
            }
            else
            {
                // Preserve the exact order of the incoming Ids
                var order = ids
                    .Select((id, index) => new { id, index })
                    .ToDictionary(x => x.id, x => x.index);

                skills = skills
                    .OrderBy(c => order.TryGetValue(c.Id, out var idx) ? idx : int.MaxValue)
                    .ToList();
            }

            return skills.Select(ce => new SkillsItem
            {
                Skill = ce.SkillName,
                SkillLevel = ce.ProficiencyLevel
            }).ToList();

        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Skill;
using Portfolio.Core.Entities;
using Portfolio.Core.Models;
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

        public async Task<List<Guid>> AddSkillsAsync(Guid userId, List<AddSkill> skills)
        {
            var entities = skills
                .Select(skill => new Skill
                {
                    SkillName = skill.Skill,
                    ProficiencyLevel = skill.ProficiencyLevel,
                    UserId = userId,
                })
                .ToList();

            await _dbContext.Skill.AddRangeAsync(entities);
            await _dbContext.SaveChangesAsync();

            return entities.Select(e => e.Id).ToList();
        }

        public async Task<bool> DeleteSkillsAsync(Guid userId, List<Guid> skillIds)
        {
            if (skillIds.IsNullOrEmpty())
                return false;

            var skillsToDelete = await _dbContext
                .Skill.Where(skill => skill.UserId == userId && skillIds.Contains(skill.Id))
                .ToListAsync();

            if (skillsToDelete.Count != skillIds.Count)
                return false;

            _dbContext.Skill.RemoveRange(skillsToDelete);

            var saved = await _dbContext.SaveChangesAsync();
            return saved > 0;
        }

        public async Task<List<SkillModel>> GetSkillsByUserIdAsync(Guid userId)
        {
            return await _dbContext
                .Skill.Where(skill => skill.UserId == userId)
                .Select(skill => new SkillModel
                {
                    SkillName = skill.SkillName,
                    ProficiencyLevel = skill.ProficiencyLevel,
                })
                .ToListAsync();
        }

        public async Task<List<SkillsItem>> GetAllSkillsByIds(ItemListRequest request)
        {
            var ids = request.Ids;
            if (ids.Count == 0)
                return [];

            var skills = await _dbContext
                .Skill.Where(skill => ids.Contains(skill.Id))
                .Select(sk => new
                {
                    sk.Id,
                    sk.SkillName,
                    sk.ProficiencyLevel,
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
                    var order = ids.Select((id, idx) => new { id, idx })
                        .ToDictionary(x => x.id, x => x.idx);
                    skills = skills
                        .OrderBy(s => order.TryGetValue(s.Id, out var idx) ? idx : int.MaxValue)
                        .ToList();
                    break;
            }

            return skills
                .Select(sk => new SkillsItem
                {
                    Skill = sk.SkillName,
                    SkillLevel = sk.ProficiencyLevel,
                })
                .ToList();
        }

        public async Task<bool> PatchSkillsAsync(Guid userId, List<PatchSkill> patches)
        {
            if (patches == null || patches.Count == 0)
                return false;

            var ids = patches.Select(p => p.Id).Distinct().ToList();

            // Load only skills owned by the user and included in the patch list
            var skills = await _dbContext
                .Skill.Where(s => s.UserId == userId && ids.Contains(s.Id))
                .ToListAsync();

            if (skills.Count == 0)
                return false;

            var patchMap = patches.ToDictionary(p => p.Id, p => p);
            var anyChange = false;

            foreach (var skill in skills)
            {
                var patch = patchMap[skill.Id];

                // Update name if provided or different
                if (patch.Skill != null && patch.Skill != skill.SkillName)
                {
                    skill.SkillName = patch.Skill;
                    anyChange = true;
                }

                // Update proficiency if provided
                if (patch.ProficiencyLevel != null)
                {
                    var newLevel = string.IsNullOrWhiteSpace(patch.ProficiencyLevel)
                        ? null
                        : patch.ProficiencyLevel;
                    if (newLevel != skill.ProficiencyLevel)
                    {
                        skill.ProficiencyLevel = newLevel;
                        anyChange = true;
                    }
                }
            }

            if (!anyChange)
                return false;

            var saved = await _dbContext.SaveChangesAsync();
            return saved > 0;
        }
    }
}

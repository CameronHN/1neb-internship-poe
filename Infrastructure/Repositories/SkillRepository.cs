using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Resume;
using Portfolio.Core.DTOs.Skill;
using Portfolio.Core.Entities;
using Portfolio.Core.Exceptions;
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

        public async Task<List<SkillsItem>> GetAllSkillsByIdsAsync(ItemListRequest request)
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

        public async Task<bool> PatchSkillAsync(Guid userId, PatchSkill patch)
        {
            if (patch == null)
                return false;

            var skill = await _dbContext.Skill.FirstOrDefaultAsync(s =>
                s.UserId == userId && s.Id == patch.Id
            );

            if (skill == null)
                return false;

            var anyChange = false;

            // Update name if provided or different
            if (patch.Skill != null && patch.Skill != skill.SkillName)
            {
                skill.SkillName = patch.Skill;
                anyChange = true;
            }

            var profLevel = string.IsNullOrWhiteSpace(patch.ProficiencyLevel)
                ? null
                : patch.ProficiencyLevel;
            if (profLevel != skill.ProficiencyLevel)
            {
                skill.ProficiencyLevel = profLevel;
                anyChange = true;
            }

            if (!anyChange)
                return false;

            var saved = await _dbContext.SaveChangesAsync();
            return saved > 0;
        }

        public async Task<SkillModel> GetSkillByIdAsync(Guid id, Guid userId)
        {
            var skill =
                await _dbContext
                    .Skill.Where(s => s.Id == id && s.UserId == userId)
                    .Select(s => new SkillModel
                    {
                        SkillName = s.SkillName,
                        ProficiencyLevel = s.ProficiencyLevel,
                    })
                    .FirstOrDefaultAsync() ?? throw new NotFoundException("Skill does not exist");

            return skill;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Experience;
using Portfolio.Core.DTOs.Resume;
using Portfolio.Core.Entities;
using Portfolio.Core.Exceptions;
using Portfolio.Core.Models;
using Portfolio.Infrastructure.Persistence;

namespace Portfolio.Infrastructure.Repositories
{
    public class ExperienceRepository : IExperienceRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ExperienceRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ExperienceModel>> GetAllExperiencesByIds(List<Guid> ids)
        {
            return await _dbContext
                .Experience.Where(e => ids.Contains(e.Id))
                .Include(e => e.Responsibilities)
                .OrderBy(e => e.EndDate)
                .Select(e => new ExperienceModel
                {
                    CompanyName = e.CompanyName,
                    JobTitle = e.JobTitle,
                    StartDate = e.StartDate.ToString("MMMM yyyy"),
                    EndDate = e.EndDate.ToString("MMMM yyyy"),
                })
                .ToListAsync();
        }

        public async Task<
            List<ExperienceWithResponsibilitiesModel>
        > GetAllExperiencesIncludingResponsibilitiesByUserIdAsync(Guid id)
        {
            return await _dbContext
                .Experience.Where(e => e.UserId == id)
                .Include(e => e.Responsibilities)
                .OrderBy(e => e.EndDate)
                .Select(e => new ExperienceWithResponsibilitiesModel
                {
                    Id = e.Id,
                    CompanyName = e.CompanyName,
                    JobTitle = e.JobTitle,
                    StartDate = e.StartDate.ToString("MMMM yyyy"),
                    EndDate = e.EndDate.ToString("MMMM yyyy"),
                    Responsibilities = e.Responsibilities.Select(r => r.Responsibility).ToList(),
                })
                .ToListAsync();
        }

        public async Task<ExperienceWithResponsibilitiesModel?> GetExperienceById(Guid id)
        {
            var experience = await _dbContext
                .Experience.Include(e => e.Responsibilities)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (experience == null)
            {
                throw new NotFoundException("Experience not found.");
            }

            return new ExperienceWithResponsibilitiesModel
            {
                Id = experience.Id,
                CompanyName = experience.CompanyName,
                JobTitle = experience.JobTitle,
                StartDate = experience.StartDate.ToString("MMMM yyyy"),
                EndDate = experience.EndDate.ToString("MMMM yyyy"),
                Responsibilities = experience
                    .Responsibilities.Select(r => r.Responsibility)
                    .ToList(),
            };
        }

        public async Task<List<ExperienceItem>> GetAllExperiencesByIds(ItemListRequest request)
        {
            var ids = request.Ids;
            if (ids.Count == 0)
                return [];

            var experiences = await _dbContext
                .Experience.Where(exp => ids.Contains(exp.Id))
                .Select(ex => new
                {
                    ex.Id,
                    ex.CompanyName,
                    ex.JobTitle,
                    ex.StartDate,
                    ex.EndDate,
                    ex.Responsibilities,
                })
                .ToListAsync();

            switch (request.Order)
            {
                case SortOrder.Descending:
                    experiences = experiences.OrderByDescending(e => e.EndDate).ToList();
                    break;
                case SortOrder.Ascending:
                    experiences = experiences.OrderBy(e => e.EndDate).ToList();
                    break;
                case SortOrder.None:
                default:
                    var order = ids.Select((id, idx) => new { id, idx })
                        .ToDictionary(x => x.id, x => x.idx);
                    experiences = experiences
                        .OrderBy(e => order.TryGetValue(e.Id, out var idx) ? idx : int.MaxValue)
                        .ToList();
                    break;
            }

            return experiences
                .Select(ex => new ExperienceItem
                {
                    Company = ex.CompanyName,
                    JobTitle = ex.JobTitle,
                    StartDate = ex.StartDate.ToString("MMMM yyyy"),
                    EndDate = ex.EndDate.ToString("MMMM yyyy"),
                    Responsibilities = ex.Responsibilities.Select(r => r.Responsibility).ToList(),
                })
                .ToList();
        }

        public async Task<List<Guid>> AddExperiencesAsync(
            Guid userId,
            List<AddExperience> experiences
        )
        {
            var entities = experiences
                .Select(exp => new Experience
                {
                    JobTitle = exp.JobTitle,
                    CompanyName = exp.CompanyName,
                    StartDate = DateOnly.Parse(exp.StartDate),
                    EndDate = DateOnly.Parse(exp.EndDate),
                    UserId = userId,
                    Responsibilities = exp
                        .Responsibilities.Select(r => new ExperienceResponsibility
                        {
                            Responsibility = r.Responsibility,
                        })
                        .ToList(),
                })
                .ToList();

            await _dbContext.Experience.AddRangeAsync(entities);
            await _dbContext.SaveChangesAsync();

            return entities.Select(e => e.Id).ToList();
        }

        public async Task<bool> PatchExperiencesAsync(Guid userId, List<PatchExperience> patches)
        {
            if (patches == null || patches.Count == 0)
                return false;

            var ids = patches.Select(p => p.Id).Distinct().ToList();

            var experiences = await _dbContext
                .Set<Experience>()
                .Where(e => e.UserId == userId && ids.Contains(e.Id))
                .ToListAsync();

            if (experiences.Count == 0)
                return false;

            var patchMap = patches.ToDictionary(p => p.Id, p => p);
            var anyChange = false;

            var experienceIds = experiences.Select(e => e.Id).ToList();
            var responsibilities = await _dbContext
                .Set<ExperienceResponsibility>()
                .Where(r => experienceIds.Contains(r.ExperienceId))
                .ToListAsync();

            foreach (var exp in experiences)
            {
                var patch = patchMap[exp.Id];

                if (patch.JobTitle != null && patch.JobTitle != exp.JobTitle)
                {
                    exp.JobTitle = patch.JobTitle;
                    anyChange = true;
                }

                if (patch.CompanyName != null && patch.CompanyName != exp.CompanyName)
                {
                    exp.CompanyName = patch.CompanyName;
                    anyChange = true;
                }

                if (patch.StartDate != null)
                {
                    var newValue = string.IsNullOrWhiteSpace(patch.StartDate)
                        ? null
                        : patch.StartDate;
                    if (newValue != null)
                    {
                        var newStart = DateOnly.Parse(newValue);
                        if (newStart != exp.StartDate)
                        {
                            exp.StartDate = newStart;
                            anyChange = true;
                        }
                    }
                }

                if (patch.EndDate != null)
                {
                    var newValue = string.IsNullOrWhiteSpace(patch.EndDate) ? null : patch.EndDate;
                    if (newValue != null)
                    {
                        var newEnd = DateOnly.Parse(newValue);
                        if (newEnd != exp.EndDate)
                        {
                            exp.EndDate = newEnd;
                            anyChange = true;
                        }
                    }
                }

                if (patch.Responsibilities != null && patch.Responsibilities.Count != 0)
                {
                    foreach (var respPatch in patch.Responsibilities)
                    {
                        var respEntity = responsibilities.FirstOrDefault(r =>
                            r.Id == respPatch.Id && r.ExperienceId == exp.Id
                        );
                        if (
                            respEntity != null
                            && respPatch.Responsibility != null
                            && respPatch.Responsibility != respEntity.Responsibility
                        )
                        {
                            respEntity.Responsibility = respPatch.Responsibility;
                            anyChange = true;
                        }
                    }
                }
            }

            if (!anyChange)
                return false;

            var saved = await _dbContext.SaveChangesAsync();
            return saved > 0;
        }

        public async Task<bool> DeleteExperiencesAsync(Guid userId, List<Guid> experienceIds)
        {
            if (experienceIds.IsNullOrEmpty())
                return false;

            var experiencesToDelete = await _dbContext
                .Experience.Where(exp => exp.UserId == userId && experienceIds.Contains(exp.Id))
                .ToListAsync();

            if (experiencesToDelete.Count != experienceIds.Count)
                return false;

            _dbContext.Experience.RemoveRange(experiencesToDelete);

            var saved = await _dbContext.SaveChangesAsync();
            return saved > 0;
        }
    }
}

using Microsoft.EntityFrameworkCore;
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

        public async Task<List<ExperienceModel>> GetAllExperiencesByIdsAsync(List<Guid> ids)
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

        public async Task<ExperienceWithResponsibilitiesModel> GetExperienceByIdAsync(
            Guid id,
            Guid userId
        )
        {
            var experience =
                await _dbContext
                    .Experience.Where(e => e.Id == id && e.UserId == userId)
                    .Include(e => e.Responsibilities)
                    .Select(edu => new ExperienceWithResponsibilitiesModel
                    {
                        Id = edu.Id,
                        CompanyName = edu.CompanyName,
                        JobTitle = edu.JobTitle,
                        StartDate = edu.StartDate.ToString("dd MMMM yyyy"),
                        EndDate = edu.EndDate.ToString("dd MMMM yyyy"),
                        Responsibilities = edu
                            .Responsibilities.Select(r => new Responsibilities
                            {
                                Id = r.Id,
                                Responsibility = r.Responsibility,
                            })
                            .ToList(),
                    })
                    .FirstOrDefaultAsync() ?? throw new NotFoundException("Experience not found.");

            return experience;
        }

        public async Task<List<ExperienceItem>> GetAllExperiencesByIdsAsync(ItemListRequest request)
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

        public async Task<bool> PatchExperienceAsync(Guid userId, PatchExperience patch)
        {
            if (patch == null)
                return false;

            var exp = await _dbContext.Experience.FirstOrDefaultAsync(e =>
                e.UserId == userId && e.Id == patch.Id
            );

            if (exp == null)
                return false;

            var anyChange = false;

            var responsibilities = await _dbContext
                .ExperienceResponsibility.Where(r => r.ExperienceId == exp.Id)
                .ToListAsync();

            if (patch.JobTitle != null && patch.JobTitle != exp.JobTitle)
            {
                exp.JobTitle = patch.JobTitle;
                anyChange = true;
            }

            if (patch.CompanyName != exp.CompanyName)
            {
                exp.CompanyName = patch.CompanyName;
                anyChange = true;
            }

            if (patch.StartDate != null)
            {
                var newValue = string.IsNullOrWhiteSpace(patch.StartDate) ? null : patch.StartDate;
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

            if (!anyChange)
                return false;

            var saved = await _dbContext.SaveChangesAsync();
            return saved > 0;
        }

        public async Task<bool> DeleteExperiencesAsync(Guid userId, List<Guid> experienceIds)
        {
            if (experienceIds.Count == 0)
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

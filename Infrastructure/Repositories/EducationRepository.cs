using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Education;
using Portfolio.Core.Entities;
using Portfolio.Core.Models;
using Portfolio.Infrastructure.Persistence;

namespace Portfolio.Infrastructure.Repositories
{
    public class EducationRepository : IEducationRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public EducationRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Guid>> AddEducationsAsync(List<AddEducation> educations)
        {
            var entities = educations
                .Select(edu => new Education
                {
                    InstitutionName = edu.InstitutionName,
                    Qualification = edu.Qualification,
                    StartDate = DateOnly.Parse(edu.StartDate),
                    EndDate = DateOnly.Parse(edu.EndDate),
                    Major = edu.Major,
                    Achievement = edu.Achievement,
                    UserId = edu.UserId,
                })
                .ToList();

            await _dbContext.Education.AddRangeAsync(entities);
            await _dbContext.SaveChangesAsync();

            return entities.Select(e => e.Id).ToList();
        }

        public async Task<bool> DeleteEducationsAsync(Guid userId, List<Guid> educationIds)
        {
            if (educationIds.IsNullOrEmpty())
                return false;

            var educationsToDelete = await _dbContext
                .Education.Where(edu => edu.UserId == userId && educationIds.Contains(edu.Id))
                .ToListAsync();

            if (educationsToDelete.Count != educationIds.Count)
                return false;

            _dbContext.Education.RemoveRange(educationsToDelete);

            var saved = await _dbContext.SaveChangesAsync();
            return saved > 0;
        }

        public async Task<List<EducationItem>> GetAllEducationsByIds(ItemListRequest request)
        {
            var ids = request.Ids;
            if (ids.Count == 0)
                return new List<EducationItem>();

            var educations = await _dbContext
                .Education.Where(edu => ids.Contains(edu.Id))
                .Select(ed => new
                {
                    ed.Id,
                    ed.InstitutionName,
                    ed.Qualification,
                    ed.Major,
                    ed.StartDate,
                    ed.EndDate,
                })
                .ToListAsync();

            switch (request.Order)
            {
                case SortOrder.Descending:
                    educations = educations.OrderByDescending(e => e.EndDate).ToList();
                    break;
                case SortOrder.Ascending:
                    educations = educations.OrderBy(e => e.EndDate).ToList();
                    break;
                case SortOrder.None:
                default:
                    var order = ids.Select((id, idx) => new { id, idx })
                        .ToDictionary(x => x.id, x => x.idx);
                    educations = educations
                        .OrderBy(e => order.TryGetValue(e.Id, out var idx) ? idx : int.MaxValue)
                        .ToList();
                    break;
            }

            return educations
                .Select(ed => new EducationItem
                {
                    Institution = ed.InstitutionName,
                    Qualification = ed.Qualification,
                    Major = ed.Major,
                    StartDate = ed.StartDate.ToString("MMMM yyyy"),
                    EndDate = ed.EndDate.ToString("MMMM yyyy"),
                })
                .ToList();
        }

        public async Task<List<EducationModel>> GetEducationsByUserIdAsync(Guid userId)
        {
            return await _dbContext
                .Education.Where(edu => edu.UserId == userId)
                .Select(edu => new EducationModel
                {
                    InstitutionName = edu.InstitutionName,
                    Qualification = edu.Qualification,
                    StartDate = edu.StartDate,
                    EndDate = edu.EndDate,
                    Major = edu.Major,
                    Achievement = edu.Achievement,
                })
                .ToListAsync();
        }

        public async Task<bool> PatchEducationsAsync(Guid userId, List<PatchEducation> patches)
        {
            if (patches == null || patches.Count == 0)
                return false;

            var ids = patches.Select(p => p.Id).Distinct().ToList();

            var educations = await _dbContext
                .Set<Education>()
                .Where(e => e.UserId == userId && ids.Contains(e.Id))
                .ToListAsync();

            if (educations.Count == 0)
                return false;

            var patchMap = patches.ToDictionary(p => p.Id, p => p);
            var anyChange = false;

            foreach (var edu in educations)
            {
                var patch = patchMap[edu.Id];

                if (patch.InstitutionName != null && patch.InstitutionName != edu.InstitutionName)
                {
                    edu.InstitutionName = patch.InstitutionName;
                    anyChange = true;
                }

                if (patch.Qualification != null && patch.Qualification != edu.Qualification)
                {
                    edu.Qualification = patch.Qualification;
                    anyChange = true;
                }

                if (patch.StartDate != null)
                {
                    var newValue = string.IsNullOrWhiteSpace(patch.StartDate)
                        ? null
                        : patch.StartDate;
                    DateOnly? newStartDate = !string.IsNullOrWhiteSpace(newValue)
                        ? DateOnly.Parse(newValue)
                        : null;
                    if (newStartDate != edu.StartDate)
                    {
                        edu.StartDate = newStartDate ?? edu.StartDate;
                        anyChange = true;
                    }
                }

                if (patch.EndDate != null)
                {
                    var newValue = string.IsNullOrWhiteSpace(patch.EndDate) ? null : patch.EndDate;
                    DateOnly? newEndDate = !string.IsNullOrWhiteSpace(newValue)
                        ? DateOnly.Parse(newValue)
                        : null;
                    if (newEndDate != edu.EndDate)
                    {
                        edu.EndDate = newEndDate ?? edu.EndDate;
                        anyChange = true;
                    }
                }

                if (patch.Major != null)
                {
                    var newValue = string.IsNullOrWhiteSpace(patch.Major) ? null : patch.Major;
                    if (newValue != edu.Major)
                    {
                        edu.Major = newValue;
                        anyChange = true;
                    }
                }

                if (patch.Achievement != null)
                {
                    var newValue = string.IsNullOrWhiteSpace(patch.Achievement)
                        ? null
                        : patch.Achievement;
                    if (newValue != edu.Achievement)
                    {
                        edu.Achievement = newValue;
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

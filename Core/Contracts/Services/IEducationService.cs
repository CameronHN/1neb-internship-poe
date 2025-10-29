using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Education;
using Portfolio.Core.Models;

namespace Portfolio.Core.Contracts.Services
{
    public interface IEducationService
    {
        Task<List<Guid>> AddEducationsAsync(Guid userId, List<AddEducation> educations);

        Task<List<EducationItem>> GetAllEducationsByIds(ItemListRequest request);
        Task<List<EducationModel>> GetEducationsByUserIdAsync(Guid userId);

        Task<bool> PatchEducationsAsync(Guid userId, List<PatchEducation> patches);

        Task<bool> DeleteEducationsAsync(Guid userId, List<Guid> educationIds);
    }
}

using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Education;
using Portfolio.Core.DTOs.Resume;
using Portfolio.Core.Models;

namespace Portfolio.Core.Contracts.Repositories
{
    public interface IEducationRepository
    {
        Task<List<Guid>> AddEducationsAsync(Guid userId, List<AddEducation> educations);

        Task<List<EducationItem>> GetAllEducationsByIds(ItemListRequest request);
        Task<List<EducationModel>> GetEducationsByUserIdAsync(Guid userId);
        Task<EducationModel> GetEducationById(Guid id);

        Task<bool> PatchEducationsAsync(Guid userId, List<PatchEducation> patches);

        Task<bool> DeleteEducationsAsync(Guid userId, List<Guid> educationIds);
    }
}

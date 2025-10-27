using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Education;

namespace Portfolio.Core.Contracts.Repositories
{
    public interface IEducationRepository
    {
        Task<List<Guid>> AddEducationsAsync(List<AddEducation> educations);

        Task<List<EducationItem>> GetAllEducationsByIds(ItemListRequest request);

        Task<bool> PatchEducationsAsync(Guid userId, List<PatchEducation> patches);
    }
}

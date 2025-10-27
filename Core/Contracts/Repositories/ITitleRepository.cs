using Portfolio.Core.DTOs.ResumeTitle;

namespace Portfolio.Core.Contracts.Repositories
{
    public interface ITitleRepository
    {
        Task<Guid> AddTitleAsync(AddResumeTitle title);
        Task<string?> GetTitleById(Guid id);

        Task<bool> PatchTitlesAsync(Guid userId, List<PatchResumeTitle> patches);

        Task<bool> DeleteTitlesAsync(Guid userId, List<Guid> titleIds);
    }
}

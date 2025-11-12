using Portfolio.Core.DTOs.ResumeTitle;

namespace Portfolio.Core.Contracts.Services
{
    public interface ITitleService
    {
        Task<List<Guid>> AddTitlesAsync(Guid userId, List<AddResumeTitle> titles);

        Task<string?> GetTitleById(Guid id);

        Task<string> GetResumeTitleById(Guid id);

        Task<List<string>> GetTitlesByUserIdAsync(Guid userId);

        Task<bool> PatchTitlesAsync(Guid userId, List<PatchResumeTitle> patches);

        Task<bool> DeleteTitlesAsync(Guid userId, List<Guid> titleIds);
    }
}

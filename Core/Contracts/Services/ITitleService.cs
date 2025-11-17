using Portfolio.Core.DTOs.ResumeTitle;

namespace Portfolio.Core.Contracts.Services
{
    public interface ITitleService
    {
        Task<List<Guid>> AddTitlesAsync(Guid userId, List<AddResumeTitle> titles);

        Task<string?> GetTitleById(Guid id);

        Task<string> GetResumeTitleById(Guid id, Guid userId);

        Task<List<string>> GetTitlesByUserIdAsync(Guid userId);

        // Update
        Task<bool> PatchTitleAsync(Guid userId, PatchResumeTitle patch);

        Task<bool> DeleteTitlesAsync(Guid userId, List<Guid> titleIds);
    }
}

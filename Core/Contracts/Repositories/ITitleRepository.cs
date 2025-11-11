using Portfolio.Core.DTOs.ResumeTitle;

namespace Portfolio.Core.Contracts.Repositories
{
    public interface ITitleRepository
    {
        // Add
        Task<Guid> AddTitleAsync(Guid userId, AddResumeTitle title);

        // Get
        Task<string?> GetTitleById(Guid id); // Used for resume service
        Task<List<string>> GetTitlesByUserIdAsync(Guid userId);
        Task<string> GetResumeTitleById(Guid id); // Throws NotFound exception

        // Patch
        Task<bool> PatchTitlesAsync(Guid userId, List<PatchResumeTitle> patches);

        // Delete
        Task<bool> DeleteTitlesAsync(Guid userId, List<Guid> titleIds);
    }
}

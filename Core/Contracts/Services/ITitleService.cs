using Portfolio.Core.DTOs.ResumeTitle;

namespace Portfolio.Core.Contracts.Services
{
    public interface ITitleService
    {
        Task<Guid> AddTitleAsync(AddResumeTitle title);
        Task<string?> GetTitleById(Guid id);
    }
}

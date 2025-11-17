using Portfolio.Core.DTOs.ProfessionalSummary;

namespace Portfolio.Core.Contracts.Repositories
{
    public interface IProfessionalSummaryRepository
    {
        Task<List<Guid>> AddSummariesAsync(Guid userId, List<AddSummary> summaries);
        Task<string?> GetSummaryById(Guid id);
        Task<string> GetProfessionalSummaryById(Guid id, Guid userId);

        Task<List<string>> GetSummariesByUserId(Guid userId);

        Task<bool> PatchSummaryAsync(Guid userId, PatchSummary patch);

        Task<bool> DeleteProfessionalSummariesAsync(Guid userId, List<Guid> summaryIds);
    }
}

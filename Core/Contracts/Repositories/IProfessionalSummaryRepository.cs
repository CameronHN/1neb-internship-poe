using Portfolio.Core.DTOs.ProfessionalSummary;

namespace Portfolio.Core.Contracts.Repositories
{
    public interface IProfessionalSummaryRepository
    {
        Task<List<Guid>> AddSummariesAsync(Guid userId, List<AddSummary> summaries);
        Task<string?> GetSummaryById(Guid id);
        Task<string> GetProfessionalSummaryById(Guid id);

        Task<List<string>> GetSummariesByUserId(Guid userId);

        Task<bool> PatchSummariesAsync(Guid userId, List<PatchSummary> patches);

        Task<bool> DeleteProfessionalSummariesAsync(Guid userId, List<Guid> summaryIds);
    }
}

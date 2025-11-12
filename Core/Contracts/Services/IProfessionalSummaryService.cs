using Portfolio.Core.DTOs.ProfessionalSummary;

namespace Portfolio.Core.Contracts.Services
{
    public interface IProfessionalSummaryService
    {
        Task<List<Guid>> AddSummariesAsync(Guid userId, List<AddSummary> summaries);

        Task<List<string>> GetSummariesByUserId(Guid userId);

        Task<string> GetProfessionalSummaryById(Guid id);

        Task<bool> PatchSummariesAsync(Guid userId, List<PatchSummary> patches);

        Task<bool> DeleteProfessionalSummariesAsync(Guid userId, List<Guid> summaryIds);
    }
}

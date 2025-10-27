using Portfolio.Core.DTOs.ProfessionalSummary;

namespace Portfolio.Core.Contracts.Repositories
{
    public interface IProfessionalSummaryRepository
    {
        Task<Guid> AddSummaryAsync(AddSummary summary);
        Task<string?> GetSummaryById(Guid id);
        Task<List<string>> GetSummariesByUserId(Guid userId);


        Task<bool> PatchSummariesAsync(Guid userId, List<PatchSummary> patches);

        Task<bool> DeleteProfessionalSummariesAsync(Guid userId, List<Guid> summaryIds);
    }
}

using Portfolio.Core.DTOs.ProfessionalSummary;

namespace Portfolio.Core.Contracts.Services
{
    public interface IProfessionalSummaryService
    {
        Task<Guid> AddSummaryAsync(AddSummary summary);

        Task<bool> PatchSummariesAsync(Guid userId, List<PatchSummary> patches);

        Task<bool> DeleteProfessionalSummariesAsync(Guid userId, List<Guid> summaryIds);
    }
}

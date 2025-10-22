using Portfolio.Core.DTOs.ProfessionalSummary;

namespace Portfolio.Core.Contracts.Repositories
{
    public interface IProfessionalSummaryRepository
    {
        Task AddSummariesAsync(List<AddSummary> summaries);
    }
}

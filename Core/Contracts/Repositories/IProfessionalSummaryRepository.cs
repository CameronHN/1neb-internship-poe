using Portfolio.Core.DTOs.ProfessionalSummary;

namespace Portfolio.Core.Contracts.Repositories
{
    public interface IProfessionalSummaryRepository
    {
        Task<Guid> AddSummariesAsync(AddSummary summary);
    }
}

using Portfolio.Core.DTOs.ProfessionalSummary;

namespace Portfolio.Core.Contracts.Services
{
    public interface IProfessionalSummaryService
    {
        Task<Guid> AddSummaryAsync(AddSummary summary);
    }
}

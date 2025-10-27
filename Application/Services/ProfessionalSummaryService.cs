using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs.ProfessionalSummary;

namespace Portfolio.Application.Services
{
    public class ProfessionalSummaryService : IProfessionalSummaryService
    {
        private readonly IProfessionalSummaryRepository _professionalSummaryRepository;

        public ProfessionalSummaryService(
            IProfessionalSummaryRepository professionalSummaryRepository
        )
        {
            _professionalSummaryRepository = professionalSummaryRepository;
        }

        public async Task<Guid> AddSummaryAsync(AddSummary summary)
        {
            return await _professionalSummaryRepository.AddSummaryAsync(summary);
        }

        public Task<bool> PatchSummariesAsync(Guid userId, List<PatchSummary> patches)
        {
            return _professionalSummaryRepository.PatchSummariesAsync(userId, patches);
        }
    }
}

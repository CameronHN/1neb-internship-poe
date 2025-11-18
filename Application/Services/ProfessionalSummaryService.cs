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

        public async Task<List<Guid>> AddSummariesAsync(Guid userId, List<AddSummary> summaries)
        {
            return await _professionalSummaryRepository.AddSummariesAsync(userId, summaries);
        }

        public async Task<bool> DeleteProfessionalSummariesAsync(Guid userId, List<Guid> summaryIds)
        {
            return await _professionalSummaryRepository.DeleteProfessionalSummariesAsync(
                userId,
                summaryIds
            );
        }

        public async Task<string> GetProfessionalSummaryByIdAsync(Guid id, Guid userId)
        {
            return await _professionalSummaryRepository.GetProfessionalSummaryByIdAsync(id, userId);
        }

        public async Task<List<string>> GetSummariesByUserIdAsync(Guid userId)
        {
            return await _professionalSummaryRepository.GetSummariesByUserIdAsync(userId);
        }

        public async Task<bool> PatchSummaryAsync(Guid userId, PatchSummary patch)
        {
            return await _professionalSummaryRepository.PatchSummaryAsync(userId, patch);
        }
    }
}

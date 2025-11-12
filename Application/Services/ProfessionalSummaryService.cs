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

        public async Task<string> GetProfessionalSummaryById(Guid id)
        {
            return await _professionalSummaryRepository.GetProfessionalSummaryById(id);
        }

        public async Task<List<string>> GetSummariesByUserId(Guid userId)
        {
            return await _professionalSummaryRepository.GetSummariesByUserId(userId);
        }

        public async Task<bool> PatchSummariesAsync(Guid userId, List<PatchSummary> patches)
        {
            return await _professionalSummaryRepository.PatchSummariesAsync(userId, patches);
        }
    }
}

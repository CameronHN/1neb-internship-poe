using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs.ResumeTitle;

namespace Portfolio.Application.Services
{
    public class TitleService : ITitleService
    {
        private readonly ITitleRepository _titleRepository;

        public TitleService(ITitleRepository titleRepository)
        {
            _titleRepository = titleRepository;
        }

        public async Task<List<Guid>> AddTitlesAsync(Guid userId, List<AddResumeTitle> titles)
        {
            return await _titleRepository.AddTitlesAsync(userId, titles);
        }

        public async Task<bool> DeleteTitlesAsync(Guid userId, List<Guid> titleIds)
        {
            return await _titleRepository.DeleteTitlesAsync(userId, titleIds);
        }

        public async Task<string> GetResumeTitleById(Guid id, Guid userId)
        {
            return await _titleRepository.GetResumeTitleById(id, userId);
        }

        public async Task<string?> GetTitleById(Guid id)
        {
            return await _titleRepository.GetTitleById(id);
        }

        public async Task<List<string>> GetTitlesByUserIdAsync(Guid userId)
        {
            return await _titleRepository.GetTitlesByUserIdAsync(userId);
        }

        public Task<bool> PatchTitleAsync(Guid userId, PatchResumeTitle patch)
        {
            return _titleRepository.PatchTitleAsync(userId, patch);
        }
    }
}

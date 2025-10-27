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

        public async Task<Guid> AddTitleAsync(AddResumeTitle title)
        {
            return await _titleRepository.AddTitleAsync(title);
        }

        public async Task<bool> DeleteTitlesAsync(Guid userId, List<Guid> titleIds)
        {
            return await _titleRepository.DeleteTitlesAsync(userId, titleIds);
        }

        public async Task<string?> GetTitleById(Guid id)
        {
            return await _titleRepository.GetTitleById(id);
        }

        public async Task<bool> PatchTitlesAsync(Guid userId, List<PatchResumeTitle> patches)
        {
            return await _titleRepository.PatchTitlesAsync(userId, patches);
        }
    }
}

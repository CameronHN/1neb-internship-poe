using Portfolio.Application.Documents;
using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs;
using QuestPDF.Fluent;

namespace Portfolio.Application.Services
{
    public class ResumeService : IResumeService
    {
        private readonly IUserRepository _userRepository;

        public ResumeService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ResumeDto> GetResumeByUserId(Guid userId)
        {
            return await _userRepository.GetResumeDtoByUserId(userId);
        }

        public Task<byte[]> RenderPdfAsync(ResumeDto dto)
        {
            var pdf = new ResumeDocument(dto ?? new()).GeneratePdf();
            return Task.FromResult(pdf);
        }
    }
}

using Portfolio.Application.Documents;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs.Resume;
using QuestPDF.Fluent;

namespace Portfolio.Application.Services
{
    public class ResumeGenerationService : IResumeGenerationService
    {
        public async Task<byte[]> GenerateResumePdfAsync(ResumeDto dto)
        {
            var document = new ResumePdfGenerator(dto ?? new());
            return await Task.FromResult(document.GeneratePdf());
        }
    }
}

using System.Text.Json;
using Portfolio.Application.Documents;
using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs.Resume;
using Portfolio.Core.DTOs.SavedResume;
using Portfolio.Core.Entities;
using Portfolio.Core.Exceptions;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Portfolio.Application.Services
{
    public class SavedResumeService : ISavedResumeService
    {
        private readonly ISavedResumeRepository _savedResumeRepository;

        public SavedResumeService(ISavedResumeRepository savedResumeRepository)
        {
            _savedResumeRepository = savedResumeRepository;
        }

        public async Task<Guid> SaveResumeAsync(Guid userId, SaveResumeDataRequest request)
        {
            var savedResume = new SavedResume
            {
                Name = request.SavedResumeName,
                Data = JsonSerializer.Serialize(request.ResumeData),
                TemplateType = request.TemplateType,
                UserId = userId,
            };

            return await _savedResumeRepository.CreateAsync(savedResume);
        }

        public async Task<SavedResumeDetail?> GetSavedResumeByIdAsync(Guid id, Guid userId)
        {
            var savedResume = await _savedResumeRepository.GetByIdAsync(id, userId);

            if (savedResume == null)
                return null;

            var resumeDto = JsonSerializer.Deserialize<ResumeDto>(savedResume.Data);

            if (resumeDto == null)
                throw new InvalidOperationException("Failed to deserialize resume data.");

            return new SavedResumeDetail
            {
                Id = savedResume.Id,
                Name = savedResume.Name,
                ResumeData = resumeDto,
                TemplateType = savedResume.TemplateType,
                CreatedAt = savedResume.CreatedAt,
            };
        }

        public async Task<List<SavedResumeListItem>> GetAllSavedResumesByUserIdAsync(Guid userId)
        {
            var savedResumes = await _savedResumeRepository.GetAllByUserIdAsync(userId);

            return savedResumes
                .Select(sr => new SavedResumeListItem
                {
                    Id = sr.Id,
                    Name = sr.Name,
                    TemplateType = sr.TemplateType,
                    CreatedAt = sr.CreatedAt,
                })
                .ToList();
        }

        public async Task<bool> DeleteSavedResumeAsync(Guid id, Guid userId)
        {
            return await _savedResumeRepository.DeleteAsync(id, userId);
        }

        public async Task<byte[]> GeneratePdfFromSavedResumeAsync(Guid id, Guid userId)
        {
            var savedResume = await _savedResumeRepository.GetByIdAsync(id, userId);

            if (savedResume == null)
                throw new NotFoundException("Saved resume not found.");

            var resumeDto = JsonSerializer.Deserialize<ResumeDto>(savedResume.Data);

            if (resumeDto == null)
                throw new InvalidOperationException("Failed to deserialize resume data.");

            var document = GetBuilderForTemplate(savedResume.TemplateType, resumeDto);

            return document.GeneratePdf();
        }

        private IDocument GetBuilderForTemplate(string templateType, ResumeDto resumeDto)
        {
            return templateType.ToLowerInvariant() switch
            {
                "classic" or "resumebuilder" => new ResumeBuilder(resumeDto),
                // Add more templates here
                _ => throw new ArgumentException($"Unknown template type: {templateType}"),
            };
        }
    }
}

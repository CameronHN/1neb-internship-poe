using System.Reflection;
using System.Text;
using System.Text.Json;
using NSubstitute;
using Portfolio.Application.Documents;
using Portfolio.Application.Services;
using Portfolio.Core.Constants;
using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.DTOs.Resume;
using Portfolio.Core.DTOs.SavedResume;
using Portfolio.Core.Exceptions;
using Portfolio.Core.Models;
using QuestPDF.Infrastructure;
using Xunit;

namespace Portfolio.Tests.UnitTests
{
    public class SavedResumeServiceTests
    {
        private readonly ISavedResumeRepository _repository = Substitute.For<ISavedResumeRepository>();
        private readonly SavedResumeService _service;

        public SavedResumeServiceTests()
        {
            // Program.cs sets this at startup. Unit tests do not run Program.cs, so set it here.
            QuestPDF.Settings.License = LicenseType.Community;
            _service = new SavedResumeService(_repository);
        }

        /// <summary>
        /// Every public constant in TemplateTypes (found by reflection, so new templates are
        /// picked up automatically).
        /// </summary>
        public static TheoryData<string> AllTemplateTypes()
        {
            var data = new TheoryData<string>();
            var constants = typeof(TemplateTypes)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(field => field.IsLiteral && field.FieldType == typeof(string));

            foreach (var field in constants)
            {
                data.Add((string)field.GetRawConstantValue()!);
            }

            return data;
        }

        [Fact]
        public void TemplateTypes_HasAtLeastOneConstant()
        {
            Assert.NotEmpty(AllTemplateTypes());
        }

        [Theory]
        [MemberData(nameof(AllTemplateTypes))]
        public async Task SaveResumeAsync_AcceptsEveryTemplateType(string templateType)
        {
            var userId = Guid.NewGuid();
            var savedId = Guid.NewGuid();
            _repository.CreateAsync(userId, Arg.Any<AddSavedResume>()).Returns(savedId);

            var result = await _service.SaveResumeAsync(userId, CreateSaveRequest(templateType));

            Assert.Equal(savedId, result);
            await _repository
                .Received(1)
                .CreateAsync(userId, Arg.Is<AddSavedResume>(r => r.TemplateType == templateType));
        }

        [Theory]
        [MemberData(nameof(AllTemplateTypes))]
        public async Task GetSavedResumePdfFromIdAsync_RendersEveryTemplateType(string templateType)
        {
            var userId = Guid.NewGuid();
            var resumeId = Guid.NewGuid();
            _repository.GetByIdAsync(resumeId, userId).Returns(CreateSavedModel(resumeId, templateType));

            var pdf = await _service.GetSavedResumePdfFromIdAsync(resumeId, userId);

            Assert.True(pdf.Length > 4, "The PDF should not be empty.");
            Assert.Equal("%PDF", Encoding.ASCII.GetString(pdf, 0, 4));
        }

        [Fact]
        public async Task SaveResumeAsync_UnknownTemplate_ThrowsAndDoesNotSave()
        {
            var userId = Guid.NewGuid();

            await Assert.ThrowsAsync<TemplateTypeNotImplementedException>(() =>
                _service.SaveResumeAsync(userId, CreateSaveRequest("does-not-exist"))
            );

            await _repository.DidNotReceive().CreateAsync(Arg.Any<Guid>(), Arg.Any<AddSavedResume>());
        }

        [Fact]
        public async Task SaveResumeAsync_AtTheLimitOf50_ThrowsAndDoesNotSave()
        {
            var userId = Guid.NewGuid();
            _repository.CountByUserIdAsync(userId).Returns(Constants.MaxSavedResumesPerUser);

            var exception = await Assert.ThrowsAsync<BusinessRuleViolationException>(() =>
                _service.SaveResumeAsync(userId, CreateSaveRequest(TemplateTypes.Classic))
            );

            Assert.Equal(50, Constants.MaxSavedResumesPerUser);
            Assert.Contains("50", exception.Message);
            await _repository.DidNotReceive().CreateAsync(Arg.Any<Guid>(), Arg.Any<AddSavedResume>());
        }

        [Fact]
        public async Task SaveResumeAsync_With49Saved_SavesThe50th()
        {
            var userId = Guid.NewGuid();
            _repository.CountByUserIdAsync(userId).Returns(Constants.MaxSavedResumesPerUser - 1);

            await _service.SaveResumeAsync(userId, CreateSaveRequest(TemplateTypes.Classic));

            await _repository.Received(1).CreateAsync(userId, Arg.Any<AddSavedResume>());
        }

        [Fact]
        public async Task GetSavedResumePdfFromIdAsync_UnknownStoredTemplate_Throws()
        {
            var userId = Guid.NewGuid();
            var resumeId = Guid.NewGuid();
            _repository
                .GetByIdAsync(resumeId, userId)
                .Returns(CreateSavedModel(resumeId, "does-not-exist"));

            await Assert.ThrowsAsync<TemplateTypeNotImplementedException>(() =>
                _service.GetSavedResumePdfFromIdAsync(resumeId, userId)
            );
        }

        [Fact]
        public async Task GetSavedResumePdfFromIdAsync_MissingResume_ThrowsNotFound()
        {
            var userId = Guid.NewGuid();
            var resumeId = Guid.NewGuid();
            _repository.GetByIdAsync(resumeId, userId).Returns((SavedResumeModel?)null);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.GetSavedResumePdfFromIdAsync(resumeId, userId)
            );
        }

        private static SaveResumeDataRequest CreateSaveRequest(string templateType)
        {
            return new SaveResumeDataRequest
            {
                SavedResumeName = "Unit test resume",
                ResumeData = new ResumeDTO { Name = "Test User" },
                TemplateType = templateType,
            };
        }

        private static SavedResumeModel CreateSavedModel(Guid id, string templateType)
        {
            return new SavedResumeModel
            {
                Id = id,
                Name = "Unit test resume",
                Data = JsonSerializer.Serialize(new ResumeDTO { Name = "Test User" }),
                TemplateType = templateType,
                CreatedAt = DateTime.UtcNow.ToString("O"),
            };
        }
    }
}

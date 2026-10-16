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
using Portfolio.Tests.Common;
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
        public void TemplateTypes_ConstantsClass_HasAtLeastOneConstant()
        {
            Assert.NotEmpty(AllTemplateTypes());
        }

        [Theory]
        [MemberData(nameof(AllTemplateTypes))]
        public async Task SaveResumeAsync_EachKnownTemplateType_Saves(string templateType)
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
        public async Task GetSavedResumePdfFromIdAsync_EachKnownTemplateType_RendersPdf(string templateType)
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

        [Fact]
        public async Task SaveResumeAsync_WithSectionOrder_StoresItCleanedInTheJson()
        {
            var userId = Guid.NewGuid();
            AddSavedResume? stored = null;
            _repository
                .CreateAsync(userId, Arg.Do<AddSavedResume>(r => stored = r))
                .Returns(Guid.NewGuid());
            var request = CreateSaveRequest(TemplateTypes.Classic);
            request.ResumeData.SectionOrder = [" Certifications", "SKILLS", "skills", "bogus"];

            await _service.SaveResumeAsync(userId, request);

            Assert.NotNull(stored);
            // The stored property name is "SectionOrder": snapshots rely on it staying stable.
            using var json = JsonDocument.Parse(stored.Data);
            var order = json.RootElement.GetProperty("SectionOrder").EnumerateArray();
            Assert.Equal(["certifications", "skills"], order.Select(e => e.GetString()));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task SaveResumeAsync_NullOrEmptySectionOrder_StoresNull(bool empty)
        {
            var userId = Guid.NewGuid();
            AddSavedResume? stored = null;
            _repository
                .CreateAsync(userId, Arg.Do<AddSavedResume>(r => stored = r))
                .Returns(Guid.NewGuid());
            var request = CreateSaveRequest(TemplateTypes.Classic);
            request.ResumeData.SectionOrder = empty ? [] : null;

            await _service.SaveResumeAsync(userId, request);

            Assert.Null(JsonSerializer.Deserialize<ResumeDTO>(stored!.Data)!.SectionOrder);
        }

        [Fact]
        public async Task GetSavedResumeByIdAsync_StoredSectionOrder_IsReturned()
        {
            var userId = Guid.NewGuid();
            var resumeId = Guid.NewGuid();
            var data = JsonSerializer.Serialize(
                new ResumeDTO { Name = "Test User", SectionOrder = ["education", "skills"] }
            );
            _repository
                .GetByIdAsync(resumeId, userId)
                .Returns(CreateSavedModel(resumeId, TemplateTypes.Classic, data));

            var detail = await _service.GetSavedResumeByIdAsync(resumeId, userId);

            Assert.Equal(["education", "skills"], detail!.ResumeData.SectionOrder);
        }

        /// <summary>
        /// A snapshot saved before section ordering existed has no SectionOrder property at all.
        /// </summary>
        [Fact]
        public async Task GetSavedResumeByIdAsync_LegacySnapshotWithoutSectionOrder_LoadsAsNull()
        {
            var userId = Guid.NewGuid();
            var resumeId = Guid.NewGuid();
            var legacy = "{\"Name\":\"Old User\",\"Skills\":[{\"Skill\":\"C#\",\"SkillLevel\":null}]}";
            _repository
                .GetByIdAsync(resumeId, userId)
                .Returns(CreateSavedModel(resumeId, TemplateTypes.Classic, legacy));

            var detail = await _service.GetSavedResumeByIdAsync(resumeId, userId);

            Assert.Null(detail!.ResumeData.SectionOrder);
            Assert.Equal("Old User", detail.ResumeData.Name);
        }

        [Theory]
        [MemberData(nameof(AllTemplateTypes))]
        public async Task GetSavedResumePdfFromIdAsync_StoredSectionOrder_RendersInThatOrder(string templateType)
        {
            var userId = Guid.NewGuid();
            var resumeId = Guid.NewGuid();
            var data = JsonSerializer.Serialize(SectionedResume(["certifications", "education"]));
            _repository
                .GetByIdAsync(resumeId, userId)
                .Returns(CreateSavedModel(resumeId, templateType, data));

            var pdf = await _service.GetSavedResumePdfFromIdAsync(resumeId, userId);

            // Listed sections first, then the rest in the template's default order.
            var expected = templateType == TemplateTypes.Classic
                ? new[] { "CERTIFICATIONS", "EDUCATION", "EXPERIENCE", "SKILLS" }
                : new[] { "CERTIFICATIONS", "EDUCATION", "SKILLS", "EXPERIENCE" };
            Assert.Equal(expected, PdfText.HeadingsInOrder(pdf, SectionHeadings));
        }

        [Theory]
        [MemberData(nameof(AllTemplateTypes))]
        public async Task GetSavedResumePdfFromIdAsync_LegacySnapshotWithoutSectionOrder_UsesTheDefaultOrder(string templateType)
        {
            var userId = Guid.NewGuid();
            var resumeId = Guid.NewGuid();
            // Serialised without the SectionOrder key, as every snapshot saved before this feature.
            var legacy = JsonSerializer.Serialize(SectionedResume(null));
            var legacyWithoutKey = legacy.Replace(",\"SectionOrder\":null", string.Empty);
            Assert.DoesNotContain("SectionOrder", legacyWithoutKey);
            _repository
                .GetByIdAsync(resumeId, userId)
                .Returns(CreateSavedModel(resumeId, templateType, legacyWithoutKey));

            var pdf = await _service.GetSavedResumePdfFromIdAsync(resumeId, userId);

            var expected = templateType == TemplateTypes.Classic
                ? new[] { "EXPERIENCE", "SKILLS", "EDUCATION", "CERTIFICATIONS" }
                : new[] { "SKILLS", "EXPERIENCE", "EDUCATION", "CERTIFICATIONS" };
            Assert.Equal(expected, PdfText.HeadingsInOrder(pdf, SectionHeadings));
        }

        [Theory]
        [MemberData(nameof(AllTemplateTypes))]
        public async Task GetSavedResumePdfFromIdAsync_StoredOrderWithUnknownKeysAndDuplicates_RendersIgnoringThem(string templateType)
        {
            var userId = Guid.NewGuid();
            var resumeId = Guid.NewGuid();
            var data = JsonSerializer.Serialize(
                SectionedResume(["summary", "Certifications", "bogus", "certifications", "title"])
            );
            _repository
                .GetByIdAsync(resumeId, userId)
                .Returns(CreateSavedModel(resumeId, templateType, data));

            var pdf = await _service.GetSavedResumePdfFromIdAsync(resumeId, userId);

            var headings = PdfText.HeadingsInOrder(pdf, SectionHeadings);
            Assert.Equal(4, headings.Count);
            Assert.Equal("CERTIFICATIONS", headings[0]);
        }

        // Upper-case section titles as drawn; Simplified calls Experience "PROFESSIONAL EXPERIENCE",
        // which still contains "EXPERIENCE" once whitespace is stripped.
        private static readonly string[] SectionHeadings = ["SKILLS", "EXPERIENCE", "EDUCATION", "CERTIFICATIONS"];

        private static ResumeDTO SectionedResume(List<string>? order)
        {
            return new ResumeDTO
            {
                Name = "Test User",
                SectionOrder = order,
                Skills = [new SkillsItem { Skill = "CSharp" }],
                Experience = [new ExperienceItem { JobTitle = "Developer", Company = "Acme" }],
                Education = [new EducationItem { Institution = "Uni", Qualification = "BSc" }],
                Certification = [new CertificationItem { Name = "AZ-900" }],
            };
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

        private static SavedResumeModel CreateSavedModel(Guid id, string templateType, string? data = null)
        {
            return new SavedResumeModel
            {
                Id = id,
                Name = "Unit test resume",
                Data = data ?? JsonSerializer.Serialize(new ResumeDTO { Name = "Test User" }),
                TemplateType = templateType,
                CreatedAt = DateTime.UtcNow.ToString("O"),
            };
        }
    }
}

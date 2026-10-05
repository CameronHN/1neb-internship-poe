using System.Text.Json;
using Portfolio.Application.Documents;
using Portfolio.Core.DTOs.Resume;
using Portfolio.Core.Entities;
using Portfolio.Infrastructure.Persistence;

namespace Portfolio.Tests.IntegrationTests
{
    /// <summary>
    /// The ids of one user and one item of each type that belongs to that user.
    /// </summary>
    public sealed record UserWithItems(
        Guid UserId,
        string TitleText,
        Guid TitleId,
        string SummaryText,
        Guid SummaryId,
        Guid SkillId,
        Guid EducationId,
        Guid ExperienceId,
        Guid CertificationId,
        Guid LinkId,
        Guid SavedResumeId
    );

    public static class OwnershipTestData
    {
        /// <summary>
        /// Inserts a new user plus one title, summary, skill, education, experience,
        /// certification, professional link and saved resume owned by that user.
        /// Every call creates a brand-new user, so tests never share data.
        /// </summary>
        public static async Task<UserWithItems> CreateUserWithItemsAsync(
            ApplicationDbContext db,
            string label
        )
        {
            var unique = Guid.NewGuid().ToString("N")[..8];
            var email = $"{label.ToLowerInvariant()}-{unique}@test.local";

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                FirstName = label,
                LastName = "Test",
                UserName = email,
                Email = email,
            };

            var title = new Title { ResumeTitle = $"{label} title {unique}", UserId = user.Id };
            var summary = new ProfessionalSummary
            {
                Summary = $"{label} summary {unique}",
                UserId = user.Id,
            };
            var skill = new Skill { SkillName = $"{label} skill", UserId = user.Id };
            var education = new Education
            {
                InstitutionName = $"{label} university",
                Qualification = "BSc",
                StartDate = new DateOnly(2018, 1, 1),
                EndDate = new DateOnly(2021, 12, 31),
                UserId = user.Id,
            };
            var experience = new Experience
            {
                JobTitle = $"{label} developer",
                StartDate = new DateOnly(2022, 1, 1),
                EndDate = new DateOnly(2023, 12, 31),
                UserId = user.Id,
            };
            var certification = new Certification
            {
                CertificationName = $"{label} certification",
                UserId = user.Id,
            };
            var link = new ProfessionalLink
            {
                LinkType = "Github",
                Link = $"https://github.com/{label.ToLowerInvariant()}-{unique}",
                UserId = user.Id,
            };
            var savedResume = new SavedResume
            {
                Name = $"{label} saved resume",
                Data = JsonSerializer.Serialize(new ResumeDTO { Name = label }),
                TemplateType = TemplateTypes.Classic,
                UserId = user.Id,
            };

            db.User.Add(user);
            db.Title.Add(title);
            db.ProfessionalSummary.Add(summary);
            db.Skill.Add(skill);
            db.Education.Add(education);
            db.Experience.Add(experience);
            db.Certification.Add(certification);
            db.ProfessionalLink.Add(link);
            db.SavedResume.Add(savedResume);
            await db.SaveChangesAsync();

            return new UserWithItems(
                UserId: user.Id,
                TitleText: title.ResumeTitle,
                TitleId: title.Id,
                SummaryText: summary.Summary,
                SummaryId: summary.Id,
                SkillId: skill.Id,
                EducationId: education.Id,
                ExperienceId: experience.Id,
                CertificationId: certification.Id,
                LinkId: link.Id,
                SavedResumeId: savedResume.Id
            );
        }
    }
}

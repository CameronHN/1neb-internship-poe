using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.DTOs.Resume;
using Portfolio.Core.Entities;
using Portfolio.Infrastructure.Persistence;
using Portfolio.Tests.Common;
using Xunit;

namespace Portfolio.Tests.IntegrationTests
{
    /// <summary>
    /// Issue #33: GetAllResumeDetailsByUserIdAsync is being moved from Include graph loading to
    /// LINQ projections. These tests pin the payload (fields, ordering, date formats and empty
    /// collections) so the rewrite is proven to return exactly what it returned before.
    /// </summary>
    [Collection(ApiCollection.Name)]
    public class UserResumeDetailsTests
    {
        private readonly PortfolioApiFactory _factory;

        public UserResumeDetailsTests(PortfolioApiFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetAllResumeDetailsByUserIdAsync_ReturnsEveryFieldFormattedAndOrdered()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var user = await CreateBareUserAsync(db, "Full");
            var data = await AddFullDataAsync(db, user.Id);
            var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

            var result = await repository.GetAllResumeDetailsByUserIdAsync(user.Id);

            Assert.NotNull(result);
            Assert.Equal("Full Test", result.Name);
            Assert.Equal(user.Email, result.Email);
            Assert.Equal("0820000001", result.PhoneNumber);

            var title = Assert.Single(result.Title);
            Assert.Equal(data.TitleId, title.Id);
            Assert.Equal("Senior Developer", title.Title);

            var summary = Assert.Single(result.Summaries);
            Assert.Equal(data.SummaryId, summary.Id);
            Assert.Equal("Builds things.", summary.Summary);

            // Mixed case: culture-aware ordering, not ordinal (ordinal would put "alpha" last).
            Assert.Equal(
                ["https://A.example", "https://b.example", "https://c.example"],
                result.Socials.Select(s => s.SocialMediaUrl)
            );
            var firstSocial = result.Socials.First();
            Assert.Equal(data.LinkIds["https://A.example"], firstSocial.Id);
            Assert.Equal("Github", firstSocial.SocialMediaType);

            Assert.Equal(["alpha", "Bravo", "Charlie"], result.Skills.Select(s => s.Skill));
            var alpha = result.Skills.First();
            Assert.Equal(data.SkillIds["alpha"], alpha.Id);
            Assert.Equal("Expert", alpha.SkillLevel);
            Assert.Null(result.Skills.Last().SkillLevel);

            // Newest end date first.
            Assert.Equal(["Newest Co", "Oldest Co"], result.Experience.Select(e => e.Company));
            var newest = result.Experience.First();
            Assert.Equal(data.ExperienceIds["Newest Co"], newest.Id);
            Assert.Equal("Engineer", newest.JobTitle);
            Assert.Equal("March 2022", newest.StartDate);
            Assert.Equal("November 2023", newest.EndDate);
            Assert.Equivalent(new[] { "Built APIs", "Wrote tests" }, newest.Responsibilities);

            Assert.Equal(
                ["Newer University", "Older University"],
                result.Education.Select(e => e.Institution)
            );
            var newer = result.Education.First();
            Assert.Equal(data.EducationIds["Newer University"], newer.Id);
            Assert.Equal("MSc", newer.Qualification);
            Assert.Equal("February 2020", newer.StartDate);
            Assert.Equal("December 2021", newer.EndDate);
            Assert.Equal("Computer Science", newer.Major);
            Assert.Equal("Cum laude", newer.Achievement);
            Assert.Null(result.Education.Last().Major);

            // Newest issued date first, certifications without an issued date last.
            Assert.Equal(
                ["Cert 2022", "Cert 2020", "Cert Undated"],
                result.Certification.Select(c => c.Name)
            );
            var cert2022 = result.Certification[0];
            Assert.Equal(data.CertificationIds["Cert 2022"], cert2022.Id);
            Assert.Equal("Acme", cert2022.Organisation);
            Assert.Equal("https://acme.example/cred", cert2022.CredentialUrl);
            Assert.Equal("June 2022", cert2022.IssuedDate);
            Assert.Equal("June 2025", cert2022.ExpirationDate);
            Assert.Null(result.Certification[1].ExpirationDate);
        }

        [Fact]
        public async Task GetAllResumeDetailsByUserIdAsync_UserWithNoItems_ReturnsEmptyListsNotNull()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var user = await CreateBareUserAsync(db, "Bare");
            var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

            var result = await repository.GetAllResumeDetailsByUserIdAsync(user.Id);

            Assert.NotNull(result);
            Assert.Equal("Bare Test", result.Name);
            Assert.Equal(user.Email, result.Email);
            Assert.NotNull(result.Title);
            Assert.Empty(result.Title);
            Assert.NotNull(result.Summaries);
            Assert.Empty(result.Summaries);
            Assert.NotNull(result.Socials);
            Assert.Empty(result.Socials);
            Assert.NotNull(result.Skills);
            Assert.Empty(result.Skills);
            Assert.NotNull(result.Experience);
            Assert.Empty(result.Experience);
            Assert.NotNull(result.Education);
            Assert.Empty(result.Education);
            Assert.NotNull(result.Certification);
            Assert.Empty(result.Certification);
        }

        [Fact]
        public async Task GetAllResumeDetailsByUserIdAsync_ExperienceWithoutResponsibilities_ReturnsEmptyList()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var user = await CreateBareUserAsync(db, "NoResp");
            db.Experience.Add(
                new Experience
                {
                    JobTitle = "Developer",
                    CompanyName = "Quiet Co",
                    StartDate = new DateOnly(2020, 1, 1),
                    EndDate = new DateOnly(2021, 1, 1),
                    UserId = user.Id,
                }
            );
            await db.SaveChangesAsync();
            var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

            var result = await repository.GetAllResumeDetailsByUserIdAsync(user.Id);

            Assert.NotNull(result);
            var experience = Assert.Single(result.Experience);
            Assert.NotNull(experience.Responsibilities);
            Assert.Empty(experience.Responsibilities);
        }

        [Fact]
        public async Task GetAllResumeDetailsByUserIdAsync_CertificationWithoutDates_ReturnsNullDates()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var user = await CreateBareUserAsync(db, "NoDates");
            db.Certification.Add(
                new Certification { CertificationName = "Undated", UserId = user.Id }
            );
            await db.SaveChangesAsync();
            var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

            var result = await repository.GetAllResumeDetailsByUserIdAsync(user.Id);

            Assert.NotNull(result);
            var certification = Assert.Single(result.Certification);
            Assert.Equal("Undated", certification.Name);
            Assert.Null(certification.IssuedDate);
            Assert.Null(certification.ExpirationDate);
            Assert.Null(certification.Organisation);
            Assert.Null(certification.CredentialUrl);
        }

        [Fact]
        public async Task GetAllResumeDetailsByUserIdAsync_UnknownUser_ReturnsNull()
        {
            using var scope = _factory.Services.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

            var result = await repository.GetAllResumeDetailsByUserIdAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllResumeDetailsByUserIdAsync_OnlyReturnsOwnersData()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var owner = await OwnershipTestData.CreateUserWithItemsAsync(db, "Owner");
            var other = await OwnershipTestData.CreateUserWithItemsAsync(db, "Other");
            var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

            var result = await repository.GetAllResumeDetailsByUserIdAsync(owner.UserId);

            Assert.NotNull(result);
            Assert.Equal("Owner Test", result.Name);
            Assert.Equal(owner.TitleId, Assert.Single(result.Title).Id);
            Assert.Equal(owner.SummaryId, Assert.Single(result.Summaries).Id);
            Assert.Equal(owner.SkillId, Assert.Single(result.Skills).Id);
            Assert.Equal(owner.EducationId, Assert.Single(result.Education).Id);
            Assert.Equal(owner.ExperienceId, Assert.Single(result.Experience).Id);
            Assert.Equal(owner.CertificationId, Assert.Single(result.Certification).Id);
            Assert.Equal(owner.LinkId, Assert.Single(result.Socials).Id);

            var ownerIds = new[]
            {
                other.TitleId,
                other.SummaryId,
                other.SkillId,
                other.EducationId,
                other.ExperienceId,
                other.CertificationId,
                other.LinkId,
            };
            var returnedIds = result
                .Title.Select(i => i.Id)
                .Concat(result.Summaries.Select(i => i.Id))
                .Concat(result.Skills.Select(i => i.Id))
                .Concat(result.Education.Select(i => i.Id))
                .Concat(result.Experience.Select(i => i.Id))
                .Concat(result.Certification.Select(i => i.Id))
                .Concat(result.Socials.Select(i => i.Id));
            Assert.Empty(returnedIds.Intersect(ownerIds));
        }

        [Fact]
        public async Task GetUserResumeDetails_ReturnsTheCallersPayloadAsJson()
        {
            using var client = TestClients.CreateHttpsClient(_factory);
            var registered = await TestClients.RegisterAsync(client, "Apiuser");
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await AddFullDataAsync(db, registered.Id);
            }

            using var response = await client.GetAsync("/api/Resume/get-user-resume-details");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var details = await response.Content.ReadFromJsonAsync<GetAllResumeDetails>(
                new JsonSerializerOptions(JsonSerializerDefaults.Web)
            );
            Assert.NotNull(details);
            Assert.Equal("Apiuser Test", details.Name);
            Assert.Equal(registered.Email, details.Email, ignoreCase: true);
            Assert.Single(details.Title);
            Assert.Single(details.Summaries);
            Assert.Equal(3, details.Socials.Count);
            Assert.Equal(3, details.Skills.Count);
            Assert.Equal(["Newest Co", "Oldest Co"], details.Experience.Select(e => e.Company));
            Assert.Equal("March 2022", details.Experience[0].StartDate);
            Assert.Equal(2, details.Education.Count);
            Assert.Equal(3, details.Certification.Count);
            Assert.Equal("June 2022", details.Certification[0].IssuedDate);
        }

        private sealed record FullData(
            Guid TitleId,
            Guid SummaryId,
            Dictionary<string, Guid> LinkIds,
            Dictionary<string, Guid> SkillIds,
            Dictionary<string, Guid> ExperienceIds,
            Dictionary<string, Guid> EducationIds,
            Dictionary<string, Guid> CertificationIds
        );

        private static async Task<ApplicationUser> CreateBareUserAsync(
            ApplicationDbContext db,
            string firstName
        )
        {
            var email = TestClients.UniqueEmail(firstName);
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                FirstName = firstName,
                LastName = "Test",
                UserName = email,
                Email = email,
                PhoneNumber = "0820000001",
            };
            db.User.Add(user);
            await db.SaveChangesAsync();
            return user;
        }

        /// <summary>
        /// Adds items that are deliberately inserted out of order, so every sort rule is visible.
        /// </summary>
        private static async Task<FullData> AddFullDataAsync(ApplicationDbContext db, Guid userId)
        {
            var title = new Title { ResumeTitle = "Senior Developer", UserId = userId };
            var summary = new ProfessionalSummary { Summary = "Builds things.", UserId = userId };

            var links = new[] { "https://c.example", "https://A.example", "https://b.example" }
                .Select(link => new ProfessionalLink
                {
                    LinkType = "Github",
                    Link = link,
                    UserId = userId,
                })
                .ToList();

            var skills = new[]
            {
                new Skill { SkillName = "Charlie", UserId = userId },
                new Skill
                {
                    SkillName = "alpha",
                    ProficiencyLevel = "Expert",
                    UserId = userId,
                },
                new Skill { SkillName = "Bravo", UserId = userId },
            };

            var experiences = new[]
            {
                new Experience
                {
                    JobTitle = "Developer",
                    CompanyName = "Oldest Co",
                    StartDate = new DateOnly(2018, 1, 15),
                    EndDate = new DateOnly(2020, 5, 31),
                    UserId = userId,
                },
                new Experience
                {
                    JobTitle = "Engineer",
                    CompanyName = "Newest Co",
                    StartDate = new DateOnly(2022, 3, 1),
                    EndDate = new DateOnly(2023, 11, 30),
                    UserId = userId,
                    Responsibilities =
                    [
                        new ExperienceResponsibility { Responsibility = "Built APIs" },
                        new ExperienceResponsibility { Responsibility = "Wrote tests" },
                    ],
                },
            };

            var educations = new[]
            {
                new Education
                {
                    InstitutionName = "Older University",
                    Qualification = "BSc",
                    StartDate = new DateOnly(2014, 2, 1),
                    EndDate = new DateOnly(2017, 11, 30),
                    UserId = userId,
                },
                new Education
                {
                    InstitutionName = "Newer University",
                    Qualification = "MSc",
                    StartDate = new DateOnly(2020, 2, 1),
                    EndDate = new DateOnly(2021, 12, 15),
                    Major = "Computer Science",
                    Achievement = "Cum laude",
                    UserId = userId,
                },
            };

            var certifications = new[]
            {
                new Certification { CertificationName = "Cert Undated", UserId = userId },
                new Certification
                {
                    CertificationName = "Cert 2020",
                    IssuedDate = new DateOnly(2020, 9, 1),
                    UserId = userId,
                },
                new Certification
                {
                    CertificationName = "Cert 2022",
                    IssuingOrganisation = "Acme",
                    CredentialUrl = "https://acme.example/cred",
                    IssuedDate = new DateOnly(2022, 6, 10),
                    ExpiryDate = new DateOnly(2025, 6, 10),
                    UserId = userId,
                },
            };

            db.Title.Add(title);
            db.ProfessionalSummary.Add(summary);
            db.ProfessionalLink.AddRange(links);
            db.Skill.AddRange(skills);
            db.Experience.AddRange(experiences);
            db.Education.AddRange(educations);
            db.Certification.AddRange(certifications);
            await db.SaveChangesAsync();

            return new FullData(
                title.Id,
                summary.Id,
                links.ToDictionary(l => l.Link, l => l.Id),
                skills.ToDictionary(s => s.SkillName, s => s.Id),
                experiences.ToDictionary(e => e.CompanyName!, e => e.Id),
                educations.ToDictionary(e => e.InstitutionName, e => e.Id),
                certifications.ToDictionary(c => c.CertificationName, c => c.Id)
            );
        }
    }
}

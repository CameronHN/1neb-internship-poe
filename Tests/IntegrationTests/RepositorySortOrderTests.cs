using Microsoft.Extensions.DependencyInjection;
using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.DTOs;
using Portfolio.Core.Entities;
using Portfolio.Infrastructure.Persistence;
using Portfolio.Tests.Common;
using Xunit;

namespace Portfolio.Tests.IntegrationTests
{
    /// <summary>
    /// Issue #10: the list lookups sort by request.Order (Ascending, Descending or None, which
    /// keeps the order of the requested ids). Skills sort by name, education and experience by
    /// end date, certifications by issued date.
    /// Every theory requests the ids in an order that matches neither sort, so Ascending,
    /// Descending and None each give a different result and swapping any two is caught.
    /// </summary>
    [Collection(ApiCollection.Name)]
    public class RepositorySortOrderTests
    {
        private readonly PortfolioApiFactory _factory;

        public RepositorySortOrderTests(PortfolioApiFactory factory)
        {
            _factory = factory;
        }

        // Items are requested as Charlie, Alpha, Bravo.
        public static TheoryData<SortOrder, string[]> SkillOrders =>
            new()
            {
                { SortOrder.Ascending, ["Alpha", "Bravo", "Charlie"] },
                { SortOrder.Descending, ["Charlie", "Bravo", "Alpha"] },
                { SortOrder.None, ["Charlie", "Alpha", "Bravo"] },
            };

        // Items are requested as Third, First, Second (by date).
        public static TheoryData<SortOrder, string[]> DateOrders =>
            new()
            {
                { SortOrder.Ascending, ["First", "Second", "Third"] },
                { SortOrder.Descending, ["Third", "Second", "First"] },
                { SortOrder.None, ["Third", "First", "Second"] },
            };

        private static readonly (string Name, int Year)[] DatedItems =
        [
            ("Third", 2023),
            ("First", 2021),
            ("Second", 2022),
        ];

        [Theory]
        [MemberData(nameof(SkillOrders))]
        public async Task GetAllSkillsByIdsAsync_ReturnsSkillsInRequestedOrder(
            SortOrder order,
            string[] expected
        )
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var owner = await OwnershipTestData.CreateUserWithItemsAsync(db, "Owner");
            var ids = await AddSkillsAsync(db, owner.UserId, "Charlie", "Alpha", "Bravo");
            var repository = scope.ServiceProvider.GetRequiredService<ISkillRepository>();

            var result = await repository.GetAllSkillsByIdsAsync(owner.UserId, Request(order, ids));

            Assert.Equal(expected, result.Select(skill => skill.Skill));
        }

        [Theory]
        [MemberData(nameof(DateOrders))]
        public async Task GetAllEducationsByIdsAsync_ReturnsEducationsInRequestedOrder(
            SortOrder order,
            string[] expected
        )
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var owner = await OwnershipTestData.CreateUserWithItemsAsync(db, "Owner");
            var ids = await AddEducationsAsync(db, owner.UserId, DatedItems);
            var repository = scope.ServiceProvider.GetRequiredService<IEducationRepository>();

            var result = await repository.GetAllEducationsByIdsAsync(
                owner.UserId,
                Request(order, ids)
            );

            Assert.Equal(expected, result.Select(education => education.Institution));
        }

        [Theory]
        [MemberData(nameof(DateOrders))]
        public async Task GetAllExperiencesByIdsAsync_ReturnsExperiencesInRequestedOrder(
            SortOrder order,
            string[] expected
        )
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var owner = await OwnershipTestData.CreateUserWithItemsAsync(db, "Owner");
            var ids = await AddExperiencesAsync(db, owner.UserId, DatedItems);
            var repository = scope.ServiceProvider.GetRequiredService<IExperienceRepository>();

            var result = await repository.GetAllExperiencesByIdsAsync(
                owner.UserId,
                Request(order, ids)
            );

            Assert.Equal(expected, result.Select(experience => experience.Company));
        }

        [Theory]
        [MemberData(nameof(DateOrders))]
        public async Task GetAllCertificationsByTheirIdsAsync_ReturnsCertificationsInRequestedOrder(
            SortOrder order,
            string[] expected
        )
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var owner = await OwnershipTestData.CreateUserWithItemsAsync(db, "Owner");
            var ids = await AddCertificationsAsync(db, owner.UserId, DatedItems);
            var repository = scope.ServiceProvider.GetRequiredService<ICertificationRepository>();

            var result = await repository.GetAllCertificationsByTheirIdsAsync(
                owner.UserId,
                Request(order, ids)
            );

            Assert.Equal(expected, result.Select(certification => certification.Name));
        }

        [Fact]
        public async Task GetAllSkillsByIdsAsync_SortedResultSkipsOtherUsersAndUnknownIds()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var (owner, other) = await CreateTwoUsersAsync(db);
            var ownIds = await AddSkillsAsync(db, owner.UserId, "Charlie", "Alpha");
            var otherIds = await AddSkillsAsync(db, other.UserId, "Bravo");
            var repository = scope.ServiceProvider.GetRequiredService<ISkillRepository>();

            var result = await repository.GetAllSkillsByIdsAsync(
                owner.UserId,
                Request(SortOrder.Ascending, [.. ownIds, .. otherIds, Guid.NewGuid()])
            );

            Assert.Equal(["Alpha", "Charlie"], result.Select(skill => skill.Skill));
        }

        [Fact]
        public async Task GetAllEducationsByIdsAsync_SortedResultSkipsOtherUsersAndUnknownIds()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var (owner, other) = await CreateTwoUsersAsync(db);
            var ownIds = await AddEducationsAsync(db, owner.UserId, [("Third", 2023), ("First", 2021)]);
            var otherIds = await AddEducationsAsync(db, other.UserId, [("Second", 2022)]);
            var repository = scope.ServiceProvider.GetRequiredService<IEducationRepository>();

            var result = await repository.GetAllEducationsByIdsAsync(
                owner.UserId,
                Request(SortOrder.Ascending, [.. ownIds, .. otherIds, Guid.NewGuid()])
            );

            Assert.Equal(["First", "Third"], result.Select(education => education.Institution));
        }

        [Fact]
        public async Task GetAllExperiencesByIdsAsync_SortedResultSkipsOtherUsersAndUnknownIds()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var (owner, other) = await CreateTwoUsersAsync(db);
            var ownIds = await AddExperiencesAsync(db, owner.UserId, [("Third", 2023), ("First", 2021)]);
            var otherIds = await AddExperiencesAsync(db, other.UserId, [("Second", 2022)]);
            var repository = scope.ServiceProvider.GetRequiredService<IExperienceRepository>();

            var result = await repository.GetAllExperiencesByIdsAsync(
                owner.UserId,
                Request(SortOrder.Ascending, [.. ownIds, .. otherIds, Guid.NewGuid()])
            );

            Assert.Equal(["First", "Third"], result.Select(experience => experience.Company));
        }

        [Fact]
        public async Task GetAllCertificationsByTheirIdsAsync_SortedResultSkipsOtherUsersAndUnknownIds()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var (owner, other) = await CreateTwoUsersAsync(db);
            var ownIds = await AddCertificationsAsync(db, owner.UserId, [("Third", 2023), ("First", 2021)]);
            var otherIds = await AddCertificationsAsync(db, other.UserId, [("Second", 2022)]);
            var repository = scope.ServiceProvider.GetRequiredService<ICertificationRepository>();

            var result = await repository.GetAllCertificationsByTheirIdsAsync(
                owner.UserId,
                Request(SortOrder.Ascending, [.. ownIds, .. otherIds, Guid.NewGuid()])
            );

            Assert.Equal(["First", "Third"], result.Select(certification => certification.Name));
        }

        private static async Task<(UserWithItems Owner, UserWithItems Other)> CreateTwoUsersAsync(
            ApplicationDbContext db
        )
        {
            var owner = await OwnershipTestData.CreateUserWithItemsAsync(db, "Owner");
            var other = await OwnershipTestData.CreateUserWithItemsAsync(db, "Other");
            return (owner, other);
        }

        private static ItemListRequest Request(SortOrder order, IEnumerable<Guid> ids)
        {
            return new ItemListRequest { Ids = ids.ToList(), Order = order };
        }

        // The Add helpers return the new ids in the same order as the items were given.

        private static async Task<List<Guid>> AddSkillsAsync(
            ApplicationDbContext db,
            Guid userId,
            params string[] names
        )
        {
            var skills = names.Select(name => new Skill { SkillName = name, UserId = userId }).ToList();
            db.Skill.AddRange(skills);
            await db.SaveChangesAsync();
            return skills.Select(skill => skill.Id).ToList();
        }

        private static async Task<List<Guid>> AddEducationsAsync(
            ApplicationDbContext db,
            Guid userId,
            (string Name, int Year)[] items
        )
        {
            var educations = items
                .Select(item => new Education
                {
                    InstitutionName = item.Name,
                    Qualification = "BSc",
                    StartDate = new DateOnly(item.Year - 3, 1, 1),
                    EndDate = new DateOnly(item.Year, 12, 31),
                    UserId = userId,
                })
                .ToList();
            db.Education.AddRange(educations);
            await db.SaveChangesAsync();
            return educations.Select(education => education.Id).ToList();
        }

        private static async Task<List<Guid>> AddExperiencesAsync(
            ApplicationDbContext db,
            Guid userId,
            (string Name, int Year)[] items
        )
        {
            var experiences = items
                .Select(item => new Experience
                {
                    CompanyName = item.Name,
                    JobTitle = "Developer",
                    StartDate = new DateOnly(item.Year - 1, 1, 1),
                    EndDate = new DateOnly(item.Year, 12, 31),
                    UserId = userId,
                })
                .ToList();
            db.Experience.AddRange(experiences);
            await db.SaveChangesAsync();
            return experiences.Select(experience => experience.Id).ToList();
        }

        private static async Task<List<Guid>> AddCertificationsAsync(
            ApplicationDbContext db,
            Guid userId,
            (string Name, int Year)[] items
        )
        {
            var certifications = items
                .Select(item => new Certification
                {
                    CertificationName = item.Name,
                    IssuedDate = new DateOnly(item.Year, 1, 1),
                    UserId = userId,
                })
                .ToList();
            db.Certification.AddRange(certifications);
            await db.SaveChangesAsync();
            return certifications.Select(certification => certification.Id).ToList();
        }
    }
}

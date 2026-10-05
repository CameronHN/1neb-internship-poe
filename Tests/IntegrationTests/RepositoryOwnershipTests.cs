using Microsoft.Extensions.DependencyInjection;
using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.DTOs;
using Portfolio.Core.Exceptions;
using Portfolio.Infrastructure.Persistence;
using Portfolio.Tests.Common;
using Xunit;

namespace Portfolio.Tests.IntegrationTests
{
    /// <summary>
    /// Regression tests for issue #5: every by-id lookup used to build a resume must only
    /// return items owned by the user passed in. Each test checks both directions:
    /// the owner gets their item (so the filter is not too strict), and another user's id
    /// returns nothing (so the filter exists).
    /// </summary>
    [Collection(ApiCollection.Name)]
    public class RepositoryOwnershipTests
    {
        private readonly PortfolioApiFactory _factory;

        public RepositoryOwnershipTests(PortfolioApiFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetTitleByIdAsync_OnlyReturnsOwnersTitle()
        {
            using var scope = _factory.Services.CreateScope();
            var (owner, other) = await CreateTwoUsersAsync(scope);
            var repository = scope.ServiceProvider.GetRequiredService<ITitleRepository>();

            var own = await repository.GetTitleByIdAsync(id: owner.TitleId, userId: owner.UserId);
            var foreign = await repository.GetTitleByIdAsync(id: other.TitleId, userId: owner.UserId);

            Assert.Equal(owner.TitleText, own);
            Assert.Null(foreign);
        }

        [Fact]
        public async Task GetSummaryByIdAsync_OnlyReturnsOwnersSummary()
        {
            using var scope = _factory.Services.CreateScope();
            var (owner, other) = await CreateTwoUsersAsync(scope);
            var repository =
                scope.ServiceProvider.GetRequiredService<IProfessionalSummaryRepository>();

            var own = await repository.GetSummaryByIdAsync(id: owner.SummaryId, userId: owner.UserId);
            var foreign = await repository.GetSummaryByIdAsync(
                id: other.SummaryId,
                userId: owner.UserId
            );

            Assert.Equal(owner.SummaryText, own);
            Assert.Null(foreign);
        }

        [Fact]
        public async Task GetAllSkillsByIdsAsync_OnlyReturnsOwnersSkills()
        {
            using var scope = _factory.Services.CreateScope();
            var (owner, other) = await CreateTwoUsersAsync(scope);
            var repository = scope.ServiceProvider.GetRequiredService<ISkillRepository>();

            var own = await repository.GetAllSkillsByIdsAsync(owner.UserId, Ids(owner.SkillId));
            var foreign = await repository.GetAllSkillsByIdsAsync(owner.UserId, Ids(other.SkillId));

            Assert.Single(own);
            Assert.Empty(foreign);
        }

        [Fact]
        public async Task GetAllEducationsByIdsAsync_OnlyReturnsOwnersEducation()
        {
            using var scope = _factory.Services.CreateScope();
            var (owner, other) = await CreateTwoUsersAsync(scope);
            var repository = scope.ServiceProvider.GetRequiredService<IEducationRepository>();

            var own = await repository.GetAllEducationsByIdsAsync(
                owner.UserId,
                Ids(owner.EducationId)
            );
            var foreign = await repository.GetAllEducationsByIdsAsync(
                owner.UserId,
                Ids(other.EducationId)
            );

            Assert.Single(own);
            Assert.Empty(foreign);
        }

        [Fact]
        public async Task GetAllExperiencesByIdsAsync_OnlyReturnsOwnersExperience()
        {
            using var scope = _factory.Services.CreateScope();
            var (owner, other) = await CreateTwoUsersAsync(scope);
            var repository = scope.ServiceProvider.GetRequiredService<IExperienceRepository>();

            var own = await repository.GetAllExperiencesByIdsAsync(
                owner.UserId,
                Ids(owner.ExperienceId)
            );
            var foreign = await repository.GetAllExperiencesByIdsAsync(
                owner.UserId,
                Ids(other.ExperienceId)
            );

            Assert.Single(own);
            Assert.Empty(foreign);
        }

        [Fact]
        public async Task GetAllCertificationsByTheirIdsAsync_OnlyReturnsOwnersCertifications()
        {
            using var scope = _factory.Services.CreateScope();
            var (owner, other) = await CreateTwoUsersAsync(scope);
            var repository = scope.ServiceProvider.GetRequiredService<ICertificationRepository>();

            var own = await repository.GetAllCertificationsByTheirIdsAsync(
                owner.UserId,
                Ids(owner.CertificationId)
            );
            var foreign = await repository.GetAllCertificationsByTheirIdsAsync(
                owner.UserId,
                Ids(other.CertificationId)
            );

            Assert.Single(own);
            Assert.Empty(foreign);
        }

        [Fact]
        public async Task GetProfessionalLinksByIdsAsync_OnlyReturnsOwnersLinks()
        {
            using var scope = _factory.Services.CreateScope();
            var (owner, other) = await CreateTwoUsersAsync(scope);
            var repository =
                scope.ServiceProvider.GetRequiredService<IProfessionalLinkRepository>();

            var own = await repository.GetProfessionalLinksByIdsAsync(
                owner.UserId,
                Ids(owner.LinkId)
            );
            var foreign = await repository.GetProfessionalLinksByIdsAsync(
                owner.UserId,
                Ids(other.LinkId)
            );

            Assert.Single(own);
            Assert.Empty(foreign);
        }

        /// <summary>
        /// Issue #8: named arguments bind to the interface's parameter names, so this call only
        /// works if the interface and the implementation agree on (id, userId).
        /// This repository throws instead of returning null when nothing matches.
        /// </summary>
        [Fact]
        public async Task GetSavedResumeByIdAsync_OnlyReturnsOwnersSavedResume()
        {
            using var scope = _factory.Services.CreateScope();
            var (owner, other) = await CreateTwoUsersAsync(scope);
            var repository = scope.ServiceProvider.GetRequiredService<ISavedResumeRepository>();

            var own = await repository.GetByIdAsync(id: owner.SavedResumeId, userId: owner.UserId);

            Assert.Equal(owner.SavedResumeId, own?.Id);
            await Assert.ThrowsAsync<NotFoundException>(() =>
                repository.GetByIdAsync(id: other.SavedResumeId, userId: owner.UserId)
            );
        }

        private static async Task<(UserWithItems Owner, UserWithItems Other)> CreateTwoUsersAsync(
            IServiceScope scope
        )
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var owner = await OwnershipTestData.CreateUserWithItemsAsync(db, "Owner");
            var other = await OwnershipTestData.CreateUserWithItemsAsync(db, "Other");
            return (owner, other);
        }

        private static ItemListRequest Ids(params Guid[] ids)
        {
            return new ItemListRequest { Ids = ids.ToList() };
        }
    }
}

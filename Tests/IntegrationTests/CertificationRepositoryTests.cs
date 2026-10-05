using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.DTOs.Certification;
using Portfolio.Infrastructure.Persistence;
using Portfolio.Tests.Common;
using Xunit;

namespace Portfolio.Tests.IntegrationTests
{
    /// <summary>
    /// Issue #12: ICertificationRepository.AddCertificationsAsync must name its list parameter
    /// "certifications", as CertificationRepository does. A caller using named arguments binds
    /// to the interface's names, so this test calls it with "certifications:" and would not
    /// compile against the old name.
    /// </summary>
    [Collection(ApiCollection.Name)]
    public class CertificationRepositoryTests
    {
        private readonly PortfolioApiFactory _factory;

        public CertificationRepositoryTests(PortfolioApiFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task AddCertificationsAsync_WithNamedArguments_SavesCertificationsForUser()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var owner = await OwnershipTestData.CreateUserWithItemsAsync(db, "CertOwner");
            var repository = scope.ServiceProvider.GetRequiredService<ICertificationRepository>();

            var ids = await repository.AddCertificationsAsync(
                userId: owner.UserId,
                certifications:
                [
                    new AddCertification { CertificationName = "Named One" },
                    new AddCertification { CertificationName = "Named Two" },
                ]
            );

            Assert.Equal(2, ids.Count);

            using var readScope = _factory.Services.CreateScope();
            var readDb = readScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var saved = await readDb
                .Certification.Where(c => ids.Contains(c.Id))
                .Select(c => new { c.CertificationName, c.UserId })
                .ToListAsync();

            Assert.Equal(2, saved.Count);
            Assert.All(saved, c => Assert.Equal(owner.UserId, c.UserId));
            Assert.Equal(
                new[] { "Named One", "Named Two" },
                saved.Select(c => c.CertificationName).OrderBy(n => n).ToArray()
            );
        }
    }
}

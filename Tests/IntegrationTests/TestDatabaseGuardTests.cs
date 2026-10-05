using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Portfolio.Infrastructure.Persistence;
using Portfolio.Tests.Common;
using Xunit;

namespace Portfolio.Tests.IntegrationTests
{
    /// <summary>
    /// Proves the tests use the Docker database, never the developer's ProjectDb on LocalDB.
    /// If this fails, stop: the other integration tests may be writing to the real database.
    /// </summary>
    [Collection(ApiCollection.Name)]
    public class TestDatabaseGuardTests
    {
        private readonly PortfolioApiFactory _factory;

        public TestDatabaseGuardTests(PortfolioApiFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public void DbContext_UsesTheTestContainerDatabase()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var connection = db.Database.GetDbConnection();

            Assert.Equal(PortfolioApiFactory.TestDatabaseName, connection.Database);
            Assert.DoesNotContain("localdb", connection.DataSource, StringComparison.OrdinalIgnoreCase);
        }
    }
}

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Portfolio.Infrastructure.Persistence;
using Portfolio.Tests.Common;
using Xunit;

namespace Portfolio.Tests.IntegrationTests
{
    /// <summary>
    /// Fake users are only seeded in Development, or when Database:SeedOnStartup is true.
    /// Migrations still run everywhere. The Production tests use their own fresh database
    /// on the test container, so the shared database's seed data does not hide the result.
    /// </summary>
    [Collection(ApiCollection.Name)]
    public class SeedingTests
    {
        private readonly PortfolioApiFactory _factory;

        public SeedingTests(PortfolioApiFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Development_SeedsFakeUsers()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            Assert.True(await db.User.CountAsync(u => u.PasswordHash == null) >= 10);
        }

        [Fact]
        public async Task Production_MigratesButDoesNotSeed()
        {
            await using var production = ProductionHostWithFreshDatabase(seedOnStartup: null);

            Assert.Equal(0, await CountUsersAsync(production));
        }

        [Fact]
        public async Task Production_WithSeedOnStartup_Seeds()
        {
            await using var production = ProductionHostWithFreshDatabase(seedOnStartup: "true");

            Assert.Equal(10, await CountUsersAsync(production));
        }

        private WebApplicationFactory<Program> ProductionHostWithFreshDatabase(string? seedOnStartup)
        {
            var connectionString = new SqlConnectionStringBuilder(_factory.ConnectionString)
            {
                InitialCatalog = $"ProjectDb_Seeding_{Guid.NewGuid():N}",
            }.ConnectionString;

            return _factory.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Production");
                if (seedOnStartup is not null)
                {
                    builder.ConfigureAppConfiguration(
                        (_, config) =>
                            config.AddInMemoryCollection(
                                new Dictionary<string, string?>
                                {
                                    ["Database:SeedOnStartup"] = seedOnStartup,
                                }
                            )
                    );
                }
                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseSqlServer(
                            connectionString,
                            b => b.MigrationsAssembly("Portfolio.Infrastructure")
                        )
                    );
                });
            });
        }

        /// <summary>
        /// Starting the host runs Program.cs, which migrates and (maybe) seeds.
        /// </summary>
        private static async Task<int> CountUsersAsync(WebApplicationFactory<Program> host)
        {
            using var client = host.CreateClient();
            using var scope = host.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            return await db.User.CountAsync();
        }
    }
}

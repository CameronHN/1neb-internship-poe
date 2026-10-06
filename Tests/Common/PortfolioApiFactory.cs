using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Portfolio.Infrastructure.Persistence;
using Testcontainers.MsSql;
using Xunit;

namespace Portfolio.Tests.Common
{
    /// <summary>
    /// Starts the real API in memory, backed by a throwaway SQL Server running in Docker.
    /// One instance is shared by every test in the "Api" collection (see ApiCollection).
    /// </summary>
    public sealed class PortfolioApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        public const string TestDatabaseName = "ProjectDb_Tests";

        private readonly MsSqlContainer _sqlContainer = new MsSqlBuilder(
            "mcr.microsoft.com/mssql/server:2022-latest"
        ).Build();

        public string ConnectionString { get; private set; } = string.Empty;

        public async Task InitializeAsync()
        {
            await _sqlContainer.StartAsync();

            var builder = new SqlConnectionStringBuilder(_sqlContainer.GetConnectionString())
            {
                InitialCatalog = TestDatabaseName,
            };
            ConnectionString = builder.ConnectionString;
        }

        /// <summary>
        /// Every test client comes from the same in-memory "IP address", so the real limits
        /// would trip across unrelated tests. Tests that check the limits start their own host
        /// with low values (see TestHosts.WithSettings).
        /// </summary>
        public static readonly Dictionary<string, string?> HighRateLimits = new()
        {
            ["RateLimiting:Auth:PermitLimit"] = "100000",
            ["RateLimiting:Pdf:PermitLimit"] = "100000",
            ["RateLimiting:Demo:BurstLimit"] = "100000",
            ["RateLimiting:Demo:DailyLimit"] = "100000",
            ["RateLimiting:PdfConcurrency:PermitLimit"] = "100",
            ["RateLimiting:PdfConcurrency:QueueLimit"] = "100",
        };

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");

            builder.ConfigureAppConfiguration(
                (_, config) => config.AddInMemoryCollection(HighRateLimits)
            );

            // Fail at startup if any registration is missing or has an invalid lifetime.
            builder.UseDefaultServiceProvider(options =>
            {
                options.ValidateOnBuild = true;
                options.ValidateScopes = true;
            });

            builder.ConfigureTestServices(services =>
            {
                // Replace the ProjectDb registration from AddInfrastructure with the test container.
                services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(
                        ConnectionString,
                        b => b.MigrationsAssembly("Portfolio.Infrastructure")
                    )
                );

                // Register controllers as services so ValidateOnBuild checks their dependencies too.
                services.AddControllers().AddControllersAsServices();
            });
        }

        async Task IAsyncLifetime.DisposeAsync()
        {
            await base.DisposeAsync();
            await _sqlContainer.DisposeAsync();
        }
    }
}

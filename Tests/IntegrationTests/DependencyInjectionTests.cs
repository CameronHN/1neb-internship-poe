using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Portfolio.Core.Contracts.Repositories;
using Portfolio.Tests.Common;
using Portfolio.WebApi.Controllers;
using Xunit;

namespace Portfolio.Tests.IntegrationTests
{
    /// <summary>
    /// Regression tests for issue #3. PortfolioApiFactory turns on ValidateOnBuild and
    /// ValidateScopes and registers controllers as services, so a missing registration or a
    /// singleton capturing a scoped service makes the host fail to start.
    /// </summary>
    [Collection(ApiCollection.Name)]
    public class DependencyInjectionTests
    {
        private readonly PortfolioApiFactory _factory;

        public DependencyInjectionTests(PortfolioApiFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Every concrete controller class in the Portfolio.WebApi project.
        /// </summary>
        private static List<Type> FindControllerTypes()
        {
            return typeof(AuthController)
                .Assembly.GetTypes()
                .Where(type =>
                    typeof(ControllerBase).IsAssignableFrom(type)
                    && type is { IsClass: true, IsAbstract: false }
                )
                .ToList();
        }

        public static TheoryData<Type> AllControllerTypes()
        {
            var data = new TheoryData<Type>();
            foreach (var controller in FindControllerTypes())
            {
                data.Add(controller);
            }

            return data;
        }

        [Fact]
        public void FindsAllElevenControllers()
        {
            Assert.Equal(11, FindControllerTypes().Count);
        }

        [Theory]
        [MemberData(nameof(AllControllerTypes))]
        public void EveryControllerResolves(Type controllerType)
        {
            using var scope = _factory.Services.CreateScope();

            var controller = scope.ServiceProvider.GetRequiredService(controllerType);

            Assert.IsType(controllerType, controller);
        }

        [Fact]
        public void MissingRegistration_FailsAtStartup()
        {
            using var brokenFactory = _factory.WithWebHostBuilder(builder =>
                builder.ConfigureTestServices(services => services.RemoveAll<ITitleRepository>())
            );

            var exception = Record.Exception(() => brokenFactory.CreateClient());

            Assert.NotNull(exception);
            Assert.Contains(nameof(ITitleRepository), exception.ToString());
        }
    }
}

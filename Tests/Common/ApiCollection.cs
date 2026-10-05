using Xunit;

namespace Portfolio.Tests.Common
{
    /// <summary>
    /// Every test class marked [Collection(ApiCollection.Name)] shares one PortfolioApiFactory,
    /// so the SQL Server container is started once per test run, not once per class.
    /// </summary>
    [CollectionDefinition(Name)]
    public class ApiCollection : ICollectionFixture<PortfolioApiFactory>
    {
        public const string Name = "Api";
    }
}

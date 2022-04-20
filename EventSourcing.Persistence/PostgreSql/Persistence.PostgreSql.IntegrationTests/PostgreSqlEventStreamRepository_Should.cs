using EventSourcing.Persistence;
using Persistence.IntegrationTests.Base;
using Xunit;
using Xunit.Abstractions;

namespace Persistence.PostgreSql.IntegrationTests
{
    [Collection(nameof(PostgreSqlCollectionDefinition))]
    public class PostgreSqlEventStreamRepository_Should : EventStreamRepository_Should
    {
        private readonly PostgreSqlCollectionFixture _fixture;

        protected override IEventStreamRepository Repository => _fixture.GetService<IEventStreamRepository>();

        protected override IEventStreamTestReadRepository TestReadRepository =>
            _fixture.GetService<IEventStreamTestReadRepository>();

        public PostgreSqlEventStreamRepository_Should(PostgreSqlCollectionFixture fixture, ITestOutputHelper testOutputHelper)
        {
            _fixture = fixture.SetTestOutputHelper(testOutputHelper);
        }
    }
}
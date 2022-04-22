using EventSourcing.Persistence;
using Persistence.IntegrationTests.Base;
using Xunit;
using Xunit.Abstractions;

namespace Persistence.PostgreSql.IntegrationTests
{
    [Collection(nameof(PostgreSqlCollectionDefinition))]
    public class PostgreSqlEventStreamStagingRepository_Should : EventStreamStagingRepository_Should
    {
        private readonly PostgreSqlCollectionFixture _fixture;

        protected override IEventStreamStagingRepository Repository =>
            _fixture.GetService<IEventStreamStagingRepository>();

        protected override IEventStreamStagingTestRepository TestRepository =>
            _fixture.GetService<IEventStreamStagingTestRepository>();

        public PostgreSqlEventStreamStagingRepository_Should(PostgreSqlCollectionFixture fixture, ITestOutputHelper testOutputHelper)
        {
            _fixture = fixture.SetTestOutputHelper(testOutputHelper);
        }
    }
}
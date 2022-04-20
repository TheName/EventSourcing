using EventSourcing.Persistence;
using Persistence.IntegrationTests.Base;
using Xunit;
using Xunit.Abstractions;

namespace Persistence.PostgreSql.IntegrationTests
{
    [Collection(nameof(PostgreSqlCollectionDefinition))]
    public class PostgreSqlEventStreamStagingWriteRepository_Should : EventStreamStagingWriteRepository_Should
    {
        private readonly PostgreSqlCollectionFixture _fixture;

        protected override IEventStreamStagingWriteRepository Repository =>
            _fixture.GetService<IEventStreamStagingWriteRepository>();

        protected override IEventStreamStagingTestReadRepository TestReadRepository =>
            _fixture.GetService<IEventStreamStagingTestReadRepository>();

        public PostgreSqlEventStreamStagingWriteRepository_Should(PostgreSqlCollectionFixture fixture, ITestOutputHelper testOutputHelper)
        {
            _fixture = fixture.SetTestOutputHelper(testOutputHelper);
        }
    }
}
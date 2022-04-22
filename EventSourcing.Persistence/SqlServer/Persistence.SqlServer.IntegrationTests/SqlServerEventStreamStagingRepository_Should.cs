using EventSourcing.Persistence;
using Persistence.IntegrationTests.Base;
using Xunit;
using Xunit.Abstractions;

namespace Persistence.SqlServer.IntegrationTests
{
    [Collection(nameof(SqlServerCollectionDefinition))]
    public class SqlServerEventStreamStagingRepository_Should : EventStreamStagingRepository_Should
    {
        private readonly SqlServerCollectionFixture _fixture;

        protected override IEventStreamStagingRepository Repository =>
            _fixture.GetService<IEventStreamStagingRepository>();

        protected override IEventStreamStagingTestRepository TestRepository =>
            _fixture.GetService<IEventStreamStagingTestRepository>();

        public SqlServerEventStreamStagingRepository_Should(SqlServerCollectionFixture fixture, ITestOutputHelper testOutputHelper)
        {
            _fixture = fixture.SetTestOutputHelper(testOutputHelper);
        }
    }
}
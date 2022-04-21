﻿using EventSourcing.Persistence;
using Persistence.IntegrationTests.Base;
using Xunit;
using Xunit.Abstractions;

namespace Persistence.SqlServer.IntegrationTests
{
    [Collection(nameof(SqlServerCollectionDefinition))]
    public class SqlServerEventStreamStagingWriteRepository_Should : EventStreamStagingWriteRepository_Should
    {
        private readonly SqlServerCollectionFixture _fixture;

        protected override IEventStreamStagingWriteRepository Repository =>
            _fixture.GetService<IEventStreamStagingWriteRepository>();

        protected override IEventStreamStagingTestReadRepository TestReadRepository =>
            _fixture.GetService<IEventStreamStagingTestReadRepository>();

        public SqlServerEventStreamStagingWriteRepository_Should(SqlServerCollectionFixture fixture, ITestOutputHelper testOutputHelper)
        {
            _fixture = fixture.SetTestOutputHelper(testOutputHelper);
        }
    }
}
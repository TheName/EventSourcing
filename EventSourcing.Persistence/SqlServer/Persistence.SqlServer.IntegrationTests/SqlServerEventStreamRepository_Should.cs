﻿using EventSourcing.Persistence;
using Persistence.IntegrationTests.Base;
using Xunit;
using Xunit.Abstractions;

namespace Persistence.SqlServer.IntegrationTests
{
    [Collection(nameof(SqlServerCollectionDefinition))]
    public class SqlServerEventStreamRepository_Should : EventStreamRepository_Should
    {
        private readonly SqlServerCollectionFixture _fixture;

        protected override IEventStreamRepository Repository => _fixture.GetService<IEventStreamRepository>();

        protected override IEventStreamTestReadRepository TestReadRepository =>
            _fixture.GetService<IEventStreamTestReadRepository>();

        public SqlServerEventStreamRepository_Should(SqlServerCollectionFixture fixture, ITestOutputHelper testOutputHelper)
        {
            _fixture = fixture.SetTestOutputHelper(testOutputHelper);
        }
    }
}
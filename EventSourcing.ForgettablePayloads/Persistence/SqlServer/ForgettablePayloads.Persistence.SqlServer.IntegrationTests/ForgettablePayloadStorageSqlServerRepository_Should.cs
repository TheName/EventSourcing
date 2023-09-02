using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.ForgettablePayloads.Persistence;
using EventSourcing.ForgettablePayloads.Persistence.SqlServer;
using ForgettablePayloads.Persistence.IntegrationTests.Base;
using Xunit;
using Xunit.Abstractions;

namespace ForgettablePayloads.Persistence.SqlServer.IntegrationTests
{
    [Collection(nameof(SqlServerCollectionDefinition))]
    public class ForgettablePayloadStorageSqlServerRepository_Should : ForgettablePayloadStorageRepository_Should
    {
        private readonly SqlServerCollectionFixture _fixture;

        protected override IForgettablePayloadStorageRepository Repository => _fixture.GetService<IForgettablePayloadStorageRepository>();

        public ForgettablePayloadStorageSqlServerRepository_Should(SqlServerCollectionFixture fixture, ITestOutputHelper testOutputHelper)
        {
            _fixture = fixture.SetTestOutputHelper(testOutputHelper);
        }

        protected override async Task TruncateAsync(CancellationToken cancellationToken)
        {
            var connectionString = _fixture.GetService<ISqlServerEventStreamForgettablePayloadPersistenceConfiguration>().ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "TRUNCATE TABLE [EventStream.ForgettablePayloads]";
                    await connection.OpenAsync(cancellationToken);
                    await command.ExecuteNonQueryAsync(cancellationToken);
                }
            }
        }
    }
}

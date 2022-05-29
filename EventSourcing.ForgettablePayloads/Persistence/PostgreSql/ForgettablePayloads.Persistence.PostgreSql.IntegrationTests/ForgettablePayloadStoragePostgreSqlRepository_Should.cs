using System.Threading;
using System.Threading.Tasks;
using EventSourcing.ForgettablePayloads.Persistence;
using EventSourcing.ForgettablePayloads.Persistence.PostgreSql;
using ForgettablePayloads.Persistence.IntegrationTests.Base;
using Npgsql;
using Xunit;
using Xunit.Abstractions;

namespace ForgettablePayloads.Persistence.PostgreSql.IntegrationTests
{
    [Collection(nameof(PostgreSqlCollectionDefinition))]
    public class ForgettablePayloadStoragePostgreSqlRepository_Should : ForgettablePayloadStorageRepository_Should
    {
        private readonly PostgreSqlCollectionFixture _fixture;

        protected override IForgettablePayloadStorageRepository Repository => _fixture.GetService<IForgettablePayloadStorageRepository>();

        public ForgettablePayloadStoragePostgreSqlRepository_Should(PostgreSqlCollectionFixture fixture, ITestOutputHelper testOutputHelper)
        {
            _fixture = fixture.SetTestOutputHelper(testOutputHelper);
        }
        
        protected override async Task TruncateAsync(CancellationToken cancellationToken)
        {
            var connectionString = _fixture.GetService<IPostgreSqlEventStreamForgettablePayloadPersistenceConfiguration>().ConnectionString;
            await using var connection = new NpgsqlConnection(connectionString);
            await using var command = connection.CreateCommand();
            command.CommandText = "TRUNCATE TABLE \"EventStream.ForgettablePayloads\"";
            await connection.OpenAsync(cancellationToken);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
    }
}
using Xunit;

namespace ForgettablePayloads.Persistence.PostgreSql.IntegrationTests
{
    [CollectionDefinition(nameof(PostgreSqlCollectionDefinition))]
    public class PostgreSqlCollectionDefinition : ICollectionFixture<PostgreSqlCollectionFixture>
    {
    }
}
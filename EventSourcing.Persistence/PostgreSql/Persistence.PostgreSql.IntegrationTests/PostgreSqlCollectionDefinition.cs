using Xunit;

namespace Persistence.PostgreSql.IntegrationTests
{
    [CollectionDefinition(nameof(PostgreSqlCollectionDefinition))]
    public class PostgreSqlCollectionDefinition : ICollectionFixture<PostgreSqlCollectionFixture>
    {
    }
}
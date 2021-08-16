using Xunit;

namespace Persistence.SqlServer.IntegrationTests
{
    [CollectionDefinition(nameof(SqlServerCollectionDefinition))]
    public class SqlServerCollectionDefinition : ICollectionFixture<SqlServerCollectionFixture>
    {
    }
}
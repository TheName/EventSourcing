using Xunit;

namespace ForgettablePayloads.Persistence.SqlServer.IntegrationTests
{
    [CollectionDefinition(nameof(SqlServerCollectionDefinition))]
    public class SqlServerCollectionDefinition : ICollectionFixture<SqlServerCollectionFixture>
    {
    }
}
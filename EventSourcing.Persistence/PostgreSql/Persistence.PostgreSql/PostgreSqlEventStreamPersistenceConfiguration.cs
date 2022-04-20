namespace EventSourcing.Persistence.PostgreSql
{
    internal class PostgreSqlEventStreamPersistenceConfiguration : IPostgreSqlEventStreamPersistenceConfiguration
    {
        public string ConnectionString { get; set; }
    }
}
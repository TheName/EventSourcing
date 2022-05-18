namespace EventSourcing.ForgettablePayloads.Persistence.PostgreSql
{
    internal class PostgreSqlEventStreamForgettablePayloadPersistenceConfiguration : IPostgreSqlEventStreamForgettablePayloadPersistenceConfiguration
    {
        public string ConnectionString { get; set; }
    }
}
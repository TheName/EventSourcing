namespace EventSourcing.Persistence.SqlServer
{
    internal class SqlServerEventStreamPersistenceConfiguration : ISqlServerEventStreamPersistenceConfiguration
    {
        public string ConnectionString { get; set; }
    }
}
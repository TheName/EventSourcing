namespace EventSourcing.Persistence.SqlServer
{
    internal interface ISqlServerEventStreamPersistenceConfiguration
    {
        string ConnectionString { get; }
    }
}
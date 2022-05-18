namespace EventSourcing.ForgettablePayloads.Persistence.SqlServer
{
    internal class SqlServerEventStreamForgettablePayloadPersistenceConfiguration : ISqlServerEventStreamForgettablePayloadPersistenceConfiguration
    {
        public string ConnectionString { get; set; }
    }
}
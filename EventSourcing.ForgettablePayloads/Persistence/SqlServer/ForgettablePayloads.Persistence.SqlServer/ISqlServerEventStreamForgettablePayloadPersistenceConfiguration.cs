namespace EventSourcing.ForgettablePayloads.Persistence.SqlServer
{
    /// <summary>
    /// SQL Server EventStream.ForgettablePayload persistence configuration
    /// </summary>
    public interface ISqlServerEventStreamForgettablePayloadPersistenceConfiguration
    {
        /// <summary>
        /// SQL Server connection string
        /// </summary>
        string ConnectionString { get; }
    }
}
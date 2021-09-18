namespace EventSourcing.Persistence.SqlServer
{
    /// <summary>
    /// SQL Server EventStream persistence configuration
    /// </summary>
    public interface ISqlServerEventStreamPersistenceConfiguration
    {
        /// <summary>
        /// SQL Server connection string
        /// </summary>
        string ConnectionString { get; }
    }
}
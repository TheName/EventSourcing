namespace EventSourcing.Persistence.PostgreSql
{
    /// <summary>
    /// PostgreSQL EventStream persistence configuration
    /// </summary>
    public interface IPostgreSqlEventStreamPersistenceConfiguration
    {
        /// <summary>
        /// PostgreSQL connection string
        /// </summary>
        string ConnectionString { get; }
    }
}
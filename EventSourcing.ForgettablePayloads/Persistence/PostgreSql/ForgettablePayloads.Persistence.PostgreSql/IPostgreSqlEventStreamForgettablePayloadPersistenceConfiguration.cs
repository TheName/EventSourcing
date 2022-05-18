namespace EventSourcing.ForgettablePayloads.Persistence.PostgreSql
{
    /// <summary>
    /// PostgreSQL EventStream.ForgettablePayload persistence configuration
    /// </summary>
    public interface IPostgreSqlEventStreamForgettablePayloadPersistenceConfiguration
    {
        /// <summary>
        /// PostgreSQL connection string
        /// </summary>
        string ConnectionString { get; }
    }
}
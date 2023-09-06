namespace EventSourcing.ForgettablePayloads.Extensions.DatabaseMigrations.Persistence.PostgreSql.DbUp
{
    /// <summary>
    /// The PostgreSql script
    /// </summary>
    public class PostgreSqlScript
    {
        /// <summary>
        /// The script's name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The script's content
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlScript"/> class.
        /// </summary>
        /// <param name="name">
        /// The <see cref="string"/> representing script's name.
        /// </param>
        /// <param name="content">
        /// The <see cref="string"/> representing script's content.
        /// </param>
        public PostgreSqlScript(string name, string content)
        {
            Name = name;
            Content = content;
        }
    }
}

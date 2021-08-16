namespace EventSourcing.Extensions.DatabaseMigrations.Persistence.SqlServer
{
    /// <summary>
    /// The SqlServer script
    /// </summary>
    public class SqlServerScript
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
        /// Initializes a new instance of the <see cref="SqlServerScript"/> class.
        /// </summary>
        /// <param name="name">
        /// The <see cref="string"/> representing script's name.
        /// </param>
        /// <param name="content">
        /// The <see cref="string"/> representing script's content.
        /// </param>
        public SqlServerScript(string name, string content)
        {
            Name = name;
            Content = content;
        }
    }
}
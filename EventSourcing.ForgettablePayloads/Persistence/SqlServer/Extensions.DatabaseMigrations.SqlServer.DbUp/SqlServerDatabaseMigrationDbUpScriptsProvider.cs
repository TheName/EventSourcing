using System.Collections.Generic;
using System.Linq;
using DbUp.Engine;
using DbUp.Engine.Transactions;
using DbUp.Support;

namespace EventSourcing.ForgettablePayloads.Extensions.DatabaseMigrations.Persistence.SqlServer.DbUp
{
    /// <summary>
    /// Database migration DbUp scripts provider for SqlServer implementation of EventSourcing.Persistence.Abstraction package
    /// </summary>
    public class SqlServerDatabaseMigrationDbUpScriptsProvider : IScriptProvider
    {
        /// <inheritdoc />
        public IEnumerable<SqlScript> GetScripts(IConnectionManager connectionManager) =>
            SqlServerDatabaseMigrationScriptsProvider.GetDatabaseMigrationScripts()
                .Select((script, i) => new SqlScript(
                    script.Name,
                    script.Content,
                    new SqlScriptOptions {RunGroupOrder = i, ScriptType = ScriptType.RunOnce}));
    }
}
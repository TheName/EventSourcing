using System.Collections.Generic;
using System.Linq;
using DbUp.Engine;
using DbUp.Engine.Transactions;
using DbUp.Support;

namespace EventSourcing.ForgettablePayloads.Extensions.DatabaseMigrations.Persistence.PostgreSql.DbUp
{
    /// <summary>
    /// Database migration DbUp scripts provider for PostgreSQL implementation of EventSourcing.ForgettablePayloads.Persistence package
    /// </summary>
    public class PostgreSqlDatabaseMigrationDbUpScriptsProvider : IScriptProvider
    {
        /// <inheritdoc />
        public IEnumerable<SqlScript> GetScripts(IConnectionManager connectionManager) =>
            PostgreSqlDatabaseMigrationScriptsProvider.GetDatabaseMigrationScripts()
                .Select((script, i) => new SqlScript(
                    script.Name,
                    script.Content,
                    new SqlScriptOptions {RunGroupOrder = i, ScriptType = ScriptType.RunOnce}));
    }
}
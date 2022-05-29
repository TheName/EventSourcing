using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EventSourcing.ForgettablePayloads.Extensions.DatabaseMigrations.Persistence.PostgreSql
{
    /// <summary>
    /// Provider of PostgreSQL database migration scripts
    /// </summary>
    public static class PostgreSqlDatabaseMigrationScriptsProvider
    {
        private const string ScriptResourceNamePrefix = "EventSourcing.ForgettablePayloads.Extensions.DatabaseMigrations.Persistence.PostgreSql.Scripts.";

        private static Assembly CurrentAssembly { get; } = typeof(PostgreSqlDatabaseMigrationScriptsProvider).Assembly;

        private static IReadOnlyList<string> ResourceNames { get; } = CurrentAssembly
            .GetManifestResourceNames()
            .Where(name => name.StartsWith(ScriptResourceNamePrefix))
            .OrderBy(name => name)
            .ToList();
        
        /// <summary>
        /// Gets database migration scripts for PostgreSQL.
        /// </summary>
        /// <returns>
        /// A collection of database migration scripts for PostgreSQL.
        /// </returns>
        public static IReadOnlyList<PostgreSqlScript> GetDatabaseMigrationScripts()
        {
            var result = new List<PostgreSqlScript>();
            foreach (var resourceName in ResourceNames)
            {
                using (var resourceStream = CurrentAssembly.GetManifestResourceStream(resourceName))
                {
                    if (resourceStream == null)
                    {
                        throw new InvalidOperationException($"Could not load resource stream with name {resourceName}");
                    }

                    using (var streamReader = new StreamReader(resourceStream))
                    {
                        var content = streamReader.ReadToEnd();
                        var script = new PostgreSqlScript(
                            resourceName,
                            content);
                        
                        result.Add(script);
                    }
                }
            }

            return result;
        }
        
        /// <summary>
        /// Gets database migration scripts for PostgreSQL in an async manner.
        /// </summary>
        /// <returns>
        /// A task that will return a collection of database migration scripts for PostgreSQL.
        /// </returns>
        public static async Task<IReadOnlyList<PostgreSqlScript>> GetDatabaseMigrationScriptsAsync()
        {
            var result = new List<PostgreSqlScript>();
            foreach (var resourceName in ResourceNames)
            {
                using (var resourceStream = CurrentAssembly.GetManifestResourceStream(resourceName))
                {
                    if (resourceStream == null)
                    {
                        throw new InvalidOperationException($"Could not load resource stream with name {resourceName}");
                    }

                    using (var streamReader = new StreamReader(resourceStream))
                    {
                        var content = await streamReader.ReadToEndAsync().ConfigureAwait(false);
                        var script = new PostgreSqlScript(
                            resourceName,
                            content);
                        
                        result.Add(script);
                    }
                }
            }

            return result;
        }
    }
}
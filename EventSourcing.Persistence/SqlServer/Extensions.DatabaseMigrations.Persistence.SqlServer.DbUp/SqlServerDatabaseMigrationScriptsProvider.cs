using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EventSourcing.Extensions.DatabaseMigrations.Persistence.SqlServer.DbUp
{
    /// <summary>
    /// Provider of SQL Server database migration scripts
    /// </summary>
    public static class SqlServerDatabaseMigrationScriptsProvider
    {
        private static Assembly CurrentAssembly { get; } = typeof(SqlServerDatabaseMigrationScriptsProvider).Assembly;

        private static IReadOnlyList<string> ResourceNames { get; } = CurrentAssembly
            .GetManifestResourceNames()
            .Where(name => name.StartsWith($"{CurrentAssembly.GetName().Name}.Scripts."))
            .OrderBy(name => name)
            .ToList();

        /// <summary>
        /// Gets database migration scripts for SQL Server.
        /// </summary>
        /// <returns>
        /// A collection of database migration scripts for SQL Server.
        /// </returns>
        public static IReadOnlyList<SqlServerScript> GetDatabaseMigrationScripts()
        {
            var result = new List<SqlServerScript>();
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
                        var script = new SqlServerScript(
                            resourceName,
                            content);

                        result.Add(script);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets database migration scripts for SQL Server in an async manner.
        /// </summary>
        /// <returns>
        /// A task that will return a collection of database migration scripts for SQL Server.
        /// </returns>
        public static async Task<IReadOnlyList<SqlServerScript>> GetDatabaseMigrationScriptsAsync()
        {
            var result = new List<SqlServerScript>();
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
                        var script = new SqlServerScript(
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

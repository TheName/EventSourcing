﻿using DbUp.Builder;

namespace EventSourcing.ForgettablePayloads.Extensions.DatabaseMigrations.Persistence.SqlServer.DbUp.Extensions
{
    /// <summary>
    /// Extensions for <see cref="UpgradeEngineBuilder"/>
    /// </summary>
    public static class UpgradeEngineBuilderExtensions
    {
        /// <summary>
        /// Adds database migration DbUp scripts provider for SqlServer implementation of EventSourcing.ForgettablePayloads.Persistence package to the builder.
        /// </summary>
        /// <param name="builder">
        /// The <see cref="UpgradeEngineBuilder"/>.
        /// </param>
        /// <returns>
        /// The <see cref="UpgradeEngineBuilder"/>.
        /// </returns>
        public static UpgradeEngineBuilder WithSqlServerScriptsForEventSourcingForgettablePayloads(
            this UpgradeEngineBuilder builder) =>
            builder.WithScripts(new SqlServerDatabaseMigrationDbUpScriptsProvider());
    }
}
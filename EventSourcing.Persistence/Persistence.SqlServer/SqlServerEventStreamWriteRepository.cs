using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions;
using EventSourcing.Persistence.Abstractions;
using Microsoft.Extensions.Logging;

namespace EventSourcing.Persistence.SqlServer
{
    internal class SqlServerEventStreamWriteRepository : IEventStreamWriteRepository
    {
        private const string TableName = "EventStream";
        
        private readonly ISqlServerEventStreamPersistenceConfiguration _configuration;
        private readonly ILogger<SqlServerEventStreamWriteRepository> _logger;

        public SqlServerEventStreamWriteRepository(
            ISqlServerEventStreamPersistenceConfiguration configuration,
            ILogger<SqlServerEventStreamWriteRepository> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<EventStreamWriteResult> WriteAsync(
            EventStreamEntries eventStreamEntries,
            CancellationToken cancellationToken)
        {
            if (eventStreamEntries.Count == 0)
            {
                return EventStreamWriteResult.EmptyInput;
            }

            var (sqlCommand, sqlParameters) = PrepareInsertCommand(eventStreamEntries);
            
            try
            {
                var executionResult = await ExecuteCommand(sqlCommand, sqlParameters, cancellationToken).ConfigureAwait(false);
                if (executionResult == eventStreamEntries.Count)
                {
                    return EventStreamWriteResult.Success;
                }

                return executionResult == 0
                    ? EventStreamWriteResult.SequenceAlreadyTaken
                    : EventStreamWriteResult.UnknownFailure;
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "An unexpected failure happened when trying to insert event stream entries. Entries to insert: {Entries}",
                    eventStreamEntries);
                
                return EventStreamWriteResult.UnknownFailure;
            }
        }

        private async Task<int> ExecuteCommand(string sqlCommandText, List<SqlParameter> parameters, CancellationToken cancellationToken)
        {
            using (var connection = new SqlConnection(_configuration.ConnectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sqlCommandText;
                    command.Parameters.AddRange(parameters.ToArray());
                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                    return await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                }
            }
        }

        private static (string Command, List<SqlParameter> Parameters) PrepareInsertCommand(EventStreamEntries eventStreamEntries)
        {
            var parameters = new List<SqlParameter>();
            var insertCommand = new StringBuilder();
            insertCommand.AppendLine("BEGIN TRY");
            insertCommand.Append($"INSERT INTO {TableName} (StreamId, EntrySequence, EntryId, EventContent, EventTypeIdentifier, CausationId, CreationTime, CorrelationId) VALUES");

            var separator = " ";
            for (var i = 0; i < eventStreamEntries.Count; i++)
            {
                insertCommand.Append(
                    $"{separator}(@StreamId_{i}, @EntrySequence_{i}, @EntryId_{i}, @EventContent_{i}, @EventTypeIdentifier_{i}, @CausationId_{i}, @CreationTime_{i}, @CorrelationId_{i})");
                var entry = eventStreamEntries[i];
                parameters.AddRange(new []
                {
                    new SqlParameter($"@StreamId_{i}", SqlDbType.UniqueIdentifier) {Value = (Guid) entry.StreamId},
                    new SqlParameter($"@EntrySequence_{i}", SqlDbType.BigInt) {Value = (uint) entry.EntrySequence},
                    new SqlParameter($"@EntryId_{i}", SqlDbType.UniqueIdentifier) {Value = (Guid) entry.EntryId},
                    new SqlParameter($"@EventContent_{i}", SqlDbType.VarChar) {Value = (string) entry.EventDescriptor.EventContent},
                    new SqlParameter($"@EventTypeIdentifier_{i}", SqlDbType.VarChar) {Value = (string) entry.EventDescriptor.EventTypeIdentifier},
                    new SqlParameter($"@CausationId_{i}", SqlDbType.UniqueIdentifier) {Value = (Guid) entry.CausationId},
                    new SqlParameter($"@CreationTime_{i}", SqlDbType.DateTimeOffset) {Value = (DateTime) entry.CreationTime},
                    new SqlParameter($"@CorrelationId_{i}", SqlDbType.UniqueIdentifier) {Value = (Guid) entry.CorrelationId}
                });
                
                separator = ", ";
            }
            insertCommand.AppendLine("END TRY");
            insertCommand.Append("BEGIN CATCH" +
                                 "  IF ERROR_NUMBER() <> 2627" +
                                 "      THROW " +
                                 "END CATCH");

            return (insertCommand.ToString(), parameters);
        }
    }
}
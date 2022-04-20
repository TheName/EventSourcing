using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Persistence.Abstractions.Enums;
using Microsoft.Extensions.Logging;

namespace EventSourcing.Persistence.SqlServer
{
    internal class SqlServerEventStreamRepository : IEventStreamRepository
    {
        private const string TableName = "EventStream";
        
        private readonly ISqlServerEventStreamPersistenceConfiguration _configuration;
        private readonly ILogger<SqlServerEventStreamRepository> _logger;

        public SqlServerEventStreamRepository(
            ISqlServerEventStreamPersistenceConfiguration configuration,
            ILogger<SqlServerEventStreamRepository> logger)
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

        public async Task<EventStreamEntries> ReadAsync(EventStreamId streamId, CancellationToken cancellationToken)
        {
            var (sqlCommand, sqlParameters) = PrepareSelectCommand(streamId);
            
            try
            {
                return await ExecuteReader(sqlCommand, sqlParameters, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "An unexpected failure happened when trying to select event stream entries. EventStreamId to select: {EventStreamId}",
                    streamId);

                throw;
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

        private async Task<EventStreamEntries> ExecuteReader(string sqlCommandText, List<SqlParameter> parameters, CancellationToken cancellationToken)
        {
            using (var connection = new SqlConnection(_configuration.ConnectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sqlCommandText;
                    command.Parameters.AddRange(parameters.ToArray());
                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                    var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
                    using (reader)
                    {
                        var entries = new List<EventStreamEntry>();
                        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                        {
                            entries.Add(new EventStreamEntry(
                                reader.GetGuid(0),
                                reader.GetGuid(1),
                                Convert.ToUInt32(reader.GetInt64(2)),
                                new EventStreamEventDescriptor(
                                    reader.GetString(3),
                                    reader.GetString(4),
                                    reader.GetString(5),
                                    reader.GetString(6)),
                                reader.GetGuid(7),
                                reader.GetDateTimeOffset(8).UtcDateTime,
                                reader.GetGuid(9)));
                        }

                        return new EventStreamEntries(entries);
                    }
                }
            }
        }

        private static (string Command, List<SqlParameter> Parameters) PrepareInsertCommand(EventStreamEntries eventStreamEntries)
        {
            var parameters = new List<SqlParameter>();
            var insertCommand = new StringBuilder();
            insertCommand.AppendLine("BEGIN TRY");
            insertCommand.Append($"INSERT INTO {TableName} (StreamId, EntryId, EntrySequence, EventContent, EventContentSerializationFormat, EventTypeIdentifier, EventTypeIdentifierFormat, CausationId, CreationTime, CorrelationId) VALUES");

            var separator = " ";
            for (var i = 0; i < eventStreamEntries.Count; i++)
            {
                insertCommand.Append(
                    $"{separator}(@StreamId_{i}, @EntryId_{i}, @EntrySequence_{i}, @EventContent_{i}, @EventContentSerializationFormat_{i}, @EventTypeIdentifier_{i}, @EventTypeIdentifierFormat_{i}, @CausationId_{i}, @CreationTime_{i}, @CorrelationId_{i})");
                var entry = eventStreamEntries[i];
                parameters.AddRange(new []
                {
                    new SqlParameter($"@StreamId_{i}", SqlDbType.UniqueIdentifier) {Value = entry.StreamId.Value},
                    new SqlParameter($"@EntryId_{i}", SqlDbType.UniqueIdentifier) {Value = entry.EntryId.Value},
                    new SqlParameter($"@EntrySequence_{i}", SqlDbType.BigInt) {Value = entry.EntrySequence.Value},
                    new SqlParameter($"@EventContent_{i}", SqlDbType.VarChar) {Value = entry.EventDescriptor.EventContent.Value},
                    new SqlParameter($"@EventContentSerializationFormat_{i}", SqlDbType.VarChar) {Value = entry.EventDescriptor.EventContentSerializationFormat.Value},
                    new SqlParameter($"@EventTypeIdentifier_{i}", SqlDbType.VarChar) {Value = entry.EventDescriptor.EventTypeIdentifier.Value},
                    new SqlParameter($"@EventTypeIdentifierFormat_{i}", SqlDbType.VarChar) {Value = entry.EventDescriptor.EventTypeIdentifierFormat.Value},
                    new SqlParameter($"@CausationId_{i}", SqlDbType.UniqueIdentifier) {Value = entry.CausationId.Value},
                    new SqlParameter($"@CreationTime_{i}", SqlDbType.DateTimeOffset) {Value = entry.CreationTime.Value},
                    new SqlParameter($"@CorrelationId_{i}", SqlDbType.UniqueIdentifier) {Value = entry.CorrelationId.Value}
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

        private static (string Command, List<SqlParameter> Parameters) PrepareSelectCommand(EventStreamId streamId)
        {
            var command = $"SELECT StreamId, EntryId, EntrySequence, EventContent, EventContentSerializationFormat, EventTypeIdentifier, EventTypeIdentifierFormat, CausationId, CreationTime, CorrelationId FROM {TableName} WHERE StreamId = @StreamId";
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@StreamId", SqlDbType.UniqueIdentifier)
                {
                    Value = streamId.Value
                }
            };

            return (command, parameters);
        }
    }
}
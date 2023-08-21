using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Persistence.Enums;
using EventSourcing.ValueObjects;
using Microsoft.Extensions.Logging;
using Npgsql;
using NpgsqlTypes;

namespace EventSourcing.Persistence.PostgreSql
{
    internal class PostgreSqlEventStreamRepository : IEventStreamRepository
    {
        private const string TableName = "EventStream";

        private readonly IPostgreSqlEventStreamPersistenceConfiguration _configuration;
        private readonly ILogger<PostgreSqlEventStreamRepository> _logger;

        public PostgreSqlEventStreamRepository(
            IPostgreSqlEventStreamPersistenceConfiguration configuration,
            ILogger<PostgreSqlEventStreamRepository> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<EventStreamWriteResult> WriteAsync(EventStreamEntries eventStreamEntries, CancellationToken cancellationToken)
        {
            if (eventStreamEntries.Count == 0)
            {
                return EventStreamWriteResult.EmptyInput;
            }

            var (sqlCommand, sqlParameters) = PrepareInsertCommand(eventStreamEntries);

            try
            {
                var executionResult = await ExecuteCommand(sqlCommand, sqlParameters, cancellationToken).ConfigureAwait(false);
                return executionResult == eventStreamEntries.Count
                    ? EventStreamWriteResult.Success
                    : EventStreamWriteResult.UnknownFailure;
            }
            catch (Exception exception)
            {
                if (exception.Data.Contains("SqlState") && int.TryParse(exception.Data["SqlState"].ToString(), out var sqlStateCode) && sqlStateCode == 23505)
                {
                    return EventStreamWriteResult.SequenceAlreadyTaken;
                }

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

        public async Task<EventStreamEntries> ReadAsync(
            EventStreamId streamId,
            EventStreamEntrySequence minimumSequenceInclusive,
            EventStreamEntrySequence maximumSequenceInclusive,
            CancellationToken cancellationToken)
        {
            var (sqlCommand, sqlParameters) = PrepareSelectInRangeCommand(streamId, minimumSequenceInclusive, maximumSequenceInclusive);

            try
            {
                return await ExecuteReader(sqlCommand, sqlParameters, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "An unexpected failure happened when trying to select event stream entries within inclusive range {MinimumSequence} - {MaximumSequence}. EventStreamId to select: {EventStreamId}",
                    minimumSequenceInclusive,
                    maximumSequenceInclusive,
                    streamId);

                throw;
            }
        }

        private async Task<int> ExecuteCommand(string sqlCommandText, List<NpgsqlParameter> parameters, CancellationToken cancellationToken)
        {
            using (var connection = new NpgsqlConnection(_configuration.ConnectionString))
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

        private async Task<EventStreamEntries> ExecuteReader(string sqlCommandText, List<NpgsqlParameter> parameters, CancellationToken cancellationToken)
        {
            using (var connection = new NpgsqlConnection(_configuration.ConnectionString))
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
                            var creationDateTime = reader.GetFieldValue<DateTime>(8);
                            creationDateTime = creationDateTime.AddMillisecondsLeftover(reader.GetInt64(9));

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
                                new DateTime(creationDateTime.Ticks, DateTimeKind.Utc),
                                reader.GetGuid(10)));
                        }

                        return new EventStreamEntries(entries);
                    }
                }
            }
        }

        private static (string Command, List<NpgsqlParameter> Parameters) PrepareInsertCommand(EventStreamEntries eventStreamEntries)
        {
            var parameters = new List<NpgsqlParameter>();
            var insertCommand = new StringBuilder();
            insertCommand.Append($"INSERT INTO {TableName} (StreamId, EntryId, EntrySequence, EventContent, EventContentSerializationFormat, EventTypeIdentifier, EventTypeIdentifierFormat, CausationId, CreationTime, CreationTimeNanoSeconds, CorrelationId) VALUES");

            var separator = " ";
            for (var i = 0; i < eventStreamEntries.Count; i++)
            {
                insertCommand.Append(
                    $"{separator}(@StreamId_{i}, @EntryId_{i}, @EntrySequence_{i}, @EventContent_{i}, @EventContentSerializationFormat_{i}, @EventTypeIdentifier_{i}, @EventTypeIdentifierFormat_{i}, @CausationId_{i}, @CreationTime_{i}, @CreationTimeNanoSeconds_{i}, @CorrelationId_{i})");
                var entry = eventStreamEntries[i];

                var creationTimeWithUnspecifiedKind = new DateTime(entry.CreationTime.Value.Ticks, DateTimeKind.Unspecified);
                parameters.AddRange(new []
                {
                    new NpgsqlParameter($"@StreamId_{i}", NpgsqlDbType.Uuid) {Value = entry.StreamId.Value},
                    new NpgsqlParameter($"@EntryId_{i}", NpgsqlDbType.Uuid) {Value = entry.EntryId.Value},
                    new NpgsqlParameter($"@EntrySequence_{i}", NpgsqlDbType.Oid) {Value = entry.EntrySequence.Value},
                    new NpgsqlParameter($"@EventContent_{i}", NpgsqlDbType.Text) {Value = entry.EventDescriptor.EventContent.Value},
                    new NpgsqlParameter($"@EventContentSerializationFormat_{i}", NpgsqlDbType.Text) {Value = entry.EventDescriptor.EventContentSerializationFormat.Value},
                    new NpgsqlParameter($"@EventTypeIdentifier_{i}", NpgsqlDbType.Text) {Value = entry.EventDescriptor.EventTypeIdentifier.Value},
                    new NpgsqlParameter($"@EventTypeIdentifierFormat_{i}", NpgsqlDbType.Text) {Value = entry.EventDescriptor.EventTypeIdentifierFormat.Value},
                    new NpgsqlParameter($"@CausationId_{i}", NpgsqlDbType.Uuid) {Value = entry.CausationId.Value},
                    new NpgsqlParameter($"@CreationTime_{i}", NpgsqlDbType.Timestamp) {Value = creationTimeWithUnspecifiedKind.RoundToMilliseconds()},
                    new NpgsqlParameter($"@CreationTimeNanoSeconds_{i}", NpgsqlDbType.Bigint) {Value = creationTimeWithUnspecifiedKind.GetMillisecondsLeftover() },
                    new NpgsqlParameter($"@CorrelationId_{i}", NpgsqlDbType.Uuid) {Value = entry.CorrelationId.Value}
                });

                separator = ", ";
            }

            return (insertCommand.ToString(), parameters);
        }

        private static (string Command, List<NpgsqlParameter> Parameters) PrepareSelectCommand(EventStreamId streamId)
        {
            var command = $"SELECT StreamId, EntryId, EntrySequence, EventContent, EventContentSerializationFormat, EventTypeIdentifier, EventTypeIdentifierFormat, CausationId, CreationTime, CreationTimeNanoSeconds, CorrelationId FROM {TableName} WHERE StreamId = @StreamId ORDER BY EntrySequence ASC";
            var parameters = new List<NpgsqlParameter>
            {
                new NpgsqlParameter("@StreamId", NpgsqlDbType.Uuid)
                {
                    Value = streamId.Value
                }
            };

            return (command, parameters);
        }

        private static (string Command, List<NpgsqlParameter> Parameters) PrepareSelectInRangeCommand(
            EventStreamId streamId,
            EventStreamEntrySequence minimumSequence,
            EventStreamEntrySequence maximumSequence)
        {
            var command = $"SELECT StreamId, EntryId, EntrySequence, EventContent, EventContentSerializationFormat, EventTypeIdentifier, EventTypeIdentifierFormat, CausationId, CreationTime, CreationTimeNanoSeconds, CorrelationId FROM {TableName} WHERE StreamId = @StreamId AND EntrySequence >= @MinimumSequence AND EntrySequence <= @MaximumSequence ORDER BY EntrySequence ASC";
            var parameters = new List<NpgsqlParameter>
            {
                new NpgsqlParameter("@StreamId", NpgsqlDbType.Uuid)
                {
                    Value = streamId.Value
                },
                new NpgsqlParameter("@MinimumSequence", NpgsqlDbType.Oid)
                {
                    Value = minimumSequence.Value
                },
                new NpgsqlParameter("@MaximumSequence", NpgsqlDbType.Oid)
                {
                    Value = maximumSequence.Value
                },
            };

            return (command, parameters);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Persistence.ValueObjects;
using EventSourcing.ValueObjects;
using Microsoft.Data.SqlClient;

namespace EventSourcing.Persistence.SqlServer
{
    internal class SqlServerEventStreamStagingRepository : IEventStreamStagingRepository
    {
        private const string TableName = "EventStreamStaging";

        private readonly ISqlServerEventStreamPersistenceConfiguration _configuration;

        public SqlServerEventStreamStagingRepository(ISqlServerEventStreamPersistenceConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<IReadOnlyCollection<EventStreamStagedEntries>> SelectAsync(CancellationToken cancellationToken)
        {
            var (sqlCommand, sqlParameters) = PrepareSelectCommand();

            return await ExecuteReader(sqlCommand, sqlParameters, cancellationToken).ConfigureAwait(false);
        }

        public async Task<EventStreamStagedEntries> SelectAsync(EventStreamStagingId stagingId, CancellationToken cancellationToken)
        {
            var (sqlCommand, sqlParameters) = PrepareSelectByStagingIdCommand(stagingId);

            var result = await ExecuteReader(sqlCommand, sqlParameters, cancellationToken).ConfigureAwait(false);
            return result.SingleOrDefault();
        }

        public async Task InsertAsync(
            EventStreamStagedEntries stagedEntries,
            CancellationToken cancellationToken)
        {
            if (stagedEntries.Entries.Count == 0)
            {
                throw new ArgumentException("There must be at least one entry provided.", nameof(stagedEntries));
            }

            var (sqlCommand, sqlParameters) = PrepareInsertCommand(stagedEntries);

            var executionResult = await ExecuteCommand(sqlCommand, sqlParameters, cancellationToken).ConfigureAwait(false);
            if (executionResult == stagedEntries.Entries.Count)
            {
                return;
            }

            throw new Exception("Insertion failed.");
        }

        public async Task DeleteAsync(EventStreamStagingId stagingId, CancellationToken cancellationToken)
        {
            await ExecuteCommand(
                    $"DELETE FROM {TableName} WHERE StagingId = @StagingId",
                    new List<SqlParameter> {new SqlParameter("@StagingId", SqlDbType.UniqueIdentifier) {Value = stagingId.Value}},
                    cancellationToken)
                .ConfigureAwait(false);
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

        private async Task<IReadOnlyCollection<EventStreamStagedEntries>> ExecuteReader(
            string sqlCommandText,
            List<SqlParameter> parameters,
            CancellationToken cancellationToken)
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
                        var entries = new List<EventStreamEntryWithStagingIdAndStagingTime>();
                        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                        {
                            entries.Add(new EventStreamEntryWithStagingIdAndStagingTime(
                                reader.GetGuid(0),
                                reader.GetDateTimeOffset(1).UtcDateTime,
                                reader.GetGuid(2),
                                reader.GetGuid(3),
                                Convert.ToUInt32(reader.GetInt64(4)),
                                new EventStreamEventDescriptor(
                                    reader.GetString(5),
                                    reader.GetString(6),
                                    reader.GetString(7),
                                    reader.GetString(8)),
                                reader.GetGuid(9),
                                reader.GetDateTimeOffset(10).UtcDateTime,
                                reader.GetGuid(11)));
                        }

                        return GroupByStagingId(entries);
                    }
                }
            }
        }

        private static (string Command, List<SqlParameter> Parameters) PrepareInsertCommand(EventStreamStagedEntries stagedEntries)
        {
            var parameters = new List<SqlParameter>();
            var insertCommand = new StringBuilder();
            insertCommand.Append($"INSERT INTO {TableName} (StagingId, StagingTime, StreamId, EntrySequence, EntryId, EventContent, EventContentSerializationFormat, EventTypeIdentifier, EventTypeIdentifierFormat, CausationId, CreationTime, CorrelationId) VALUES");

            var separator = " ";
            for (var i = 0; i < stagedEntries.Entries.Count; i++)
            {
                insertCommand.Append(
                    $"{separator}(@StagingId_{i}, @StagingTime_{i}, @StreamId_{i}, @EntrySequence_{i}, @EntryId_{i}, @EventContent_{i}, @EventContentSerializationFormat_{i}, @EventTypeIdentifier_{i}, @EventTypeIdentifierFormat_{i}, @CausationId_{i}, @CreationTime_{i}, @CorrelationId_{i})");
                var entry = stagedEntries.Entries[i];
                parameters.AddRange(new []
                {
                    new SqlParameter($"@StagingId_{i}", SqlDbType.UniqueIdentifier) {Value = stagedEntries.StagingId.Value},
                    new SqlParameter($"@StagingTime_{i}", SqlDbType.DateTimeOffset) {Value = stagedEntries.StagingTime.Value},
                    new SqlParameter($"@StreamId_{i}", SqlDbType.UniqueIdentifier) {Value = entry.StreamId.Value},
                    new SqlParameter($"@EntrySequence_{i}", SqlDbType.BigInt) {Value = entry.EntrySequence.Value},
                    new SqlParameter($"@EntryId_{i}", SqlDbType.UniqueIdentifier) {Value = entry.EntryId.Value},
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

            return (insertCommand.ToString(), parameters);
        }

        private static (string Command, List<SqlParameter> Parameters) PrepareSelectCommand()
        {
            var command = $"SELECT StagingId, StagingTime, StreamId, EntryId, EntrySequence, EventContent, EventContentSerializationFormat, EventTypeIdentifier, EventTypeIdentifierFormat, CausationId, CreationTime, CorrelationId FROM {TableName}";
            var parameters = new List<SqlParameter>();

            return (command, parameters);
        }

        private static (string Command, List<SqlParameter> Parameters) PrepareSelectByStagingIdCommand(EventStreamStagingId stagingId)
        {
            var command = $"SELECT StagingId, StagingTime, StreamId, EntryId, EntrySequence, EventContent, EventContentSerializationFormat, EventTypeIdentifier, EventTypeIdentifierFormat, CausationId, CreationTime, CorrelationId FROM {TableName} WHERE StagingId = @StagingId";
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@StagingId", SqlDbType.UniqueIdentifier)
                {
                    Value = stagingId.Value
                }
            };

            return (command, parameters);
        }

        private static IReadOnlyCollection<EventStreamStagedEntries> GroupByStagingId(
            IEnumerable<EventStreamEntryWithStagingIdAndStagingTime> stagedEntries)
        {
            return stagedEntries
                .GroupBy(entry => entry.StagingId)
                .Select(entries =>
                    new EventStreamStagedEntries(
                        entries.Key,
                        entries
                            .Select(entry => entry.StagingTime)
                            .Distinct()
                            .Single(),
                        new EventStreamEntries(
                            entries
                                .Select(entry => (EventStreamEntry)entry))))
                .ToList();
        }

        private class EventStreamEntryWithStagingIdAndStagingTime : EventStreamEntry
        {
            public EventStreamStagingId StagingId { get; }

            public EventStreamStagedEntriesStagingTime StagingTime { get; }

            public EventStreamEntryWithStagingIdAndStagingTime(
                EventStreamStagingId stagingId,
                EventStreamStagedEntriesStagingTime stagingTime,
                EventStreamId streamId,
                EventStreamEntryId entryId,
                EventStreamEntrySequence entrySequence,
                EventStreamEventDescriptor eventDescriptor,
                EventStreamEntryCausationId causationId,
                EventStreamEntryCreationTime creationTime,
                EventStreamEntryCorrelationId correlationId)
                : base(
                    streamId,
                    entryId,
                    entrySequence,
                    eventDescriptor,
                    causationId,
                    creationTime,
                    correlationId)
            {
                StagingId = stagingId ?? throw new ArgumentNullException(nameof(stagingId));
                StagingTime = stagingTime ?? throw new ArgumentNullException(nameof(stagingTime));
            }
        }
    }
}

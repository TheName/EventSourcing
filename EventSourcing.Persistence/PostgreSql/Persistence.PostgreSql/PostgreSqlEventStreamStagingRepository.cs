using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Persistence.Abstractions.ValueObjects;
using Npgsql;
using NpgsqlTypes;

namespace EventSourcing.Persistence.PostgreSql
{
    internal class PostgreSqlEventStreamStagingRepository : IEventStreamStagingRepository
    {
        private const string TableName = "EventStreamStaging";
        
        private readonly IPostgreSqlEventStreamPersistenceConfiguration _configuration;

        public PostgreSqlEventStreamStagingRepository(IPostgreSqlEventStreamPersistenceConfiguration configuration)
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

        public async Task InsertAsync(EventStreamStagedEntries stagedEntries, CancellationToken cancellationToken)
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
                    new List<NpgsqlParameter> {new NpgsqlParameter("@StagingId", NpgsqlDbType.Uuid) {Value = stagingId.Value}},
                    cancellationToken)
                .ConfigureAwait(false);
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

        private async Task<IReadOnlyCollection<EventStreamStagedEntries>> ExecuteReader(
            string sqlCommandText,
            List<NpgsqlParameter> parameters,
            CancellationToken cancellationToken)
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
                        var entries = new List<EventStreamEntryWithStagingIdAndStagingTime>();
                        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                        {
                            var stagingDateTime = reader.GetFieldValue<DateTime>(1);
                            stagingDateTime = stagingDateTime.AddMillisecondsLeftover(reader.GetInt64(2));
                            stagingDateTime = new DateTime(stagingDateTime.Ticks, DateTimeKind.Utc);
                            
                            var creationDateTime = reader.GetFieldValue<DateTime>(11);
                            creationDateTime = creationDateTime.AddMillisecondsLeftover(reader.GetInt64(12));
                            creationDateTime = new DateTime(creationDateTime.Ticks, DateTimeKind.Utc);
                            
                            entries.Add(new EventStreamEntryWithStagingIdAndStagingTime(
                                reader.GetGuid(0),
                                stagingDateTime,
                                reader.GetGuid(3),
                                reader.GetGuid(4),
                                Convert.ToUInt32(reader.GetInt64(5)),
                                new EventStreamEventDescriptor(
                                    reader.GetString(6),
                                    reader.GetString(7),
                                    reader.GetString(8),
                                    reader.GetString(9)),
                                reader.GetGuid(10),
                                creationDateTime,
                                reader.GetGuid(13)));
                        }

                        return GroupByStagingId(entries);
                    }
                }
            }
        }

        private static (string Command, List<NpgsqlParameter> Parameters) PrepareInsertCommand(EventStreamStagedEntries stagedEntries)
        {
            var parameters = new List<NpgsqlParameter>();
            var insertCommand = new StringBuilder();
            insertCommand.Append($"INSERT INTO {TableName} (StagingId, StagingTime, StagingTimeNanoSeconds, StreamId, EntrySequence, EntryId, EventContent, EventContentSerializationFormat, EventTypeIdentifier, EventTypeIdentifierFormat, CausationId, CreationTime, CreationTimeNanoSeconds, CorrelationId) VALUES");

            var stagingTimeWithUnspecifiedKind = new DateTime(stagedEntries.StagingTime.Value.Ticks, DateTimeKind.Unspecified);
            var separator = " ";
            for (var i = 0; i < stagedEntries.Entries.Count; i++)
            {
                insertCommand.Append(
                    $"{separator}(@StagingId_{i}, @StagingTime_{i}, @StagingTimeNanoSeconds_{i}, @StreamId_{i}, @EntrySequence_{i}, @EntryId_{i}, @EventContent_{i}, @EventContentSerializationFormat_{i}, @EventTypeIdentifier_{i}, @EventTypeIdentifierFormat_{i}, @CausationId_{i}, @CreationTime_{i}, @CreationTimeNanoSeconds_{i}, @CorrelationId_{i})");
                var entry = stagedEntries.Entries[i];

                var creationTimeWithUnspecifiedKind = new DateTime(entry.CreationTime.Value.Ticks, DateTimeKind.Unspecified);
                parameters.AddRange(new []
                {
                    new NpgsqlParameter($"@StagingId_{i}", NpgsqlDbType.Uuid) {Value = stagedEntries.StagingId.Value},
                    new NpgsqlParameter($"@StagingTime_{i}", NpgsqlDbType.Timestamp) {Value = stagingTimeWithUnspecifiedKind.RoundToMilliseconds()},
                    new NpgsqlParameter($"@StagingTimeNanoSeconds_{i}", NpgsqlDbType.Bigint) {Value = stagingTimeWithUnspecifiedKind.GetMillisecondsLeftover()},
                    new NpgsqlParameter($"@StreamId_{i}", NpgsqlDbType.Uuid) {Value = entry.StreamId.Value},
                    new NpgsqlParameter($"@EntrySequence_{i}", NpgsqlDbType.Oid) {Value = entry.EntrySequence.Value},
                    new NpgsqlParameter($"@EntryId_{i}", NpgsqlDbType.Uuid) {Value = entry.EntryId.Value},
                    new NpgsqlParameter($"@EventContent_{i}", NpgsqlDbType.Text) {Value = entry.EventDescriptor.EventContent.Value},
                    new NpgsqlParameter($"@EventContentSerializationFormat_{i}", NpgsqlDbType.Text) {Value = entry.EventDescriptor.EventContentSerializationFormat.Value},
                    new NpgsqlParameter($"@EventTypeIdentifier_{i}", NpgsqlDbType.Text) {Value = entry.EventDescriptor.EventTypeIdentifier.Value},
                    new NpgsqlParameter($"@EventTypeIdentifierFormat_{i}", NpgsqlDbType.Text) {Value = entry.EventDescriptor.EventTypeIdentifierFormat.Value},
                    new NpgsqlParameter($"@CausationId_{i}", NpgsqlDbType.Uuid) {Value = entry.CausationId.Value},
                    new NpgsqlParameter($"@CreationTime_{i}", NpgsqlDbType.Timestamp) {Value = creationTimeWithUnspecifiedKind.RoundToMilliseconds()},
                    new NpgsqlParameter($"@CreationTimeNanoSeconds_{i}", NpgsqlDbType.Bigint) {Value = creationTimeWithUnspecifiedKind.GetMillisecondsLeftover()},
                    new NpgsqlParameter($"@CorrelationId_{i}", NpgsqlDbType.Uuid) {Value = entry.CorrelationId.Value}
                });
                
                separator = ", ";
            }

            return (insertCommand.ToString(), parameters);
        }

        private static (string Command, List<NpgsqlParameter> Parameters) PrepareSelectCommand()
        {
            var command = $"SELECT StagingId, StagingTime, StagingTimeNanoSeconds, StreamId, EntryId, EntrySequence, EventContent, EventContentSerializationFormat, EventTypeIdentifier, EventTypeIdentifierFormat, CausationId, CreationTime, CreationTimeNanoSeconds, CorrelationId FROM {TableName}";
            var parameters = new List<NpgsqlParameter>();

            return (command, parameters);
        }

        private static (string Command, List<NpgsqlParameter> Parameters) PrepareSelectByStagingIdCommand(EventStreamStagingId stagingId)
        {
            var command = $"SELECT StagingId, StagingTime, StagingTimeNanoSeconds, StreamId, EntryId, EntrySequence, EventContent, EventContentSerializationFormat, EventTypeIdentifier, EventTypeIdentifierFormat, CausationId, CreationTime, CreationTimeNanoSeconds, CorrelationId FROM {TableName} WHERE StagingId = @StagingId";
            var parameters = new List<NpgsqlParameter>
            {
                new NpgsqlParameter("@StagingId", NpgsqlDbType.Uuid)
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
                            .Select(entry => entry.StagingTime.Value)
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
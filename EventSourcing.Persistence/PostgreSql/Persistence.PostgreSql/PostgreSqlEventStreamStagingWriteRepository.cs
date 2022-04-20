using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Persistence.Abstractions.ValueObjects;
using Npgsql;
using NpgsqlTypes;

namespace EventSourcing.Persistence.PostgreSql
{
    internal class PostgreSqlEventStreamStagingWriteRepository : IEventStreamStagingWriteRepository
    {
        private const string TableName = "EventStreamStaging";
        
        private readonly IPostgreSqlEventStreamPersistenceConfiguration _configuration;

        public PostgreSqlEventStreamStagingWriteRepository(IPostgreSqlEventStreamPersistenceConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        
        public async Task InsertAsync(
            EventStreamStagingId stagingId,
            EventStreamEntries entries,
            CancellationToken cancellationToken)
        {
            if (entries.Count == 0)
            {
                throw new ArgumentException("There must be at least one entry provided.", nameof(entries));
            }

            var (sqlCommand, sqlParameters) = PrepareInsertCommand(stagingId, entries);
            
            var executionResult = await ExecuteCommand(sqlCommand, sqlParameters, cancellationToken).ConfigureAwait(false);
            if (executionResult == entries.Count)
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

        private static (string Command, List<NpgsqlParameter> Parameters) PrepareInsertCommand(EventStreamStagingId stagingId, EventStreamEntries eventStreamEntries)
        {
            var parameters = new List<NpgsqlParameter>();
            var insertCommand = new StringBuilder();
            insertCommand.Append($"INSERT INTO {TableName} (StagingId, StreamId, EntrySequence, EntryId, EventContent, EventContentSerializationFormat, EventTypeIdentifier, EventTypeIdentifierFormat, CausationId, CreationTime, CreationTimeNanoSeconds, CorrelationId) VALUES");

            var separator = " ";
            for (var i = 0; i < eventStreamEntries.Count; i++)
            {
                insertCommand.Append(
                    $"{separator}(@StagingId_{i}, @StreamId_{i}, @EntrySequence_{i}, @EntryId_{i}, @EventContent_{i}, @EventContentSerializationFormat_{i}, @EventTypeIdentifier_{i}, @EventTypeIdentifierFormat_{i}, @CausationId_{i}, @CreationTime_{i}, @CreationTimeNanoSeconds_{i}, @CorrelationId_{i})");
                var entry = eventStreamEntries[i];

                var creationTimeWithUnspecifiedKind = new DateTime(entry.CreationTime.Value.Ticks, DateTimeKind.Unspecified);
                parameters.AddRange(new []
                {
                    new NpgsqlParameter($"@StagingId_{i}", NpgsqlDbType.Uuid) {Value = stagingId.Value},
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
    }
}
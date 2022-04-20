using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Persistence.Abstractions.ValueObjects;

namespace EventSourcing.Persistence.SqlServer
{
    internal class SqlServerEventStreamStagingWriteRepository : IEventStreamStagingWriteRepository
    {
        private const string TableName = "EventStreamStaging";
        
        private readonly ISqlServerEventStreamPersistenceConfiguration _configuration;

        public SqlServerEventStreamStagingWriteRepository(ISqlServerEventStreamPersistenceConfiguration configuration)
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

        private static (string Command, List<SqlParameter> Parameters) PrepareInsertCommand(EventStreamStagingId stagingId, EventStreamEntries eventStreamEntries)
        {
            var parameters = new List<SqlParameter>();
            var insertCommand = new StringBuilder();
            insertCommand.Append($"INSERT INTO {TableName} (StagingId, StreamId, EntrySequence, EntryId, EventContent, EventContentSerializationFormat, EventTypeIdentifier, EventTypeIdentifierFormat, CausationId, CreationTime, CorrelationId) VALUES");

            var separator = " ";
            for (var i = 0; i < eventStreamEntries.Count; i++)
            {
                insertCommand.Append(
                    $"{separator}(@StagingId_{i}, @StreamId_{i}, @EntrySequence_{i}, @EntryId_{i}, @EventContent_{i}, @EventContentSerializationFormat_{i}, @EventTypeIdentifier_{i}, @EventTypeIdentifierFormat_{i}, @CausationId_{i}, @CreationTime_{i}, @CorrelationId_{i})");
                var entry = eventStreamEntries[i];
                parameters.AddRange(new []
                {
                    new SqlParameter($"@StagingId_{i}", SqlDbType.UniqueIdentifier) {Value = stagingId.Value},
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
    }
}
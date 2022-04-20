using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Persistence.SqlServer;
using Persistence.IntegrationTests.Base;

namespace Persistence.SqlServer.IntegrationTests
{
    public class SqlServerEventStreamStagingTestReadRepository : IEventStreamStagingTestReadRepository
    {
        private readonly ISqlServerEventStreamPersistenceConfiguration _configuration;

        public SqlServerEventStreamStagingTestReadRepository(ISqlServerEventStreamPersistenceConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        
        public async Task<IReadOnlyList<EventStreamStagingEntryTestReadModel>> SelectAsync(Guid stagingId)
        {
            await using var connection = new SqlConnection(_configuration.ConnectionString);
            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT StagingId, StreamId, EntrySequence, EntryId, EventContent, EventContentSerializationFormat, EventTypeIdentifier, EventTypeIdentifierFormat, CausationId, CreationTime, CorrelationId FROM EventStreamStaging WHERE StagingId = @StagingId";
            command.Parameters.AddWithValue("@StagingId", stagingId);
            await connection.OpenAsync(CancellationToken.None).ConfigureAwait(false);
            await using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
            var result = new List<EventStreamStagingEntryTestReadModel>();
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                result.Add(new EventStreamStagingEntryTestReadModel
                {
                    StagingId = reader.GetGuid(0),
                    StreamId = reader.GetGuid(1),
                    EntrySequence = Convert.ToUInt32(reader.GetInt64(2)),
                    EntryId = reader.GetGuid(3),
                    EventContent = reader.GetString(4),
                    EventContentSerializationFormat = reader.GetString(5),
                    EventTypeIdentifier = reader.GetString(6),
                    EventTypeIdentifierFormat = reader.GetString(7),
                    CausationId = reader.GetGuid(8),
                    CreationTime = reader.GetDateTimeOffset(9).UtcDateTime,
                    CorrelationId = reader.GetGuid(10)
                });
            }

            return result;
        }
    }
}
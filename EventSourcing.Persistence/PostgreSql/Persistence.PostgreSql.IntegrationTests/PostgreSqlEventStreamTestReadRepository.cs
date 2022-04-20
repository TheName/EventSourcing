using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Persistence.PostgreSql;
using Npgsql;
using Persistence.IntegrationTests.Base;

namespace Persistence.PostgreSql.IntegrationTests
{
    public class PostgreSqlEventStreamTestReadRepository : IEventStreamTestReadRepository
    {
        private readonly IPostgreSqlEventStreamPersistenceConfiguration _configuration;

        public PostgreSqlEventStreamTestReadRepository(IPostgreSqlEventStreamPersistenceConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        
        public async Task<IReadOnlyList<EventStreamEntryTestReadModel>> SelectAsync(Guid streamId)
        {
            await using var connection = new NpgsqlConnection(_configuration.ConnectionString);
            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT StreamId, EntrySequence, EntryId, EventContent, EventContentSerializationFormat, EventTypeIdentifier, EventTypeIdentifierFormat, CausationId, CreationTime, CreationTimeNanoSeconds, CorrelationId FROM EventStream WHERE StreamId = @StreamId";
            command.Parameters.AddWithValue("@StreamId", streamId);
            await connection.OpenAsync(CancellationToken.None).ConfigureAwait(false);
            await using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
            var result = new List<EventStreamEntryTestReadModel>();
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                var creationDateTime = reader.GetFieldValue<DateTime>(8);
                creationDateTime = creationDateTime.AddMillisecondsLeftover(reader.GetInt64(9));
                
                result.Add(new EventStreamEntryTestReadModel
                {
                    StreamId = reader.GetGuid(0),
                    EntrySequence = Convert.ToUInt32(reader.GetInt64(1)),
                    EntryId = reader.GetGuid(2),
                    EventContent = reader.GetString(3),
                    EventContentSerializationFormat = reader.GetString(4),
                    EventTypeIdentifier = reader.GetString(5),
                    EventTypeIdentifierFormat = reader.GetString(6),
                    CausationId = reader.GetGuid(7),
                    CreationTime = new DateTime(creationDateTime.Ticks, DateTimeKind.Utc),
                    CorrelationId = reader.GetGuid(10)
                });
            }

            return result;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Persistence.PostgreSql;
using Npgsql;
using Persistence.IntegrationTests.Base;

namespace Persistence.PostgreSql.IntegrationTests
{
    public class PostgreSqlEventStreamStagingTestRepository : IEventStreamStagingTestRepository
    {
        private readonly IPostgreSqlEventStreamPersistenceConfiguration _configuration;

        public PostgreSqlEventStreamStagingTestRepository(IPostgreSqlEventStreamPersistenceConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<IReadOnlyList<EventStreamStagingEntryTestReadModel>> SelectAsync(Guid stagingId)
        {
            using (var connection = new NpgsqlConnection(_configuration.ConnectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText =
                        "SELECT StagingId, StreamId, EntrySequence, EntryId, EventContent, EventContentSerializationFormat, EventTypeIdentifier, EventTypeIdentifierFormat, CausationId, CreationTime, CreationTimeNanoSeconds, CorrelationId FROM EventStreamStaging WHERE StagingId = @StagingId";
                    command.Parameters.AddWithValue("@StagingId", stagingId);
                    await connection.OpenAsync(CancellationToken.None).ConfigureAwait(false);
                    using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        var result = new List<EventStreamStagingEntryTestReadModel>();
                        while (await reader.ReadAsync().ConfigureAwait(false))
                        {
                            var creationDateTime = reader.GetFieldValue<DateTime>(9);
                            creationDateTime = creationDateTime.AddMillisecondsLeftover(reader.GetInt64(10));

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
                                CreationTime = new DateTime(creationDateTime.Ticks, DateTimeKind.Utc),
                                CorrelationId = reader.GetGuid(11)
                            });
                        }

                        return result;
                    }
                }
            }
        }

        public async Task DeleteAllAsync()
        {
            using (var connection = new NpgsqlConnection(_configuration.ConnectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "DELETE FROM EventStreamStaging";
                    await connection.OpenAsync(CancellationToken.None).ConfigureAwait(false);
                    await command.ExecuteNonQueryAsync(CancellationToken.None).ConfigureAwait(false);
                }
            }
        }
    }
}

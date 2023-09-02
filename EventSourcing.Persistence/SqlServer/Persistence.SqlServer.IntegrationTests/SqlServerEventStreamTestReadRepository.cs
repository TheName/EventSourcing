using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Persistence.SqlServer;
using Persistence.IntegrationTests.Base;

namespace Persistence.SqlServer.IntegrationTests
{
    public class SqlServerEventStreamTestReadRepository : IEventStreamTestReadRepository
    {
        private readonly ISqlServerEventStreamPersistenceConfiguration _configuration;

        public SqlServerEventStreamTestReadRepository(ISqlServerEventStreamPersistenceConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<IReadOnlyList<EventStreamEntryTestReadModel>> SelectAsync(Guid streamId)
        {
            using (var connection = new SqlConnection(_configuration.ConnectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText =
                        "SELECT StreamId, EntrySequence, EntryId, EventContent, EventContentSerializationFormat, EventTypeIdentifier, EventTypeIdentifierFormat, CausationId, CreationTime, CorrelationId FROM EventStream WHERE StreamId = @StreamId";
                    command.Parameters.AddWithValue("@StreamId", streamId);
                    await connection.OpenAsync(CancellationToken.None).ConfigureAwait(false);
                    using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        var result = new List<EventStreamEntryTestReadModel>();
                        while (await reader.ReadAsync().ConfigureAwait(false))
                        {
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
                                CreationTime = reader.GetDateTimeOffset(8).UtcDateTime,
                                CorrelationId = reader.GetGuid(9)
                            });
                        }

                        return result;
                    }
                }
            }
        }
    }
}

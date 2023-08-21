using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.ForgettablePayloads.ValueObjects;
using EventSourcing.ValueObjects;
using Microsoft.Extensions.Logging;
using Npgsql;
using NpgsqlTypes;

namespace EventSourcing.ForgettablePayloads.Persistence.PostgreSql
{
    internal class ForgettablePayloadStoragePostgreSqlRepository : IForgettablePayloadStorageRepository
    {
        private const string TableName = "\"EventStream.ForgettablePayloads\"";
        
        private readonly IPostgreSqlEventStreamForgettablePayloadPersistenceConfiguration _configuration;
        private readonly ILogger<ForgettablePayloadStoragePostgreSqlRepository> _logger;

        public ForgettablePayloadStoragePostgreSqlRepository(
            IPostgreSqlEventStreamForgettablePayloadPersistenceConfiguration configuration,
            ILogger<ForgettablePayloadStoragePostgreSqlRepository> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<IReadOnlyCollection<ForgettablePayloadDescriptor>> ReadAsync(EventStreamId eventStreamId, CancellationToken cancellationToken)
        {
            var sqlCommand = $"SELECT EventStreamId, EventStreamEntryId, PayloadId, PayloadState, PayloadCreationTime, PayloadCreationTimeNanoSeconds, PayloadLastModifiedTime, PayloadLastModifiedTimeNanoSeconds, PayloadSequence, PayloadContent, PayloadContentSerializationFormat, PayloadTypeIdentifier, PayloadTypeIdentifierFormat FROM {TableName} WHERE EventStreamId = @EventStreamId";
            var sqlParameters = new List<NpgsqlParameter>
            {
                new NpgsqlParameter("@EventStreamId", NpgsqlDbType.Uuid) { Value = eventStreamId.Value }
            };
            
            try
            {
                return await ExecuteReader(sqlCommand, sqlParameters, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "An unexpected failure happened when trying to select forgettable payloads for EventStreamId \"{EventStreamId}\"",
                    eventStreamId);

                throw;
            }
        }

        public async Task<IReadOnlyCollection<ForgettablePayloadDescriptor>> ReadAsync(
            EventStreamId eventStreamId,
            EventStreamEntryId eventStreamEntryId,
            CancellationToken cancellationToken)
        {
            var sqlCommand = $"SELECT EventStreamId, EventStreamEntryId, PayloadId, PayloadState, PayloadCreationTime, PayloadCreationTimeNanoSeconds, PayloadLastModifiedTime, PayloadLastModifiedTimeNanoSeconds, PayloadSequence, PayloadContent, PayloadContentSerializationFormat, PayloadTypeIdentifier, PayloadTypeIdentifierFormat FROM {TableName} WHERE EventStreamId = @EventStreamId AND EventStreamEntryId = @EventStreamEntryId";
            var sqlParameters = new List<NpgsqlParameter>
            {
                new NpgsqlParameter("@EventStreamId", NpgsqlDbType.Uuid) { Value = eventStreamId.Value },
                new NpgsqlParameter("@EventStreamEntryId", NpgsqlDbType.Uuid) { Value = eventStreamEntryId.Value }
            };
            
            try
            {
                return await ExecuteReader(sqlCommand, sqlParameters, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "An unexpected failure happened when trying to select forgettable payloads for EventStreamId \"{EventStreamId}\" and EventStreamEntryId \"{EventStreamEntryId}\"",
                    eventStreamId,
                    eventStreamEntryId);

                throw;
            }
        }

        public async Task<ForgettablePayloadDescriptor> ReadAsync(ForgettablePayloadId forgettablePayloadId, CancellationToken cancellationToken)
        {
            var sqlCommand = $"SELECT EventStreamId, EventStreamEntryId, PayloadId, PayloadState, PayloadCreationTime, PayloadCreationTimeNanoSeconds, PayloadLastModifiedTime, PayloadLastModifiedTimeNanoSeconds, PayloadSequence, PayloadContent, PayloadContentSerializationFormat, PayloadTypeIdentifier, PayloadTypeIdentifierFormat FROM {TableName} WHERE PayloadId = @PayloadId";
            var sqlParameters = new List<NpgsqlParameter>
            {
                new NpgsqlParameter("@PayloadId", NpgsqlDbType.Uuid) { Value = forgettablePayloadId.Value }
            };
            
            try
            {
                var result = await ExecuteReader(sqlCommand, sqlParameters, cancellationToken).ConfigureAwait(false);
                return result.SingleOrDefault();
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "An unexpected failure happened when trying to select forgettable payloads for PayloadId \"{PayloadId}\"",
                    forgettablePayloadId);

                throw;
            }
        }

        public async Task<IReadOnlyCollection<ForgettablePayloadDescriptor>> ReadAsync(ForgettablePayloadState forgettablePayloadState, CancellationToken cancellationToken)
        {
            var sqlCommand = $"SELECT EventStreamId, EventStreamEntryId, PayloadId, PayloadState, PayloadCreationTime, PayloadCreationTimeNanoSeconds, PayloadLastModifiedTime, PayloadLastModifiedTimeNanoSeconds, PayloadSequence, PayloadContent, PayloadContentSerializationFormat, PayloadTypeIdentifier, PayloadTypeIdentifierFormat FROM {TableName} WHERE PayloadState = @PayloadState";
            var sqlParameters = new List<NpgsqlParameter>
            {
                new NpgsqlParameter("@PayloadState", NpgsqlDbType.Text) { Value = forgettablePayloadState.Value }
            };
            
            try
            {
                return await ExecuteReader(sqlCommand, sqlParameters, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "An unexpected failure happened when trying to select forgettable payloads with state \"{PayloadState}\"",
                    forgettablePayloadState);

                throw;
            }
        }

        public async Task<bool> TryInsertAsync(ForgettablePayloadDescriptor forgettablePayloadDescriptor, CancellationToken cancellationToken)
        {
            var sqlCommand = $"INSERT INTO {TableName} (EventStreamId, EventStreamEntryId, PayloadId, PayloadState, PayloadCreationTime, PayloadCreationTimeNanoSeconds, PayloadLastModifiedTime, PayloadLastModifiedTimeNanoSeconds, PayloadSequence, PayloadContent, PayloadContentSerializationFormat, PayloadTypeIdentifier, PayloadTypeIdentifierFormat) VALUES (@EventStreamId, @EventStreamEntryId, @PayloadId, @PayloadState, @PayloadCreationTime, @PayloadCreationTimeNanoSeconds, @PayloadLastModifiedTime, @PayloadLastModifiedTimeNanoSeconds, @PayloadSequence, @PayloadContent, @PayloadContentSerializationFormat, @PayloadTypeIdentifier, @PayloadTypeIdentifierFormat)";
            var creationTimeWithUnspecifiedKind = new DateTime(forgettablePayloadDescriptor.PayloadCreationTime.Value.Ticks, DateTimeKind.Unspecified);
            var lastModifiedTimeWithUnspecifiedKind = new DateTime(forgettablePayloadDescriptor.PayloadLastModifiedTime.Value.Ticks, DateTimeKind.Unspecified);
            var sqlParameters = new List<NpgsqlParameter>
            {
                new NpgsqlParameter("@EventStreamId", NpgsqlDbType.Uuid) { Value = forgettablePayloadDescriptor.EventStreamId.Value },
                new NpgsqlParameter("@EventStreamEntryId", NpgsqlDbType.Uuid) { Value = forgettablePayloadDescriptor.EventStreamEntryId.Value },
                new NpgsqlParameter("@PayloadId", NpgsqlDbType.Uuid) { Value = forgettablePayloadDescriptor.PayloadId.Value },
                new NpgsqlParameter("@PayloadState", NpgsqlDbType.Text) { Value = forgettablePayloadDescriptor.PayloadState.Value },
                new NpgsqlParameter("@PayloadCreationTime", NpgsqlDbType.Timestamp) { Value = creationTimeWithUnspecifiedKind.RoundToMilliseconds() },
                new NpgsqlParameter("@PayloadCreationTimeNanoSeconds", NpgsqlDbType.Bigint) { Value = creationTimeWithUnspecifiedKind.GetMillisecondsLeftover() },
                new NpgsqlParameter("@PayloadLastModifiedTime", NpgsqlDbType.Timestamp) { Value = lastModifiedTimeWithUnspecifiedKind.RoundToMilliseconds() },
                new NpgsqlParameter("@PayloadLastModifiedTimeNanoSeconds", NpgsqlDbType.Bigint) { Value = lastModifiedTimeWithUnspecifiedKind.GetMillisecondsLeftover() },
                new NpgsqlParameter("@PayloadSequence", NpgsqlDbType.Oid) { Value = forgettablePayloadDescriptor.PayloadSequence.Value },
                new NpgsqlParameter("@PayloadContent", NpgsqlDbType.Text) { Value = forgettablePayloadDescriptor.PayloadContent.Value },
                new NpgsqlParameter("@PayloadContentSerializationFormat", NpgsqlDbType.Text) { Value = forgettablePayloadDescriptor.PayloadContentSerializationFormat.Value },
                new NpgsqlParameter("@PayloadTypeIdentifier", NpgsqlDbType.Text) { Value = forgettablePayloadDescriptor.PayloadTypeIdentifier.Value },
                new NpgsqlParameter("@PayloadTypeIdentifierFormat", NpgsqlDbType.Text) { Value = forgettablePayloadDescriptor.PayloadTypeIdentifierFormat.Value }
            };
            
            try
            {
                var executionResult = await ExecuteCommand(sqlCommand, sqlParameters, cancellationToken).ConfigureAwait(false);
                return executionResult == 1;
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "An unexpected failure happened when trying to insert a forgettable payload descriptor. Instance: {ForgettablePayloadDescriptor}",
                    forgettablePayloadDescriptor);
                
                throw;
            }
        }

        public async Task<bool> TryUpdateAsync(ForgettablePayloadDescriptor forgettablePayloadDescriptor, CancellationToken cancellationToken)
        {
            var sqlCommand = $"UPDATE {TableName} SET EventStreamId=@EventStreamId, EventStreamEntryId=@EventStreamEntryId, PayloadId=@PayloadId, PayloadState=@PayloadState, PayloadCreationTime=@PayloadCreationTime, PayloadCreationTimeNanoSeconds=@PayloadCreationTimeNanoSeconds, PayloadLastModifiedTime=@PayloadLastModifiedTime, PayloadLastModifiedTimeNanoSeconds=@PayloadLastModifiedTimeNanoSeconds, PayloadSequence=@PayloadSequence, PayloadContent=@PayloadContent, PayloadContentSerializationFormat=@PayloadContentSerializationFormat, PayloadTypeIdentifier=@PayloadTypeIdentifier, PayloadTypeIdentifierFormat=@PayloadTypeIdentifierFormat WHERE EventStreamId=@EventStreamId AND EventStreamEntryId=@EventStreamEntryId AND PayloadId=@PayloadId AND PayloadSequence=@ExpectedPayloadSequence";
            var creationTimeWithUnspecifiedKind = new DateTime(forgettablePayloadDescriptor.PayloadCreationTime.Value.Ticks, DateTimeKind.Unspecified);
            var lastModifiedTimeWithUnspecifiedKind = new DateTime(forgettablePayloadDescriptor.PayloadLastModifiedTime.Value.Ticks, DateTimeKind.Unspecified);
            var sqlParameters = new List<NpgsqlParameter>
            {
                new NpgsqlParameter("@EventStreamId", NpgsqlDbType.Uuid) { Value = forgettablePayloadDescriptor.EventStreamId.Value },
                new NpgsqlParameter("@EventStreamEntryId", NpgsqlDbType.Uuid) { Value = forgettablePayloadDescriptor.EventStreamEntryId.Value },
                new NpgsqlParameter("@PayloadId", NpgsqlDbType.Uuid) { Value = forgettablePayloadDescriptor.PayloadId.Value },
                new NpgsqlParameter("@PayloadState", NpgsqlDbType.Text) { Value = forgettablePayloadDescriptor.PayloadState.Value },
                new NpgsqlParameter("@PayloadCreationTime", NpgsqlDbType.Timestamp) { Value = creationTimeWithUnspecifiedKind.RoundToMilliseconds() },
                new NpgsqlParameter("@PayloadCreationTimeNanoSeconds", NpgsqlDbType.Bigint) { Value = creationTimeWithUnspecifiedKind.GetMillisecondsLeftover() },
                new NpgsqlParameter("@PayloadLastModifiedTime", NpgsqlDbType.Timestamp) { Value = lastModifiedTimeWithUnspecifiedKind.RoundToMilliseconds() },
                new NpgsqlParameter("@PayloadLastModifiedTimeNanoSeconds", NpgsqlDbType.Bigint) { Value = lastModifiedTimeWithUnspecifiedKind.GetMillisecondsLeftover() },
                new NpgsqlParameter("@PayloadSequence", NpgsqlDbType.Oid) { Value = forgettablePayloadDescriptor.PayloadSequence.Value },
                new NpgsqlParameter("@PayloadContent", NpgsqlDbType.Text) { Value = forgettablePayloadDescriptor.PayloadContent.Value },
                new NpgsqlParameter("@PayloadContentSerializationFormat", NpgsqlDbType.Text) { Value = forgettablePayloadDescriptor.PayloadContentSerializationFormat.Value },
                new NpgsqlParameter("@PayloadTypeIdentifier", NpgsqlDbType.Text) { Value = forgettablePayloadDescriptor.PayloadTypeIdentifier.Value },
                new NpgsqlParameter("@PayloadTypeIdentifierFormat", NpgsqlDbType.Text) { Value = forgettablePayloadDescriptor.PayloadTypeIdentifierFormat.Value },
                new NpgsqlParameter("@ExpectedPayloadSequence", NpgsqlDbType.Oid) { Value = forgettablePayloadDescriptor.PayloadSequence.Value - 1 }
            };
            
            try
            {
                var executionResult = await ExecuteCommand(sqlCommand, sqlParameters, cancellationToken).ConfigureAwait(false);
                return executionResult == 1;
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "An unexpected failure happened when trying to update a forgettable payload descriptor. Instance: {ForgettablePayloadDescriptor}",
                    forgettablePayloadDescriptor);
                
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

        private async Task<IReadOnlyCollection<ForgettablePayloadDescriptor>> ExecuteReader(string sqlCommandText, List<NpgsqlParameter> parameters, CancellationToken cancellationToken)
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
                        var forgettablePayloadDescriptors = new List<ForgettablePayloadDescriptor>();
                        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                        {
                            var creationDateTime = reader.GetFieldValue<DateTime>(4);
                            creationDateTime = creationDateTime.AddMillisecondsLeftover(reader.GetInt64(5));
                            
                            var lastModifiedDateTime = reader.GetFieldValue<DateTime>(6);
                            lastModifiedDateTime = lastModifiedDateTime.AddMillisecondsLeftover(reader.GetInt64(7));
                            
                            forgettablePayloadDescriptors.Add(new ForgettablePayloadDescriptor(
                                reader.GetGuid(0),
                                reader.GetGuid(1),
                                reader.GetGuid(2),
                                reader.GetString(3),
                                new DateTime(creationDateTime.Ticks, DateTimeKind.Utc),
                                new DateTime(lastModifiedDateTime.Ticks, DateTimeKind.Utc),
                                Convert.ToUInt32(reader.GetInt64(8)),
                                reader.GetString(9),
                                reader.GetString(10),
                                reader.GetString(11),
                                reader.GetString(12)));
                        }

                        return forgettablePayloadDescriptors;
                    }
                }
            }
        }
    }
}
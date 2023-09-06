using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.ForgettablePayloads.ValueObjects;
using EventSourcing.ValueObjects;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace EventSourcing.ForgettablePayloads.Persistence.SqlServer
{
    internal class ForgettablePayloadStorageSqlServerRepository : IForgettablePayloadStorageRepository
    {
        private const string TableName = "[EventStream.ForgettablePayloads]";

        private readonly ISqlServerEventStreamForgettablePayloadPersistenceConfiguration _configuration;
        private readonly ILogger<ForgettablePayloadStorageSqlServerRepository> _logger;

        public ForgettablePayloadStorageSqlServerRepository(
            ISqlServerEventStreamForgettablePayloadPersistenceConfiguration configuration,
            ILogger<ForgettablePayloadStorageSqlServerRepository> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IReadOnlyCollection<ForgettablePayloadDescriptor>> ReadAsync(EventStreamId eventStreamId, CancellationToken cancellationToken)
        {
            var sqlCommand = $"SELECT EventStreamId, EventStreamEntryId, PayloadId, PayloadState, PayloadCreationTime, PayloadLastModifiedTime, PayloadSequence, PayloadContent, PayloadContentSerializationFormat, PayloadTypeIdentifier, PayloadTypeIdentifierFormat FROM {TableName} WHERE EventStreamId = @EventStreamId";
            var sqlParameters = new List<SqlParameter>
            {
                new SqlParameter("@EventStreamId", SqlDbType.UniqueIdentifier) { Value = eventStreamId.Value }
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
            var sqlCommand = $"SELECT EventStreamId, EventStreamEntryId, PayloadId, PayloadState, PayloadCreationTime, PayloadLastModifiedTime, PayloadSequence, PayloadContent, PayloadContentSerializationFormat, PayloadTypeIdentifier, PayloadTypeIdentifierFormat FROM {TableName} WHERE EventStreamId = @EventStreamId AND EventStreamEntryId = @EventStreamEntryId";
            var sqlParameters = new List<SqlParameter>
            {
                new SqlParameter("@EventStreamId", SqlDbType.UniqueIdentifier) { Value = eventStreamId.Value },
                new SqlParameter("@EventStreamEntryId", SqlDbType.UniqueIdentifier) { Value = eventStreamEntryId.Value }
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
            var sqlCommand = $"SELECT EventStreamId, EventStreamEntryId, PayloadId, PayloadState, PayloadCreationTime, PayloadLastModifiedTime, PayloadSequence, PayloadContent, PayloadContentSerializationFormat, PayloadTypeIdentifier, PayloadTypeIdentifierFormat FROM {TableName} WHERE PayloadId = @PayloadId";
            var sqlParameters = new List<SqlParameter>
            {
                new SqlParameter("@PayloadId", SqlDbType.UniqueIdentifier) { Value = forgettablePayloadId.Value }
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
            var sqlCommand = $"SELECT EventStreamId, EventStreamEntryId, PayloadId, PayloadState, PayloadCreationTime, PayloadLastModifiedTime, PayloadSequence, PayloadContent, PayloadContentSerializationFormat, PayloadTypeIdentifier, PayloadTypeIdentifierFormat FROM {TableName} WHERE PayloadState = @PayloadState";
            var sqlParameters = new List<SqlParameter>
            {
                new SqlParameter("@PayloadState", SqlDbType.VarChar) { Value = forgettablePayloadState.Value }
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
            var sqlCommand = $"INSERT INTO {TableName} (EventStreamId, EventStreamEntryId, PayloadId, PayloadState, PayloadCreationTime, PayloadLastModifiedTime, PayloadSequence, PayloadContent, PayloadContentSerializationFormat, PayloadTypeIdentifier, PayloadTypeIdentifierFormat) VALUES (@EventStreamId, @EventStreamEntryId, @PayloadId, @PayloadState, @PayloadCreationTime, @PayloadLastModifiedTime, @PayloadSequence, @PayloadContent, @PayloadContentSerializationFormat, @PayloadTypeIdentifier, @PayloadTypeIdentifierFormat)";
            var sqlParameters = new List<SqlParameter>
            {
                new SqlParameter("@EventStreamId", SqlDbType.UniqueIdentifier) { Value = forgettablePayloadDescriptor.EventStreamId.Value },
                new SqlParameter("@EventStreamEntryId", SqlDbType.UniqueIdentifier) { Value = forgettablePayloadDescriptor.EventStreamEntryId.Value },
                new SqlParameter("@PayloadId", SqlDbType.UniqueIdentifier) { Value = forgettablePayloadDescriptor.PayloadId.Value },
                new SqlParameter("@PayloadState", SqlDbType.Text) { Value = forgettablePayloadDescriptor.PayloadState.Value },
                new SqlParameter("@PayloadCreationTime", SqlDbType.DateTimeOffset) { Value = forgettablePayloadDescriptor.PayloadCreationTime.Value },
                new SqlParameter("@PayloadLastModifiedTime", SqlDbType.DateTimeOffset) { Value = forgettablePayloadDescriptor.PayloadLastModifiedTime.Value },
                new SqlParameter("@PayloadSequence", SqlDbType.BigInt) { Value = forgettablePayloadDescriptor.PayloadSequence.Value },
                new SqlParameter("@PayloadContent", SqlDbType.VarChar) { Value = forgettablePayloadDescriptor.PayloadContent.Value },
                new SqlParameter("@PayloadContentSerializationFormat", SqlDbType.VarChar) { Value = forgettablePayloadDescriptor.PayloadContentSerializationFormat.Value },
                new SqlParameter("@PayloadTypeIdentifier", SqlDbType.VarChar) { Value = forgettablePayloadDescriptor.PayloadTypeIdentifier.Value },
                new SqlParameter("@PayloadTypeIdentifierFormat", SqlDbType.VarChar) { Value = forgettablePayloadDescriptor.PayloadTypeIdentifierFormat.Value }
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
            var sqlCommand = $"UPDATE {TableName} SET EventStreamId=@EventStreamId, EventStreamEntryId=@EventStreamEntryId, PayloadId=@PayloadId, PayloadState=@PayloadState, PayloadCreationTime=@PayloadCreationTime, PayloadLastModifiedTime=@PayloadLastModifiedTime, PayloadSequence=@PayloadSequence, PayloadContent=@PayloadContent, PayloadContentSerializationFormat=@PayloadContentSerializationFormat, PayloadTypeIdentifier=@PayloadTypeIdentifier, PayloadTypeIdentifierFormat=@PayloadTypeIdentifierFormat WHERE EventStreamId=@EventStreamId AND EventStreamEntryId=@EventStreamEntryId AND PayloadId=@PayloadId AND PayloadSequence=@ExpectedPayloadSequence";
            var sqlParameters = new List<SqlParameter>
            {
                new SqlParameter("@EventStreamId", SqlDbType.UniqueIdentifier) { Value = forgettablePayloadDescriptor.EventStreamId.Value },
                new SqlParameter("@EventStreamEntryId", SqlDbType.UniqueIdentifier) { Value = forgettablePayloadDescriptor.EventStreamEntryId.Value },
                new SqlParameter("@PayloadId", SqlDbType.UniqueIdentifier) { Value = forgettablePayloadDescriptor.PayloadId.Value },
                new SqlParameter("@PayloadState", SqlDbType.VarChar) { Value = forgettablePayloadDescriptor.PayloadState.Value },
                new SqlParameter("@PayloadCreationTime", SqlDbType.DateTimeOffset) { Value = forgettablePayloadDescriptor.PayloadCreationTime.Value },
                new SqlParameter("@PayloadLastModifiedTime", SqlDbType.DateTimeOffset) { Value = forgettablePayloadDescriptor.PayloadLastModifiedTime.Value },
                new SqlParameter("@PayloadSequence", SqlDbType.BigInt) { Value = forgettablePayloadDescriptor.PayloadSequence.Value },
                new SqlParameter("@PayloadContent", SqlDbType.VarChar) { Value = forgettablePayloadDescriptor.PayloadContent.Value },
                new SqlParameter("@PayloadContentSerializationFormat", SqlDbType.VarChar) { Value = forgettablePayloadDescriptor.PayloadContentSerializationFormat.Value },
                new SqlParameter("@PayloadTypeIdentifier", SqlDbType.VarChar) { Value = forgettablePayloadDescriptor.PayloadTypeIdentifier.Value },
                new SqlParameter("@PayloadTypeIdentifierFormat", SqlDbType.VarChar) { Value = forgettablePayloadDescriptor.PayloadTypeIdentifierFormat.Value },
                new SqlParameter("@ExpectedPayloadSequence", SqlDbType.BigInt) { Value = forgettablePayloadDescriptor.PayloadSequence.Value - 1 }
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

        private async Task<IReadOnlyCollection<ForgettablePayloadDescriptor>> ExecuteReader(string sqlCommandText, List<SqlParameter> parameters, CancellationToken cancellationToken)
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
                        var forgettablePayloadDescriptors = new List<ForgettablePayloadDescriptor>();
                        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                        {
                            forgettablePayloadDescriptors.Add(new ForgettablePayloadDescriptor(
                                reader.GetGuid(0),
                                reader.GetGuid(1),
                                reader.GetGuid(2),
                                reader.GetString(3),
                                reader.GetDateTimeOffset(4).UtcDateTime,
                                reader.GetDateTimeOffset(5).UtcDateTime,
                                Convert.ToUInt32(reader.GetInt64(6)),
                                reader.GetString(7),
                                reader.GetString(8),
                                reader.GetString(9),
                                reader.GetString(10)));
                        }

                        return forgettablePayloadDescriptors;
                    }
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions;
using EventSourcing.Persistence;
using EventSourcing.Persistence.Abstractions;
using EventSourcing.Persistence.SqlServer;
using TestHelpers.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace Persistence.SqlServer.IntegrationTests
{
    [Collection(nameof(SqlServerCollectionDefinition))]
    public class SqlServerEventStreamStagingWriteRepository_Should
    {
        private readonly SqlServerCollectionFixture _fixture;

        private IEventStreamStagingWriteRepository Repository => _fixture.GetService<IEventStreamStagingWriteRepository>();

        public SqlServerEventStreamStagingWriteRepository_Should(SqlServerCollectionFixture fixture, ITestOutputHelper testOutputHelper)
        {
            _fixture = fixture.SetTestOutputHelper(testOutputHelper);
        }

        [Theory]
        [AutoMoqData]
        public async Task InsertSingleEntry(EventStreamStagingId stagingId, EventStreamEntry entry)
        {
            await Repository.InsertAsync(stagingId, new EventStreamEntries(new[] {entry}), CancellationToken.None);

            var result = await SelectAsync(stagingId);
            var singleEntry = Assert.Single(result);
            Assert.NotNull(singleEntry);
            Assert.Equal<Guid>(stagingId, singleEntry.StagingId);
            AssertEqual(entry, singleEntry);
        }

        [Theory]
        [AutoMoqData]
        public async Task Throw_When_WritingSingleEntryUnderSameStagingIdTwice(EventStreamStagingId stagingId, EventStreamEntry entry)
        {
            await Repository.InsertAsync(stagingId, new EventStreamEntries(new[] {entry}), CancellationToken.None);
            
            await Assert.ThrowsAsync<SqlException>(() => Repository.InsertAsync(stagingId, new EventStreamEntries(new[] {entry}), CancellationToken.None));
        }

        [Theory]
        [AutoMoqData]
        public async Task NotThrow_When_WritingSingleEntryUnderDifferentStagingIdTwice(
            EventStreamStagingId stagingId,
            EventStreamStagingId differentStagingId,
            EventStreamEntry entry)
        {
            await Repository.InsertAsync(stagingId, new EventStreamEntries(new[] {entry}), CancellationToken.None);
            
            await Repository.InsertAsync(differentStagingId, new EventStreamEntries(new[] {entry}), CancellationToken.None);
        }
        
        [Theory]
        [AutoMoqData]
        public async Task InsertMultipleEntries(EventStreamStagingId stagingId, EventStreamEntries entries)
        {
            await Repository.InsertAsync(stagingId, entries, CancellationToken.None);

            var result = await SelectAsync(stagingId);
            Assert.Equal(entries.Count, result.Count);
            for (var i = 0; i < entries.Count; i++)
            {
                AssertEqual(entries[i], result[i]);
            }
        }
        
        [Theory]
        [AutoMoqData]
        public async Task Throw_When_WritingMultipleEntriesUnderSameStagingIdTwice(EventStreamStagingId stagingId, EventStreamEntries entries)
        {
            await Repository.InsertAsync(stagingId, entries, CancellationToken.None);
            
            await Assert.ThrowsAsync<SqlException>(() => Repository.InsertAsync(stagingId, entries, CancellationToken.None));
        }
        
        [Theory]
        [AutoMoqData]
        public async Task NotThrow_When_WritingMultipleEntriesUnderDifferentStagingIdTwice(
            EventStreamStagingId stagingId,
            EventStreamStagingId differentStagingId,
            EventStreamEntries entries)
        {
            await Repository.InsertAsync(stagingId, entries, CancellationToken.None);
            
            await Repository.InsertAsync(differentStagingId, entries, CancellationToken.None);
        }

        [Theory]
        [AutoMoqData]
        public async Task DeleteEntries(EventStreamStagingId stagingId, EventStreamEntries entries)
        {
            await Repository.InsertAsync(stagingId, entries, CancellationToken.None);
            await Repository.DeleteAsync(stagingId, CancellationToken.None);

            var result = await SelectAsync(stagingId);
            Assert.Empty(result);
        }

        [Theory]
        [AutoMoqData]
        public async Task NotThrow_When_DeletingNonExistingStagingId(EventStreamStagingId stagingId)
        {
            var result = await SelectAsync(stagingId);
            Assert.Empty(result);
            
            await Repository.DeleteAsync(stagingId, CancellationToken.None);

            result = await SelectAsync(stagingId);
            Assert.Empty(result);
        }

        private async Task<IReadOnlyList<EventStreamStagingEntryReadModel>> SelectAsync(Guid stagingId)
        {
            await using var connection = new SqlConnection(_fixture.GetService<ISqlServerEventStreamPersistenceConfiguration>().ConnectionString);
            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT StagingId, StreamId, EntrySequence, EntryId, EventContent, EventTypeIdentifier, CausationId, CreationTime, CorrelationId FROM EventStreamStaging WHERE StagingId = @StagingId";
            command.Parameters.AddWithValue("@StagingId", stagingId);
            await connection.OpenAsync(CancellationToken.None).ConfigureAwait(false);
            await using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
            var result = new List<EventStreamStagingEntryReadModel>();
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                result.Add(new EventStreamStagingEntryReadModel
                {
                    StagingId = reader.GetGuid(0),
                    StreamId = reader.GetGuid(1),
                    EntrySequence = Convert.ToUInt32(reader.GetInt64(2)),
                    EntryId = reader.GetGuid(3),
                    EventContent = reader.GetString(4),
                    EventTypeIdentifier = reader.GetString(5),
                    CausationId = reader.GetGuid(6),
                    CreationTime = reader.GetDateTimeOffset(7).UtcDateTime,
                    CorrelationId = reader.GetGuid(8)
                });
            }

            return result;
        }
        
        private class EventStreamStagingEntryReadModel
        {
            public Guid StagingId { get; set; }
            public Guid StreamId { get; set; }
            public uint EntrySequence { get; set; }
            public Guid EntryId { get; set; }
            public string EventContent { get; set; }
            public string EventTypeIdentifier { get; set; }
            public Guid CausationId { get; set; }
            public DateTime CreationTime { get; set; }
            public Guid CorrelationId { get; set; }
        }

        private static void AssertEqual(EventStreamEntry entry, EventStreamStagingEntryReadModel readModel)
        {
            Assert.Equal<EventStreamId>(entry.StreamId, readModel.StreamId);
            Assert.Equal<EventStreamEntrySequence>(entry.EntrySequence, readModel.EntrySequence);
            Assert.Equal<EventStreamEntryId>(entry.EntryId, readModel.EntryId);
            Assert.Equal<EventStreamEventContent>(entry.EventDescriptor.EventContent, readModel.EventContent);
            Assert.Equal<EventStreamEventTypeIdentifier>(entry.EventDescriptor.EventTypeIdentifier, readModel.EventTypeIdentifier);
            Assert.Equal<EventStreamEntryCausationId>(entry.CausationId, readModel.CausationId);
            Assert.Equal<EventStreamEntryCreationTime>(entry.CreationTime, readModel.CreationTime);
            Assert.Equal<EventStreamEntryCorrelationId>(entry.CorrelationId, readModel.CorrelationId);
        }
    }
}
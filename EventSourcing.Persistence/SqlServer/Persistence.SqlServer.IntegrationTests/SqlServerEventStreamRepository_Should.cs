using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Persistence;
using EventSourcing.Persistence.Abstractions.Enums;
using EventSourcing.Persistence.SqlServer;
using TestHelpers.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace Persistence.SqlServer.IntegrationTests
{
    [Collection(nameof(SqlServerCollectionDefinition))]
    public class SqlServerEventStreamRepository_Should
    {
        private readonly SqlServerCollectionFixture _fixture;

        private IEventStreamRepository Repository => _fixture.GetService<IEventStreamRepository>();

        public SqlServerEventStreamRepository_Should(SqlServerCollectionFixture fixture, ITestOutputHelper testOutputHelper)
        {
            _fixture = fixture.SetTestOutputHelper(testOutputHelper);
        }

        [Theory]
        [AutoMoqData]
        public async Task ReturnSuccess_When_WritingSingleEntry(EventStreamEntry entry)
        {
            var result = await Repository.WriteAsync(new EventStreamEntries(new[] {entry}), CancellationToken.None);
            
            Assert.Equal(EventStreamWriteResult.Success, result);
        }

        [Theory]
        [AutoMoqData]
        public async Task ProperlyInsertData_When_WritingSingleEntry(EventStreamEntry entry)
        {
            _ = await Repository.WriteAsync(new EventStreamEntries(new[] {entry}), CancellationToken.None);

            var result = await SelectAsync(entry.StreamId);
            var singleEntry = Assert.Single(result);
            Assert.NotNull(singleEntry);
            AssertEqual(entry, singleEntry);
        }

        [Theory]
        [AutoMoqData]
        public async Task ReturnSequenceAlreadyTaken_When_WritingSingleEntryTwice(EventStreamEntry entry)
        {
            var firstResult = await Repository.WriteAsync(new EventStreamEntries(new[] {entry}), CancellationToken.None);
            Assert.Equal(EventStreamWriteResult.Success, firstResult);
            
            var secondResult = await Repository.WriteAsync(new EventStreamEntries(new[] {entry}), CancellationToken.None);
            Assert.Equal(EventStreamWriteResult.SequenceAlreadyTaken, secondResult);
        }

        [Theory]
        [AutoMoqData]
        public async Task ReturnSuccess_When_WritingMultipleEntries(EventStreamEntries entries)
        {
            var result = await Repository.WriteAsync(entries, CancellationToken.None);
            
            Assert.Equal(EventStreamWriteResult.Success, result);
        }

        [Theory]
        [AutoMoqData]
        public async Task ProperlyInsertData_When_WritingMultipleEntries(EventStreamEntries entries)
        {
            _ = await Repository.WriteAsync(entries, CancellationToken.None);
            var streamId = entries
                .Select(entry => entry.StreamId)
                .Distinct()
                .Single();

            var result = await SelectAsync(streamId);
            Assert.Equal(entries.Count, result.Count);
            for (var i = 0; i < entries.Count; i++)
            {
                AssertEqual(entries[i], result[i]);
            }
        }

        [Theory]
        [AutoMoqData]
        public async Task ReturnSequenceAlreadyTaken_When_WritingMultipleEntriesTwice(EventStreamEntries entries)
        {
            var firstResult = await Repository.WriteAsync(entries, CancellationToken.None);
            Assert.Equal(EventStreamWriteResult.Success, firstResult);
            
            var secondResult = await Repository.WriteAsync(entries, CancellationToken.None);
            Assert.Equal(EventStreamWriteResult.SequenceAlreadyTaken, secondResult);
        }

        [Theory]
        [AutoMoqData]
        public async Task ReturnNoData_When_ReadingNonExistingStreamId(EventStreamId streamId)
        {
            var result = await Repository.ReadAsync(streamId, CancellationToken.None);

            Assert.Empty(result);
        }

        [Theory]
        [AutoMoqData]
        public async Task ReturnWrittenEntry_When_ReadingStreamId_And_SingleEntryUnderProvidedStreamIdExists(EventStreamEntry entry)
        {
            var writeResult = await Repository.WriteAsync(new EventStreamEntries(new[] {entry}), CancellationToken.None);
            Assert.Equal(EventStreamWriteResult.Success, writeResult);
            
            var result = await Repository.ReadAsync(entry.StreamId, CancellationToken.None);

            var singleEntry = Assert.Single(result);
            AssertEqual(entry, singleEntry);
        }

        [Theory]
        [AutoMoqData]
        public async Task ReturnWrittenEntries_When_ReadingStreamId_And_MultipleEntriesUnderProvidedStreamIdExist(EventStreamEntries entries)
        {
            var writeResult = await Repository.WriteAsync(entries, CancellationToken.None);
            Assert.Equal(EventStreamWriteResult.Success, writeResult);
            var streamId = entries
                .Select(entry => entry.StreamId)
                .Distinct()
                .Single();

            var result = await Repository.ReadAsync(streamId, CancellationToken.None);
            Assert.Equal(entries.Count, result.Count);
            for (var i = 0; i < entries.Count; i++)
            {
                AssertEqual(entries[i], result[i]);
            }
        }

        private async Task<IReadOnlyList<EventStreamEntryReadModel>> SelectAsync(Guid streamId)
        {
            await using var connection = new SqlConnection(_fixture.GetService<ISqlServerEventStreamPersistenceConfiguration>().ConnectionString);
            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT StreamId, EntrySequence, EntryId, EventContent, EventContentSerializationFormat, EventTypeIdentifier, EventTypeIdentifierFormat, CausationId, CreationTime, CorrelationId FROM EventStream WHERE StreamId = @StreamId";
            command.Parameters.AddWithValue("@StreamId", streamId);
            await connection.OpenAsync(CancellationToken.None).ConfigureAwait(false);
            await using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
            var result = new List<EventStreamEntryReadModel>();
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                result.Add(new EventStreamEntryReadModel
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
        
        private class EventStreamEntryReadModel
        {
            public Guid StreamId { get; set; }
            public uint EntrySequence { get; set; }
            public Guid EntryId { get; set; }
            public string EventContent { get; set; }
            public string EventContentSerializationFormat { get; set; }
            public string EventTypeIdentifier { get; set; }
            public string EventTypeIdentifierFormat { get; set; }
            public Guid CausationId { get; set; }
            public DateTime CreationTime { get; set; }
            public Guid CorrelationId { get; set; }
        }

        private static void AssertEqual(EventStreamEntry expectedEntry, EventStreamEntryReadModel actualEntry)
        {
            var actualEntryMapped = new EventStreamEntry(
                actualEntry.StreamId,
                actualEntry.EntryId,
                actualEntry.EntrySequence,
                new EventStreamEventDescriptor(
                    actualEntry.EventContent,
                    actualEntry.EventContentSerializationFormat,
                    actualEntry.EventTypeIdentifier,
                    actualEntry.EventTypeIdentifierFormat),
                actualEntry.CausationId,
                actualEntry.CreationTime,
                actualEntry.CorrelationId);
            
            AssertEqual(expectedEntry, actualEntryMapped);
        }

        private static void AssertEqual(EventStreamEntry expectedEntry, EventStreamEntry actualEntry)
        {
            Assert.Equal(expectedEntry.StreamId, actualEntry.StreamId);
            Assert.Equal(expectedEntry.EntrySequence, actualEntry.EntrySequence);
            Assert.Equal(expectedEntry.EntryId, actualEntry.EntryId);
            Assert.Equal(expectedEntry.EventDescriptor, actualEntry.EventDescriptor);
            Assert.Equal(expectedEntry.CausationId, actualEntry.CausationId);
            Assert.Equal(expectedEntry.CreationTime, actualEntry.CreationTime);
            Assert.Equal(expectedEntry.CorrelationId, actualEntry.CorrelationId);
        }
    }
}
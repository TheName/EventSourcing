using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Persistence;
using EventSourcing.Persistence.Abstractions.Enums;
using TestHelpers.Attributes;
using Xunit;

namespace Persistence.IntegrationTests.Base
{
    public abstract class EventStreamRepository_Should
    {
        protected abstract IEventStreamRepository Repository { get; }
        
        protected abstract IEventStreamTestReadRepository TestReadRepository { get; }

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
            var writeResult = await Repository.WriteAsync(new EventStreamEntries(new[] {entry}), CancellationToken.None);
            Assert.Equal(EventStreamWriteResult.Success, writeResult);

            var result = await TestReadRepository.SelectAsync(entry.StreamId);
            var singleEntry = Assert.Single(result);
            Assert.NotNull(singleEntry);
            Assertions.Equal(entry, singleEntry);
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

            var result = await TestReadRepository.SelectAsync(streamId);
            Assert.Equal(entries.Count, result.Count);
            for (var i = 0; i < entries.Count; i++)
            {
                Assertions.Equal(entries[i], result[i]);
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
            Assert.Equal(entry, singleEntry);
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
                Assert.Equal(entries[i], result[i]);
            }
        }
    }
}
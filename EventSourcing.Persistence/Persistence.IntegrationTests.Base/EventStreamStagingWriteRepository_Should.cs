using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Persistence;
using EventSourcing.Persistence.Abstractions.ValueObjects;
using TestHelpers.Attributes;
using Xunit;

namespace Persistence.IntegrationTests.Base
{
    public abstract class EventStreamStagingWriteRepository_Should
    {
        protected abstract IEventStreamStagingWriteRepository Repository { get; }
        
        protected abstract IEventStreamStagingTestReadRepository TestReadRepository { get; }

        [Theory]
        [AutoMoqData]
        public async Task InsertSingleEntry(EventStreamStagingId stagingId, EventStreamEntry entry)
        {
            await Repository.InsertAsync(stagingId, new EventStreamEntries(new[] {entry}), CancellationToken.None);

            var result = await TestReadRepository.SelectAsync(stagingId);
            var singleEntry = Assert.Single(result);
            Assert.NotNull(singleEntry);
            Assert.Equal(stagingId.Value, singleEntry.StagingId);
            Assertions.Equal(entry, singleEntry);
        }

        [Theory]
        [AutoMoqData]
        public async Task Throw_When_WritingSingleEntryUnderSameStagingIdTwice(EventStreamStagingId stagingId, EventStreamEntry entry)
        {
            await Repository.InsertAsync(stagingId, new EventStreamEntries(new[] {entry}), CancellationToken.None);
            
            await Assert.ThrowsAnyAsync<Exception>(() => Repository.InsertAsync(stagingId, new EventStreamEntries(new[] {entry}), CancellationToken.None));
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

            var result = await TestReadRepository.SelectAsync(stagingId);
            Assert.Equal(entries.Count, result.Count);
            for (var i = 0; i < entries.Count; i++)
            {
                Assertions.Equal(entries[i], result[i]);
            }
        }
        
        [Theory]
        [AutoMoqData]
        public async Task Throw_When_WritingMultipleEntriesUnderSameStagingIdTwice(EventStreamStagingId stagingId, EventStreamEntries entries)
        {
            await Repository.InsertAsync(stagingId, entries, CancellationToken.None);
            
            await Assert.ThrowsAnyAsync<Exception>(() => Repository.InsertAsync(stagingId, entries, CancellationToken.None));
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

            var result = await TestReadRepository.SelectAsync(stagingId);
            Assert.Empty(result);
        }

        [Theory]
        [AutoMoqData]
        public async Task NotThrow_When_DeletingNonExistingStagingId(EventStreamStagingId stagingId)
        {
            var result = await TestReadRepository.SelectAsync(stagingId);
            Assert.Empty(result);
            
            await Repository.DeleteAsync(stagingId, CancellationToken.None);

            result = await TestReadRepository.SelectAsync(stagingId);
            Assert.Empty(result);
        }
    }
}
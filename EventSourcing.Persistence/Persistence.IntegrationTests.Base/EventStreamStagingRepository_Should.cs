using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Persistence;
using EventSourcing.Persistence.Abstractions.ValueObjects;
using TestHelpers.Attributes;
using Xunit;

namespace Persistence.IntegrationTests.Base
{
    public abstract class EventStreamStagingRepository_Should
    {
        protected abstract IEventStreamStagingRepository Repository { get; }
        
        protected abstract IEventStreamStagingTestRepository TestRepository { get; }

        [Theory]
        [AutoMoqData]
        public async Task InsertSingleEntry(
            EventStreamStagingId stagingId,
            EventStreamStagedEntriesStagingTime stagingTime,
            EventStreamEntry entry)
        {
            var stagedEntries = new EventStreamStagedEntries(
                stagingId,
                stagingTime,
                new EventStreamEntries(new[] { entry }));
            
            await Repository.InsertAsync(stagedEntries, CancellationToken.None);

            var result = await TestRepository.SelectAsync(stagingId);
            var singleEntry = Assert.Single(result);
            Assert.NotNull(singleEntry);
            Assert.Equal(stagingId.Value, singleEntry.StagingId);
            Assertions.Equal(entry, singleEntry);
        }

        [Theory]
        [AutoMoqData]
        public async Task SelectInsertedSingleEntry(
            EventStreamStagingId stagingId,
            EventStreamStagedEntriesStagingTime stagingTime,
            EventStreamEntry entry)
        {
            var stagedEntries = new EventStreamStagedEntries(
                stagingId,
                stagingTime,
                new EventStreamEntries(new[] { entry }));
            
            await Repository.InsertAsync(stagedEntries, CancellationToken.None);

            var selectedEntry = await Repository.SelectAsync(stagingId, CancellationToken.None);
            Assert.Equal(stagedEntries, selectedEntry);
        }

        [Theory]
        [AutoMoqData]
        public async Task Throw_When_WritingSingleEntryUnderSameStagingIdTwice(
            EventStreamStagingId stagingId,
            EventStreamStagedEntriesStagingTime stagingTime,
            EventStreamEntry entry)
        {
            var stagedEntries = new EventStreamStagedEntries(
                stagingId,
                stagingTime,
                new EventStreamEntries(new[] { entry }));
            
            await Repository.InsertAsync(stagedEntries, CancellationToken.None);
            
            await Assert.ThrowsAnyAsync<Exception>(() => Repository.InsertAsync(stagedEntries, CancellationToken.None));
        }

        [Theory]
        [AutoMoqData]
        public async Task NotThrow_When_WritingSingleEntryUnderDifferentStagingIdTwice(
            EventStreamStagingId stagingId,
            EventStreamStagingId differentStagingId,
            EventStreamStagedEntriesStagingTime stagingTime,
            EventStreamEntry entry)
        {
            var stagedEntries = new EventStreamStagedEntries(
                stagingId,
                stagingTime,
                new EventStreamEntries(new[] { entry }));
            
            await Repository.InsertAsync(stagedEntries, CancellationToken.None);
            
            var stagedEntriesWithDifferentId = new EventStreamStagedEntries(
                differentStagingId,
                stagingTime,
                new EventStreamEntries(new[] { entry }));
            
            await Repository.InsertAsync(stagedEntriesWithDifferentId, CancellationToken.None);
        }
        
        [Theory]
        [AutoMoqData]
        public async Task InsertMultipleEntries(
            EventStreamStagingId stagingId,
            EventStreamStagedEntriesStagingTime stagingTime,
            EventStreamEntries entries)
        {
            var stagedEntries = new EventStreamStagedEntries(
                stagingId,
                stagingTime,
                entries);
            
            await Repository.InsertAsync(stagedEntries, CancellationToken.None);

            var result = await TestRepository.SelectAsync(stagingId);
            Assert.Equal(entries.Count, result.Count);
            for (var i = 0; i < entries.Count; i++)
            {
                Assertions.Equal(entries[i], result[i]);
            }
        }
        
        [Theory]
        [AutoMoqData]
        public async Task SelectInsertedMultipleEntries(
            EventStreamStagingId stagingId,
            EventStreamStagedEntriesStagingTime stagingTime,
            EventStreamEntries entries)
        {
            var stagedEntries = new EventStreamStagedEntries(
                stagingId,
                stagingTime,
                entries);
            
            await Repository.InsertAsync(stagedEntries, CancellationToken.None);
            var selectedEntries = await Repository.SelectAsync(stagingId, CancellationToken.None);
            
            Assert.Equal(stagedEntries, selectedEntries);
        }
        
        [Theory]
        [AutoMoqData]
        public async Task Throw_When_WritingMultipleEntriesUnderSameStagingIdTwice(EventStreamStagedEntries stagedEntries)
        {
            await Repository.InsertAsync(stagedEntries, CancellationToken.None);
            
            await Assert.ThrowsAnyAsync<Exception>(() => Repository.InsertAsync(stagedEntries, CancellationToken.None));
        }
        
        [Theory]
        [AutoMoqData]
        public async Task NotThrow_When_WritingMultipleEntriesUnderDifferentStagingIdTwice(
            EventStreamStagingId stagingId,
            EventStreamStagingId differentStagingId,
            EventStreamStagedEntriesStagingTime stagingTime,
            EventStreamEntries entries)
        {
            var stagedEntries = new EventStreamStagedEntries(
                stagingId,
                stagingTime,
                entries);
            
            await Repository.InsertAsync(stagedEntries, CancellationToken.None);
            
            var stagedEntriesWithDifferentId = new EventStreamStagedEntries(
                differentStagingId,
                stagingTime,
                entries);
            
            await Repository.InsertAsync(stagedEntriesWithDifferentId, CancellationToken.None);
        }

        [Theory]
        [AutoMoqData]
        public async Task DeleteEntries(EventStreamStagedEntries stagedEntries)
        {
            await Repository.InsertAsync(stagedEntries, CancellationToken.None);
            await Repository.DeleteAsync(stagedEntries.StagingId, CancellationToken.None);

            var result = await TestRepository.SelectAsync(stagedEntries.StagingId);
            Assert.Empty(result);
        }

        [Theory]
        [AutoMoqData]
        public async Task ReturnNull_When_Selecting_AfterDeletingEntries(EventStreamStagedEntries stagedEntries)
        {
            await Repository.InsertAsync(stagedEntries, CancellationToken.None);
            await Repository.DeleteAsync(stagedEntries.StagingId, CancellationToken.None);

            var result = await Repository.SelectAsync(stagedEntries.StagingId, CancellationToken.None);
            Assert.Null(result);
        }

        [Theory]
        [AutoMoqData]
        public async Task NotThrow_When_DeletingNonExistingStagingId(EventStreamStagingId stagingId)
        {
            var result = await TestRepository.SelectAsync(stagingId);
            Assert.Empty(result);
            
            await Repository.DeleteAsync(stagingId, CancellationToken.None);

            result = await TestRepository.SelectAsync(stagingId);
            Assert.Empty(result);
        }

        [Theory]
        [AutoMoqData]
        public async Task ReturnNull_When_SelectingNonExistingStagingId(EventStreamStagingId stagingId)
        {
            var result = await Repository.SelectAsync(stagingId, CancellationToken.None);
            Assert.Null(result);
        }

        [Theory]
        [AutoMoqData]
        public async Task SelectAllInsertedEntries(List<EventStreamStagedEntries> stagedEntriesList)
        {
            await TestRepository.DeleteAllAsync();
            await Task.WhenAll(stagedEntriesList.Select(entries => Repository.InsertAsync(entries, CancellationToken.None)));

            var allStagedEntries = await Repository.SelectAsync(CancellationToken.None);

            Assert.Equal(stagedEntriesList.Count, allStagedEntries.Count);
            Assert.All(stagedEntriesList, entries => Assert.Contains(entries, allStagedEntries));
        }

        [Theory]
        [AutoMoqData]
        public async Task SelectAllInsertedEntriesApartFromDeletedOnes(
            List<EventStreamStagedEntries> stagedEntriesList,
            List<EventStreamStagedEntries> stagedEntriesListToBeDeleted)
        {
            await TestRepository.DeleteAllAsync();
            await Task.WhenAll(stagedEntriesList.Select(entries => Repository.InsertAsync(entries, CancellationToken.None)));
            await Task.WhenAll(stagedEntriesListToBeDeleted.Select(entries => Repository.InsertAsync(entries, CancellationToken.None)));
            await Task.WhenAll(stagedEntriesListToBeDeleted.Select(entries => Repository.DeleteAsync(entries.StagingId, CancellationToken.None)));

            var allStagedEntries = await Repository.SelectAsync(CancellationToken.None);

            Assert.Equal(stagedEntriesList.Count, allStagedEntries.Count);
            Assert.All(stagedEntriesList, entries => Assert.Contains(entries, allStagedEntries));
        }
    }
}
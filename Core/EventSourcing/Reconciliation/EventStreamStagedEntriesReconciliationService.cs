using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions.Configurations;
using EventSourcing.Abstractions.Reconciliation;
using EventSourcing.Bus.Abstractions;
using EventSourcing.Extensions;
using EventSourcing.Persistence.Abstractions;
using EventSourcing.Persistence.Abstractions.ValueObjects;
using Microsoft.Extensions.Logging;

namespace EventSourcing.Reconciliation
{
    internal class EventStreamStagedEntriesReconciliationService : IEventStreamStagedEntriesReconciliationService
    {
        private readonly IEventSourcingConfiguration _configuration;
        private readonly IEventStreamStagingReader _stagingReader;
        private readonly IEventStreamStagingWriter _stagingWriter;
        private readonly IEventStreamReader _eventStreamReader;
        private readonly IEventSourcingBusPublisher _busPublisher;
        private readonly ILogger<EventStreamStagedEntriesReconciliationService> _logger;

        public EventStreamStagedEntriesReconciliationService(
            IEventSourcingConfiguration configuration,
            IEventStreamStagingReader stagingReader,
            IEventStreamStagingWriter stagingWriter,
            IEventStreamReader eventStreamReader,
            IEventSourcingBusPublisher busPublisher,
            ILogger<EventStreamStagedEntriesReconciliationService> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _stagingReader = stagingReader ?? throw new ArgumentNullException(nameof(stagingReader));
            _stagingWriter = stagingWriter ?? throw new ArgumentNullException(nameof(stagingWriter));
            _eventStreamReader = eventStreamReader ?? throw new ArgumentNullException(nameof(eventStreamReader));
            _busPublisher = busPublisher ?? throw new ArgumentNullException(nameof(busPublisher));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task TryToReconcileStagedEntriesAsync(
            EventStreamStagedEntries stagedEntries,
            CancellationToken cancellationToken)
        {
            if (stagedEntries == null)
            {
                throw new ArgumentNullException(nameof(stagedEntries));
            }
            
            var timeSinceStaging = DateTime.UtcNow - stagedEntries.StagingTime.Value;
            if (timeSinceStaging < _configuration.ReconciliationJobGracePeriodAfterStagingTime)
            {
                _logger.LogDebug(
                    "Grace period after staging time which is configured to {GracePeriodAfterStagingTime} has not yet passed since staging time {StagingTime} of staged entries {StagedEntries}",
                    _configuration.ReconciliationJobGracePeriodAfterStagingTime,
                    stagedEntries.StagingTime,
                    stagedEntries);
                
                return;
            }
            
            var storedEntries = await _eventStreamReader.ReadAsync(
                    stagedEntries.Entries.StreamId,
                    stagedEntries.Entries.MinimumSequence,
                    stagedEntries.Entries.MaximumSequence,
                    cancellationToken)
                .ConfigureAwait(false);

            if (storedEntries.Count == 0)
            {
                // since we cannot be sure why the staged entries are not stored yet
                // (the storing can be still in progress or it might have already failed)
                // it is safest to do nothing and wait until we have some entries stored with same sequences.

                _logger.LogDebug(
                    "Did not find any stored events for stream id {StreamId} with sequences in inclusive range of {MinimumSequence}-{MaximumSequence} when trying to reconcile staged entry {StagedEntry}",
                    stagedEntries.Entries.StreamId,
                    stagedEntries.Entries.MinimumSequence,
                    stagedEntries.Entries.MaximumSequence,
                    stagedEntries);
                
                return;
            }

            if (storedEntries.Count > stagedEntries.Entries.Count)
            {
                throw new InvalidOperationException(
                    $"It is not possible to get {storedEntries.Count} entries when requesting {stagedEntries.Entries.MaximumSequence.Value - stagedEntries.Entries.MinimumSequence.Value + 1} entries in inclusive range of {stagedEntries.Entries.MinimumSequence}-{stagedEntries.Entries.MaximumSequence}");
            }

            if (storedEntries.Count < stagedEntries.Entries.Count)
            {
                // Since storing entries is an all-or-nothing operation
                // and the count of stored entries is different than staged entries
                // we know for sure that the staged entries have not been stored.
                await _stagingWriter.MarkAsFailedToStoreAsync(stagedEntries.StagingId, cancellationToken)
                    .ConfigureAwait(false);

                _logger.LogDebug(
                    "Found a different number of stored entries for stream id {StreamId} in inclusive sequences range of {MinimumSequence}-{MaximumSequence} when tried to reconcile staged entry {StagedEntry}",
                    stagedEntries.Entries.StreamId,
                    stagedEntries.Entries.MinimumSequence,
                    stagedEntries.Entries.MaximumSequence,
                    stagedEntries);
                
                return;
            }

            for (var i = 0; i < storedEntries.Count; i++)
            {
                var storedEntry = storedEntries[i];
                var stagedEntry = stagedEntries.Entries[i];

                if (storedEntry != stagedEntry)
                {
                    // Since storing entries is an all-or-nothing operation
                    // and at least one of the staged entries is different than it's corresponding stored entry
                    // we know for sure that the staged entries have not been stored.
                    await _stagingWriter.MarkAsFailedToStoreAsync(stagedEntries.StagingId, cancellationToken)
                        .ConfigureAwait(false);

                    _logger.LogDebug(
                        "Found a different staged entry than stored entry in stream id {StreamId} and sequence {Sequence} when tried to reconcile staged entry {StagedEntry}",
                        stagedEntries.Entries.StreamId,
                        stagedEntry.EntrySequence,
                        stagedEntries);
                
                    return;
                }
            }
            
            // since all stored entries are the same as their corresponding staged entries
            // we know that staged entries should be published
            // but before doing that let's see if the main process already managed to publish them
            var stagedEntriesReRead = await _stagingReader
                .ReadUnmarkedStagedEntriesAsync(stagedEntries.StagingId, cancellationToken)
                .ConfigureAwait(false);

            if (stagedEntriesReRead == null)
            {
                // These entries must have been already published
                _logger.LogDebug(
                    "Found same staged entries and stored entries but after comparison staged entries became marked before managed to actually reconcile them for stream id {StreamId} in inclusive sequences range of {MinimumSequence}-{MaximumSequence} when tried to reconcile staged entry {StagedEntry}",
                    stagedEntries.Entries.StreamId,
                    stagedEntries.Entries.MinimumSequence,
                    stagedEntries.Entries.MaximumSequence,
                    stagedEntries);
                
                return;
            }

            if (stagedEntriesReRead != stagedEntries)
            {
                throw new InvalidOperationException(
                    $"It is not possible to get different staged entries under same staging id {stagedEntries.StagingId}. Originally provided staged entries: {stagedEntries}, Re-read staged entries under same staging id: {stagedEntriesReRead}");
            }

            await _busPublisher.PublishAsync(storedEntries, cancellationToken).ConfigureAwait(false);
            await _stagingWriter.MarkAsPublishedAsync(stagedEntries.StagingId, cancellationToken).ConfigureAwait(false);
            
            _logger.LogInformation(
                "Successfully reconciled staged entries for stream id {StreamId} in inclusive sequences range of {MinimumSequence}-{MaximumSequence}: {StagedEntry}",
                stagedEntries.Entries.StreamId,
                stagedEntries.Entries.MinimumSequence,
                stagedEntries.Entries.MaximumSequence,
                stagedEntries);
        }
    }
}
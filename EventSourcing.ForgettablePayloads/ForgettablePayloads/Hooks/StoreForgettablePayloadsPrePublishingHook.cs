using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions.Hooks;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.ForgettablePayloads.Abstractions;
using EventSourcing.ForgettablePayloads.Abstractions.Conversion;
using EventSourcing.ForgettablePayloads.Abstractions.Services;
using EventSourcing.ForgettablePayloads.Abstractions.ValueObjects;
using EventSourcing.ForgettablePayloads.Helpers;
using EventSourcing.ForgettablePayloads.Persistence.Abstractions;

namespace EventSourcing.ForgettablePayloads.Hooks
{
    internal class StoreForgettablePayloadsPrePublishingHook : IEventStreamEventWithMetadataPrePublishingHook
    {
        private readonly IForgettablePayloadFinder _forgettablePayloadFinder;
        private readonly IForgettablePayloadStorageWriter _forgettablePayloadStorageWriter;
        private readonly IForgettablePayloadContentConverter _forgettablePayloadConverter;

        public StoreForgettablePayloadsPrePublishingHook(
            IForgettablePayloadFinder forgettablePayloadFinder,
            IForgettablePayloadStorageWriter forgettablePayloadStorageWriter,
            IForgettablePayloadContentConverter forgettablePayloadConverter)
        {
            _forgettablePayloadFinder = forgettablePayloadFinder ?? throw new ArgumentNullException(nameof(forgettablePayloadFinder));
            _forgettablePayloadStorageWriter = forgettablePayloadStorageWriter ?? throw new ArgumentNullException(nameof(forgettablePayloadStorageWriter));
            _forgettablePayloadConverter = forgettablePayloadConverter ?? throw new ArgumentNullException(nameof(forgettablePayloadConverter));
        }
        
        public async Task PreEventStreamEventWithMetadataPublishHookAsync(
            EventStreamEventWithMetadata eventWithMetadata, 
            CancellationToken cancellationToken)
        {
            if (eventWithMetadata == null)
            {
                throw new ArgumentNullException(nameof(eventWithMetadata));
            }
            
            var forgettablePayloadEntities = _forgettablePayloadFinder.Find(eventWithMetadata.Event);
            if (forgettablePayloadEntities == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(IForgettablePayloadFinder)} of type {_forgettablePayloadFinder.GetType()} returned null when trying to find forgettable payloads for event type {eventWithMetadata.Event.GetType()} and instance {eventWithMetadata.Event}");
            }

            if (forgettablePayloadEntities.Any(payload => payload == null))
            {
                throw new InvalidOperationException(
                    $"{nameof(IForgettablePayloadFinder)} of type {_forgettablePayloadFinder.GetType()} returned a collection of forgettable payloads with at least one null item when trying to find forgettable payloads for event type {eventWithMetadata.Event.GetType()} and instance {eventWithMetadata.Event}");
            }

            await TaskHelpers.WhenAllWithAggregateException(
                    forgettablePayloadEntities
                        .Select(forgettablePayload => InsertPayloadAsync(
                            forgettablePayload,
                            eventWithMetadata.EventMetadata,
                            cancellationToken)))
                .ConfigureAwait(false);
        }

        private async Task InsertPayloadAsync(
            ForgettablePayload forgettablePayload,
            EventStreamEventMetadata eventMetadata,
            CancellationToken cancellationToken)
        {
            if (!forgettablePayload.TryCreateMetadataForEventStreamIdAndEntryId(
                    eventMetadata.StreamId,
                    eventMetadata.EntryId,
                    out var payloadMetadata))
            {
                // this ForgettablePayload instance was not created; must've been reused
                
                return;
            }

            var actualPayload = await forgettablePayload.GetPayloadAsync(cancellationToken).ConfigureAwait(false);
            var payloadContentDescriptor = _forgettablePayloadConverter.ToPayloadContentDescriptor(actualPayload);
            if (payloadContentDescriptor == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(IForgettablePayloadContentConverter)} of type {_forgettablePayloadConverter.GetType()} returned null when trying to convert {actualPayload} to content descriptor. PayloadId: {forgettablePayload.PayloadId}");
            }

            var descriptor = ForgettablePayloadDescriptor.CreateFromMetadataAndContentDescriptor(
                payloadMetadata,
                payloadContentDescriptor);

            await _forgettablePayloadStorageWriter.InsertAsync(descriptor, cancellationToken).ConfigureAwait(false);
        }
    }
}
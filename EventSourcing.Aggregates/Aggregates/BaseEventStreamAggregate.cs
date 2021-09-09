using System;
using EventSourcing.Abstractions;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Aggregates.Abstractions;
using EventSourcing.Aggregates.Helpers;

namespace EventSourcing.Aggregates
{
    /// <summary>
    /// A basic implementation of <see cref="IEventStreamAggregate"/>.
    /// </summary>
    public abstract class BaseEventStreamAggregate : IEventStreamAggregate
    {
        private AppendableEventStream _appendableEventStream = new AppendableEventStream(EventStream.NewEventStream());

        /// <summary>
        /// Defines if in case of missing handler methods for event types an exception should be thrown.
        /// </summary>
        protected virtual bool ShouldIgnoreMissingHandlers { get; } = false;

        /// <inheritdoc />
        public PublishableEventStream PublishableEventStream => new PublishableEventStream(_appendableEventStream);

        private EventStreamId StreamId => _appendableEventStream.StreamId;

        /// <inheritdoc />
        public void Replay(EventStream eventStream)
        {
            foreach (var eventWithMetadata in eventStream.EventsWithMetadata)
            {
                ApplyEvent(eventWithMetadata);
            }
            
            _appendableEventStream = new AppendableEventStream(eventStream);
        }

        /// <summary>
        /// Appends an event with provided <see cref="EventStreamEntryCausationId"/> and <see cref="EventStreamEntryCorrelationId"/> to the in-memory stream.
        /// </summary>
        /// <param name="event">
        /// The <see cref="object"/> representing an event.
        /// </param>
        /// <param name="causationId">
        /// The <see cref="EventStreamEntryCausationId"/>.
        /// </param>
        /// <param name="correlationId">
        /// The <see cref="EventStreamEntryCorrelationId"/>.
        /// </param>
        protected void Append(
            object @event,
            EventStreamEntryCausationId causationId,
            EventStreamEntryCorrelationId correlationId)
        {
            var metadata = new EventStreamEventMetadata(
                StreamId,
                EventStreamEntryId.NewEventStreamEntryId(),
                _appendableEventStream.NextSequence,
                causationId,
                DateTime.UtcNow,
                correlationId);

            Append(@event, metadata);
        }

        /// <summary>
        /// Appends an event with provided metadata to the in-memory stream.
        /// </summary>
        /// <param name="event">
        /// The <see cref="object"/> representing an event.
        /// </param>
        /// <param name="eventMetadata">
        /// The <see cref="EventStreamEventMetadata"/>.
        /// </param>
        protected void Append(object @event, EventStreamEventMetadata eventMetadata)
        {
            _appendableEventStream.AppendEventWithMetadata(new EventStreamEventWithMetadata(@event, eventMetadata));
        }

        private void ApplyEvent(EventStreamEventWithMetadata eventWithMetadata)
        {
            var eventType = eventWithMetadata.Event.GetType();
            if (EventStreamAggregateMethodInfoRepository.TryGetMethodInfoForAggregateTypeAndEventTypeAndEventMetadata(GetType(), eventType, out var eventWithMetadataHandler))
            {
                if (EventStreamAggregateMethodInfoRepository.TryGetMethodInfoForAggregateTypeAndEventType(GetType(), eventType, out _))
                {
                    throw new InvalidOperationException(
                        $"Found both too many event handling methods for event type {eventType}. One accepting {typeof(EventStreamEventMetadata)} as additional parameter and one accepting only {eventType}.");
                }

                eventWithMetadataHandler.Invoke(this, new[] {eventWithMetadata.Event, eventWithMetadata.EventMetadata});
            }
            else if (EventStreamAggregateMethodInfoRepository.TryGetMethodInfoForAggregateTypeAndEventType(GetType(), eventType, out var eventHandler))
            {
                eventHandler.Invoke(this, new[] {eventWithMetadata.Event});
            }
            else
            {
                if (ShouldIgnoreMissingHandlers)
                {
                    return;
                }

                throw new MissingMethodException($"Did not find a method that would handle event of type {eventType}.");
            }
        }
    }
}
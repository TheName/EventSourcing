using System;
using EventSourcing.Abstractions;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Aggregates.Abstractions;
using EventSourcing.Aggregates.Helpers;

namespace EventSourcing.Aggregates
{
    public abstract class BaseEventStreamAggregate : IEventStreamAggregate
    {
        private AppendableEventStream _appendableEventStream = new AppendableEventStream(EventStream.NewEventStream());
        
        protected virtual bool ShouldIgnoreMissingHandlers { get; } = false;

        /// <inheritdoc />
        public PublishableEventStream PublishableEventStream => new PublishableEventStream(_appendableEventStream);

        /// <inheritdoc />
        public void Replay(EventStream eventStream)
        {
            foreach (var eventWithMetadata in eventStream.EventsWithMetadata)
            {
                ApplyEvent(eventWithMetadata);
            }
            
            _appendableEventStream = new AppendableEventStream(eventStream);
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
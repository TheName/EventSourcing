using EventSourcing.Abstractions.ValueObjects;

namespace EventSourcing.Aggregates.Abstractions
{
    public interface IEventStreamAggregate
    {
        PublishableEventStream PublishableEventStream { get; }

        void Replay(EventStream eventStream);
    }
}
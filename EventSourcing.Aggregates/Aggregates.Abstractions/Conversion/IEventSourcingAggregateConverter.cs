using EventSourcing.Abstractions.ValueObjects;

namespace EventSourcing.Aggregates.Abstractions.Conversion
{
    public interface IEventSourcingAggregateConverter
    {
        PublishableEventStream ToPublishableEventStream(object aggregate);
    }
}
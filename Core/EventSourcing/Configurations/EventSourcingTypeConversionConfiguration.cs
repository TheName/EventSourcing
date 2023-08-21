using EventSourcing.ValueObjects;

namespace EventSourcing.Configurations
{
    internal class EventSourcingTypeConversionConfiguration : IEventSourcingTypeConversionConfiguration
    {
        public EventStreamEventTypeIdentifierFormat EventTypeIdentifierFormat { get; set; }
    }
}

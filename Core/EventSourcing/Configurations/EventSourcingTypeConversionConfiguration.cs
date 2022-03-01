using EventSourcing.Abstractions.Configurations;
using EventSourcing.Abstractions.ValueObjects;

namespace EventSourcing.Configurations
{
    internal class EventSourcingTypeConversionConfiguration : IEventSourcingTypeConversionConfiguration
    {
        public EventStreamEventTypeIdentifierFormat EventTypeIdentifierFormat { get; set; }
    }
}
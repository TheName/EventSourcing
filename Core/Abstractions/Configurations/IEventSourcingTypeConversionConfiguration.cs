using EventSourcing.Abstractions.ValueObjects;

namespace EventSourcing.Abstractions.Configurations
{
    /// <summary>
    /// Configuration for type conversion of EventSourcing
    /// </summary>
    public interface IEventSourcingTypeConversionConfiguration
    {
        /// <summary>
        /// The <see cref="EventStreamEventTypeIdentifierFormat"/> that should be used when converting event's type to <see cref="EventStreamEventTypeIdentifier"/>.
        /// </summary>
        EventStreamEventTypeIdentifierFormat EventTypeIdentifierFormat { get; }
    }
}
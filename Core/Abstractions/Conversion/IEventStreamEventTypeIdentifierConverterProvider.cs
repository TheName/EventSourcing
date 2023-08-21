using EventSourcing.ValueObjects;

namespace EventSourcing.Conversion
{
    /// <summary>
    /// Provides instances of <see cref="IEventStreamEventTypeIdentifierConverter"/>.
    /// </summary>
    public interface IEventStreamEventTypeIdentifierConverterProvider
    {
        /// <summary>
        /// Gets <see cref="IEventStreamEventTypeIdentifierConverter"/> configured to be used for conversion of event type to <see cref="EventStreamEventTypeIdentifier"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="IEventStreamEventTypeIdentifierConverter"/>.
        /// </returns>
        IEventStreamEventTypeIdentifierConverter GetEventTypeIdentifierConverter();

        /// <summary>
        /// Gets <see cref="IEventStreamEventTypeIdentifierConverter"/> registered for provided <paramref name="eventTypeIdentifierFormat"/>.
        /// </summary>
        /// <param name="eventTypeIdentifierFormat">
        /// The <see cref="EventStreamEventTypeIdentifierFormat"/> that returned <see cref="IEventStreamEventTypeIdentifierConverter"/> should be handling.
        /// </param>
        /// <returns>
        /// The <see cref="IEventStreamEventTypeIdentifierConverter"/>.
        /// </returns>
        IEventStreamEventTypeIdentifierConverter GetConverter(EventStreamEventTypeIdentifierFormat eventTypeIdentifierFormat);
    }
}

using EventSourcing.Abstractions.ValueObjects;

namespace EventSourcing.Abstractions.Conversion
{
    /// <summary>
    /// Converts event object to <see cref="EventStreamEventDescriptor"/> and <see cref="EventStreamEventDescriptor"/> to event object.
    /// </summary>
    public interface IEventStreamEventConverter
    {
        /// <summary>
        /// Converts <paramref name="eventStreamEvent"/> to <see cref="EventStreamEventDescriptor"/>.
        /// </summary>
        /// <param name="eventStreamEvent">
        /// An object representing an event.
        /// </param>
        /// <returns>
        /// The <see cref="EventStreamEventDescriptor"/>.
        /// </returns>
        EventStreamEventDescriptor ToEventDescriptor(object eventStreamEvent);

        /// <summary>
        /// Converts <paramref name="eventStreamEventDescriptor"/> to <see cref="object"/>.
        /// </summary>
        /// <param name="eventStreamEventDescriptor">
        /// Describes an event.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        object FromEventDescriptor(EventStreamEventDescriptor eventStreamEventDescriptor);
    }
}
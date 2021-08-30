using System;
using EventSourcing.Abstractions.ValueObjects;

namespace EventSourcing.Abstractions.Conversion
{
    /// <summary>
    /// Converts <see cref="EventStreamEventTypeIdentifier"/> to <see cref="Type"/> and <see cref="Type"/> to <see cref="EventStreamEventTypeIdentifier"/>.
    /// </summary>
    public interface IEventStreamEventTypeIdentifierConverter
    {
        /// <summary>
        /// Converts <paramref name="type"/> to <see cref="EventStreamEventTypeIdentifier"/>.
        /// </summary>
        /// <param name="type">
        /// The <see cref="Type"/> of an event.
        /// </param>
        /// <returns>
        /// The <see cref="EventStreamEventTypeIdentifier"/> that uniquely identifies the <see cref="Type"/> of an event.
        /// </returns>
        EventStreamEventTypeIdentifier ToTypeIdentifier(Type type);
        
        /// <summary>
        /// Converts <paramref name="identifier"/> to <see cref="Type"/>.
        /// </summary>
        /// <param name="identifier">
        /// The <see cref="EventStreamEventTypeIdentifier"/> that uniquely identifies the <see cref="Type"/> of an event.
        /// </param>
        /// <returns>
        /// The <see cref="Type"/> of an event.
        /// </returns>
        Type FromTypeIdentifier(EventStreamEventTypeIdentifier identifier);
    }
}
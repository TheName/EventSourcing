using System;
using EventSourcing.Abstractions;
using EventSourcing.Abstractions.ValueObjects;

namespace EventSourcing.Extensions.Abstractions
{
    /// <summary>
    /// Extensions of <see cref="AppendableEventStream"/>.
    /// </summary>
    public static class AppendableEventStreamExtensions
    {
        /// <summary>
        /// Appends event with provided metadata
        /// </summary>
        /// <param name="appendableEventStream">
        /// The <see cref="AppendableEventStream"/>.
        /// </param>
        /// <param name="event">
        /// The <see cref="object"/> representing an object.
        /// </param>
        /// <param name="entryId">
        /// The <see cref="EventStreamEntryId"/>. If not provided, a new <see cref="Guid"/> is generated and used as <see cref="EventStreamEntryId"/>.
        /// </param>
        /// <param name="causationId">
        /// The <see cref="EventStreamEntryCausationId"/>. If not provided, a new <see cref="Guid"/> is generated and used as <see cref="EventStreamEntryCausationId"/>.
        /// </param>
        /// <param name="creationTime">
        /// The <see cref="EventStreamEntryCreationTime"/>. If not provided, a new <see cref="DateTime"/> with current time (UTC) is generated and used as <see cref="EventStreamEntryCreationTime"/>.
        /// </param>
        /// <param name="correlationId">
        /// The <see cref="EventStreamEntryCorrelationId"/>. If not provided, a new <see cref="Guid"/> is generated and used as <see cref="EventStreamEntryCorrelationId"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="appendableEventStream"/> or <paramref name="event"/> is null.
        /// </exception>
        public static void AppendEventWithMetadata(
            this AppendableEventStream appendableEventStream, 
            object @event,
            EventStreamEntryId entryId = null,
            EventStreamEntryCausationId causationId = null,
            EventStreamEntryCreationTime creationTime = null,
            EventStreamEntryCorrelationId correlationId = null)
        {
            if (appendableEventStream == null)
            {
                throw new ArgumentNullException(nameof(appendableEventStream));
            }

            appendableEventStream.AppendEventWithMetadata(new EventStreamEventWithMetadata(
                @event,
                new EventStreamEventMetadata(
                    appendableEventStream.StreamId,
                    entryId ?? EventStreamEntryId.NewEventStreamEntryId(),
                    appendableEventStream.NextSequence,
                    causationId ?? Guid.NewGuid(),
                    creationTime ?? DateTime.UtcNow,
                    correlationId ?? Guid.NewGuid())));
        }
    }
}
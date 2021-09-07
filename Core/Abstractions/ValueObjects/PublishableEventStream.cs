using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventSourcing.Abstractions.ValueObjects
{
    /// <summary>
    /// Represents a read-only event stream of events with metadata that should be published.
    /// </summary>
    public class PublishableEventStream
    {
        /// <summary>
        /// The <see cref="EventStreamId"/> that identifies given stream of events.
        /// </summary>
        public EventStreamId StreamId { get; }

        /// <summary>
        /// The <see cref="IReadOnlyList{EventStreamEventWithMetadata}"/> already persisted in the stream of events.
        /// </summary>
        public IReadOnlyList<EventStreamEventWithMetadata> EventsWithMetadata { get; }

        /// <summary>
        /// Gets the <see cref="IReadOnlyList{EventStreamEventWithMetadata}"/> that is to be published to the stream of events.
        /// </summary>
        public IReadOnlyList<EventStreamEventWithMetadata> EventsWithMetadataToPublish { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PublishableEventStream"/> class.
        /// </summary>
        /// <param name="appendableEventStream">
        /// The <see cref="AppendableEventStream"/> that identifies the stream of events with events proposed to be appended.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when provided <paramref name="appendableEventStream"/> is null.
        /// </exception>
        public PublishableEventStream(AppendableEventStream appendableEventStream)
        {
            if (appendableEventStream == null)
            {
                throw new ArgumentNullException(nameof(appendableEventStream));
            }
            
            StreamId = appendableEventStream.StreamId;
            EventsWithMetadata = appendableEventStream.EventsWithMetadata.ToList().AsReadOnly();
            EventsWithMetadataToPublish = appendableEventStream.EventsWithMetadataToAppend.ToList().AsReadOnly();
        }

        #region Operators

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="eventStream">
        /// The <see cref="PublishableEventStream"/>.
        /// </param>
        /// <param name="otherEventStream">
        /// The <see cref="PublishableEventStream"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="eventStream"/> and <paramref name="otherEventStream"/> are equal, false otherwise.
        /// </returns>
        public static bool operator ==(PublishableEventStream eventStream, PublishableEventStream otherEventStream) =>
            Equals(eventStream, otherEventStream);

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="eventStream">
        /// The <see cref="PublishableEventStream"/>.
        /// </param>
        /// <param name="otherEventStream">
        /// The <see cref="PublishableEventStream"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="eventStream"/> and <paramref name="otherEventStream"/> are not equal, false otherwise.
        /// </returns>
        public static bool operator !=(PublishableEventStream eventStream, PublishableEventStream otherEventStream) =>
            !(eventStream == otherEventStream);

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is PublishableEventStream other &&
            other.GetPropertiesForHashCode().SequenceEqual(GetPropertiesForHashCode());

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return GetPropertiesForHashCode()
                    .Select(o => o.GetHashCode())
                    .Where(i => i != 0)
                    .Aggregate(17, (current, hashCode) => current * 23 * hashCode);
            }
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"Event Stream ID: {StreamId}, {EventsWithMetadataString()}, {EventsWithMetadataToPublishString()}";

        private string EventsWithMetadataString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("EventsWithMetadata: ");
            foreach (var eventWithMetadata in EventsWithMetadata)
            {
                stringBuilder.Append($"\n\t{eventWithMetadata}");
            }

            return stringBuilder.ToString();
        }

        private string EventsWithMetadataToPublishString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("EventsWithMetadata to publish: ");
            foreach (var eventWithMetadata in EventsWithMetadataToPublish)
            {
                stringBuilder.Append($"\n\t{eventWithMetadata}");
            }

            return stringBuilder.ToString();
        }

        private IEnumerable<object> GetPropertiesForHashCode()
        {
            yield return StreamId;
            foreach (var eventStreamEventWithMetadata in EventsWithMetadata)
            {
                yield return eventStreamEventWithMetadata;
            }
            foreach (var eventWithMetadata in EventsWithMetadataToPublish)
            {
                yield return eventWithMetadata;
            }
        }
    }
}
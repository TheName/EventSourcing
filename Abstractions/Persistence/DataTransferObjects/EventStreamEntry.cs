using System;
using System.Collections.Generic;
using System.Linq;
using EventSourcing.Abstractions.ValueObjects;

namespace EventSourcing.Abstractions.Persistence.DataTransferObjects
{
    /// <summary>
    /// The event stream entry; represents a single entry in an event stream.
    /// </summary>
    public class EventStreamEntry
    {
        /// <summary>
        /// The event stream id, see <see cref="StreamId"/>.
        /// </summary>
        public EventStreamId StreamId { get; }
        
        /// <summary>
        /// The event stream sequence, see <see cref="EventStreamEntrySequence"/>.
        /// </summary>
        public EventStreamEntrySequence Sequence { get; }
        
        /// <summary>
        /// The event stream entry id, see <see cref="EventStreamEntryId"/>.
        /// </summary>
        public EventStreamEntryId EntryId { get; }
        
        /// <summary>
        /// The event stream entry content, see <see cref="EventStreamEntryContent"/>.
        /// </summary>
        public EventStreamEntryContent Content { get; }

        /// <summary>
        /// The event stream entry metadata, see <see cref="EventStreamEntryMetadata"/>.
        /// </summary>
        public EventStreamEntryMetadata Metadata { get; }

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="streamId">
        /// The <see cref="EventStreamId"/>
        /// </param>
        /// <param name="sequence">
        /// The <see cref="EventStreamEntrySequence"/>
        /// </param>
        /// <param name="entryId">
        /// The <see cref="EventStreamEntryId"/>
        /// </param>
        /// <param name="content">
        /// The <see cref="EventStreamEntryContent"/>
        /// </param>
        /// <param name="metadata">
        /// The <see cref="EventStreamEntryMetadata"/>
        /// </param>
        public EventStreamEntry(
            EventStreamId streamId,
            EventStreamEntrySequence sequence,
            EventStreamEntryId entryId,
            EventStreamEntryContent content,
            EventStreamEntryMetadata metadata)
        {
            StreamId = streamId ?? throw new ArgumentNullException(nameof(streamId));
            Sequence = sequence ?? throw new ArgumentNullException(nameof(sequence));
            EntryId = entryId ?? throw new ArgumentNullException(nameof(entryId));
            Content = content ?? throw new ArgumentNullException(nameof(content));
            Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
        }

        #region Operators

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="entry">
        /// The <see cref="EventStreamEntry"/>.
        /// </param>
        /// <param name="otherEntry">
        /// The <see cref="EventStreamEntry"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="entry"/> and <paramref name="otherEntry"/> are equal, false otherwise.
        /// </returns>
        public static bool operator ==(EventStreamEntry entry, EventStreamEntry otherEntry) =>
            Equals(entry, otherEntry);

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="entry">
        /// The <see cref="EventStreamEntry"/>.
        /// </param>
        /// <param name="otherEntry">
        /// The <see cref="EventStreamEntry"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="entry"/> and <paramref name="otherEntry"/> are not equal, false otherwise.
        /// </returns>
        public static bool operator !=(EventStreamEntry entry, EventStreamEntry otherEntry) =>
            !(entry == otherEntry);

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is EventStreamEntry other &&
            other.GetHashCode() == GetHashCode();

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return GetPropertiesForHashCode()
                    .Aggregate(17, (current, property) => current * 23 * property.GetHashCode());
            }
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"Event Stream ID: {StreamId}, Sequence: {Sequence}, Entry ID: {EntryId}, Content: {Content}, Metadata: {Metadata}";

        private IEnumerable<object> GetPropertiesForHashCode()
        {
            yield return StreamId;
            yield return Sequence;
            yield return EntryId;
            yield return Content;
            yield return Metadata;
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventSourcing.Abstractions.Exceptions;

namespace EventSourcing.Abstractions.ValueObjects
{
    /// <summary>
    /// Represents a read-only collection of consecutive entries assigned to the same stream. 
    /// </summary>
    public class EventStreamEntries : IReadOnlyList<EventStreamEntry>
    {
        private IReadOnlyList<EventStreamEntry> Value { get; }

        /// <summary>
        /// A read-only instance of the <see cref="EventStreamEntries"/> that contains no entries.
        /// </summary>
        public static EventStreamEntries Empty { get; } = new EventStreamEntries(new EventStreamEntry[0]);
        
        /// <summary>
        /// The minimum <see cref="EventStreamEntrySequence"/> in this collection of entries. 
        /// </summary>
        public EventStreamEntrySequence MinimumSequence { get; }
        
        /// <summary>
        /// The maximum <see cref="EventStreamEntrySequence"/> in this collection of entries.
        /// </summary>
        public EventStreamEntrySequence MaximumSequence { get; }
        
        internal EventStreamId StreamId { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="EventStreamEntries"/> class.
        /// </summary>
        /// <param name="value">
        /// An <see cref="IEnumerable{T}"/> of <see cref="EventStreamEntry"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="value"/> is null.
        /// </exception>
        /// <exception cref="InvalidEventStreamEntrySequenceException">
        /// Thrown when entries provided in <paramref name="value"/> are not ordered increasingly by sequence or the sequence is increasing by more than one. 
        /// </exception>
        /// <exception cref="InvalidEventStreamIdException">
        /// Thrown when any of the entries provided in <paramref name="value"/> has a different stream id than others. 
        /// </exception>
        public EventStreamEntries(IEnumerable<EventStreamEntry> value)
        {
            Value = value?.ToList() ?? throw new ArgumentNullException(nameof(value));
            MaximumSequence = 0;
            MinimumSequence = 0;

            if (Value.Count == 0)
            {
                return;
            }

            MinimumSequence = Value[0].EntrySequence;
            StreamId = Value[0].StreamId;
            
            var previousSequence = MinimumSequence;
            for (var i = 1; i < Value.Count; i++)
            {
                if (Value[i].EntrySequence != previousSequence + 1)
                {
                    throw InvalidEventStreamEntrySequenceException.New(
                        previousSequence + 1,
                        Value[i].EntrySequence,
                        $"{nameof(EventStreamEntries)} have to be ordered increasingly by sequence and sequence has to increase by one.",
                        nameof(value));
                }

                if (Value[i].StreamId != StreamId)
                {
                    throw InvalidEventStreamIdException.New(
                        StreamId,
                        Value[i].StreamId,
                        $"{nameof(EventStreamEntries)} have to have same stream id.",
                        nameof(value));
                }

                previousSequence = Value[i].EntrySequence;
            }

            MaximumSequence = previousSequence;
        }

        #region Operators

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="entries">
        /// The <see cref="EventStreamEntries"/>.
        /// </param>
        /// <param name="otherEntries">
        /// The <see cref="EventStreamEntries"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="entries"/> and <paramref name="otherEntries"/> are equal, false otherwise.
        /// </returns>
        public static bool operator ==(EventStreamEntries entries, EventStreamEntries otherEntries) =>
            Equals(entries, otherEntries);

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="entries">
        /// The <see cref="EventStreamEntries"/>.
        /// </param>
        /// <param name="otherEntries">
        /// The <see cref="EventStreamEntries"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="entries"/> and <paramref name="otherEntries"/> are not equal, false otherwise.
        /// </returns>
        public static bool operator !=(EventStreamEntries entries, EventStreamEntries otherEntries) =>
            !(entries == otherEntries);

        #endregion

        #region IReadOnlyList<EventStreamEvent> proxy

        /// <inheritdoc />
        public IEnumerator<EventStreamEntry> GetEnumerator() => 
            Value.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        /// <inheritdoc />
        public int Count =>
            Value.Count;

        /// <inheritdoc />
        public EventStreamEntry this[int index] =>
            Value[index];
        
        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is EventStreamEntries other &&
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
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("Entries: ");
            foreach (var eventStreamEntry in Value)
            {
                stringBuilder.Append($"\n\t{eventStreamEntry}");
            }

            return stringBuilder.ToString();
        }

        private IEnumerable<object> GetPropertiesForHashCode() =>
            Value;
    }
}
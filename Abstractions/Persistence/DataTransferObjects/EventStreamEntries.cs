using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventSourcing.Abstractions.Persistence.DataTransferObjects
{
    /// <summary>
    /// The event stream entries; represents a entries in an event stream;
    /// entries have to be ordered by sequence and sequence has to increase by one.
    /// </summary>
    public class EventStreamEntries : IReadOnlyList<EventStreamEntry>
    {
        private IReadOnlyList<EventStreamEntry> Value { get; }

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="entries">
        /// A collection of <see cref="EventStreamEntry"/> objects as a readonly list.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If provided entries do not have the same stream id or the sequence is not increasing by one.
        /// </exception>
        public EventStreamEntries(IReadOnlyList<EventStreamEntry> entries)
        {
            Value = entries ?? throw new ArgumentNullException(nameof(entries));

            if (Value.Count == 0)
            {
                return;
            }

            var previousSequence = Value[0].Sequence;
            var streamId = Value[0].StreamId;
            for (var i = 1; i < Value.Count; i++)
            {
                if (Value[i].Sequence != previousSequence + 1)
                {
                    throw new ArgumentException($"{nameof(EventStreamEntries)} have to be ordered increasingly by sequence and sequence has to increase by one.");
                }

                if (Value[i].StreamId != streamId)
                {
                    throw new ArgumentException($"{nameof(EventStreamEntries)} have to have same stream id.");
                }

                previousSequence = Value[i].Sequence;
            }
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

        #region IReadOnlyList<EventStreamEntry> proxy

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
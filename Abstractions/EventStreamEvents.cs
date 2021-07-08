using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventSourcing.Abstractions.Exceptions;

namespace EventSourcing.Abstractions
{
    /// <summary>
    /// Represents a read-only collection of consecutive events assigned to the same stream. 
    /// </summary>
    public class EventStreamEvents : IReadOnlyList<EventStreamEvent>
    {
        private IReadOnlyList<EventStreamEvent> Value { get; }

        /// <summary>
        /// A read-only instance of the <see cref="EventStreamEvents"/> that contains no events.
        /// </summary>
        public static EventStreamEvents Empty { get; } = new EventStreamEvents(new EventStreamEvent[0]);
        
        /// <summary>
        /// The minimum <see cref="EventStreamEventSequence"/> in this collection of events. 
        /// </summary>
        public EventStreamEventSequence MinimumSequence { get; }
        
        /// <summary>
        /// The maximum <see cref="EventStreamEventSequence"/> in this collection of events.
        /// </summary>
        public EventStreamEventSequence MaximumSequence { get; }
        
        internal EventStreamId StreamId { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="EventStreamEvents"/> class.
        /// </summary>
        /// <param name="value">
        /// An <see cref="IEnumerable{T}"/> of <see cref="EventStreamEvent"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="value"/> is null.
        /// </exception>
        /// <exception cref="InvalidEventStreamEventSequenceException">
        /// Thrown when events provided in <paramref name="value"/> are not ordered increasingly by sequence or the sequence is increasing by more than one. 
        /// </exception>
        /// <exception cref="InvalidEventStreamIdException">
        /// Thrown when any of the events provided in <paramref name="value"/> has a different stream id than others. 
        /// </exception>
        public EventStreamEvents(IEnumerable<EventStreamEvent> value)
        {
            Value = value?.ToList() ?? throw new ArgumentNullException(nameof(value));
            MaximumSequence = 0;
            MinimumSequence = 0;

            if (Value.Count == 0)
            {
                return;
            }

            MinimumSequence = Value[0].EventSequence;
            StreamId = Value[0].StreamId;
            
            var previousSequence = MinimumSequence;
            for (var i = 1; i < Value.Count; i++)
            {
                if (Value[i].EventSequence != previousSequence + 1)
                {
                    throw InvalidEventStreamEventSequenceException.New(
                        previousSequence + 1,
                        Value[i].EventSequence,
                        $"{nameof(EventStreamEvents)} have to be ordered increasingly by sequence and sequence has to increase by one.",
                        nameof(value));
                }

                if (Value[i].StreamId != StreamId)
                {
                    throw InvalidEventStreamIdException.New(
                        StreamId,
                        Value[i].StreamId,
                        $"{nameof(EventStreamEvents)} have to have same stream id.",
                        nameof(value));
                }

                previousSequence = Value[i].EventSequence;
            }

            MaximumSequence = previousSequence;
        }

        #region Operators

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="events">
        /// The <see cref="EventStreamEvents"/>.
        /// </param>
        /// <param name="otherEvents">
        /// The <see cref="EventStreamEvents"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="events"/> and <paramref name="otherEvents"/> are equal, false otherwise.
        /// </returns>
        public static bool operator ==(EventStreamEvents events, EventStreamEvents otherEvents) =>
            Equals(events, otherEvents);

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="events">
        /// The <see cref="EventStreamEvents"/>.
        /// </param>
        /// <param name="otherEvents">
        /// The <see cref="EventStreamEvents"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="events"/> and <paramref name="otherEvents"/> are not equal, false otherwise.
        /// </returns>
        public static bool operator !=(EventStreamEvents events, EventStreamEvents otherEvents) =>
            !(events == otherEvents);

        #endregion

        #region IReadOnlyList<EventStreamEvent> proxy

        /// <inheritdoc />
        public IEnumerator<EventStreamEvent> GetEnumerator() => 
            Value.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        /// <inheritdoc />
        public int Count =>
            Value.Count;

        /// <inheritdoc />
        public EventStreamEvent this[int index] =>
            Value[index];
        
        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is EventStreamEvents other &&
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
            stringBuilder.Append("Events: ");
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
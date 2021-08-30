namespace EventSourcing.Abstractions.ValueObjects
{
    /// <summary>
    /// The event stream entry sequence value object.
    /// <remarks>
    /// Identifies a sequence of event store entry.
    /// </remarks>
    /// </summary>
    public class EventStreamEntrySequence
    {
        private uint Value { get; }

        private EventStreamEntrySequence(uint value)
        {
            Value = value;
        }

        #region Operators

        /// <summary>
        /// Implicit operator that converts the <see cref="EventStreamEntrySequence"/> to <see cref="uint"/>.
        /// </summary>
        /// <param name="sequence">
        /// The <see cref="EventStreamEntrySequence"/>.
        /// </param>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        public static implicit operator uint(EventStreamEntrySequence sequence) => sequence.Value;
        
        /// <summary>
        /// Implicit operator that converts the <see cref="uint"/> to <see cref="EventStreamEntrySequence"/>.
        /// </summary>
        /// <param name="sequence">
        /// The <see cref="uint"/>.
        /// </param>
        /// <returns>
        /// The <see cref="EventStreamEntrySequence"/>.
        /// </returns>
        public static implicit operator EventStreamEntrySequence(uint sequence) => new EventStreamEntrySequence(sequence);

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="sequence">
        /// The <see cref="EventStreamId"/>.
        /// </param>
        /// <param name="otherSequence">
        /// The <see cref="EventStreamId"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="sequence"/> and <paramref name="otherSequence"/> are equal, false otherwise.
        /// </returns>
        public static bool operator ==(EventStreamEntrySequence sequence, EventStreamEntrySequence otherSequence) =>
            Equals(sequence, otherSequence);

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="sequence">
        /// The <see cref="EventStreamId"/>.
        /// </param>
        /// <param name="otherSequence">
        /// The <see cref="EventStreamId"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="sequence"/> and <paramref name="otherSequence"/> are not equal, false otherwise.
        /// </returns>
        public static bool operator !=(EventStreamEntrySequence sequence, EventStreamEntrySequence otherSequence) =>
            !(sequence == otherSequence);

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is EventStreamEntrySequence other &&
            other.Value == Value;

        /// <inheritdoc />
        public override int GetHashCode() =>
            Value.GetHashCode();

        /// <inheritdoc />
        public override string ToString() =>
            Value.ToString();
    }
}
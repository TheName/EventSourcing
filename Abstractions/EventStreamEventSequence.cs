namespace EventSourcing.Abstractions
{
    /// <summary>
    /// The event stream entry sequence value object.
    /// <remarks>
    /// Identifies a sequence of event store entry.
    /// </remarks>
    /// </summary>
    public class EventStreamEventSequence
    {
        private uint Value { get; }

        private EventStreamEventSequence(uint value)
        {
            Value = value;
        }

        #region Operators

        /// <summary>
        /// Implicit operator that converts the <see cref="EventStreamEventSequence"/> to <see cref="uint"/>.
        /// </summary>
        /// <param name="sequence">
        /// The <see cref="EventStreamEventSequence"/>.
        /// </param>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        public static implicit operator uint(EventStreamEventSequence sequence) => sequence.Value;
        
        /// <summary>
        /// Implicit operator that converts the <see cref="uint"/> to <see cref="EventStreamEventSequence"/>.
        /// </summary>
        /// <param name="sequence">
        /// The <see cref="uint"/>.
        /// </param>
        /// <returns>
        /// The <see cref="EventStreamEventSequence"/>.
        /// </returns>
        public static implicit operator EventStreamEventSequence(uint sequence) => new EventStreamEventSequence(sequence);

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
        public static bool operator ==(EventStreamEventSequence sequence, EventStreamEventSequence otherSequence) =>
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
        public static bool operator !=(EventStreamEventSequence sequence, EventStreamEventSequence otherSequence) =>
            !(sequence == otherSequence);

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is EventStreamEventSequence other &&
            other.Value == Value;

        /// <inheritdoc />
        public override int GetHashCode() =>
            Value.GetHashCode();

        /// <inheritdoc />
        public override string ToString() =>
            Value.ToString();
    }
}
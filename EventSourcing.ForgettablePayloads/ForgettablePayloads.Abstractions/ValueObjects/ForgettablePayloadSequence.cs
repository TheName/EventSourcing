namespace EventSourcing.ForgettablePayloads.ValueObjects
{
    /// <summary>
    /// The forgettable payload sequence value object.
    /// <remarks>
    /// Is used for optimistic concurrency checks when updating the payload
    /// </remarks>
    /// </summary>
    public class ForgettablePayloadSequence
    {
        /// <summary>
        /// The actual value of sequence
        /// </summary>
        public uint Value { get; }

        /// <summary>
        /// Creates a new instance of <see cref="ForgettablePayloadSequence"/>
        /// </summary>
        /// <param name="value">
        /// The actual value of sequence
        /// </param>
        public ForgettablePayloadSequence(uint value)
        {
            Value = value;
        }

        #region Operators

        /// <summary>
        /// Implicit operator that converts the <see cref="ForgettablePayloadSequence"/> to <see cref="uint"/>.
        /// </summary>
        /// <param name="sequence">
        /// The <see cref="ForgettablePayloadSequence"/>.
        /// </param>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        public static implicit operator uint(ForgettablePayloadSequence sequence) => sequence.Value;

        /// <summary>
        /// Implicit operator that converts the <see cref="uint"/> to <see cref="ForgettablePayloadSequence"/>.
        /// </summary>
        /// <param name="sequence">
        /// The <see cref="uint"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ForgettablePayloadSequence"/>.
        /// </returns>
        public static implicit operator ForgettablePayloadSequence(uint sequence) => new ForgettablePayloadSequence(sequence);

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="sequence">
        /// The <see cref="ForgettablePayloadSequence"/>.
        /// </param>
        /// <param name="otherSequence">
        /// The <see cref="ForgettablePayloadSequence"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="sequence"/> and <paramref name="otherSequence"/> are equal, false otherwise.
        /// </returns>
        public static bool operator ==(ForgettablePayloadSequence sequence, ForgettablePayloadSequence otherSequence) =>
            Equals(sequence, otherSequence);

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="sequence">
        /// The <see cref="ForgettablePayloadSequence"/>.
        /// </param>
        /// <param name="otherSequence">
        /// The <see cref="ForgettablePayloadSequence"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="sequence"/> and <paramref name="otherSequence"/> are not equal, false otherwise.
        /// </returns>
        public static bool operator !=(ForgettablePayloadSequence sequence, ForgettablePayloadSequence otherSequence) =>
            !(sequence == otherSequence);

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is ForgettablePayloadSequence other &&
            other.Value == Value;

        /// <inheritdoc />
        public override int GetHashCode() =>
            Value.GetHashCode();

        /// <inheritdoc />
        public override string ToString() =>
            Value.ToString();
    }
}

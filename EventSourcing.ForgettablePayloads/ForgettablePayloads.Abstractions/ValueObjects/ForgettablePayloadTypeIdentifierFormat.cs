using System;

namespace EventSourcing.ForgettablePayloads.ValueObjects
{
    /// <summary>
    /// Represents <see cref="ForgettablePayloadTypeIdentifier"/>'s format
    /// </summary>
    public class ForgettablePayloadTypeIdentifierFormat
    {
        /// <summary>
        /// The ClassName <see cref="ForgettablePayloadTypeIdentifier"/>'s format.
        /// </summary>
        public static ForgettablePayloadTypeIdentifierFormat ClassName = new ForgettablePayloadTypeIdentifierFormat(nameof(ClassName));

        /// <summary>
        /// The actual value of forgettable payload type identifier format
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForgettablePayloadTypeIdentifierFormat"/> class.
        /// </summary>
        /// <param name="value">
        /// The actual value of forgettable payload type identifier format
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="value"/> is null or whitespace
        /// </exception>
        public ForgettablePayloadTypeIdentifierFormat(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"{nameof(ForgettablePayloadTypeIdentifierFormat)} cannot be null or whitespace.", nameof(value));
            }

            Value = value;
        }

        #region Operators

        /// <summary>
        /// Implicit operator that converts the <see cref="ForgettablePayloadTypeIdentifierFormat"/> to <see cref="string"/>.
        /// </summary>
        /// <param name="typeIdentifierFormat">
        /// The <see cref="ForgettablePayloadTypeIdentifierFormat"/>.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static implicit operator string(ForgettablePayloadTypeIdentifierFormat typeIdentifierFormat) => typeIdentifierFormat.Value;

        /// <summary>
        /// Implicit operator that converts the <see cref="string"/> to <see cref="ForgettablePayloadTypeIdentifierFormat"/>.
        /// </summary>
        /// <param name="typeIdentifierFormat">
        /// The <see cref="string"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ForgettablePayloadTypeIdentifierFormat"/>.
        /// </returns>
        public static implicit operator ForgettablePayloadTypeIdentifierFormat(string typeIdentifierFormat) => new ForgettablePayloadTypeIdentifierFormat(typeIdentifierFormat);

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="forgettablePayloadTypeIdentifierFormat">
        /// The <see cref="ForgettablePayloadTypeIdentifierFormat"/>.
        /// </param>
        /// <param name="otherForgettablePayloadTypeIdentifierFormat">
        /// The <see cref="ForgettablePayloadTypeIdentifierFormat"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="forgettablePayloadTypeIdentifierFormat"/> and <paramref name="otherForgettablePayloadTypeIdentifierFormat"/> are equal, false otherwise.
        /// </returns>
        public static bool operator ==(ForgettablePayloadTypeIdentifierFormat forgettablePayloadTypeIdentifierFormat, ForgettablePayloadTypeIdentifierFormat otherForgettablePayloadTypeIdentifierFormat) =>
            Equals(forgettablePayloadTypeIdentifierFormat, otherForgettablePayloadTypeIdentifierFormat);

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="forgettablePayloadTypeIdentifierFormat">
        /// The <see cref="ForgettablePayloadTypeIdentifierFormat"/>.
        /// </param>
        /// <param name="otherForgettablePayloadTypeIdentifierFormat">
        /// The <see cref="ForgettablePayloadTypeIdentifierFormat"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="forgettablePayloadTypeIdentifierFormat"/> and <paramref name="otherForgettablePayloadTypeIdentifierFormat"/> are not equal, false otherwise.
        /// </returns>
        public static bool operator !=(ForgettablePayloadTypeIdentifierFormat forgettablePayloadTypeIdentifierFormat, ForgettablePayloadTypeIdentifierFormat otherForgettablePayloadTypeIdentifierFormat) =>
            !(forgettablePayloadTypeIdentifierFormat == otherForgettablePayloadTypeIdentifierFormat);

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is ForgettablePayloadTypeIdentifierFormat other &&
            other.Value == Value;

        /// <inheritdoc />
        public override int GetHashCode() =>
            Value.GetHashCode();

        /// <inheritdoc />
        public override string ToString() =>
            Value;
    }
}

using System;

namespace EventSourcing.ForgettablePayloads.Abstractions.ValueObjects
{
    /// <summary>
    /// The forgettable payload content value object.
    /// <remarks>
    /// The actual serialized content of forgettable payload.
    /// </remarks>
    /// </summary>
    public class ForgettablePayloadContent
    {
        /// <summary>
        /// The actual value of forgettable payload content.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForgettablePayloadContentDescriptor"/> class.
        /// </summary>
        /// <param name="value">
        /// The actual value of forgettable payload content.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="value"/> is null or whitespace.
        /// </exception>
        public ForgettablePayloadContent(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"{nameof(ForgettablePayloadContent)} cannot be null or whitespace.", nameof(value));
            }
            
            Value = value;
        }

        #region Operators

        /// <summary>
        /// Implicit operator that converts the <see cref="ForgettablePayloadContent"/> to <see cref="string"/>.
        /// </summary>
        /// <param name="content">
        /// The <see cref="ForgettablePayloadContent"/>.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static implicit operator string(ForgettablePayloadContent content) => content.Value;
        
        /// <summary>
        /// Implicit operator that converts the <see cref="string"/> to <see cref="ForgettablePayloadContent"/>.
        /// </summary>
        /// <param name="content">
        /// The <see cref="string"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ForgettablePayloadContent"/>.
        /// </returns>
        public static implicit operator ForgettablePayloadContent(string content) => new ForgettablePayloadContent(content);

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="content">
        /// The <see cref="ForgettablePayloadContent"/>.
        /// </param>
        /// <param name="otherContent">
        /// The <see cref="ForgettablePayloadContent"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="content"/> and <paramref name="otherContent"/> are equal, false otherwise.
        /// </returns>
        public static bool operator ==(ForgettablePayloadContent content, ForgettablePayloadContent otherContent) =>
            Equals(content, otherContent);

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="content">
        /// The <see cref="ForgettablePayloadContent"/>.
        /// </param>
        /// <param name="otherContent">
        /// The <see cref="ForgettablePayloadContent"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="content"/> and <paramref name="otherContent"/> are not equal, false otherwise.
        /// </returns>
        public static bool operator !=(ForgettablePayloadContent content, ForgettablePayloadContent otherContent) =>
            !(content == otherContent);

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is ForgettablePayloadContent other &&
            other.Value == Value;

        /// <inheritdoc />
        public override int GetHashCode() =>
            Value.GetHashCode();

        /// <inheritdoc />
        public override string ToString() =>
            Value;
    }
}
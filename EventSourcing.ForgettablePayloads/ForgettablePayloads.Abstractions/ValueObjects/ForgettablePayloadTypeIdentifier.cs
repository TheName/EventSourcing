﻿using System;

namespace EventSourcing.ForgettablePayloads.ValueObjects
{
    /// <summary>
    /// Represents actual forgettable payload's object type.
    /// </summary>
    public class ForgettablePayloadTypeIdentifier
    {
        /// <summary>
        /// The actual value of forgettable payload type identifier
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForgettablePayloadTypeIdentifier"/> class.
        /// </summary>
        /// <param name="value">
        /// The actual value of forgettable payload type identifier
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="value"/> is null or whitespace
        /// </exception>
        public ForgettablePayloadTypeIdentifier(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"{nameof(ForgettablePayloadTypeIdentifier)} cannot be null or whitespace.", nameof(value));
            }

            Value = value;
        }

        #region Operators

        /// <summary>
        /// Implicit operator that converts the <see cref="ForgettablePayloadTypeIdentifier"/> to <see cref="string"/>.
        /// </summary>
        /// <param name="typeIdentifier">
        /// The <see cref="ForgettablePayloadTypeIdentifier"/>.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static implicit operator string(ForgettablePayloadTypeIdentifier typeIdentifier) => typeIdentifier.Value;

        /// <summary>
        /// Implicit operator that converts the <see cref="string"/> to <see cref="ForgettablePayloadTypeIdentifier"/>.
        /// </summary>
        /// <param name="typeIdentifier">
        /// The <see cref="string"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ForgettablePayloadTypeIdentifier"/>.
        /// </returns>
        public static implicit operator ForgettablePayloadTypeIdentifier(string typeIdentifier) => new ForgettablePayloadTypeIdentifier(typeIdentifier);

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="forgettablePayloadTypeIdentifier">
        /// The <see cref="ForgettablePayloadTypeIdentifier"/>.
        /// </param>
        /// <param name="otherForgettablePayloadTypeIdentifier">
        /// The <see cref="ForgettablePayloadTypeIdentifier"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="forgettablePayloadTypeIdentifier"/> and <paramref name="otherForgettablePayloadTypeIdentifier"/> are equal, false otherwise.
        /// </returns>
        public static bool operator ==(ForgettablePayloadTypeIdentifier forgettablePayloadTypeIdentifier, ForgettablePayloadTypeIdentifier otherForgettablePayloadTypeIdentifier) =>
            Equals(forgettablePayloadTypeIdentifier, otherForgettablePayloadTypeIdentifier);

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="forgettablePayloadTypeIdentifier">
        /// The <see cref="ForgettablePayloadTypeIdentifier"/>.
        /// </param>
        /// <param name="otherForgettablePayloadTypeIdentifier">
        /// The <see cref="ForgettablePayloadTypeIdentifier"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="forgettablePayloadTypeIdentifier"/> and <paramref name="otherForgettablePayloadTypeIdentifier"/> are not equal, false otherwise.
        /// </returns>
        public static bool operator !=(ForgettablePayloadTypeIdentifier forgettablePayloadTypeIdentifier, ForgettablePayloadTypeIdentifier otherForgettablePayloadTypeIdentifier) =>
            !(forgettablePayloadTypeIdentifier == otherForgettablePayloadTypeIdentifier);

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is ForgettablePayloadTypeIdentifier other &&
            other.Value == Value;

        /// <inheritdoc />
        public override int GetHashCode() =>
            Value.GetHashCode();

        /// <inheritdoc />
        public override string ToString() =>
            Value;
    }
}

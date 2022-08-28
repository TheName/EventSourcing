using System;
using System.Collections.Generic;
using System.Linq;

namespace EventSourcing.ForgettablePayloads.Abstractions.ValueObjects
{
    /// <summary>
    /// The forgettable payload state value object.
    /// </summary>
    public class ForgettablePayloadState
    {
        private static readonly IReadOnlyCollection<string> ValidValues = new[]
        {
            nameof(Created),
            nameof(CreatedAndClaimed),
            nameof(Forgotten)
        };
        
        /// <summary>
        /// The Created state
        /// <remarks>
        /// Forgotten payload is in created state since creation until it gets claimed by an actually stored event.
        /// </remarks>
        /// </summary>
        public static readonly ForgettablePayloadState Created = new ForgettablePayloadState(nameof(Created));
        
        /// <summary>
        /// The Created and Claimed state
        /// <remarks>
        /// After creation, once event that caused creation of this payload is actually stored and published, the payload gets claimed.
        /// Unclaimed payloads get automatically forgotten.
        /// </remarks>
        /// </summary>
        public static readonly ForgettablePayloadState CreatedAndClaimed = new ForgettablePayloadState(nameof(CreatedAndClaimed));
        
        /// <summary>
        /// The Forgotten state
        /// <remarks>
        /// The original payload is no longer stored. 
        /// </remarks>
        /// </summary>
        public static readonly ForgettablePayloadState Forgotten = new ForgettablePayloadState(nameof(Forgotten));
        
        /// <summary>
        /// The actual value of forgettable payload state.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForgettablePayloadState"/> class.
        /// </summary>
        /// <param name="value">
        /// The actual value of forgettable payload state.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="value"/> is null or whitespace or not one of valid values (Created, CreatedAndClaimed, Forgotten)
        /// </exception>
        public ForgettablePayloadState(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"{nameof(ForgettablePayloadState)} cannot be null or whitespace.", nameof(value));
            }

            if (!ValidValues.Any(
                    validState => string.Equals(validState, value, StringComparison.InvariantCulture)))
            {
                throw new ArgumentException($"{nameof(ForgettablePayloadState)} cannot be \"{value}\".", nameof(value));
            }
            
            Value = value;
        }

        #region Operators

        /// <summary>
        /// Implicit operator that converts the <see cref="ForgettablePayloadState"/> to <see cref="string"/>.
        /// </summary>
        /// <param name="state">
        /// The <see cref="ForgettablePayloadState"/>.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static implicit operator string(ForgettablePayloadState state) => state.Value;
        
        /// <summary>
        /// Implicit operator that converts the <see cref="string"/> to <see cref="ForgettablePayloadState"/>.
        /// </summary>
        /// <param name="state">
        /// The <see cref="string"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ForgettablePayloadState"/>.
        /// </returns>
        public static implicit operator ForgettablePayloadState(string state) => new ForgettablePayloadState(state);

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="state">
        /// The <see cref="ForgettablePayloadState"/>.
        /// </param>
        /// <param name="otherState">
        /// The <see cref="ForgettablePayloadState"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="state"/> and <paramref name="otherState"/> are equal, false otherwise.
        /// </returns>
        public static bool operator ==(ForgettablePayloadState state, ForgettablePayloadState otherState) =>
            Equals(state, otherState);

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="state">
        /// The <see cref="ForgettablePayloadState"/>.
        /// </param>
        /// <param name="otherState">
        /// The <see cref="ForgettablePayloadState"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="state"/> and <paramref name="otherState"/> are not equal, false otherwise.
        /// </returns>
        public static bool operator !=(ForgettablePayloadState state, ForgettablePayloadState otherState) =>
            !(state == otherState);

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is ForgettablePayloadState other &&
            other.Value == Value;

        /// <inheritdoc />
        public override int GetHashCode() =>
            Value.GetHashCode();

        /// <inheritdoc />
        public override string ToString() =>
            Value;
    }
}
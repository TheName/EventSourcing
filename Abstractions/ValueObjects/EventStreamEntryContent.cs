using System;
using EventSourcing.Abstractions.Persistence.DataTransferObjects;

namespace EventSourcing.Abstractions.ValueObjects
{
    /// <summary>
    /// The event stream entry content value object.
    /// <remarks>
    /// The actual content of event stream entry.
    /// </remarks>
    /// </summary>
    public class EventStreamEntryContent
    {
        private string Value { get; }

        private EventStreamEntryContent(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"{nameof(EventStreamEntryContent)} cannot be null or whitespace.", nameof(value));
            }
            
            Value = value;
        }

        #region Operators

        /// <summary>
        /// Implicit operator that converts the <see cref="EventStreamEntryContent"/> to <see cref="string"/>.
        /// </summary>
        /// <param name="content">
        /// The <see cref="EventStreamEntryContent"/>.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static implicit operator string(EventStreamEntryContent content) => content.Value;
        
        /// <summary>
        /// Implicit operator that converts the <see cref="string"/> to <see cref="EventStreamEntryId"/>.
        /// </summary>
        /// <param name="content">
        /// The <see cref="string"/>.
        /// </param>
        /// <returns>
        /// The <see cref="EventStreamEntryId"/>.
        /// </returns>
        public static implicit operator EventStreamEntryContent(string content) => new EventStreamEntryContent(content);

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="eventStreamEntryContent">
        /// The <see cref="EventStreamEntryContent"/>.
        /// </param>
        /// <param name="otherEventStreamEntryContent">
        /// The <see cref="EventStreamEntryContent"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="eventStreamEntryContent"/> and <paramref name="otherEventStreamEntryContent"/> are equal, false otherwise.
        /// </returns>
        public static bool operator ==(EventStreamEntryContent eventStreamEntryContent, EventStreamEntryContent otherEventStreamEntryContent) =>
            Equals(eventStreamEntryContent, otherEventStreamEntryContent);

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="eventStreamEntryContent">
        /// The <see cref="EventStreamEntryContent"/>.
        /// </param>
        /// <param name="otherEventStreamEntryContent">
        /// The <see cref="EventStreamEntryContent"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="eventStreamEntryContent"/> and <paramref name="otherEventStreamEntryContent"/> are not equal, false otherwise.
        /// </returns>
        public static bool operator !=(EventStreamEntryContent eventStreamEntryContent, EventStreamEntryContent otherEventStreamEntryContent) =>
            !(eventStreamEntryContent == otherEventStreamEntryContent);

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is EventStreamEntryContent other &&
            other.GetHashCode() == GetHashCode();

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString() =>
            Value.ToString();
    }
}
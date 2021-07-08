using System;

namespace EventSourcing.Abstractions
{
    /// <summary>
    /// The event stream entry content value object.
    /// <remarks>
    /// The actual content of event stream entry.
    /// </remarks>
    /// </summary>
    public class EventStreamEventContent
    {
        private string Value { get; }

        private EventStreamEventContent(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"{nameof(EventStreamEventContent)} cannot be null or whitespace.", nameof(value));
            }
            
            Value = value;
        }

        #region Operators

        /// <summary>
        /// Implicit operator that converts the <see cref="EventStreamEventContent"/> to <see cref="string"/>.
        /// </summary>
        /// <param name="content">
        /// The <see cref="EventStreamEventContent"/>.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static implicit operator string(EventStreamEventContent content) => content.Value;
        
        /// <summary>
        /// Implicit operator that converts the <see cref="string"/> to <see cref="EventStreamEventContent"/>.
        /// </summary>
        /// <param name="content">
        /// The <see cref="string"/>.
        /// </param>
        /// <returns>
        /// The <see cref="EventStreamEventContent"/>.
        /// </returns>
        public static implicit operator EventStreamEventContent(string content) => new EventStreamEventContent(content);

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="eventStreamEntryContent">
        /// The <see cref="EventStreamEventContent"/>.
        /// </param>
        /// <param name="otherEventStreamEntryContent">
        /// The <see cref="EventStreamEventContent"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="eventStreamEntryContent"/> and <paramref name="otherEventStreamEntryContent"/> are equal, false otherwise.
        /// </returns>
        public static bool operator ==(EventStreamEventContent eventStreamEntryContent, EventStreamEventContent otherEventStreamEntryContent) =>
            Equals(eventStreamEntryContent, otherEventStreamEntryContent);

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="eventStreamEntryContent">
        /// The <see cref="EventStreamEventContent"/>.
        /// </param>
        /// <param name="otherEventStreamEntryContent">
        /// The <see cref="EventStreamEventContent"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="eventStreamEntryContent"/> and <paramref name="otherEventStreamEntryContent"/> are not equal, false otherwise.
        /// </returns>
        public static bool operator !=(EventStreamEventContent eventStreamEntryContent, EventStreamEventContent otherEventStreamEntryContent) =>
            !(eventStreamEntryContent == otherEventStreamEntryContent);

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is EventStreamEventContent other &&
            other.Value == Value;

        /// <inheritdoc />
        public override int GetHashCode() =>
            Value.GetHashCode();

        /// <inheritdoc />
        public override string ToString() =>
            Value;
    }
}
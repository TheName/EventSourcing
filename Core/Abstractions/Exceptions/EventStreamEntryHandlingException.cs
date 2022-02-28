using System;
using EventSourcing.Abstractions.ValueObjects;

namespace EventSourcing.Abstractions.Exceptions
{
    /// <summary>
    /// Handling an event stream entry has failed.
    /// </summary>
    [Serializable]
    public class EventStreamEntryHandlingException : Exception
    {
        /// <summary>
        /// The entry that has failed to be handled properly
        /// </summary>
        public EventStreamEntry Entry { get; }

        /// <summary>
        /// The time of handling the entry
        /// </summary>
        public EventStreamEntryHandlingTime HandlingTime { get; }

        private EventStreamEntryHandlingException(
            string message, 
            EventStreamEntry entry,
            EventStreamEntryHandlingTime handlingTime,
            Exception innerException)
            : base(message, innerException)
        {
            Entry = entry ?? throw new ArgumentNullException(nameof(entry));
            HandlingTime = handlingTime ?? throw new ArgumentNullException(nameof(handlingTime));
            if (!(innerException is AggregateException))
            {
                throw new ArgumentException(
                    $"Inner exception must be of type {typeof(AggregateException)} and the provided value is of type {innerException.GetType()}");
            }
        }

        /// <summary>
        /// Creates a new <see cref="EventStreamEntryHandlingException"/> instance with a default message.
        /// </summary>
        /// <returns>
        /// A new <see cref="EventStreamEntryHandlingException"/> instance.
        /// </returns>
        public static EventStreamEntryHandlingException New(
            EventStreamEntry entry,
            EventStreamEntryHandlingTime handlingTime,
            Exception exception) =>
            new EventStreamEntryHandlingException(
                "Handling event stream entry has failed",
                entry,
                handlingTime,
                GetAggregateException(exception));

        private static AggregateException GetAggregateException(Exception exception)
        {
            if (exception is AggregateException aggregateException)
            {
                return aggregateException;
            }

            return new AggregateException("Handling event stream entry failed", exception);
        }
    }
}
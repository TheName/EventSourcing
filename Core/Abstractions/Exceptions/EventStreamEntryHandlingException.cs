using System;
using System.Linq;
using System.Runtime.Serialization;
using EventSourcing.Abstractions.ValueObjects;

namespace EventSourcing.Abstractions.Exceptions
{
    /// <summary>
    /// Handling an event stream entry has failed.
    /// </summary>
    public class EventStreamEntryHandlingException
    {
        /// <summary>
        /// Exception's message
        /// </summary>
        public string Message { get; }

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
            EventStreamEntryHandlingTime handlingTime)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Entry = entry ?? throw new ArgumentNullException(nameof(entry));
            HandlingTime = handlingTime ?? throw new ArgumentNullException(nameof(handlingTime));
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
                GetAggregateExceptionMessage(exception),
                entry,
                handlingTime);

        private static string GetAggregateExceptionMessage(Exception exception)
        {
            var aggregateException = GetAggregateException(exception);
            var flattenedAggregateException = aggregateException.Flatten();

            var innerExceptionsMessages = flattenedAggregateException.InnerExceptions
                .Select((ex, index) => $"    {index}) {ex}");

            var innerExceptionsMessage = string.Join(Environment.NewLine, innerExceptionsMessages);

            return $"{aggregateException.Message}{Environment.NewLine}Inner exceptions:{Environment.NewLine}{innerExceptionsMessage}";
        }

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
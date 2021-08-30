using System;
using EventSourcing.Abstractions.ValueObjects;

namespace EventSourcing.Abstractions.Exceptions
{
    /// <summary>
    /// Appending entries to an event stream has failed due to optimistic concurrency exception.
    /// Provided sequences were already persisted in the event stream.
    /// </summary>
    [Serializable]
    public class EventStreamOptimisticConcurrencyException : EventStreamAppendingFailedException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventStreamOptimisticConcurrencyException"/> class.
        /// </summary>
        public EventStreamOptimisticConcurrencyException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStreamOptimisticConcurrencyException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">
        /// The message that describes the error.
        /// </param>
        public EventStreamOptimisticConcurrencyException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStreamOptimisticConcurrencyException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">
        /// The error message that explains the reason for the exception.
        /// </param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or a null reference if no inner exception is specified.
        /// </param>
        public EventStreamOptimisticConcurrencyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        
        /// <summary>
        /// Creates a new <see cref="EventStreamOptimisticConcurrencyException"/> instance with a default message.
        /// </summary>
        /// <param name="numberOfEntriesToAppend">
        /// The number of entries that were supposed to be stored.
        /// </param>
        /// <param name="streamId">
        /// The stream id that the entries were supposed to be appended to.
        /// </param>
        /// <returns>
        /// A new <see cref="EventStreamOptimisticConcurrencyException"/> instance.
        /// </returns>
        public new static EventStreamOptimisticConcurrencyException New(int numberOfEntriesToAppend, EventStreamId streamId) =>
            new EventStreamOptimisticConcurrencyException(
                $"Appending {numberOfEntriesToAppend} entries to {streamId} stream id failed due to optimistic concurrency exception. Another thread has already appended new entries with same sequences.");
    }
}
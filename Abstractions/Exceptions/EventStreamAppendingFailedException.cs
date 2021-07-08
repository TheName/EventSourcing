﻿using System;

namespace EventSourcing.Abstractions.Exceptions
{
    /// <summary>
    /// Appending events to an event stream has failed.
    /// </summary>
    [Serializable]
    public class EventStreamAppendingFailedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventStreamAppendingFailedException"/> class.
        /// </summary>
        public EventStreamAppendingFailedException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStreamAppendingFailedException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">
        /// The message that describes the error.
        /// </param>
        public EventStreamAppendingFailedException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStreamAppendingFailedException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">
        /// The error message that explains the reason for the exception.
        /// </param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or a null reference if no inner exception is specified.
        /// </param>
        public EventStreamAppendingFailedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        
        /// <summary>
        /// Creates a new <see cref="EventStreamAppendingFailedException"/> instance with a default message.
        /// </summary>
        /// <param name="numberOfEventsToAppend">
        /// The number of events that were supposed to be stored.
        /// </param>
        /// <param name="streamId">
        /// The stream id that the events were supposed to be appended to.
        /// </param>
        /// <returns>
        /// A new <see cref="EventStreamAppendingFailedException"/> instance.
        /// </returns>
        public static EventStreamAppendingFailedException New(int numberOfEventsToAppend, EventStreamId streamId) =>
            new EventStreamAppendingFailedException(
                $"Appending {numberOfEventsToAppend} events to {streamId} stream id failed due to an unknown reason.");
    }
}
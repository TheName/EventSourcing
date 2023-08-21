using System;
using EventSourcing.ValueObjects;

namespace EventSourcing.Exceptions
{
    /// <summary>
    /// Provided <see cref="EventStreamEntrySequence"/> is invalid.
    /// </summary>
    [Serializable]
    public class InvalidEventStreamEntrySequenceException : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidEventStreamEntrySequenceException"/> class.
        /// </summary>
        public InvalidEventStreamEntrySequenceException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidEventStreamEntrySequenceException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">
        /// The message that describes the error.
        /// </param>
        public InvalidEventStreamEntrySequenceException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidEventStreamEntrySequenceException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">
        /// The message that describes the error.
        /// </param>
        /// <param name="paramName">
        /// The name of the parameter that caused the current exception.
        /// </param>
        public InvalidEventStreamEntrySequenceException(string message, string paramName)
            : base(message, paramName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidEventStreamEntrySequenceException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">
        /// The error message that explains the reason for the exception.
        /// </param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or a null reference if no inner exception is specified.
        /// </param>
        public InvalidEventStreamEntrySequenceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Creates a new <see cref="InvalidEventStreamEntrySequenceException"/> instance with a default message.
        /// </summary>
        /// <param name="expectedSequence">
        /// The expected <see cref="EventStreamEntrySequence"/>.
        /// </param>
        /// <param name="providedSequence">
        /// The provided <see cref="EventStreamEntrySequence"/>.
        /// </param>
        /// <param name="paramName">
        /// The name of the parameter that caused the current exception.
        /// </param>
        /// <returns>
        /// A new <see cref="InvalidEventStreamEntrySequenceException"/> instance.
        /// </returns>
        public static InvalidEventStreamEntrySequenceException New(
            EventStreamEntrySequence expectedSequence,
            EventStreamEntrySequence providedSequence,
            string paramName) =>
            New(expectedSequence, providedSequence, string.Empty, paramName);

        /// <summary>
        /// Creates a new <see cref="InvalidEventStreamEntrySequenceException"/> instance with a default message.
        /// </summary>
        /// <param name="expectedSequence">
        /// The expected <see cref="EventStreamEntrySequence"/>.
        /// </param>
        /// <param name="providedSequence">
        /// The provided <see cref="EventStreamEntrySequence"/>.
        /// </param>
        /// <param name="additionalMessage">
        /// An additional message that should be added after the default message.
        /// </param>
        /// <param name="paramName">
        /// The name of the parameter that caused the current exception.
        /// </param>
        /// <returns>
        /// A new <see cref="InvalidEventStreamEntrySequenceException"/> instance.
        /// </returns>
        public static InvalidEventStreamEntrySequenceException New(
            EventStreamEntrySequence expectedSequence,
            EventStreamEntrySequence providedSequence,
            string additionalMessage,
            string paramName)
        {
            var message = $"Provided {nameof(EventStreamEntrySequence)} is invalid. Expected {nameof(EventStreamEntrySequence)}: {expectedSequence}, Provided {nameof(EventStreamEntrySequence)}: {providedSequence}.";
            if (!string.IsNullOrWhiteSpace(additionalMessage))
            {
                message = $"{message} {additionalMessage}";
            }

            return new InvalidEventStreamEntrySequenceException(
                message,
                paramName);
        }
    }
}

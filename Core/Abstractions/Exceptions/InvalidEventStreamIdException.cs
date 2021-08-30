using System;
using EventSourcing.Abstractions.ValueObjects;

namespace EventSourcing.Abstractions.Exceptions
{
    /// <summary>
    /// Provided <see cref="EventStreamId"/> is invalid.
    /// </summary>
    [Serializable]
    public class InvalidEventStreamIdException : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidEventStreamIdException"/> class.
        /// </summary>
        public InvalidEventStreamIdException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidEventStreamIdException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">
        /// The message that describes the error.
        /// </param>
        public InvalidEventStreamIdException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidEventStreamIdException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">
        /// The message that describes the error.
        /// </param>
        /// <param name="paramName">
        /// The name of the parameter that caused the current exception.
        /// </param>
        public InvalidEventStreamIdException(string message, string paramName)
            : base(message, paramName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidEventStreamIdException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">
        /// The error message that explains the reason for the exception.
        /// </param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or a null reference if no inner exception is specified.
        /// </param>
        public InvalidEventStreamIdException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Creates a new <see cref="InvalidEventStreamIdException"/> instance with a default message.
        /// </summary>
        /// <param name="expectedStreamId">
        /// The expected <see cref="EventStreamId"/>.
        /// </param>
        /// <param name="providedStreamId">
        /// The provided <see cref="EventStreamId"/>.
        /// </param>
        /// <param name="paramName">
        /// The name of the parameter that caused the current exception.
        /// </param>
        /// <returns>
        /// A new <see cref="InvalidEventStreamIdException"/> instance.
        /// </returns>
        public static InvalidEventStreamIdException New(
            EventStreamId expectedStreamId,
            EventStreamId providedStreamId,
            string paramName) =>
            New(expectedStreamId, providedStreamId, string.Empty, paramName);

        /// <summary>
        /// Creates a new <see cref="InvalidEventStreamIdException"/> instance with a default message.
        /// </summary>
        /// <param name="expectedStreamId">
        /// The expected <see cref="EventStreamId"/>.
        /// </param>
        /// <param name="providedStreamId">
        /// The provided <see cref="EventStreamId"/>.
        /// </param>
        /// <param name="additionalMessage">
        /// An additional message that should be added after the default message.
        /// </param>
        /// <param name="paramName">
        /// The name of the parameter that caused the current exception.
        /// </param>
        /// <returns>
        /// A new <see cref="InvalidEventStreamIdException"/> instance.
        /// </returns>
        public static InvalidEventStreamIdException New(
            EventStreamId expectedStreamId,
            EventStreamId providedStreamId,
            string additionalMessage,
            string paramName)
        {
            var message = $"Provided {nameof(EventStreamId)} is invalid. Expected {nameof(EventStreamId)}: {expectedStreamId}, Provided {nameof(EventStreamId)}: {providedStreamId}.";
            if (!string.IsNullOrWhiteSpace(additionalMessage))
            {
                message = $"{message} {additionalMessage}";
            }

            return new InvalidEventStreamIdException(
                message,
                paramName);
        }
    }
}
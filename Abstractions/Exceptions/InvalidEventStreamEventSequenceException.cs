using System;

namespace EventSourcing.Abstractions.Exceptions
{
    /// <summary>
    /// Provided <see cref="EventStreamEventSequence"/> is invalid. 
    /// </summary>
    [Serializable]
    public class InvalidEventStreamEventSequenceException : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidEventStreamEventSequenceException"/> class.
        /// </summary>
        public InvalidEventStreamEventSequenceException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidEventStreamEventSequenceException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">
        /// The message that describes the error.
        /// </param>
        public InvalidEventStreamEventSequenceException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidEventStreamEventSequenceException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">
        /// The message that describes the error.
        /// </param>
        /// <param name="paramName">
        /// The name of the parameter that caused the current exception.
        /// </param>
        public InvalidEventStreamEventSequenceException(string message, string paramName)
            : base(message, paramName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidEventStreamEventSequenceException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">
        /// The error message that explains the reason for the exception.
        /// </param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or a null reference if no inner exception is specified.
        /// </param>
        public InvalidEventStreamEventSequenceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Creates a new <see cref="InvalidEventStreamEventSequenceException"/> instance with a default message.
        /// </summary>
        /// <param name="expectedSequence">
        /// The expected <see cref="EventStreamEventSequence"/>.
        /// </param>
        /// <param name="providedSequence">
        /// The provided <see cref="EventStreamEventSequence"/>.
        /// </param>
        /// <param name="paramName">
        /// The name of the parameter that caused the current exception.
        /// </param>
        /// <returns>
        /// A new <see cref="InvalidEventStreamEventSequenceException"/> instance.
        /// </returns>
        public static InvalidEventStreamEventSequenceException New(
            EventStreamEventSequence expectedSequence,
            EventStreamEventSequence providedSequence,
            string paramName) =>
            New(expectedSequence, providedSequence, string.Empty, paramName);

        /// <summary>
        /// Creates a new <see cref="InvalidEventStreamEventSequenceException"/> instance with a default message.
        /// </summary>
        /// <param name="expectedSequence">
        /// The expected <see cref="EventStreamEventSequence"/>.
        /// </param>
        /// <param name="providedSequence">
        /// The provided <see cref="EventStreamEventSequence"/>.
        /// </param>
        /// <param name="additionalMessage">
        /// An additional message that should be added after the default message.
        /// </param>
        /// <param name="paramName">
        /// The name of the parameter that caused the current exception.
        /// </param>
        /// <returns>
        /// A new <see cref="InvalidEventStreamEventSequenceException"/> instance.
        /// </returns>
        public static InvalidEventStreamEventSequenceException New(
            EventStreamEventSequence expectedSequence,
            EventStreamEventSequence providedSequence,
            string additionalMessage,
            string paramName)
        {
            var message = $"Provided {nameof(EventStreamEventSequence)} is invalid. Expected {nameof(EventStreamEventSequence)}: {expectedSequence}, Provided {nameof(EventStreamEventSequence)}: {providedSequence}.";
            if (!string.IsNullOrWhiteSpace(additionalMessage))
            {
                message = $"{message} {additionalMessage}";
            }

            return new InvalidEventStreamEventSequenceException(
                message,
                paramName);
        }
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions.ValueObjects;

namespace EventSourcing.Abstractions.Handling
{
    /// <summary>
    /// Handles exceptions thrown during handling of an <see cref="EventStreamEntry"/>.
    /// </summary>
    public interface IEventHandlingExceptionsHandler
    {
        /// <summary>
        /// Handles exception thrown during handling of <see cref="EventStreamEntry"/>.
        /// </summary>
        /// <param name="entry">
        /// The handled <see cref="EventStreamEntry"/>.
        /// </param>
        /// <param name="exception">
        /// The exception thrown during handling.
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> representing the action.
        /// </returns>
        Task HandleAsync(EventStreamEntry entry, Exception exception, CancellationToken cancellationToken);
    }
}
using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.Abstractions.Handling
{
    /// <summary>
    /// Handles a published event.
    /// </summary>
    /// <typeparam name="T">
    /// The event's type.
    /// </typeparam>
    public interface IEventHandler<in T>
    {
        /// <summary>
        /// Handles event.
        /// </summary>
        /// <param name="event">
        /// The published event.
        /// </param>
        /// <param name="eventMetadata">
        /// The <see cref="EventStreamEventMetadata"/> describing the event.
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> representing the handling task.
        /// </returns>
        Task HandleAsync(T @event, EventStreamEventMetadata eventMetadata, CancellationToken cancellationToken);
    }
}
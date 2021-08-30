using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.Abstractions.Handling
{
    /// <summary>
    /// Dispatches <see cref="EventStreamEntry"/> to event's handlers.
    /// </summary>
    public interface IEventStreamEntryDispatcher
    {
        /// <summary>
        /// Dispatches provided <paramref name="entry"/> to registered event's handlers.
        /// </summary>
        /// <param name="entry">
        /// The <see cref="EventStreamEntry"/>.
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task DispatchAsync(EventStreamEntry entry, CancellationToken cancellationToken);
    }
}
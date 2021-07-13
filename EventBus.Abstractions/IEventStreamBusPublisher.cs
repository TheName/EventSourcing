using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions;

namespace EventSourcing.EventBus.Abstractions
{
    /// <summary>
    /// The event stream bus publisher used to publish event descriptors to the event bus.
    /// </summary>
    public interface IEventStreamBusPublisher
    {
        /// <summary>
        /// Publishes <paramref name="eventStreamEntries"/> to the event bus.
        /// </summary>
        /// <param name="eventStreamEntries">
        /// The <see cref="EventStreamEntries"/> that should be published.
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>.
        /// </param>
        /// <returns>
        /// The task that represents the publishing.
        /// </returns>
        Task PublishAsync(
            EventStreamEntries eventStreamEntries,
            CancellationToken cancellationToken);
    }
}
using System.Collections.Generic;
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
        /// Publishes <paramref name="eventDescriptors"/> to the event bus.
        /// </summary>
        /// <param name="eventDescriptors">
        /// The <see cref="IEnumerable{T}"/> of <see cref="EventStreamEventDescriptor"/> that should be published.
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>.
        /// </param>
        /// <returns>
        /// The task that represents the publishing.
        /// </returns>
        Task PublishAsync(
            IReadOnlyList<EventStreamEventDescriptor> eventDescriptors,
            CancellationToken cancellationToken);
    }
}
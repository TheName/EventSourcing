using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions;
using EventSourcing.Abstractions.ValueObjects;

namespace EventSourcing.Bus.Abstractions
{
    /// <summary>
    /// Publishes event stream entries to the event sourcing bus.
    /// </summary>
    public interface IEventSourcingBusPublisher
    {
        /// <summary>
        /// Publishes <paramref name="eventStreamEntry"/> to the event sourcing bus.
        /// </summary>
        /// <param name="eventStreamEntry">
        /// The <see cref="EventStreamEntry"/>.
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>
        /// </returns>
        Task PublishAsync(EventStreamEntry eventStreamEntry, CancellationToken cancellationToken);
    }
}
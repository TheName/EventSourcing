using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions;

namespace EventSourcing.Bus.Abstractions
{
    /// <summary>
    /// Publishes event stream entries to the event sourcing bus.
    /// </summary>
    public interface IEventSourcingBusPublisher
    {
        /// <summary>
        /// Publishes <paramref name="eventStreamEntries"/> to the event sourcing bus.
        /// </summary>
        /// <param name="eventStreamEntries">
        /// The collection of <see cref="EventStreamEntry"/>
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>
        /// </returns>
        Task PublishAsync(IEnumerable<EventStreamEntry> eventStreamEntries, CancellationToken cancellationToken);
    }
}
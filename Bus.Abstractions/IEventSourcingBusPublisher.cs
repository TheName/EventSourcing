using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions;

namespace EventSourcing.Bus.Abstractions
{
    public interface IEventSourcingBusPublisher
    {
        Task PublishAsync(IEnumerable<EventStreamEntry> eventStreamEntries, CancellationToken cancellationToken);
    }
}
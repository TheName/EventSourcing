using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.Bus.Abstractions
{
    public interface IEventSourcingBusConsumer
    {
        Task StartConsuming(CancellationToken cancellationToken);

        Task StopConsuming(CancellationToken cancellationToken);
    }
}
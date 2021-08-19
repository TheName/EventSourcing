using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.Bus.Abstractions
{
    public interface IEventSourcingBusInitializer
    {
        Task InitializeConnectionAsync(CancellationToken cancellationToken);
        Task DisposeConnectionAsync(CancellationToken cancellationToken);
    }
}
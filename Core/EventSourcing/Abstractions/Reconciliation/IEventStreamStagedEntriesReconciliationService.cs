using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Persistence.Abstractions.ValueObjects;

namespace EventSourcing.Abstractions.Reconciliation
{
    internal interface IEventStreamStagedEntriesReconciliationService
    {
        Task TryToReconcileStagedEntriesAsync(
            EventStreamStagedEntries stagedEntries,
            CancellationToken cancellationToken);
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions.Reconciliation;
using EventSourcing.Persistence.Abstractions;
using Microsoft.Extensions.Logging;

namespace EventSourcing.Reconciliation
{
    internal class ReconciliationJob : IReconciliationJob
    {
        private readonly IEventStreamStagingReader _stagingReader;
        private readonly IEventStreamStagedEntriesReconciliationService _reconciliationService;
        private readonly ILogger<ReconciliationJob> _logger;

        public ReconciliationJob(
            IEventStreamStagingReader stagingReader,
            IEventStreamStagedEntriesReconciliationService reconciliationService,
            ILogger<ReconciliationJob> logger)
        {
            _stagingReader = stagingReader ?? throw new ArgumentNullException(nameof(stagingReader));
            _reconciliationService = reconciliationService ?? throw new ArgumentNullException(nameof(reconciliationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var unmarkedStagedEntries = await _stagingReader.ReadUnmarkedStagedEntriesAsync(cancellationToken).ConfigureAwait(false);

            if (unmarkedStagedEntries == null)
            {
                throw new InvalidOperationException($"{_stagingReader.GetType()} has returned null when trying to read unmarked staged entries");
            }
            
            // Since this is a background job, we do not want to use too many resources
            // That's why we try to reconcile each entry sequentially instead of doing it in parallel.
            foreach (var unmarkedStagedEntry in unmarkedStagedEntries)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                try
                {
                    await _reconciliationService.TryToReconcileStagedEntriesAsync(
                            unmarkedStagedEntry,
                            cancellationToken)
                        .ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    if (e is OperationCanceledException && cancellationToken.IsCancellationRequested)
                    {
                        throw;
                    }

                    _logger.LogError(
                        e,
                        "Trying to reconcile staged entry {UnmarkedStagedEntry} has failed with an exception",
                        unmarkedStagedEntry);
                }
            }
        }
    }
}
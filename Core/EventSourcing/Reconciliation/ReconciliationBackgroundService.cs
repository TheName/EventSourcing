using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Configurations;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EventSourcing.Reconciliation
{
    internal class ReconciliationBackgroundService : BackgroundService
    {
        private readonly IEventSourcingConfiguration _configuration;
        private readonly IReconciliationJob _reconciliationJob;
        private readonly ILogger<ReconciliationBackgroundService> _logger;

        public ReconciliationBackgroundService(
            IEventSourcingConfiguration configuration,
            IReconciliationJob reconciliationJob,
            ILogger<ReconciliationBackgroundService> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _reconciliationJob = reconciliationJob ?? throw new ArgumentNullException(nameof(reconciliationJob));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await WaitUntilNextReconciliationJobExecution(stoppingToken).ConfigureAwait(false);

                    try
                    {
                        await _reconciliationJob.ExecuteAsync(stoppingToken).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException e)
                    {
                        if (!stoppingToken.IsCancellationRequested)
                        {
                            _logger.LogError(e, "There was an error when running reconciliation job");
                        }
                        else
                        {
                            _logger.LogDebug("Reconciliation job has been cancelled due to stopping of reconciliation service");
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "There was an error when running reconciliation job");
                    }
                }
            }
            finally
            {
                _logger.LogDebug("Reconciliation service has been stopped");
            }
        }

        private async Task WaitUntilNextReconciliationJobExecution(CancellationToken cancellationToken)
        {
            _logger.LogDebug(
                "Next reconciliation job will be triggered in {ReconciliationJobInterval}",
                _configuration.ReconciliationJobInterval);

            try
            {
                await Task.Delay(_configuration.ReconciliationJobInterval, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                _logger.LogDebug("Next reconciliation job will not be triggered due to stopping of reconciliation service");
                throw;
            }
        }
    }
}

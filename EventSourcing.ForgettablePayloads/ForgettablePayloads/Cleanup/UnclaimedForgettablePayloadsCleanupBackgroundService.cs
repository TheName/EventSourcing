using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.ForgettablePayloads.Configurations;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EventSourcing.ForgettablePayloads.Cleanup
{
    internal class UnclaimedForgettablePayloadsCleanupBackgroundService : BackgroundService
    {
        private readonly IForgettablePayloadConfiguration _configuration;
        private readonly IUnclaimedForgettablePayloadsCleanupJob _job;
        private readonly ILogger<UnclaimedForgettablePayloadsCleanupBackgroundService> _logger;

        public UnclaimedForgettablePayloadsCleanupBackgroundService(
            IForgettablePayloadConfiguration configuration,
            IUnclaimedForgettablePayloadsCleanupJob job,
            ILogger<UnclaimedForgettablePayloadsCleanupBackgroundService> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _job = job ?? throw new ArgumentNullException(nameof(job));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await WaitUntilNextCleanupJobExecution(stoppingToken).ConfigureAwait(false);
                
                    try
                    {
                        await _job.ExecuteAsync(stoppingToken).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException e)
                    {
                        if (!stoppingToken.IsCancellationRequested)
                        {
                            _logger.LogError(e, "There was an error when running unclaimed forgettable payloads cleanup job");
                        }
                        else
                        {
                            _logger.LogDebug("Unclaimed forgettable payloads cleanup job has been cancelled due to stopping of unclaimed forgettable payloads cleanup service");
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "There was an error when running unclaimed forgettable payloads cleanup job");
                    }
                }
            }
            finally
            {
                _logger.LogDebug("Reconciliation service has been stopped");
            }
        }

        private async Task WaitUntilNextCleanupJobExecution(CancellationToken cancellationToken)
        {
            _logger.LogDebug(
                "Next unclaimed forgettable payloads cleanup job will be triggered in {UnclaimedForgettablePayloadsCleanupJobInterval}",
                _configuration.UnclaimedForgettablePayloadsCleanupJobInterval);
            
            try
            {
                await Task.Delay(_configuration.UnclaimedForgettablePayloadsCleanupJobInterval, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                _logger.LogDebug("Next unclaimed forgettable payloads cleanup job will not be triggered due to stopping of unclaimed forgettable payloads cleanup service");
                throw;
            }
        }
    }
}
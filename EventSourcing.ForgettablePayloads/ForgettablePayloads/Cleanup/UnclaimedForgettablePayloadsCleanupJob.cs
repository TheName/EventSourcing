using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.ForgettablePayloads.Configurations;
using EventSourcing.ForgettablePayloads.Persistence;
using EventSourcing.ForgettablePayloads.Services;
using EventSourcing.ForgettablePayloads.ValueObjects;
using Microsoft.Extensions.Logging;

namespace EventSourcing.ForgettablePayloads.Cleanup
{
    internal class UnclaimedForgettablePayloadsCleanupJob : IUnclaimedForgettablePayloadsCleanupJob
    {
        private readonly IForgettablePayloadStorageReader _storageReader;
        private readonly IForgettablePayloadForgettingService _forgettingService;
        private readonly IForgettablePayloadConfiguration _configuration;
        private readonly ILogger<UnclaimedForgettablePayloadsCleanupJob> _logger;

        public UnclaimedForgettablePayloadsCleanupJob(
            IForgettablePayloadStorageReader storageReader,
            IForgettablePayloadForgettingService forgettingService,
            IForgettablePayloadConfiguration configuration,
            ILogger<UnclaimedForgettablePayloadsCleanupJob> logger)
        {
            _storageReader = storageReader ?? throw new ArgumentNullException(nameof(storageReader));
            _forgettingService = forgettingService ?? throw new ArgumentNullException(nameof(forgettingService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var unclaimedDescriptors = await _storageReader.ReadUnclaimedAsync(cancellationToken).ConfigureAwait(false);

            if (unclaimedDescriptors == null)
            {
                throw new InvalidOperationException($"{_storageReader.GetType()} has returned null when trying to read unclaimed forgettable payload descriptors");
            }

            // Since this is a background job, we do not want to use too many resources
            // That's why we try to cleanup each entry sequentially instead of doing it in parallel.
            foreach (var unclaimedDescriptor in unclaimedDescriptors)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var timeSinceLastModified = DateTime.UtcNow - unclaimedDescriptor.PayloadLastModifiedTime;
                var timeout = _configuration.UnclaimedForgettablePayloadsCleanupTimeout;
                if (timeSinceLastModified < timeout)
                {
                    _logger.LogDebug(
                        "Unclaimed forgettable payloads cleanup timeout which is configured to {UnclaimedForgettablePayloadsCleanupTimeout} has not yet passed since last modified time {LastModifiedTime} of forgotten payload {ForgottenPayloadDescriptor}",
                        timeout,
                        unclaimedDescriptor.PayloadLastModifiedTime,
                        unclaimedDescriptor);

                    continue;
                }

                try
                {
                    await _forgettingService.ForgetAsync(
                            unclaimedDescriptor,
                            ForgettingPayloadReason.GetDueToBeingUnclaimedForLongerThan(timeout),
                            ForgettingPayloadRequestedBy.UnclaimedForgettablePayloadCleanupJob,
                            cancellationToken)
                        .ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    _logger.LogError(
                        e,
                        "Trying to forget unclaimed forgettable payload has failed with an exception. Descriptor: {ForgottenPayloadDescriptor}",
                        unclaimedDescriptor);

                    if (e is OperationCanceledException && cancellationToken.IsCancellationRequested)
                    {
                        throw;
                    }
                }
            }
        }
    }
}

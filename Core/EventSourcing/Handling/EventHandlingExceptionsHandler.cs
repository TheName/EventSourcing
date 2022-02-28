using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions.Exceptions;
using EventSourcing.Abstractions.Handling;
using EventSourcing.Bus.Abstractions;
using Microsoft.Extensions.Logging;

namespace EventSourcing.Handling
{
    internal class EventHandlingExceptionsHandler : IEventHandlingExceptionsHandler
    {
        private readonly IEventSourcingBusHandlingExceptionPublisher _errorMessagePublisher;
        private readonly IEventSourcingBusHandlingExceptionPublisherConfiguration _errorMessagePublisherConfiguration;
        private readonly ILogger<EventHandlingExceptionsHandler> _logger;

        public EventHandlingExceptionsHandler(
            IEventSourcingBusHandlingExceptionPublisher errorMessagePublisher,
            IEventSourcingBusHandlingExceptionPublisherConfiguration errorMessagePublisherConfiguration,
            ILogger<EventHandlingExceptionsHandler> logger)
        {
            _errorMessagePublisher = errorMessagePublisher ?? throw new ArgumentNullException(nameof(errorMessagePublisher));
            _errorMessagePublisherConfiguration = errorMessagePublisherConfiguration ?? throw new ArgumentNullException(nameof(errorMessagePublisherConfiguration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task HandleAsync(EventStreamEntryHandlingException entryHandlingException, CancellationToken cancellationToken)
        {
            // even if cancellation was already requested,
            // we still want to try and publish the error message;
            // otherwise we would loose the information that handling has failed.
            // That's why after original cancellation is requested
            // we still wait additional error message publishing timeout before actually cancelling the task.
            var errorMessagePublishingCancellationTokenSource = new CancellationTokenSource();
            cancellationToken.Register(o =>
                    errorMessagePublishingCancellationTokenSource.CancelAfter(_errorMessagePublisherConfiguration.PublishingTimeout),
                state: null,
                useSynchronizationContext: false);
            
            try
            {
                await _errorMessagePublisher
                    .PublishAsync(entryHandlingException, errorMessagePublishingCancellationTokenSource.Token)
                    .ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    "An exception occurred when trying to publish entry handling exception {@EntryHandlingException}",
                    entryHandlingException);
                
                throw;
            }

            _logger.LogInformation("Successfully published entry handling exception {@EntryHandlingException}", entryHandlingException);
        }
    }
}
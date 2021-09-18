using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions.Handling;
using EventSourcing.Abstractions.ValueObjects;
using Microsoft.Extensions.Logging;

namespace EventSourcing.Handling
{
    internal class EventHandlingExceptionsHandler : IEventHandlingExceptionsHandler
    {
        private readonly ILogger<EventHandlingExceptionsHandler> _logger;

        public EventHandlingExceptionsHandler(ILogger<EventHandlingExceptionsHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public Task HandleAsync(EventStreamEntry entry, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "An exception occurred when trying to handle an event stream entry: {eventStreamEntry}", entry);
            return Task.CompletedTask;
        }
    }
}
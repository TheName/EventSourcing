using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Bus.Abstractions;
using Microsoft.Extensions.Hosting;

namespace EventSourcing.Bus
{
    internal class EventSourcingBusInitializerHostedService : IHostedService
    {
        private readonly IEventSourcingBusInitializer _initializer;

        public EventSourcingBusInitializerHostedService(IEventSourcingBusInitializer initializer)
        {
            _initializer = initializer ?? throw new ArgumentNullException(nameof(initializer));
        }
        
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _initializer.InitializeConnectionAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _initializer.DisposeConnectionAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
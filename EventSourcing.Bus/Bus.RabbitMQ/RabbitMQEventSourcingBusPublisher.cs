using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions;
using EventSourcing.Bus.Abstractions;
using EventSourcing.Bus.RabbitMQ.Abstractions.Helpers;
using EventSourcing.Bus.RabbitMQ.Abstractions.Providers;

namespace EventSourcing.Bus.RabbitMQ
{
    internal class RabbitMQEventSourcingBusPublisher : IEventSourcingBusPublisher
    {
        private readonly IRabbitMQPublisher _publisher;
        private readonly IRabbitMQConfigurationProvider _configurationProvider;

        public RabbitMQEventSourcingBusPublisher(
            IRabbitMQPublisher publisher,
            IRabbitMQConfigurationProvider configurationProvider)
        {
            _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
            _configurationProvider = configurationProvider ?? throw new ArgumentNullException(nameof(configurationProvider));
        }

        public async Task PublishAsync(EventStreamEntry eventStreamEntry, CancellationToken cancellationToken)
        {
            await _publisher.PublishAsync(
                    eventStreamEntry,
                    _configurationProvider.ExchangeName,
                    _configurationProvider.RoutingKey,
                    new Dictionary<string, string>
                    {
                        {"event-type-identifier", eventStreamEntry.EventDescriptor.EventTypeIdentifier.ToString()}
                    },
                    cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using EventSourcing.Abstractions;
using EventSourcing.Bus.RabbitMQ;
using EventSourcing.Bus.RabbitMQ.Abstractions.Helpers;
using EventSourcing.Bus.RabbitMQ.Abstractions.Providers;
using Moq;
using TestHelpers.Attributes;
using Xunit;

namespace Bus.RabbitMQ.UnitTests
{
    public class RabbitMQEventSourcingBusPublisher_Should
    {
        [Theory]
        [AutoMoqData]
        internal async Task Publish(
            EventStreamEntry entry,
            string exchangeName,
            string routingKey,
            [Frozen] Mock<IRabbitMQPublisher> rabbitMQPublisherMock,
            [Frozen] Mock<IRabbitMQConfigurationProvider> rabbitMQConfigurationProviderMock,
            RabbitMQEventSourcingBusPublisher eventSourcingBusPublisher)
        {
            rabbitMQConfigurationProviderMock
                .SetupGet(provider => provider.ExchangeName)
                .Returns(exchangeName);

            rabbitMQConfigurationProviderMock
                .SetupGet(provider => provider.RoutingKey)
                .Returns(routingKey);
            
            await eventSourcingBusPublisher.PublishAsync(entry, CancellationToken.None);

            var assertHeaders = new Func<IReadOnlyDictionary<string, string>, bool>(dictionary =>
            {
                Assert.Equal(dictionary["event-type-identifier"], entry.EventDescriptor.EventTypeIdentifier);

                return true;
            });

            rabbitMQPublisherMock.Verify(publisher => publisher.PublishAsync(
                    entry,
                    exchangeName,
                    routingKey,
                    It.Is<IReadOnlyDictionary<string, string>>(dictionary => assertHeaders(dictionary)),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
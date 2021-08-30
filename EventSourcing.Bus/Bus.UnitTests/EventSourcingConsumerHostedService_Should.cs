using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using EventSourcing.Bus;
using EventSourcing.Bus.Abstractions;
using Moq;
using TestHelpers.Attributes;
using Xunit;

namespace Bus.UnitTests
{
    public class EventSourcingConsumerHostedService_Should
    {
        [Fact]
        public void Throw_When_CreatingService_With_NullBusConsumer()
        {
            Assert.Throws<ArgumentNullException>(() => new EventSourcingConsumerHostedService(null));
        }
        
        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingService_With_NotNullArguments(IEventSourcingBusConsumer busConsumer)
        {
            _ = new EventSourcingConsumerHostedService(busConsumer);
        }

        [Theory]
        [AutoMoqData]
        internal async Task StartConsuming_When_StartingService(
            CancellationToken cancellationToken,
            [Frozen] Mock<IEventSourcingBusConsumer> busConsumerMock,
            EventSourcingConsumerHostedService service)
        {
            await service.StartAsync(cancellationToken);
            
            busConsumerMock.Verify(consumer => consumer.StartConsuming(cancellationToken), Times.Once);
            busConsumerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task StopConsuming_When_StoppingService(
            CancellationToken cancellationToken,
            [Frozen] Mock<IEventSourcingBusConsumer> busConsumerMock,
            EventSourcingConsumerHostedService service)
        {
            await service.StopAsync(cancellationToken);
            
            busConsumerMock.Verify(consumer => consumer.StopConsuming(cancellationToken), Times.Once);
            busConsumerMock.VerifyNoOtherCalls();
        }
    }
}
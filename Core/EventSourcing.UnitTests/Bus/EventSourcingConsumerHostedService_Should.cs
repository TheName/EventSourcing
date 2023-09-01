using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using EventSourcing.Bus;
using EventSourcing.Handling;
using Moq;
using TestHelpers.Attributes;
using Xunit;

namespace EventSourcing.UnitTests.Bus
{
    public class EventSourcingConsumerHostedService_Should
    {
        [Theory]
        [AutoMoqData]
        public void Throw_When_CreatingService_With_NullBusConsumer(IEventStreamEntryDispatcher dispatcher)
        {
            Assert.Throws<ArgumentNullException>(() => new EventSourcingConsumerHostedService(null, dispatcher));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_When_CreatingService_With_NullDispatcher(IEventSourcingBusConsumer busConsumer)
        {
            Assert.Throws<ArgumentNullException>(() => new EventSourcingConsumerHostedService(busConsumer, null));
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingService_With_NotNullArguments(
            IEventSourcingBusConsumer busConsumer,
            IEventStreamEntryDispatcher dispatcher)
        {
            _ = new EventSourcingConsumerHostedService(busConsumer, dispatcher);
        }

        [Theory]
        [AutoMoqData]
        internal async Task StartConsumingWithDispatcherFunc_When_StartingService(
            CancellationToken cancellationToken,
            [Frozen] Mock<IEventSourcingBusConsumer> busConsumerMock,
            [Frozen] IEventStreamEntryDispatcher dispatcherMock,
            EventSourcingConsumerHostedService service)
        {
            await service.StartAsync(cancellationToken);

            busConsumerMock.Verify(consumer => consumer.StartConsuming(dispatcherMock.DispatchAsync, cancellationToken), Times.Once);
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

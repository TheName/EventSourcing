using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using EventSourcing.Abstractions;
using EventSourcing.Abstractions.Handling;
using EventSourcing.Bus.RabbitMQ;
using EventSourcing.Bus.RabbitMQ.Abstractions.Helpers;
using EventSourcing.Bus.RabbitMQ.Abstractions.Providers;
using EventSourcing.Serialization.Abstractions;
using Moq;
using RabbitMQ.Client.Events;
using TestHelpers.Attributes;
using Xunit;

namespace Bus.RabbitMQ.UnitTests
{
    public class RabbitMQEventSourcingBusConsumer_Should
    {
        [Theory]
        [AutoMoqData]
        internal void Throw_When_Creating_With_NullConsumer(
            IRabbitMQConfigurationProvider configurationProvider,
            ISerializer serializer,
            IEventStreamEntryDispatcher dispatcher)
        {
            Assert.Throws<ArgumentNullException>(() => new RabbitMQEventSourcingBusConsumer(
                null,
                configurationProvider,
                serializer,
                dispatcher));
        }
        
        [Theory]
        [AutoMoqData]
        internal void Throw_When_Creating_With_NullConfigurationProvider(
            IRabbitMQConsumer consumer,
            ISerializer serializer,
            IEventStreamEntryDispatcher dispatcher)
        {
            Assert.Throws<ArgumentNullException>(() => new RabbitMQEventSourcingBusConsumer(
                consumer,
                null,
                serializer,
                dispatcher));
        }
        
        [Theory]
        [AutoMoqData]
        internal void Throw_When_Creating_With_NullSerializer(
            IRabbitMQConsumer consumer,
            IRabbitMQConfigurationProvider configurationProvider,
            IEventStreamEntryDispatcher dispatcher)
        {
            Assert.Throws<ArgumentNullException>(() => new RabbitMQEventSourcingBusConsumer(
                consumer,
                configurationProvider,
                null,
                dispatcher));
        }
        
        [Theory]
        [AutoMoqData]
        internal void Throw_When_Creating_With_NullDispatcher(
            IRabbitMQConsumer consumer,
            IRabbitMQConfigurationProvider configurationProvider,
            ISerializer serializer)
        {
            Assert.Throws<ArgumentNullException>(() => new RabbitMQEventSourcingBusConsumer(
                consumer,
                configurationProvider,
                serializer,
                null));
        }
        
        [Theory]
        [AutoMoqData]
        internal void NotThrow_When_Creating_With_NotNullParameters(
            IRabbitMQConsumer consumer,
            IRabbitMQConfigurationProvider configurationProvider,
            ISerializer serializer,
            IEventStreamEntryDispatcher dispatcher)
        {
            _ = new RabbitMQEventSourcingBusConsumer(
                consumer,
                configurationProvider,
                serializer,
                dispatcher);
        }

        [Theory]
        [AutoMoqData]
        internal async Task Consume_When_Calling_StartConsuming(
            string queueName,
            [Frozen] Mock<IRabbitMQConsumer> consumerMock,
            [Frozen] Mock<IRabbitMQConfigurationProvider> configurationProviderMock,
            RabbitMQEventSourcingBusConsumer consumer)
        {
            configurationProviderMock
                .SetupGet(provider => provider.QueueName)
                .Returns(queueName);
            
            await consumer.StartConsuming(CancellationToken.None);

            consumerMock
                .Verify(
                    mqConsumer => mqConsumer.Consume(
                        queueName,
                        It.IsAny<Func<BasicDeliverEventArgs, CancellationToken, Task>>()),
                    Times.Once);
        }
        
        [Theory]
        [AutoMoqData]
        internal async Task UseHandlerThatDeserializesEntriesAndDispatchesThem_When_StartingConsuming(
            BasicDeliverEventArgs args,
            EventStreamEntry entry,
            CancellationToken cancellationToken,
            [Frozen] Mock<IEventStreamEntryDispatcher> eventStreamEntryDispatcherMock,
            [Frozen] Mock<IRabbitMQConsumer> consumerMock,
            [Frozen] Mock<ISerializer> serializerMock,
            RabbitMQEventSourcingBusConsumer consumer)
        {
            Func<BasicDeliverEventArgs, CancellationToken, Task> usedHandler = null;

            consumerMock
                .Setup(mqConsumer => mqConsumer.Consume(
                    It.IsAny<string>(),
                    It.IsAny<Func<BasicDeliverEventArgs, CancellationToken, Task>>()))
                .Callback<string, Func<BasicDeliverEventArgs, CancellationToken, Task>>((_, handler) =>
                {
                    usedHandler = handler;
                });
            
            await consumer.StartConsuming(CancellationToken.None);

            serializerMock
                .Setup(serializer => serializer.DeserializeFromUtf8Bytes(
                        It.Is<byte[]>(bytes => bytes.SequenceEqual(args.Body.ToArray())), typeof(EventStreamEntry)))
                .Returns(entry);

            await usedHandler(args, cancellationToken);
            
            eventStreamEntryDispatcherMock.Verify(dispatcher => dispatcher.DispatchAsync(entry, cancellationToken), Times.Once);
            eventStreamEntryDispatcherMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task StopConsuming_When_Calling_StopConsuming(
            [Frozen] Mock<IRabbitMQConsumer> consumerMock,
            RabbitMQEventSourcingBusConsumer consumer)
        {
            await consumer.StopConsuming(CancellationToken.None);

            consumerMock.Verify(mqConsumer => mqConsumer.StopConsuming(), Times.Once);
        }
    }
}
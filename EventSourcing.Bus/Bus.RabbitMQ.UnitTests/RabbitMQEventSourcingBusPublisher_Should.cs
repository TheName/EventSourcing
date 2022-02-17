using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Bus.RabbitMQ.UnitTests.Helpers;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Bus.RabbitMQ;
using EventSourcing.Bus.RabbitMQ.Transport;
using Moq;
using TestHelpers.Attributes;
using Xunit;

namespace Bus.RabbitMQ.UnitTests
{
    public class RabbitMQEventSourcingBusPublisher_Should
    {
        [Fact]
        public void Throw_When_Creating_And_ProducerFactoryParameterIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new RabbitMQEventSourcingBusPublisher(null));
        }
        
        [Theory]
        [AutoMoqData]
        internal void NotThrow_When_Creating_And_ProducerFactoryParameterIsNotNull(IRabbitMQProducerFactory producerFactory)
        {
            _ = new RabbitMQEventSourcingBusPublisher(producerFactory);
        }

        [Fact]
        internal void DoNothing_When_Creating()
        {
            var producerFactoryMock = new Mock<IRabbitMQProducerFactory>();

            _ = new RabbitMQEventSourcingBusPublisher(producerFactoryMock.Object);
            
            producerFactoryMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task CreateProducer_When_PublishingForTheFirstTime(
            EventStreamEntry entry,
            Mock<IRabbitMQProducer<EventStreamEntry>> producerMock,
            [Frozen] Mock<IRabbitMQProducerFactory> producerFactoryMock,
            RabbitMQEventSourcingBusPublisher publisher)
        {
            producerFactoryMock
                .Setup(factory => factory.CreateAsync<EventStreamEntry>(It.IsAny<CancellationToken>()))
                .ReturnsAsync(producerMock.Object);

            await publisher.PublishAsync(entry, CancellationToken.None);

            producerMock.Verify(producer => producer.PublishAsync(entry, It.IsAny<CancellationToken>()), Times.Once);
            producerMock.VerifyNoOtherCalls();
            producerFactoryMock.Verify(factory => factory.CreateAsync<EventStreamEntry>(It.IsAny<CancellationToken>()), Times.Once);
            producerFactoryMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task CreateProducerOnce_When_PublishingMultipleTimes(
            List<EventStreamEntry> entries,
            Mock<IRabbitMQProducer<EventStreamEntry>> producerMock,
            [Frozen] Mock<IRabbitMQProducerFactory> producerFactoryMock,
            RabbitMQEventSourcingBusPublisher publisher)
        {
            producerFactoryMock
                .Setup(factory => factory.CreateAsync<EventStreamEntry>(It.IsAny<CancellationToken>()))
                .ReturnsAsync(producerMock.Object);

            foreach (var entry in entries)
            {
                await publisher.PublishAsync(entry, CancellationToken.None);
            }

            producerMock.Verify(producer => producer.PublishAsync(It.IsAny<EventStreamEntry>(), It.IsAny<CancellationToken>()), Times.Exactly(entries.Count));
            producerMock.VerifyNoOtherCalls();
            producerFactoryMock.Verify(factory => factory.CreateAsync<EventStreamEntry>(It.IsAny<CancellationToken>()), Times.Once);
            producerFactoryMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task CreateProducerOnce_When_PublishingMultipleTimesBeforeProducerIsCreated(
            List<EventStreamEntry> entries,
            Mock<IRabbitMQProducer<EventStreamEntry>> producerMock,
            [Frozen] Mock<IRabbitMQProducerFactory> producerFactoryMock,
            RabbitMQEventSourcingBusPublisher publisher)
        {
            var producerTaskCompletionSource = new TaskCompletionSource<IRabbitMQProducer<EventStreamEntry>>();
            producerFactoryMock
                .Setup(factory => factory.CreateAsync<EventStreamEntry>(It.IsAny<CancellationToken>()))
                .Returns(producerTaskCompletionSource.Task);
            
            var publishingTasks = entries
                .Select(entry => publisher.PublishAsync(entry, CancellationToken.None))
                .ToList();

            producerTaskCompletionSource.SetResult(producerMock.Object);

            await Task.WhenAll(publishingTasks);
            producerMock.Verify(producer => producer.PublishAsync(It.IsAny<EventStreamEntry>(), It.IsAny<CancellationToken>()), Times.Exactly(entries.Count));
            producerMock.VerifyNoOtherCalls();
            producerFactoryMock.Verify(factory => factory.CreateAsync<EventStreamEntry>(It.IsAny<CancellationToken>()), Times.Once);
            producerFactoryMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task RequestCancellation_When_CancellingPublishingCancellationToken_After_CallingPublish_And_ProducerIsBeingCreated(
            EventStreamEntry entry,
            [Frozen] Mock<IRabbitMQProducerFactory> producerFactoryMock,
            RabbitMQEventSourcingBusPublisher publisher)
        {
            var cancellationRequested = false;
            var cancellingTask = CancellingTasksHelper.CreateCancellingTaskWithReturnType<IRabbitMQProducer<EventStreamEntry>>(() => cancellationRequested = true);
            producerFactoryMock
                .Setup(factory => factory.CreateAsync<EventStreamEntry>(It.IsAny<CancellationToken>()))
                .Returns(cancellingTask);

            var publishingCancellationTokenSource = new CancellationTokenSource();

            var publishingTask = publisher.PublishAsync(entry, publishingCancellationTokenSource.Token);
            Assert.False(publishingTask.IsCompleted);

            publishingCancellationTokenSource.Cancel();

            await Assert.ThrowsAsync<OperationCanceledException>(() => publishingTask);
            Assert.True(cancellationRequested);
        }

        [Theory]
        [AutoMoqData]
        internal async Task RequestCancellation_When_CancellingPublishingCancellationToken_After_CallingPublish_And_ProducerIsPublishing(
            EventStreamEntry entry,
            Mock<IRabbitMQProducer<EventStreamEntry>> rabbitMQProducerMock,
            [Frozen] Mock<IRabbitMQProducerFactory> producerFactoryMock,
            RabbitMQEventSourcingBusPublisher publisher)
        {
            var cancellationRequested = false;
            var cancellingTask = CancellingTasksHelper.CreateCancellingTask<EventStreamEntry>(() => cancellationRequested = true);
            producerFactoryMock
                .Setup(factory => factory.CreateAsync<EventStreamEntry>(It.IsAny<CancellationToken>()))
                .ReturnsAsync(rabbitMQProducerMock.Object);

            rabbitMQProducerMock
                .Setup(producer => producer.PublishAsync(It.IsAny<EventStreamEntry>(), It.IsAny<CancellationToken>()))
                .Returns(cancellingTask);

            var publishingCancellationTokenSource = new CancellationTokenSource();

            var publishingTask = publisher.PublishAsync(entry, publishingCancellationTokenSource.Token);
            Assert.False(publishingTask.IsCompleted);

            publishingCancellationTokenSource.Cancel();

            await Assert.ThrowsAsync<OperationCanceledException>(() => publishingTask);
            Assert.True(cancellationRequested);
        }

        [Theory]
        [AutoMoqData]
        internal async Task RequestCancellation_When_Disposing_And_ProducerIsAwaitingCreation(
            EventStreamEntry entry,
            [Frozen] Mock<IRabbitMQProducerFactory> producerFactoryMock,
            RabbitMQEventSourcingBusPublisher publisher)
        {
            var cancellationRequested = false;
            var cancellingTask = CancellingTasksHelper.CreateCancellingTaskWithReturnType<IRabbitMQProducer<EventStreamEntry>>(() => cancellationRequested = true);
            producerFactoryMock
                .Setup(factory => factory.CreateAsync<EventStreamEntry>(It.IsAny<CancellationToken>()))
                .Returns(cancellingTask);

            var publishingTask = publisher.PublishAsync(entry, CancellationToken.None);
            Assert.False(publishingTask.IsCompleted);
            
            publisher.Dispose();

            await Assert.ThrowsAsync<OperationCanceledException>(() => publishingTask);
            Assert.True(cancellationRequested);
        }

        [Theory]
        [AutoMoqData]
        internal async Task RequestCancellation_When_Disposing_And_ProducerIsInProcessOfPublishing(
            EventStreamEntry entry,
            Mock<IRabbitMQProducer<EventStreamEntry>> producerMock,
            [Frozen] Mock<IRabbitMQProducerFactory> producerFactoryMock,
            RabbitMQEventSourcingBusPublisher publisher)
        {
            var cancellationRequested = false;
            producerFactoryMock
                .Setup(factory => factory.CreateAsync<EventStreamEntry>(It.IsAny<CancellationToken>()))
                .ReturnsAsync(producerMock.Object);
            
            var cancellingTask = CancellingTasksHelper.CreateCancellingTask<EventStreamEntry>(() => cancellationRequested = true);
            producerMock
                .Setup(producer => producer.PublishAsync(entry, It.IsAny<CancellationToken>()))
                .Returns(cancellingTask);

            var publishingTask = publisher.PublishAsync(entry, CancellationToken.None);
            Assert.False(publishingTask.IsCompleted);
            
            publisher.Dispose();

            await Assert.ThrowsAsync<OperationCanceledException>(() => publishingTask);
            Assert.True(cancellationRequested);
        }

        [Theory]
        [AutoMoqData]
        internal async Task ThrowOperationCancelledException_When_DisposingAfterProducerIsCreated_And_BeforeProducerPublishingIsCalled(
            EventStreamEntry entry,
            Mock<IRabbitMQProducer<EventStreamEntry>> producerMock,
            [Frozen] Mock<IRabbitMQProducerFactory> producerFactoryMock,
            RabbitMQEventSourcingBusPublisher publisher)
        {
            var producerTaskCompletionSource = new TaskCompletionSource<IRabbitMQProducer<EventStreamEntry>>();
            producerFactoryMock
                .Setup(factory => factory.CreateAsync<EventStreamEntry>(It.IsAny<CancellationToken>()))
                .Returns(producerTaskCompletionSource.Task);

            var publishingTask = publisher.PublishAsync(entry, CancellationToken.None);
            Assert.False(publishingTask.IsCompleted);
            
            publisher.Dispose();
            
            producerTaskCompletionSource.SetResult(producerMock.Object);

            await Assert.ThrowsAsync<OperationCanceledException>(() => publishingTask);
            producerMock.VerifyNoOtherCalls();
            producerFactoryMock.Verify(factory => factory.CreateAsync<EventStreamEntry>(It.IsAny<CancellationToken>()), Times.Once);
            producerFactoryMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task PublishEveryEntryUsingProducer_When_PublishingEntries(
            List<EventStreamEntry> entries,
            Mock<IRabbitMQProducer<EventStreamEntry>> producerMock,
            [Frozen] Mock<IRabbitMQProducerFactory> producerFactoryMock,
            RabbitMQEventSourcingBusPublisher publisher)
        {
            producerFactoryMock
                .Setup(factory => factory.CreateAsync<EventStreamEntry>(It.IsAny<CancellationToken>()))
                .ReturnsAsync(producerMock.Object);
            
            var publishingTasks = entries
                .Select(entry => publisher.PublishAsync(entry, CancellationToken.None))
                .ToList();

            await Task.WhenAll(publishingTasks);
            foreach (var entry in entries)
            {
                producerMock.Verify(producer => producer.PublishAsync(entry, It.IsAny<CancellationToken>()), Times.Once);
            }
            producerMock.VerifyNoOtherCalls();
            producerFactoryMock.Verify(factory => factory.CreateAsync<EventStreamEntry>(It.IsAny<CancellationToken>()), Times.Once);
            producerFactoryMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task DisposeProducer_When_Disposing_And_ProducerWasCreated(
            EventStreamEntry entry,
            [Frozen] Mock<IRabbitMQProducerFactory> producerFactoryMock,
            RabbitMQEventSourcingBusPublisher publisher)
        {
            var producerMock = new Mock<IRabbitMQProducer<EventStreamEntry>>();
            var producerDisposableMock = producerMock.As<IDisposable>();
            producerFactoryMock
                .Setup(factory => factory.CreateAsync<EventStreamEntry>(It.IsAny<CancellationToken>()))
                .ReturnsAsync(producerMock.Object);

            await publisher.PublishAsync(entry, CancellationToken.None);
            publisher.Dispose();
            
            producerMock.Verify(producer => producer.PublishAsync(entry, It.IsAny<CancellationToken>()), Times.Once);
            producerDisposableMock.Verify(producer => producer.Dispose(), Times.Once);
            producerFactoryMock.Verify(factory => factory.CreateAsync<EventStreamEntry>(It.IsAny<CancellationToken>()), Times.Once);
            producerFactoryMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task DisposeProducerOnce_When_DisposingMultipleTimes_And_ProducerWasCreated(
            EventStreamEntry entry,
            [Frozen] Mock<IRabbitMQProducerFactory> producerFactoryMock,
            RabbitMQEventSourcingBusPublisher publisher)
        {
            var producerMock = new Mock<IRabbitMQProducer<EventStreamEntry>>();
            var producerDisposableMock = producerMock.As<IDisposable>();
            producerFactoryMock
                .Setup(factory => factory.CreateAsync<EventStreamEntry>(It.IsAny<CancellationToken>()))
                .ReturnsAsync(producerMock.Object);

            await publisher.PublishAsync(entry, CancellationToken.None);
            publisher.Dispose();
            publisher.Dispose();
            publisher.Dispose();
            
            producerMock.Verify(producer => producer.PublishAsync(entry, It.IsAny<CancellationToken>()), Times.Once);
            producerDisposableMock.Verify(producer => producer.Dispose(), Times.Once);
            producerFactoryMock.Verify(factory => factory.CreateAsync<EventStreamEntry>(It.IsAny<CancellationToken>()), Times.Once);
            producerFactoryMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal void NotDisposeProducerFactory_When_Disposing(
            [Frozen] Mock<IRabbitMQProducerFactory> producerFactoryMock,
            RabbitMQEventSourcingBusPublisher publisher)
        {
            publisher.Dispose();
            
            producerFactoryMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task NotDisposeProducerFactory_When_Disposing_And_ProducerWasCreated(
            EventStreamEntry entry,
            [Frozen] Mock<IRabbitMQProducerFactory> producerFactoryMock,
            RabbitMQEventSourcingBusPublisher publisher)
        {
            var producerMock = new Mock<IRabbitMQProducer<EventStreamEntry>>();
            var producerDisposableMock = producerMock.As<IDisposable>();
            producerFactoryMock
                .Setup(factory => factory.CreateAsync<EventStreamEntry>(It.IsAny<CancellationToken>()))
                .ReturnsAsync(producerMock.Object);

            await publisher.PublishAsync(entry, CancellationToken.None);
            publisher.Dispose();
            
            producerMock.Verify(producer => producer.PublishAsync(entry, It.IsAny<CancellationToken>()), Times.Once);
            producerDisposableMock.Verify(producer => producer.Dispose(), Times.Once);
            producerFactoryMock.Verify(factory => factory.CreateAsync<EventStreamEntry>(It.IsAny<CancellationToken>()), Times.Once);
            producerFactoryMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task ThrowObjectDisposedException_When_TryingToPublish_After_PublisherWasAlreadyDisposed(
            EventStreamEntry entry,
            [Frozen] Mock<IRabbitMQProducerFactory> producerFactoryMock,
            RabbitMQEventSourcingBusPublisher publisher)
        {
            publisher.Dispose();
            
            await Assert.ThrowsAsync<ObjectDisposedException>(() => publisher.PublishAsync(entry, CancellationToken.None));
            
            producerFactoryMock.VerifyNoOtherCalls();
        }
    }
}
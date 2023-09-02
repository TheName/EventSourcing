using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Bus.RabbitMQ.UnitTests.Helpers;
using EventSourcing.Bus.RabbitMQ;
using EventSourcing.Bus.RabbitMQ.Transport;
using EventSourcing.ValueObjects;
using Moq;
using TestHelpers.Attributes;
using Xunit;

namespace Bus.RabbitMQ.UnitTests
{
    public class RabbitMQEventSourcingBusConsumer_Should
    {
        [Fact]
        public void Throw_When_Creating_And_ConsumerFactoryParameterIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new RabbitMQEventSourcingBusConsumer(null));
        }

        [Theory]
        [AutoMoqData]
        internal void NotThrow_When_Creating_And_ConsumerFactoryParameterIsNotNull(IRabbitMQConsumerFactory consumerFactory)
        {
            _ = new RabbitMQEventSourcingBusConsumer(consumerFactory);
        }

        [Fact]
        internal void DoNothing_When_Creating()
        {
            var consumerFactoryMock = new Mock<IRabbitMQConsumerFactory>();

            _ = new RabbitMQEventSourcingBusConsumer(consumerFactoryMock.Object);

            consumerFactoryMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task Throw_When_CallingStartConsing_And_CancellationIsAlreadyRequested(
            Func<EventStreamEntry, CancellationToken, Task> consumingTaskFunc,
            [Frozen] Mock<IRabbitMQConsumerFactory> consumerFactoryMock,
            RabbitMQEventSourcingBusConsumer consumer)
        {
            await Assert.ThrowsAsync<OperationCanceledException>(() => consumer.StartConsuming(consumingTaskFunc, new CancellationToken(true)));

            consumerFactoryMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task CreateConsumer_When_CallingStartConsumingForTheFirstTime(
            Func<EventStreamEntry, CancellationToken, Task> consumingTaskFunc,
            Mock<IRabbitMQConsumer<EventStreamEntry>> rabbitMQConsumerMock,
            [Frozen] Mock<IRabbitMQConsumerFactory> consumerFactoryMock,
            RabbitMQEventSourcingBusConsumer consumer)
        {
            consumerFactoryMock
                .Setup(factory => factory.CreateAsync(consumingTaskFunc, It.IsAny<CancellationToken>()))
                .ReturnsAsync(rabbitMQConsumerMock.Object);

            await consumer.StartConsuming(consumingTaskFunc, CancellationToken.None);

            consumerFactoryMock.Verify(factory => factory.CreateAsync(consumingTaskFunc, It.IsAny<CancellationToken>()), Times.Once);
            consumerFactoryMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task ThrowInvalidOperationException_When_CallingStartConsumingMultipleTimesAfterRabbitMQConsumerHasBeenCreated(
            Func<EventStreamEntry, CancellationToken, Task> consumingTaskFunc,
            Mock<IRabbitMQConsumer<EventStreamEntry>> rabbitMQConsumerMock,
            [Frozen] Mock<IRabbitMQConsumerFactory> consumerFactoryMock,
            RabbitMQEventSourcingBusConsumer consumer)
        {
            consumerFactoryMock
                .Setup(factory => factory.CreateAsync(consumingTaskFunc, It.IsAny<CancellationToken>()))
                .ReturnsAsync(rabbitMQConsumerMock.Object);

            await consumer.StartConsuming(consumingTaskFunc, CancellationToken.None);

            consumerFactoryMock.Reset();

            await Assert.ThrowsAsync<InvalidOperationException>(() => consumer.StartConsuming(consumingTaskFunc, CancellationToken.None));
            await Assert.ThrowsAsync<InvalidOperationException>(() => consumer.StartConsuming(consumingTaskFunc, CancellationToken.None));
            await Assert.ThrowsAsync<InvalidOperationException>(() => consumer.StartConsuming(consumingTaskFunc, CancellationToken.None));

            consumerFactoryMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task ThrowInvalidOperationExceptionForAllTasksButOne_When_CallingStartConsumingMultipleTimesBeforeRabbitMQConsumerHasBeenCreated(
            List<Func<EventStreamEntry, CancellationToken, Task>> consumingTaskFunctions,
            Mock<IRabbitMQConsumer<EventStreamEntry>> rabbitMQConsumerMock,
            [Frozen] Mock<IRabbitMQConsumerFactory> consumerFactoryMock,
            RabbitMQEventSourcingBusConsumer consumer)
        {
            var cancellationToken = CancellationToken.None;
            var consumerTaskCompletionSource = new TaskCompletionSource<IRabbitMQConsumer<EventStreamEntry>>();
            consumerFactoryMock
                .Setup(factory => factory.CreateAsync(
                    It.IsAny<Func<EventStreamEntry, CancellationToken, Task>>(),
                    It.IsAny<CancellationToken>()))
                .Returns(consumerTaskCompletionSource.Task);

            var startConsumingTasks = consumingTaskFunctions
                .Select(func => consumer.StartConsuming(func, cancellationToken))
                .ToList();

            consumerTaskCompletionSource.SetResult(rabbitMQConsumerMock.Object);

            var whenAllTask = Task.WhenAll(startConsumingTasks);

            await Assert.ThrowsAsync<InvalidOperationException>(() => whenAllTask);
            Assert.Equal(consumingTaskFunctions.Count - 1, startConsumingTasks.Count(task => task.IsFaulted));
            await Assert.Single(startConsumingTasks.Where(task => task.Status == TaskStatus.RanToCompletion));
            Assert.NotNull(whenAllTask.Exception);
            var flattenedException = whenAllTask.Exception.Flatten();
            Assert.All(flattenedException.InnerExceptions, exception => Assert.IsType<InvalidOperationException>(exception));
            rabbitMQConsumerMock.Verify(mqConsumer => mqConsumer.StartConsumingAsync(It.IsAny<CancellationToken>()), Times.Once);
            consumerFactoryMock.Verify(factory => factory.CreateAsync(It.IsAny<Func<EventStreamEntry,CancellationToken,Task>>(), It.IsAny<CancellationToken>()), Times.Once);
            consumerFactoryMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task RequestCancellation_When_CancellingStartConsumingCancellationToken_After_CallingStartConsuming_And_ConsumerIsBeingCreated(
            Func<EventStreamEntry, CancellationToken, Task> consumingTaskFunction,
            [Frozen] Mock<IRabbitMQConsumerFactory> consumerFactoryMock,
            RabbitMQEventSourcingBusConsumer consumer)
        {
            var cancellationRequested = false;
            var cancellingTask =
                CancellingTasksHelper.CreateCancellingTaskWithReturnType<
                    Func<EventStreamEntry, CancellationToken, Task>,
                    IRabbitMQConsumer<EventStreamEntry>>(() => cancellationRequested = true);

            consumerFactoryMock
                .Setup(factory => factory.CreateAsync(consumingTaskFunction, It.IsAny<CancellationToken>()))
                .Returns(cancellingTask);

            var startConsumingCancellationTokenSource = new CancellationTokenSource();

            var startConsumingTask = consumer.StartConsuming(consumingTaskFunction, startConsumingCancellationTokenSource.Token);
            Assert.False(startConsumingTask.IsCompleted);

            startConsumingCancellationTokenSource.Cancel();

            await Assert.ThrowsAsync<OperationCanceledException>(() => startConsumingTask);
            Assert.True(cancellationRequested);
        }

        [Theory]
        [AutoMoqData]
        internal async Task RequestCancellation_When_CancellingStartConsumingCancellationToken_After_CallingStartConsuming_And_ConsumerIsStartingCreated(
            Func<EventStreamEntry, CancellationToken, Task> consumingTaskFunction,
            Mock<IRabbitMQConsumer<EventStreamEntry>> rabbitMQConsumerMock,
            [Frozen] Mock<IRabbitMQConsumerFactory> consumerFactoryMock,
            RabbitMQEventSourcingBusConsumer consumer)
        {
            var cancellationRequested = false;
            var cancellingTask = CancellingTasksHelper.CreateCancellingTask(() => cancellationRequested = true);
            rabbitMQConsumerMock
                .Setup(mqConsumer => mqConsumer.StartConsumingAsync(It.IsAny<CancellationToken>()))
                .Returns(cancellingTask);

            consumerFactoryMock
                .Setup(producer => producer.CreateAsync(consumingTaskFunction, It.IsAny<CancellationToken>()))
                .ReturnsAsync(rabbitMQConsumerMock.Object);

            var startConsumingCancellationTokenSource = new CancellationTokenSource();
            var startConsumingTask = consumer.StartConsuming(consumingTaskFunction, startConsumingCancellationTokenSource.Token);
            Assert.False(startConsumingTask.IsCompleted);

            startConsumingCancellationTokenSource.Cancel();

            await Assert.ThrowsAsync<OperationCanceledException>(() => startConsumingTask);
            Assert.True(cancellationRequested);
        }

        [Theory]
        [AutoMoqData]
        internal async Task RequestCancellation_When_Disposing_And_ConsumerIsAwaitingCreation(
            Func<EventStreamEntry, CancellationToken, Task> consumingTaskFunction,
            [Frozen] Mock<IRabbitMQConsumerFactory> consumerFactoryMock,
            RabbitMQEventSourcingBusConsumer consumer)
        {
            var cancellationRequested = false;
            var cancellingTask =
                CancellingTasksHelper.CreateCancellingTaskWithReturnType<
                    Func<EventStreamEntry, CancellationToken, Task>,
                    IRabbitMQConsumer<EventStreamEntry>>(() => cancellationRequested = true);

            consumerFactoryMock
                .Setup(factory => factory.CreateAsync(consumingTaskFunction, It.IsAny<CancellationToken>()))
                .Returns(cancellingTask);

            var startConsumingCancellationTokenSource = new CancellationTokenSource();
            var startConsumingTask = consumer.StartConsuming(consumingTaskFunction, startConsumingCancellationTokenSource.Token);
            Assert.False(startConsumingTask.IsCompleted);

            consumer.Dispose();

            await Assert.ThrowsAsync<OperationCanceledException>(() => startConsumingTask);
            Assert.True(cancellationRequested);
        }

        [Theory]
        [AutoMoqData]
        internal async Task RequestCancellation_When_Disposing_And_ConsumerIsAlreadyCreatedAndStartingConsuming(
            Func<EventStreamEntry, CancellationToken, Task> consumingTaskFunction,
            Mock<IRabbitMQConsumer<EventStreamEntry>> rabbitMQConsumerMock,
            [Frozen] Mock<IRabbitMQConsumerFactory> consumerFactoryMock,
            RabbitMQEventSourcingBusConsumer consumer)
        {
            var cancellationRequested = false;
            var cancellingTask = CancellingTasksHelper.CreateCancellingTask(() => cancellationRequested = true);

            consumerFactoryMock
                .Setup(factory => factory.CreateAsync(consumingTaskFunction, It.IsAny<CancellationToken>()))
                .ReturnsAsync(rabbitMQConsumerMock.Object);

            rabbitMQConsumerMock
                .Setup(mqConsumer => mqConsumer.StartConsumingAsync(It.IsAny<CancellationToken>()))
                .Returns(cancellingTask);

            var startConsumingCancellationTokenSource = new CancellationTokenSource();
            var startConsumingTask = consumer.StartConsuming(consumingTaskFunction, startConsumingCancellationTokenSource.Token);
            Assert.False(startConsumingTask.IsCompleted);

            consumer.Dispose();

            await Assert.ThrowsAsync<OperationCanceledException>(() => startConsumingTask);
            Assert.True(cancellationRequested);
        }

        [Theory]
        [AutoMoqData]
        internal async Task RequestCancellation_When_Stopping_And_ConsumerIsAwaitingCreation(
            Func<EventStreamEntry, CancellationToken, Task> consumingTaskFunction,
            [Frozen] Mock<IRabbitMQConsumerFactory> consumerFactoryMock,
            RabbitMQEventSourcingBusConsumer consumer)
        {
            var cancellationRequested = false;
            var cancellingTask =
                CancellingTasksHelper.CreateCancellingTaskWithReturnType<
                    Func<EventStreamEntry, CancellationToken, Task>,
                    IRabbitMQConsumer<EventStreamEntry>>(() => cancellationRequested = true);

            consumerFactoryMock
                .Setup(factory => factory.CreateAsync(consumingTaskFunction, It.IsAny<CancellationToken>()))
                .Returns(cancellingTask);

            var startConsumingCancellationTokenSource = new CancellationTokenSource();
            var startConsumingTask = consumer.StartConsuming(consumingTaskFunction, startConsumingCancellationTokenSource.Token);
            Assert.False(startConsumingTask.IsCompleted);

            await consumer.StopConsuming(CancellationToken.None);

            await Assert.ThrowsAsync<OperationCanceledException>(() => startConsumingTask);
            Assert.True(cancellationRequested);
        }

        [Theory]
        [AutoMoqData]
        internal async Task RequestCancellation_When_Stopping_And_ConsumerIsAlreadyCreatedAndStartingConsuming(
            Func<EventStreamEntry, CancellationToken, Task> consumingTaskFunction,
            Mock<IRabbitMQConsumer<EventStreamEntry>> rabbitMQConsumerMock,
            [Frozen] Mock<IRabbitMQConsumerFactory> consumerFactoryMock,
            RabbitMQEventSourcingBusConsumer consumer)
        {
            var cancellationRequested = false;
            var cancellingTask = CancellingTasksHelper.CreateCancellingTask(() => cancellationRequested = true);

            consumerFactoryMock
                .Setup(factory => factory.CreateAsync(consumingTaskFunction, It.IsAny<CancellationToken>()))
                .ReturnsAsync(rabbitMQConsumerMock.Object);

            rabbitMQConsumerMock
                .Setup(mqConsumer => mqConsumer.StartConsumingAsync(It.IsAny<CancellationToken>()))
                .Returns(cancellingTask);

            var startConsumingCancellationTokenSource = new CancellationTokenSource();
            var startConsumingTask = consumer.StartConsuming(consumingTaskFunction, startConsumingCancellationTokenSource.Token);
            Assert.False(startConsumingTask.IsCompleted);

            await consumer.StopConsuming(CancellationToken.None);

            await Assert.ThrowsAsync<OperationCanceledException>(() => startConsumingTask);
            Assert.True(cancellationRequested);
        }

        [Theory]
        [AutoMoqData]
        internal async Task StartConsuming_When_CallingStartConsumingForTheFirstTime(
            Func<EventStreamEntry, CancellationToken, Task> consumingTaskFunc,
            Mock<IRabbitMQConsumer<EventStreamEntry>> rabbitMQConsumerMock,
            [Frozen] Mock<IRabbitMQConsumerFactory> consumerFactoryMock,
            RabbitMQEventSourcingBusConsumer consumer)
        {
            consumerFactoryMock
                .Setup(factory => factory.CreateAsync(consumingTaskFunc, It.IsAny<CancellationToken>()))
                .ReturnsAsync(rabbitMQConsumerMock.Object);

            await consumer.StartConsuming(consumingTaskFunc, CancellationToken.None);

            rabbitMQConsumerMock.Verify(mqConsumer => mqConsumer.StartConsumingAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        internal async Task ThrowOperationCancelledException_When_CallingStartConsumingAfterStopConsumingWasAlreadyCalled(
            Func<EventStreamEntry, CancellationToken, Task> consumingTaskFunc,
            [Frozen] Mock<IRabbitMQConsumerFactory> consumerFactoryMock,
            RabbitMQEventSourcingBusConsumer consumer)
        {
            await consumer.StopConsuming(CancellationToken.None);

            await Assert.ThrowsAsync<OperationCanceledException>(() => consumer.StartConsuming(consumingTaskFunc, CancellationToken.None));

            consumerFactoryMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task DoNothing_When_CallingStopConsuming_And_ConsumerWasNotCreated(
            [Frozen] Mock<IRabbitMQConsumerFactory> consumerFactoryMock,
            RabbitMQEventSourcingBusConsumer consumer)
        {
            await consumer.StopConsuming(CancellationToken.None);

            consumerFactoryMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task CallStopConsumingOnConsumer_When_CallingStopConsuming_And_ConsumerWasCreated(
            Func<EventStreamEntry, CancellationToken, Task> consumingTaskFunc,
            Mock<IRabbitMQConsumer<EventStreamEntry>> rabbitMQConsumerMock,
            [Frozen] Mock<IRabbitMQConsumerFactory> consumerFactoryMock,
            RabbitMQEventSourcingBusConsumer consumer)
        {
            consumerFactoryMock
                .Setup(factory => factory.CreateAsync(consumingTaskFunc, It.IsAny<CancellationToken>()))
                .ReturnsAsync(rabbitMQConsumerMock.Object);

            await consumer.StartConsuming(consumingTaskFunc, CancellationToken.None);

            consumerFactoryMock.Reset();
            rabbitMQConsumerMock.Reset();

            await consumer.StopConsuming(CancellationToken.None);

            rabbitMQConsumerMock.Verify(mqConsumer => mqConsumer.StopConsumingAsync(It.IsAny<CancellationToken>()), Times.Once);
            rabbitMQConsumerMock.VerifyNoOtherCalls();
            consumerFactoryMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task RequestCancellation_When_CancellingStopConsumingCancellationToken_After_CallingStopConsuming_And_ConsumerWasCreated(
            Func<EventStreamEntry, CancellationToken, Task> consumingTaskFunction,
            Mock<IRabbitMQConsumer<EventStreamEntry>> rabbitMQConsumerMock,
            [Frozen] Mock<IRabbitMQConsumerFactory> consumerFactoryMock,
            RabbitMQEventSourcingBusConsumer consumer)
        {
            var cancellationRequested = false;
            var cancellingTask = CancellingTasksHelper.CreateCancellingTask(() => cancellationRequested = true);
            rabbitMQConsumerMock
                .Setup(mqConsumer => mqConsumer.StopConsumingAsync(It.IsAny<CancellationToken>()))
                .Returns(cancellingTask);

            consumerFactoryMock
                .Setup(producer => producer.CreateAsync(consumingTaskFunction, It.IsAny<CancellationToken>()))
                .ReturnsAsync(rabbitMQConsumerMock.Object);

            await consumer.StartConsuming(consumingTaskFunction, CancellationToken.None);

            var stopConsumingCancellationTokenSource = new CancellationTokenSource();
            var stopConsumingTask = consumer.StopConsuming(stopConsumingCancellationTokenSource.Token);
            Assert.False(stopConsumingTask.IsCompleted);

            stopConsumingCancellationTokenSource.Cancel();

            await Assert.ThrowsAsync<OperationCanceledException>(() => stopConsumingTask);
            Assert.True(cancellationRequested);
        }

        [Theory]
        [AutoMoqData]
        internal async Task RequestCancellation_When_Disposing_And_CallingStopConsuming_And_ConsumerWasCreated(
            Func<EventStreamEntry, CancellationToken, Task> consumingTaskFunction,
            Mock<IRabbitMQConsumer<EventStreamEntry>> rabbitMQConsumerMock,
            [Frozen] Mock<IRabbitMQConsumerFactory> consumerFactoryMock,
            RabbitMQEventSourcingBusConsumer consumer)
        {
            var cancellationRequested = false;
            var cancellingTask = CancellingTasksHelper.CreateCancellingTask(() => cancellationRequested = true);

            rabbitMQConsumerMock
                .Setup(mqConsumer => mqConsumer.StopConsumingAsync(It.IsAny<CancellationToken>()))
                .Returns(cancellingTask);

            consumerFactoryMock
                .Setup(producer => producer.CreateAsync(consumingTaskFunction, It.IsAny<CancellationToken>()))
                .ReturnsAsync(rabbitMQConsumerMock.Object);

            await consumer.StartConsuming(consumingTaskFunction, CancellationToken.None);

            var stopConsumingCancellationTokenSource = new CancellationTokenSource();
            var stopConsumingTask = consumer.StopConsuming(stopConsumingCancellationTokenSource.Token);
            Assert.False(stopConsumingTask.IsCompleted);

            consumer.Dispose();

            await Assert.ThrowsAsync<OperationCanceledException>(() => stopConsumingTask);
            Assert.True(cancellationRequested);
        }

        [Theory]
        [AutoMoqData]
        internal async Task DisposeConsumer_When_Disposing_And_ConsumerWasCreated(
            Func<EventStreamEntry, CancellationToken, Task> consumingTaskFunc,
            [Frozen] Mock<IRabbitMQConsumerFactory> consumerFactoryMock,
            RabbitMQEventSourcingBusConsumer consumer)
        {
            var rabbitMQConsumerMock = new Mock<IRabbitMQConsumer<EventStreamEntry>>();
            var rabbitMQConsumerDisposableMock = rabbitMQConsumerMock.As<IDisposable>();
            consumerFactoryMock
                .Setup(factory => factory.CreateAsync(consumingTaskFunc, It.IsAny<CancellationToken>()))
                .ReturnsAsync(rabbitMQConsumerMock.Object);

            await consumer.StartConsuming(consumingTaskFunc, CancellationToken.None);
            consumer.Dispose();

            rabbitMQConsumerMock.Verify(mqConsumer => mqConsumer.StartConsumingAsync(It.IsAny<CancellationToken>()), Times.Once);
            rabbitMQConsumerDisposableMock.Verify(mqConsumer => mqConsumer.Dispose(), Times.Once);
            rabbitMQConsumerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task DisposeConsumerOnce_When_DisposingMultipleTimes_And_ConsumerWasCreated(
            Func<EventStreamEntry, CancellationToken, Task> consumingTaskFunc,
            [Frozen] Mock<IRabbitMQConsumerFactory> consumerFactoryMock,
            RabbitMQEventSourcingBusConsumer consumer)
        {
            var rabbitMQConsumerMock = new Mock<IRabbitMQConsumer<EventStreamEntry>>();
            var rabbitMQConsumerDisposableMock = rabbitMQConsumerMock.As<IDisposable>();
            consumerFactoryMock
                .Setup(factory => factory.CreateAsync(consumingTaskFunc, It.IsAny<CancellationToken>()))
                .ReturnsAsync(rabbitMQConsumerMock.Object);

            await consumer.StartConsuming(consumingTaskFunc, CancellationToken.None);
            consumer.Dispose();
            consumer.Dispose();
            consumer.Dispose();

            rabbitMQConsumerMock.Verify(mqConsumer => mqConsumer.StartConsumingAsync(It.IsAny<CancellationToken>()), Times.Once);
            rabbitMQConsumerDisposableMock.Verify(mqConsumer => mqConsumer.Dispose(), Times.Once);
            rabbitMQConsumerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal void NotDisposeConsumerFactory_When_Disposing(
            [Frozen] Mock<IRabbitMQConsumerFactory> consumerFactoryMock,
            RabbitMQEventSourcingBusConsumer consumer)
        {
            consumer.Dispose();

            consumerFactoryMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task NotDisposeConsumerFactory_When_Disposing_And_ConsumerWasCreated(
            Func<EventStreamEntry, CancellationToken, Task> consumingTaskFunc,
            [Frozen] Mock<IRabbitMQConsumerFactory> consumerFactoryMock,
            RabbitMQEventSourcingBusConsumer consumer)
        {
            var rabbitMQConsumerMock = new Mock<IRabbitMQConsumer<EventStreamEntry>>();
            var rabbitMQConsumerDisposableMock = rabbitMQConsumerMock.As<IDisposable>();
            consumerFactoryMock
                .Setup(factory => factory.CreateAsync(consumingTaskFunc, It.IsAny<CancellationToken>()))
                .ReturnsAsync(rabbitMQConsumerMock.Object);

            await consumer.StartConsuming(consumingTaskFunc, CancellationToken.None);
            consumer.Dispose();

            rabbitMQConsumerMock.Verify(mqConsumer => mqConsumer.StartConsumingAsync(It.IsAny<CancellationToken>()), Times.Once);
            rabbitMQConsumerDisposableMock.Verify(mqConsumer => mqConsumer.Dispose(), Times.Once);
            rabbitMQConsumerMock.VerifyNoOtherCalls();
            consumerFactoryMock.Verify(factory => factory.CreateAsync(It.IsAny<Func<EventStreamEntry,CancellationToken,Task>>(), It.IsAny<CancellationToken>()), Times.Once);
            consumerFactoryMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task ThrowObjectDisposedException_When_TryingToStartConsuming_After_ConsumerWasAlreadyDisposed(
            Func<EventStreamEntry, CancellationToken, Task> consumingTaskFunc,
            [Frozen] Mock<IRabbitMQConsumerFactory> consumerFactoryMock,
            RabbitMQEventSourcingBusConsumer consumer)
        {
            consumer.Dispose();

            await Assert.ThrowsAsync<ObjectDisposedException>(() => consumer.StartConsuming(consumingTaskFunc, CancellationToken.None));

            consumerFactoryMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task ThrowObjectDisposedException_When_TryingToStopConsuming_After_ConsumerWasAlreadyDisposed(
            [Frozen] Mock<IRabbitMQConsumerFactory> consumerFactoryMock,
            RabbitMQEventSourcingBusConsumer consumer)
        {
            consumer.Dispose();

            await Assert.ThrowsAsync<ObjectDisposedException>(() => consumer.StopConsuming(CancellationToken.None));

            consumerFactoryMock.VerifyNoOtherCalls();
        }
    }
}

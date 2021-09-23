using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Bus.RabbitMQ;
using EventSourcing.Bus.RabbitMQ.Abstractions.Channels;
using EventSourcing.Bus.RabbitMQ.Abstractions.Connections;
using EventSourcing.Bus.RabbitMQ.Configurations;
using Moq;
using TestHelpers.Attributes;
using Xunit;

namespace Bus.RabbitMQ.UnitTests
{
    public class RabbitMQEventSourcingBusPublisher_Should
    {
        [Theory]
        [AutoMoqData]
        internal void Throw_ArgumentNullException_When_CreatingWithNullConnection(EventSourcingRabbitMQChannelConfiguration configuration)
        {
            Assert.Throws<ArgumentNullException>(() => new RabbitMQEventSourcingBusPublisher(null, configuration));
        }
        
        [Theory]
        [AutoMoqData]
        internal void Throw_ArgumentNullException_When_CreatingWithNullConfiguration(IRabbitMQConnection connection)
        {
            Assert.Throws<ArgumentNullException>(() => new RabbitMQEventSourcingBusPublisher(connection, null));
        }
        
        [Theory]
        [AutoMoqData]
        internal void NotThrow_When_CreatingWithNotNullParameters(
            IRabbitMQConnection connection,
            EventSourcingRabbitMQChannelConfiguration configuration)
        {
            _ = new RabbitMQEventSourcingBusPublisher(connection, configuration);
        }

        [Theory]
        [AutoMoqData]
        internal void NotCreatePublishingChannel_When_Created(
            Mock<IRabbitMQConnection> connectionMock,
            EventSourcingRabbitMQChannelConfiguration configuration)
        {
            _ = new RabbitMQEventSourcingBusPublisher(connectionMock.Object, configuration);
            
            connectionMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal void DisposeGracefully(RabbitMQEventSourcingBusPublisher publisher)
        {
            publisher.Dispose();
        }

        [Theory]
        [AutoMoqData]
        internal async Task CreatePublishingChannel_When_Publishing(
            [Frozen] Mock<IRabbitMQConnection> connectionMock,
            [Frozen] EventSourcingRabbitMQChannelConfiguration configuration,
            RabbitMQEventSourcingBusPublisher publisher,
            EventStreamEntry eventStreamEntry,
            CancellationToken cancellationToken)
        {
            await publisher.PublishAsync(eventStreamEntry, cancellationToken);
            
            connectionMock.Verify(connection => connection.CreatePublishingChannel(configuration), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        internal async Task CreatePublishingChannelOnlyOnce_When_PublishingMultipleTimes(
            [Frozen] Mock<IRabbitMQConnection> connectionMock,
            [Frozen] EventSourcingRabbitMQChannelConfiguration configuration,
            RabbitMQEventSourcingBusPublisher publisher,
            EventStreamEntry eventStreamEntry,
            CancellationToken cancellationToken)
        {
            await publisher.PublishAsync(eventStreamEntry, cancellationToken);
            await publisher.PublishAsync(eventStreamEntry, cancellationToken);
            await publisher.PublishAsync(eventStreamEntry, cancellationToken);
            
            connectionMock.Verify(connection => connection.CreatePublishingChannel(configuration), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        internal void CreatePublishingChannelMultipleTimes_When_PublishingMultipleTimes_And_EachPublishingIsDoneFromSeparateThread(
            [Frozen] Mock<IRabbitMQConnection> connectionMock,
            [Frozen] EventSourcingRabbitMQChannelConfiguration configuration,
            RabbitMQEventSourcingBusPublisher publisher,
            EventStreamEntry eventStreamEntry,
            CancellationToken cancellationToken)
        {
            var threads = Enumerable.Range(0, 3)
                .Select(i => new Thread(async () =>
                {
                    await publisher.PublishAsync(eventStreamEntry, cancellationToken);
                }))
                .ToList();
            
            threads.ForEach(thread =>
            {
                thread.Start();
                thread.Join();
            });
            
            connectionMock.Verify(connection => connection.CreatePublishingChannel(configuration), Times.Exactly(3));
        }

        [Theory]
        [AutoMoqData]
        internal async Task PublishViaPublishingChannel_When_Publishing(
            Mock<IRabbitMQPublishingChannel> publishingChannelMock,
            [Frozen] Mock<IRabbitMQConnection> connectionMock,
            [Frozen] EventSourcingRabbitMQChannelConfiguration configuration,
            RabbitMQEventSourcingBusPublisher publisher,
            EventStreamEntry eventStreamEntry,
            CancellationToken cancellationToken)
        {
            connectionMock
                .Setup(connection => connection.CreatePublishingChannel(configuration))
                .Returns(publishingChannelMock.Object);
            
            await publisher.PublishAsync(eventStreamEntry, cancellationToken);
            
            publishingChannelMock.Verify(channel => channel.PublishAsync(eventStreamEntry, cancellationToken), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        internal async Task PublishViaSamePublishingChannel_When_PublishingMultipleTimes(
            Mock<IRabbitMQPublishingChannel> publishingChannelMock,
            [Frozen] Mock<IRabbitMQConnection> connectionMock,
            [Frozen] EventSourcingRabbitMQChannelConfiguration configuration,
            RabbitMQEventSourcingBusPublisher publisher,
            EventStreamEntry eventStreamEntry,
            CancellationToken cancellationToken)
        {
            connectionMock
                .Setup(connection => connection.CreatePublishingChannel(configuration))
                .Returns(publishingChannelMock.Object);
            
            await publisher.PublishAsync(eventStreamEntry, cancellationToken);
            await publisher.PublishAsync(eventStreamEntry, cancellationToken);
            await publisher.PublishAsync(eventStreamEntry, cancellationToken);
            
            publishingChannelMock.Verify(channel => channel.PublishAsync(eventStreamEntry, cancellationToken), Times.Exactly(3));
        }

        [Theory]
        [AutoMoqData]
        internal void PublishViaDifferentPublishingChannels_When_PublishingMultipleTimes_And_EachPublishingIsDoneFromSeparateThread(
            List<Mock<IRabbitMQPublishingChannel>> publishingChannelMocks,
            [Frozen] Mock<IRabbitMQConnection> connectionMock,
            [Frozen] EventSourcingRabbitMQChannelConfiguration configuration,
            RabbitMQEventSourcingBusPublisher publisher,
            EventStreamEntry eventStreamEntry,
            CancellationToken cancellationToken)
        {
            var sequenceSetup = connectionMock.SetupSequence(connection => connection.CreatePublishingChannel(configuration));
            foreach (var publishingChannelMock in publishingChannelMocks)
            {
                sequenceSetup.Returns(publishingChannelMock.Object);
            }
            
            var threads = Enumerable.Range(0, 3)
                .Select(i => new Thread(async () =>
                {
                    await publisher.PublishAsync(eventStreamEntry, cancellationToken);
                }))
                .ToList();
            
            threads.ForEach(thread =>
            {
                thread.Start();
                thread.Join();
            });
            
            foreach (var publishingChannelMock in publishingChannelMocks)
            {
                publishingChannelMock.Verify(channel => channel.PublishAsync(eventStreamEntry, cancellationToken), Times.Once);
            }
        }

        [Theory]
        [AutoMoqData]
        internal async Task DisposePublishingChannel_When_Disposing_And_PublisherDidPublish(
            [Frozen] Mock<IRabbitMQConnection> connectionMock,
            [Frozen] EventSourcingRabbitMQChannelConfiguration configuration,
            RabbitMQEventSourcingBusPublisher publisher,
            EventStreamEntry eventStreamEntry,
            CancellationToken cancellationToken)
        {
            var publishingChannelMock = new Mock<IRabbitMQPublishingChannel>();
            var disposablePublishingChannelMock = publishingChannelMock.As<IDisposable>();
            connectionMock
                .Setup(connection => connection.CreatePublishingChannel(configuration))
                .Returns(publishingChannelMock.Object);
            
            await publisher.PublishAsync(eventStreamEntry, cancellationToken);
            
            publisher.Dispose();
            
            disposablePublishingChannelMock.Verify(channel => channel.Dispose(), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        internal void DisposeAllPublishingChannels_When_Disposing_And_PublisherDidPublishMultipleTimes_And_EachPublishingIsDoneFromSeparateThread(
            [Frozen] Mock<IRabbitMQConnection> connectionMock,
            [Frozen] EventSourcingRabbitMQChannelConfiguration configuration,
            RabbitMQEventSourcingBusPublisher publisher,
            EventStreamEntry eventStreamEntry,
            CancellationToken cancellationToken)
        {
            var sequenceSetup = connectionMock.SetupSequence(connection => connection.CreatePublishingChannel(configuration));
            var disposablePublishingChannelMocks = new List<Mock<IDisposable>>();
            var threads = new List<Thread>();
            foreach (var _ in Enumerable.Range(0, 3))
            {
                threads.Add(new Thread(async () =>
                {
                    await publisher.PublishAsync(eventStreamEntry, cancellationToken);
                }));
                var publishingChannelMock = new Mock<IRabbitMQPublishingChannel>();
                var publishingDisposableChannelMock = publishingChannelMock.As<IDisposable>();
                disposablePublishingChannelMocks.Add(publishingDisposableChannelMock);
                sequenceSetup.Returns(publishingChannelMock.Object);
            }
            
            threads.ForEach(thread =>
            {
                thread.Start();
                thread.Join();
            });
            
            publisher.Dispose();

            foreach (var disposablePublishingChannelMock in disposablePublishingChannelMocks)
            {
                disposablePublishingChannelMock.Verify(disposable => disposable.Dispose(), Times.Once);
            }
        }

        [Theory]
        [AutoMoqData]
        internal void DisposePublishingChannel_When_ThreadInWhichChannelWasUsedIsFinished(
            [Frozen] Mock<IRabbitMQConnection> connectionMock,
            [Frozen] EventSourcingRabbitMQChannelConfiguration configuration,
            RabbitMQEventSourcingBusPublisher publisher,
            EventStreamEntry eventStreamEntry,
            CancellationToken cancellationToken)
        {
            var publishingChannelMock = new Mock<IRabbitMQPublishingChannel>();
            var disposablePublishingChannelMock = publishingChannelMock.As<IDisposable>();
            connectionMock
                .Setup(connection => connection.CreatePublishingChannel(configuration))
                .Returns(publishingChannelMock.Object);

            var publishingThread = new Thread(async () =>
            {
                await publisher.PublishAsync(eventStreamEntry, cancellationToken);
            });
            
            publishingThread.Start();
            publishingThread.Join();
            GC.GetTotalMemory(forceFullCollection: true);
            
            disposablePublishingChannelMock.Verify(channel => channel.Dispose(), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        internal void DisposeAllPublishingChannels_When_ThreadsInWhichChannelsWereUsedAreFinished(
            [Frozen] Mock<IRabbitMQConnection> connectionMock,
            [Frozen] EventSourcingRabbitMQChannelConfiguration configuration,
            RabbitMQEventSourcingBusPublisher publisher,
            EventStreamEntry eventStreamEntry,
            CancellationToken cancellationToken)
        {
            var sequenceSetup = connectionMock.SetupSequence(connection => connection.CreatePublishingChannel(configuration));
            var disposablePublishingChannelMocks = new List<Mock<IDisposable>>();
            var threads = new List<Thread>();
            foreach (var _ in Enumerable.Range(0, 3))
            {
                threads.Add(new Thread(async () =>
                {
                    await publisher.PublishAsync(eventStreamEntry, cancellationToken);
                }));
                var publishingChannelMock = new Mock<IRabbitMQPublishingChannel>();
                var publishingDisposableChannelMock = publishingChannelMock.As<IDisposable>();
                disposablePublishingChannelMocks.Add(publishingDisposableChannelMock);
                sequenceSetup.Returns(publishingChannelMock.Object);
            }
            
            threads.ForEach(thread =>
            {
                thread.Start();
                thread.Join();
            });
            GC.GetTotalMemory(forceFullCollection: true);
            
            foreach (var disposablePublishingChannelMock in disposablePublishingChannelMocks)
            {
                disposablePublishingChannelMock.Verify(disposable => disposable.Dispose(), Times.Once);
            }
        }
    }
}
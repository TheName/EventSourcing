using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using EventSourcing.Bus.RabbitMQ.Abstractions.Factories;
using EventSourcing.Bus.RabbitMQ.Providers;
using Moq;
using RabbitMQ.Client;
using TestHelpers.Attributes;
using Xunit;

namespace Bus.RabbitMQ.UnitTests.Providers
{
    public class RabbitMQChannelProvider_Should
    {
        [Theory]
        [AutoMoqData]
        internal void ReturnCreatedChannel(
            IModel channel,
            [Frozen] Mock<IRabbitMQChannelFactory> rabbitMQChannelFactoryMock)
        {
            rabbitMQChannelFactoryMock
                .Setup(factory => factory.Create())
                .Returns(channel);

            var rabbitMQChannelProvider = new RabbitMQChannelProvider(rabbitMQChannelFactoryMock.Object);

            var result = rabbitMQChannelProvider.PublishingChannel;

            Assert.Equal(channel, result);
        }
        
        [Theory]
        [AutoMoqData]
        internal void DisposeChannel_When_DisposingProvider_And_ChannelWasCreated(
            [Frozen] Mock<IRabbitMQChannelFactory> rabbitMQChannelFactoryMock)
        {
            var channelMock = new Mock<IModel>();
            rabbitMQChannelFactoryMock
                .Setup(factory => factory.Create())
                .Returns(channelMock.Object);

            var rabbitMQChannelProvider = new RabbitMQChannelProvider(rabbitMQChannelFactoryMock.Object);

            _ = rabbitMQChannelProvider.PublishingChannel;
            rabbitMQChannelProvider.Dispose();

            channelMock.Verify(model => model.Dispose());
        }
        
        [Theory]
        [AutoMoqData]
        internal void NotDisposeChannel_When_DisposingProvider_And_ChannelWasNotCreated(
            [Frozen] Mock<IRabbitMQChannelFactory> rabbitMQChannelFactoryMock)
        {
            var channelMock = new Mock<IModel>();
            rabbitMQChannelFactoryMock
                .Setup(factory => factory.Create())
                .Returns(channelMock.Object);

            var rabbitMQChannelProvider = new RabbitMQChannelProvider(rabbitMQChannelFactoryMock.Object);

            rabbitMQChannelProvider.Dispose();

            channelMock.VerifyNoOtherCalls();
        }
        
        [Theory]
        [AutoMoqData]
        internal void ReturnSameChannels_When_GettingPublishingChannelTwiceInSameThread(RabbitMQChannelProvider rabbitMQChannelProvider)
        {
            var firstResult = rabbitMQChannelProvider.PublishingChannel;
            var secondResult = rabbitMQChannelProvider.PublishingChannel;

            Assert.Equal(firstResult, secondResult);
        }

        [Theory]
        [AutoMoqData]
        internal void ReturnDifferentChannels_When_GettingPublishingChannelTwiceInDifferentThreads(IRabbitMQChannelFactory rabbitMQChannelFactory)
        {
            var rabbitMQChannelProvider = new RabbitMQChannelProvider(rabbitMQChannelFactory);
            
            var cancellationTokenSource = new CancellationTokenSource();
            IModel firstChannel = null;
            IModel secondChannel = null;
            var firstThread = new Thread(() =>
            {
                firstChannel = rabbitMQChannelProvider.PublishingChannel;
                Task.Delay(-1, cancellationTokenSource.Token);
            });
            
            var secondThread = new Thread(() =>
            {
                secondChannel = rabbitMQChannelProvider.PublishingChannel;
                Task.Delay(-1, cancellationTokenSource.Token);
            });
            
            firstThread.Start();
            secondThread.Start();
            firstThread.Join();
            secondThread.Join();
            
            Assert.NotNull(firstChannel);
            Assert.NotNull(secondChannel);
            Assert.NotEqual(firstChannel, secondChannel);
            cancellationTokenSource.Cancel();
        }

        [Theory]
        [InlineData(3)]
        [InlineData(10)]
        [InlineData(100)]
        internal void DisposeAllChannels_When_DisposingProvider_And_MultipleChannelsWereCreated(int numberOfThreads)
        {
            var rabbitMQChannelFactoryMock = new Mock<IRabbitMQChannelFactory>();
            var setupSequence = rabbitMQChannelFactoryMock.SetupSequence(factory => factory.Create());
            var allChannels = new List<Mock<IModel>>();

            foreach (var i in Enumerable.Range(0, numberOfThreads))
            {
                var modelMock = new Mock<IModel>();
                allChannels.Add(modelMock);
                setupSequence.Returns(modelMock.Object);
            }
            
            var rabbitMQChannelProvider = new RabbitMQChannelProvider(rabbitMQChannelFactoryMock.Object);

            var allThreads = new List<Thread>();
            var cancellationTokenSource = new CancellationTokenSource();
            foreach (var i in Enumerable.Range(0, numberOfThreads))
            {
                var thread = new Thread(() =>
                {
                    _ = rabbitMQChannelProvider.PublishingChannel;
                    Task.Delay(-1, cancellationTokenSource.Token);
                });
                
                allThreads.Add(thread);
            }
            
            allThreads.ForEach(thread => thread.Start());
            allThreads.ForEach(thread => thread.Join());
            
            rabbitMQChannelProvider.Dispose();
            
            Assert.All(allChannels, mock => mock.Verify(model => model.Dispose(), Times.Once));
            cancellationTokenSource.Cancel();
        }

        [Theory]
        [InlineData(10)]
        [InlineData(100)]
        internal void DisposeAllChannels_When_Finalizing_And_MultipleChannelsWereCreated(int numberOfThreads)
        {
            var rabbitMQChannelFactoryMock = new Mock<IRabbitMQChannelFactory>();
            var setupSequence = rabbitMQChannelFactoryMock.SetupSequence(factory => factory.Create());
            var allChannels = new List<Mock<IModel>>();
            var rabbitMQChannelProvider = new RabbitMQChannelProvider(rabbitMQChannelFactoryMock.Object);
            var cancellationTokenSource = new CancellationTokenSource();
            var allThreads = new List<Thread>();
            foreach (var i in Enumerable.Range(0, numberOfThreads))
            {
                var modelMock = new Mock<IModel>();
                allChannels.Add(modelMock);
                setupSequence.Returns(modelMock.Object);
                var thread = new Thread(() =>
                {
                    var channel = rabbitMQChannelProvider.PublishingChannel;
                    Assert.Equal(allChannels[i].Object, channel);
                    Task.Delay(-1, cancellationTokenSource.Token);
                });

                allThreads.Add(thread);
                thread.Start();
                thread.Join();
            }
            
            cancellationTokenSource.Cancel();
            allThreads.ForEach(thread => thread.Join());
            
            GC.GetTotalMemory(forceFullCollection: true);
            Thread.Sleep(50);
            
            Assert.All(allChannels, mock => mock.Verify(model => model.Dispose(), Times.Once));
        }
    }
}
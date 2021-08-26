using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions;
using EventSourcing.Bus.Abstractions;
using EventSourcing.Bus.RabbitMQ.Abstractions.Providers;
using RabbitMQ.Client;
using TestHelpers.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace Bus.RabbitMQ.IntegrationTests
{
    [Collection(nameof(RabbitMQCollectionDefinition))]
    public class RabbitMQEventSourcingBusPublisher_Should
    {
        private readonly RabbitMQCollectionFixture _fixture;

        private IEventSourcingBusPublisher Publisher => _fixture.GetService<IEventSourcingBusPublisher>();

        private IModel PublishingChannel => _fixture.GetService<IRabbitMQChannelProvider>().PublishingChannel;

        private IRabbitMQConfigurationProvider ConfigurationProvider => _fixture.GetService<IRabbitMQConfigurationProvider>();

        public RabbitMQEventSourcingBusPublisher_Should(RabbitMQCollectionFixture fixture, ITestOutputHelper testOutputHelper)
        {
            _fixture = fixture.SetTestOutputHelper(testOutputHelper);
        }

        [Theory]
        [AutoMoqData]
        public async Task PublishSuccessfully(EventStreamEntry entry)
        {
            PublishingChannel.QueuePurge(ConfigurationProvider.QueueName);
            var messageCount = PublishingChannel.MessageCount(ConfigurationProvider.QueueName);
            Assert.Equal((uint) 0, messageCount);
            
            await Publisher.PublishAsync(entry, CancellationToken.None);
            
            messageCount = PublishingChannel.MessageCount(ConfigurationProvider.QueueName);
            Assert.Equal((uint) 1, messageCount);
        }

        [Theory]
        [AutoMoqWithInlineData(3)]
        [AutoMoqWithInlineData(10)]
        [AutoMoqWithInlineData(100)]
        public async Task PublishSuccessfully_When_PublishingInParallel(
            int numberOfThreads,
            EventStreamEntry entry)
        {
            PublishingChannel.QueuePurge(ConfigurationProvider.QueueName);
            var messageCount = PublishingChannel.MessageCount(ConfigurationProvider.QueueName);
            Assert.Equal((uint) 0, messageCount);
            
            await Task.WhenAll(Enumerable.Range(0, numberOfThreads).Select(i => Publisher.PublishAsync(entry, CancellationToken.None)));
            
            messageCount = PublishingChannel.MessageCount(ConfigurationProvider.QueueName);
            Assert.Equal((uint) numberOfThreads, messageCount);
        }
    }
}
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions;
using EventSourcing.Bus.Abstractions;
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

        public RabbitMQEventSourcingBusPublisher_Should(RabbitMQCollectionFixture fixture, ITestOutputHelper testOutputHelper)
        {
            _fixture = fixture.SetTestOutputHelper(testOutputHelper);
        }

        [Theory]
        [AutoMoqData]
        public async Task PublishEntry(EventStreamEntry entry)
        {
            await Publisher.PublishAsync(entry, CancellationToken.None);
        }
    }
}
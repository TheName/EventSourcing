using Xunit;

namespace Bus.RabbitMQ.IntegrationTests
{
    [CollectionDefinition(nameof(RabbitMQCollectionDefinition))]
    public class RabbitMQCollectionDefinition : ICollectionFixture<RabbitMQCollectionFixture>
    {
    }
}
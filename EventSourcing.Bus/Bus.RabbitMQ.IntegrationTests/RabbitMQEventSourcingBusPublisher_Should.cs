using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Abstractions.Conversion;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Bus.Abstractions;
using EventSourcing.Serialization.Abstractions;
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

        private SimpleEventHandler SimpleEventHandler => _fixture.GetService<SimpleEventHandler>();

        private ISerializer Serializer => _fixture.GetService<ISerializerProvider>().GetEventContentSerializer();

        private IEventStreamEventTypeIdentifierConverter TypeIdentifierConverter => _fixture.GetService<IEventStreamEventTypeIdentifierConverter>();

        public RabbitMQEventSourcingBusPublisher_Should(RabbitMQCollectionFixture fixture, ITestOutputHelper testOutputHelper)
        {
            _fixture = fixture.SetTestOutputHelper(testOutputHelper);
        }

        [Theory]
        [AutoMoqData]
        public async Task PublishSuccessfully(SimpleEvent simpleEvent, EventStreamEventMetadata eventMetadata)
        {
            var entry = new EventStreamEntry(
                eventMetadata.StreamId,
                eventMetadata.EntryId,
                eventMetadata.EntrySequence,
                new EventStreamEventDescriptor(
                    Serializer.Serialize(simpleEvent),
                    Serializer.SerializationFormat,
                    TypeIdentifierConverter.ToTypeIdentifier(simpleEvent.GetType()),
                    TypeIdentifierConverter.TypeIdentifierFormat),
                eventMetadata.CausationId,
                eventMetadata.CreationTime,
                eventMetadata.CorrelationId);
            
            await Publisher.PublishAsync(entry, CancellationToken.None);

            Retry(TimeSpan.FromSeconds(9), () => Assert.Equal(1, SimpleEventHandler.GetNumberOfHandledEvents(simpleEvent.EventId)));
        }

        [Theory]
        [AutoMoqWithInlineData(3)]
        [AutoMoqWithInlineData(10)]
        [AutoMoqWithInlineData(100)]
        public async Task PublishSuccessfully_When_PublishingInParallel(
            int numberOfThreads,
            SimpleEvent simpleEvent,
            EventStreamEventMetadata eventMetadata)
        {
            var entry = new EventStreamEntry(
                eventMetadata.StreamId,
                eventMetadata.EntryId,
                eventMetadata.EntrySequence,
                new EventStreamEventDescriptor(
                    Serializer.Serialize(simpleEvent),
                    Serializer.SerializationFormat,
                    TypeIdentifierConverter.ToTypeIdentifier(simpleEvent.GetType()),
                    TypeIdentifierConverter.TypeIdentifierFormat),
                eventMetadata.CausationId,
                eventMetadata.CreationTime,
                eventMetadata.CorrelationId);
            
            await Task.WhenAll(Enumerable.Range(0, numberOfThreads).Select(i => Publisher.PublishAsync(entry, CancellationToken.None)));
            
            Retry(TimeSpan.FromSeconds(5), () => Assert.Equal(numberOfThreads, SimpleEventHandler.GetNumberOfHandledEvents(simpleEvent.EventId)));
        }

        private void Retry(TimeSpan timeSpan, Action action)
        {
            var stopwatch = Stopwatch.StartNew();
            while (stopwatch.Elapsed < timeSpan)
            {
                try
                {
                    action();
                    return;
                }
                catch (Exception)
                {
                    continue;
                }
            }

            throw new TimeoutException();
        }
    }
}
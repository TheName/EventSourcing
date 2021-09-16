using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using EventSourcing.Abstractions.Conversion;
using EventSourcing.Abstractions.Handling;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Handling;
using Moq;
using TestHelpers.Attributes;
using Xunit;

namespace EventSourcing.UnitTests.Handling
{
    public class EventStreamEntryDispatcher_Should
    {
        [Theory]
        [AutoMoqData]
        internal void Throw_When_Creating_With_NullEventConverter(IEventHandlerProvider eventHandlerProvider)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEntryDispatcher(null, eventHandlerProvider));
        }
        
        [Theory]
        [AutoMoqData]
        internal void Throw_When_Creating_With_NullEventHandlerProvider(IEventStreamEventConverter eventConverter)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEntryDispatcher(eventConverter, null));
        }
        
        [Theory]
        [AutoMoqData]
        internal void NotThrow_When_Creating_With_NoNullArguments(
            IEventStreamEventConverter eventConverter,
            IEventHandlerProvider eventHandlerProvider)
        {
            _ = new EventStreamEntryDispatcher(eventConverter, eventHandlerProvider);
        }

        [Theory]
        [AutoMoqData]
        internal async Task InvokeAllHandlers_When_DispatchingAnEntry(
            object @event,
            EventStreamEntry entry,
            CancellationToken cancellationToken, 
            [Frozen] Mock<IEventStreamEventConverter> eventConverterMock,
            [Frozen] Mock<IEventHandlerProvider> eventHandlerProviderMock,
            EventStreamEntryDispatcher dispatcher)
        {
            eventConverterMock
                .Setup(converter => converter.FromEventDescriptor(entry.EventDescriptor))
                .Returns(@event);

            var eventHandlerMocks = Enumerable.Range(0, 3)
                .Select(_ => new Mock<IEventHandler<object>>())
                .ToList();

            eventHandlerProviderMock
                .Setup(provider => provider.GetHandlersForType(typeof(object)))
                .Returns(eventHandlerMocks.Select(mock => mock.Object));

            await dispatcher.DispatchAsync(entry, cancellationToken);

            foreach (var eventHandlerMock in eventHandlerMocks)
            {
                eventHandlerMock.Verify(handler => handler.HandleAsync(@event, entry.ToEventMetadata(), cancellationToken), Times.Once);
                eventHandlerMock.VerifyNoOtherCalls();
            }
        }

        [Theory]
        [AutoMoqData]
        internal async Task SetCorrelationIdFromEventMetadataSoThatHandlersCouldUseId(
            object @event,
            EventStreamEntry entry,
            CancellationToken cancellationToken, 
            [Frozen] Mock<IEventStreamEventConverter> eventConverterMock,
            [Frozen] Mock<IEventHandlerProvider> eventHandlerProviderMock,
            EventStreamEntryDispatcher dispatcher)
        {
            EventStreamEntryCorrelationId currentCorrelationIdWhenHandlingEvent = null;
            eventConverterMock
                .Setup(converter => converter.FromEventDescriptor(entry.EventDescriptor))
                .Returns(@event);

            var handler = new Mock<IEventHandler<object>>();
            handler
                .Setup(eventHandler => eventHandler.HandleAsync(
                    It.IsAny<object>(),
                    It.IsAny<EventStreamEventMetadata>(),
                    It.IsAny<CancellationToken>()))
                .Returns(() =>
                {
                    currentCorrelationIdWhenHandlingEvent = EventStreamEntryCorrelationId.Current;
                    return Task.CompletedTask;
                });

            eventHandlerProviderMock
                .Setup(provider => provider.GetHandlersForType(typeof(object)))
                .Returns(() => new[] {handler.Object});
            
            await dispatcher.DispatchAsync(entry, cancellationToken);
            
            Assert.Equal(currentCorrelationIdWhenHandlingEvent, entry.CorrelationId);
            Assert.NotEqual(entry.CorrelationId, EventStreamEntryCorrelationId.Current);
        }

        [Theory]
        [AutoMoqData]
        internal async Task SetCausationIdFromEventEntryIdSoThatHandlersCouldUseId(
            object @event,
            EventStreamEntry entry,
            CancellationToken cancellationToken, 
            [Frozen] Mock<IEventStreamEventConverter> eventConverterMock,
            [Frozen] Mock<IEventHandlerProvider> eventHandlerProviderMock,
            EventStreamEntryDispatcher dispatcher)
        {
            EventStreamEntryCausationId currentCausationIdWhenHandlingEvent = null;
            eventConverterMock
                .Setup(converter => converter.FromEventDescriptor(entry.EventDescriptor))
                .Returns(@event);

            var handler = new Mock<IEventHandler<object>>();
            handler
                .Setup(eventHandler => eventHandler.HandleAsync(
                    It.IsAny<object>(),
                    It.IsAny<EventStreamEventMetadata>(),
                    It.IsAny<CancellationToken>()))
                .Returns(() =>
                {
                    currentCausationIdWhenHandlingEvent = EventStreamEntryCausationId.Current;
                    return Task.CompletedTask;
                });

            eventHandlerProviderMock
                .Setup(provider => provider.GetHandlersForType(typeof(object)))
                .Returns(() => new[] {handler.Object});
            
            await dispatcher.DispatchAsync(entry, cancellationToken);
            
            Assert.Equal(currentCausationIdWhenHandlingEvent, entry.EntryId);
            Assert.NotEqual(entry.CausationId, EventStreamEntryCausationId.Current);
        }
    }
}
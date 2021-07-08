using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Kernel;
using EventSourcing.Abstractions;
using EventSourcing.Persistence.Abstractions;

namespace TestHelpers.AutoFixture
{
    internal static class AutoFixtureFactory
    {
        public static IFixture Create()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization {ConfigureMembers = true});
            
            fixture.Register<ISpecimenBuilder, EventStream>(CreateEventStream);
            fixture.Register<ISpecimenBuilder, EventStreamEventCausationId>(builder => builder.Create<Guid>());
            fixture.Register<ISpecimenBuilder, EventStreamEventCorrelationId>(builder => builder.Create<Guid>());
            fixture.Register<ISpecimenBuilder, EventStreamEventContent>(builder => builder.Create<string>());
            fixture.Register<ISpecimenBuilder, EventStreamEventCreationTime>(builder => new DateTime(builder.Create<DateTime>().Ticks, DateTimeKind.Utc));
            fixture.Register<ISpecimenBuilder, EventStreamEventId>(builder => builder.Create<Guid>());
            fixture.Register<ISpecimenBuilder, EventStreamEvents>(CreateEventStreamEvents);
            fixture.Register<ISpecimenBuilder, EventStreamEventSequence>(builder => builder.Create<uint>());
            fixture.Register<ISpecimenBuilder, EventStreamEventTypeIdentifier>(builder => builder.Create<string>());
            fixture.Register<ISpecimenBuilder, EventStreamId>(builder => builder.Create<Guid>());
            fixture.Register<ISpecimenBuilder, EventStreamStagingId>(builder => builder.Create<Guid>());

            return fixture;
        }

        private static EventStream CreateEventStream(ISpecimenBuilder builder)
        {
            var events = builder.Create<EventStreamEvents>();
            events = new EventStreamEvents(events
                .Select((@event, i) => new EventStreamEvent(
                    @event.StreamId,
                    @event.EventId,
                    Convert.ToUInt32(i),
                    @event.Event,
                    @event.EventMetadata)));
            
            var streamId = events[0].StreamId;
            return new EventStream(streamId, events);
        }

        private static EventStreamEvents CreateEventStreamEvents(ISpecimenBuilder builder)
        {
            var streamId = builder.Create<EventStreamId>();
            var initialSequence = builder.Create<EventStreamEventSequence>();
            var events = builder.Create<List<EventStreamEvent>>();
            return new EventStreamEvents(events
                .Select(@event => new EventStreamEvent(
                    streamId,
                    @event.EventId,
                    initialSequence++,
                    @event.Event,
                    @event.EventMetadata)));
        }
    }
}
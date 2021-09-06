using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Kernel;
using EventSourcing.Abstractions;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Persistence.Abstractions.ValueObjects;

namespace TestHelpers.AutoFixture
{
    internal static class AutoFixtureFactory
    {
        public static IFixture Create()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization {ConfigureMembers = true});
            
            fixture.Register<ISpecimenBuilder, EventStream>(CreateEventStream);
            fixture.Register<ISpecimenBuilder, EventStreamEntryCausationId>(builder => builder.Create<Guid>());
            fixture.Register<ISpecimenBuilder, EventStreamEntryCorrelationId>(builder => builder.Create<Guid>());
            fixture.Register<ISpecimenBuilder, EventStreamEventContent>(builder => builder.Create<string>());
            fixture.Register<ISpecimenBuilder, EventStreamEntryCreationTime>(builder => new DateTime(builder.Create<DateTime>().Ticks, DateTimeKind.Utc));
            fixture.Register<ISpecimenBuilder, EventStreamEntryId>(builder => builder.Create<Guid>());
            fixture.Register<ISpecimenBuilder, EventStreamEntries>(CreateEventStreamEntries);
            fixture.Register<ISpecimenBuilder, EventStreamEntrySequence>(builder => builder.Create<uint>());
            fixture.Register<ISpecimenBuilder, EventStreamEventTypeIdentifier>(builder => builder.Create<string>());
            fixture.Register<ISpecimenBuilder, EventStreamId>(builder => builder.Create<Guid>());
            fixture.Register<ISpecimenBuilder, EventStreamStagingId>(builder => builder.Create<Guid>());

            return fixture;
        }

        private static EventStream CreateEventStream(ISpecimenBuilder builder)
        {
            var streamId = builder.Create<EventStreamId>();
            var eventsWithMetadata = builder.Create<IEnumerable<EventStreamEventWithMetadata>>();
            eventsWithMetadata = eventsWithMetadata
                .Select((eventWithMetadata, i) => new EventStreamEventWithMetadata(
                    eventWithMetadata.Event,
                    new EventStreamEventMetadata(
                        streamId,
                        eventWithMetadata.EventMetadata.EntryId,
                        Convert.ToUInt32(i),
                        eventWithMetadata.EventMetadata.CausationId,
                        eventWithMetadata.EventMetadata.CreationTime,
                        eventWithMetadata.EventMetadata.CorrelationId)));
            
            return new EventStream(streamId, eventsWithMetadata);
        }

        private static EventStreamEntries CreateEventStreamEntries(ISpecimenBuilder builder)
        {
            var streamId = builder.Create<EventStreamId>();
            var initialSequence = builder.Create<EventStreamEntrySequence>();
            var entries = builder.Create<List<EventStreamEntry>>();
            return new EventStreamEntries(entries
                .Select(entry => new EventStreamEntry(
                    streamId,
                    entry.EntryId,
                    initialSequence++,
                    entry.EventDescriptor,
                    entry.CausationId,
                    entry.CreationTime,
                    entry.CorrelationId)));
        }
    }
}
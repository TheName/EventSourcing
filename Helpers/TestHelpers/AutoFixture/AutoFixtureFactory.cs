using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Kernel;
using EventSourcing.ForgettablePayloads.ValueObjects;
using EventSourcing.Persistence.ValueObjects;
using EventSourcing.ValueObjects;

namespace TestHelpers.AutoFixture
{
    internal static class AutoFixtureFactory
    {
        private static readonly Random Random = new();

        private static readonly IReadOnlyList<ForgettablePayloadState> ValidForgettablePayloadStates =
            new List<ForgettablePayloadState>
            {
                ForgettablePayloadState.Created,
                ForgettablePayloadState.CreatedAndClaimed,
                ForgettablePayloadState.Forgotten
            };

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
            fixture.Register<ISpecimenBuilder, EventStreamEventTypeIdentifierFormat>(builder => builder.Create<string>());
            fixture.Register<ISpecimenBuilder, EventStreamId>(builder => builder.Create<Guid>());
            fixture.Register<ISpecimenBuilder, EventStreamStagingId>(builder => builder.Create<Guid>());
            fixture.Register<ISpecimenBuilder, SerializationFormat>(builder => builder.Create<string>());
            fixture.Register(CreateForgettablePayloadState);
            fixture.Register<ISpecimenBuilder, ForgettingPayloadTime>(builder => new DateTime(builder.Create<DateTime>().Ticks, DateTimeKind.Utc));
            fixture.Register<ISpecimenBuilder, ForgettablePayloadCreationTime>(builder => new DateTime(builder.Create<DateTime>().Ticks, DateTimeKind.Utc));
            fixture.Register<ISpecimenBuilder, ForgettablePayloadLastModifiedTime>(builder => new DateTime(builder.Create<DateTime>().Ticks, DateTimeKind.Utc));
            fixture.Register<ISpecimenBuilder, ForgettablePayloadContent>(builder => builder.Create<string>());
            fixture.Register<ISpecimenBuilder, ForgettablePayloadTypeIdentifier>(builder => builder.Create<string>());
            fixture.Register<ISpecimenBuilder, ForgettablePayloadTypeIdentifierFormat>(builder => builder.Create<string>());

            return fixture;
        }

        private static ForgettablePayloadState CreateForgettablePayloadState()
        {
            return ValidForgettablePayloadStates[Random.Next(0, ValidForgettablePayloadStates.Count)];
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

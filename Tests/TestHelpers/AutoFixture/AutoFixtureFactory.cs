using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Kernel;
using EventSourcing.Abstractions;
using EventSourcing.Abstractions.Persistence.DataTransferObjects;
using EventSourcing.Abstractions.ValueObjects;

namespace TestHelpers.AutoFixture
{
    internal static class AutoFixtureFactory
    {
        public static IFixture Create()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization {ConfigureMembers = true});

            fixture.Register<ISpecimenBuilder, EventStreamEntryCausationId>(builder => builder.Create<Guid>());
            fixture.Register<ISpecimenBuilder, EventStreamEntryCorrelationId>(builder => builder.Create<Guid>());
            fixture.Register<ISpecimenBuilder, EventStreamEntries>(CreateEventStreamEntries);
            fixture.Register<ISpecimenBuilder, EventStreamEntryContent>(builder => builder.Create<string>());
            fixture.Register<ISpecimenBuilder, EventStreamEntryCreationTime>(builder => new DateTime(builder.Create<DateTime>().Ticks, DateTimeKind.Utc));
            fixture.Register<ISpecimenBuilder, EventStreamEntryId>(builder => builder.Create<Guid>());
            fixture.Register<ISpecimenBuilder, EventStreamId>(builder => builder.Create<Guid>());
            fixture.Register<ISpecimenBuilder, EventStreamEntrySequence>(builder => builder.Create<uint>());

            return fixture;
        }

        private static EventStreamEntries CreateEventStreamEntries(ISpecimenBuilder builder)
        {
            var streamId = builder.Create<EventStreamId>();
            var initialSequence = builder.Create<EventStreamEntrySequence>();
            var entries = builder.Create<List<EventStreamEntry>>();
            return new EventStreamEntries(entries
                .Select(entry => new EventStreamEntry(
                    streamId,
                    initialSequence++,
                    entry.EntryId,
                    entry.Content,
                    entry.Metadata))
                .ToList());
        }
    }
}
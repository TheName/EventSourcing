using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventSourcing.Abstractions;
using EventSourcing.Abstractions.Exceptions;
using TestHelpers.Attributes;
using Xunit;

namespace Abstractions.UnitTests
{
    public class EventStream_Should
    {
        [Fact]
        public void CreateNewEventStreamWithEmptyEntries_When_CallingNewEventStream()
        {
            var stream = EventStream.NewEventStream();
            Assert.NotNull(stream.StreamId);
            Assert.Empty(stream.Entries);
            Assert.Equal<uint>(0, stream.CurrentSequence);
            Assert.Empty(stream.EntriesToAppend);
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullStreamId(
            EventStreamEntries entries)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStream(
                null,
                entries));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullEntries(
            EventStreamId streamId)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStream(
                streamId,
                null));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_InvalidEventStreamIdException_When_CreatingWithDifferentStreamIdThanAssignedToEntries(
            EventStreamId streamId,
            EventStreamEntries entries)
        {
            Assert.Contains(entries, entry => entry.StreamId != streamId);
            Assert.Throws<InvalidEventStreamIdException>(() => new EventStream(
                streamId, 
                entries));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_InvalidEventStreamEntrySequenceException_When_CreatingWithSameStreamIdAsAssignedToEntries_And_EntriesSequenceDoesNotStartWithZero(
            EventStreamEntries entries)
        {
            var streamId = entries[0].StreamId;
            Assert.Throws<InvalidEventStreamEntrySequenceException>(() => new EventStream(
                streamId,
                entries));
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingWithNonNullStreamIdAndEmptyEntriesCollection(
            EventStreamId streamId)
        {
            _ = new EventStream(
                streamId,
                EventStreamEntries.Empty);
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingWithSameStreamIdAsAssignedToEntries_And_EntriesSequenceStartsWithZero(
            EventStreamEntries entries)
        {
            var streamId = entries[0].StreamId;
            entries = new EventStreamEntries(entries
                .Select((entry, i) => new EventStreamEntry(
                    entry.StreamId,
                    entry.EntryId,
                    Convert.ToUInt32(i),
                    entry.EventDescriptor,
                    entry.EntryMetadata)));

            _ = new EventStream(
                streamId,
                entries);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnStreamIdProvidedDuringCreation_When_GettingStreamId(EventStreamEntries entries)
        {
            var streamId = entries[0].StreamId;
            entries = new EventStreamEntries(entries
                .Select((entry, i) => new EventStreamEntry(
                    entry.StreamId,
                    entry.EntryId,
                    Convert.ToUInt32(i),
                    entry.EventDescriptor,
                    entry.EntryMetadata)));

            var stream = new EventStream(streamId, entries);

            Assert.Equal(streamId, stream.StreamId);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnEntriesProvidedDuringCreation_When_GettingEntries(EventStreamEntries entries)
        {
            var streamId = entries[0].StreamId;
            entries = new EventStreamEntries(entries
                .Select((entry, i) => new EventStreamEntry(
                    entry.StreamId,
                    entry.EntryId,
                    Convert.ToUInt32(i),
                    entry.EventDescriptor,
                    entry.EntryMetadata)));
            
            var stream = new EventStream(streamId, entries);

            Assert.Equal(new EventStreamEntries(entries.AsEnumerable()), stream.Entries);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnEntriesMaximumSequenceProvidedDuringCreation_When_GettingCurrentSequence(EventStreamEntries entries)
        {
            var streamId = entries[0].StreamId;
            entries = new EventStreamEntries(entries
                .Select((entry, i) => new EventStreamEntry(
                    entry.StreamId,
                    entry.EntryId,
                    Convert.ToUInt32(i),
                    entry.EventDescriptor,
                    entry.EntryMetadata)));
            
            var stream = new EventStream(streamId, entries);

            Assert.Equal(new EventStreamEntries(entries.AsEnumerable()).MaximumSequence, stream.CurrentSequence);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnEmptyCollection_When_GettingEntriesToAppend(EventStreamEntries entries)
        {
            var streamId = entries[0].StreamId;
            entries = new EventStreamEntries(entries
                .Select((entry, i) => new EventStreamEntry(
                    entry.StreamId,
                    entry.EntryId,
                    Convert.ToUInt32(i),
                    entry.EventDescriptor,
                    entry.EntryMetadata)));
            
            var stream = new EventStream(streamId, entries);

            Assert.Empty(stream.EntriesToAppend);
        }

        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_AppendingEntries_And_ProvidedEntriesIsNull(
            EventStream stream)
        {
            Assert.Throws<ArgumentNullException>(() => stream.AppendEntries(null));
        }

        [Fact]
        public void Throw_ArgumentNullException_When_AppendingEntries_And_ProvidedEntriesIsNull_And_EventStreamIsEmpty()
        {
            var stream = EventStream.NewEventStream();
            Assert.Throws<ArgumentNullException>(() => stream.AppendEntries(null));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_InvalidEventStreamIdException_When_AppendingEntries_And_SingleEntryHasInvalidStreamId(
            EventStream stream,
            EventStreamEntry entryToAppend)
        {
            Assert.Throws<InvalidEventStreamIdException>(() => stream.AppendEntries(new[] {entryToAppend}));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_InvalidEventStreamIdException_When_AppendingEntries_And_SingleEntryHasInvalidStreamId_And_EventStreamIsEmpty(
            EventStreamEntry entryToAppend)
        {
            var stream = EventStream.NewEventStream();

            Assert.Throws<InvalidEventStreamIdException>(() => stream.AppendEntries(new[] {entryToAppend}));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_InvalidEventStreamIdException_When_AppendingEntries_And_FirstEntryHasInvalidStreamId(
            EventStream stream,
            IEnumerable<EventStreamEntry> entriesToAppend)
        {
            var validStreamId = stream.StreamId;
            entriesToAppend = entriesToAppend
                .Select((entry, i) => new EventStreamEntry(
                    i == 0 ? entry.StreamId : validStreamId,
                    entry.EntryId,
                    entry.EntrySequence,
                    entry.EventDescriptor,
                    entry.EntryMetadata));

            Assert.Throws<InvalidEventStreamIdException>(() => stream.AppendEntries(entriesToAppend));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_InvalidEventStreamIdException_When_AppendingEntries_And_FirstEntryHasInvalidStreamId_And_EventStreamIsEmpty(
            IEnumerable<EventStreamEntry> entriesToAppend)
        {
            var stream = EventStream.NewEventStream();
            var validStreamId = stream.StreamId;
            entriesToAppend = entriesToAppend
                .Select((entry, i) => new EventStreamEntry(
                    i == 0 ? entry.StreamId : validStreamId,
                    entry.EntryId,
                    entry.EntrySequence,
                    entry.EventDescriptor,
                    entry.EntryMetadata));

            Assert.Throws<InvalidEventStreamIdException>(() => stream.AppendEntries(entriesToAppend));
        }

        [Theory]
        [AutoMoqWithInlineData(0)]
        [AutoMoqWithInlineData(1)]
        [AutoMoqWithInlineData(2)]
        public void Throw_InvalidEventStreamIdException_When_AppendingEntries_And_AtLeastOneOfEntriesHasInvalidStreamId_And_AllEntriesHaveCorrectSequence(
            int invalidStreamIdIndex,
            EventStream stream,
            IEnumerable<EventStreamEntry> entriesToAppend)
        {
            var validStreamId = stream.StreamId;
            var nextSequence = stream.CurrentSequence + 1;
            entriesToAppend = entriesToAppend
                .Select((entry, i) => new EventStreamEntry(
                    i == invalidStreamIdIndex ? entry.StreamId : validStreamId,
                    entry.EntryId,
                    nextSequence++,
                    entry.EventDescriptor,
                    entry.EntryMetadata));

            Assert.Throws<InvalidEventStreamIdException>(() => stream.AppendEntries(entriesToAppend));
        }

        [Theory]
        [AutoMoqWithInlineData(0)]
        [AutoMoqWithInlineData(1)]
        [AutoMoqWithInlineData(2)]
        public void Throw_InvalidEventStreamIdException_When_AppendingEntries_And_AtLeastOneOfEntriesHasInvalidStreamId_And_AllEntriesHaveCorrectSequence_And_EventStreamIsEmpty(
            int invalidStreamIdIndex,
            IEnumerable<EventStreamEntry> entriesToAppend)
        {
            var stream = EventStream.NewEventStream();
            var validStreamId = stream.StreamId;
            uint nextSequence = 0;
            entriesToAppend = entriesToAppend
                .Select((entry, i) => new EventStreamEntry(
                    i == invalidStreamIdIndex ? entry.StreamId : validStreamId,
                    entry.EntryId,
                    nextSequence++,
                    entry.EventDescriptor,
                    entry.EntryMetadata));

            Assert.Throws<InvalidEventStreamIdException>(() => stream.AppendEntries(entriesToAppend));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_InvalidEventStreamEntrySequenceException_When_AppendingEntries_And_SingleEntryHasValidStreamIdButInvalidSequence(
            EventStream stream,
            EventStreamEntry entryToAppend)
        {
            var validStreamId = stream.StreamId;
            entryToAppend = new EventStreamEntry(
                validStreamId,
                entryToAppend.EntryId,
                entryToAppend.EntrySequence,
                entryToAppend.EventDescriptor,
                entryToAppend.EntryMetadata);
            
            Assert.NotEqual<uint>(stream.CurrentSequence + 1, entryToAppend.EntrySequence);
            Assert.Throws<InvalidEventStreamEntrySequenceException>(() => stream.AppendEntries(new[] {entryToAppend}));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_InvalidEventStreamEntrySequenceException_When_AppendingEntries_And_SingleEntryHasValidStreamIdButInvalidSequence_And_EventStreamIsEmpty(
            EventStreamEntry entryToAppend)
        {
            var stream = EventStream.NewEventStream();
            var validStreamId = stream.StreamId;
            entryToAppend = new EventStreamEntry(
                validStreamId,
                entryToAppend.EntryId,
                entryToAppend.EntrySequence,
                entryToAppend.EventDescriptor,
                entryToAppend.EntryMetadata);
            
            Assert.NotEqual<uint>(stream.CurrentSequence + 1, entryToAppend.EntrySequence);

            Assert.Throws<InvalidEventStreamEntrySequenceException>(() => stream.AppendEntries(new[] {entryToAppend}));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_InvalidEventStreamEntrySequenceException_When_AppendingEntries_And_FirstEntryHasValidStreamIdButInvalidSequence(
            EventStream stream,
            List<EventStreamEntry> entriesToAppend)
        {
            var validStreamId = stream.StreamId;
            entriesToAppend = entriesToAppend
                .Select((entry, i) => new EventStreamEntry(
                    i == 0 ? validStreamId : entry.StreamId,
                    entry.EntryId,
                    entry.EntrySequence,
                    entry.EventDescriptor,
                    entry.EntryMetadata))
                .ToList();
            
            Assert.NotEqual<uint>(stream.CurrentSequence + 1, entriesToAppend[0].EntrySequence);

            Assert.Throws<InvalidEventStreamEntrySequenceException>(() => stream.AppendEntries(entriesToAppend));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_InvalidEventStreamEntrySequenceException_When_AppendingEntries_And_FirstEntryHasValidStreamIdButInvalidSequence_And_EventStreamIsEmpty(
            List<EventStreamEntry> entriesToAppend)
        {
            var stream = EventStream.NewEventStream();
            var validStreamId = stream.StreamId;
            entriesToAppend = entriesToAppend
                .Select((entry, i) => new EventStreamEntry(
                    i == 0 ? validStreamId : entry.StreamId,
                    entry.EntryId,
                    entry.EntrySequence,
                    entry.EventDescriptor,
                    entry.EntryMetadata))
                .ToList();
            
            Assert.NotEqual<uint>(stream.CurrentSequence + 1, entriesToAppend[0].EntrySequence);

            Assert.Throws<InvalidEventStreamEntrySequenceException>(() => stream.AppendEntries(entriesToAppend));
        }

        [Theory]
        [AutoMoqWithInlineData(0)]
        [AutoMoqWithInlineData(1)]
        [AutoMoqWithInlineData(2)]
        public void Throw_InvalidEventStreamEntrySequenceException_When_AppendingEntries_And_AllEntriesHaveValidStreamId_And_AtLeastOneEntryHasInvalidSequence(
            int invalidSequenceIndex,
            EventStream stream,
            IEnumerable<EventStreamEntry> entriesToAppend)
        {
            var validStreamId = stream.StreamId;
            var nextSequence = stream.CurrentSequence + 1;
            var getNextSequenceFunc = new Func<int, EventStreamEntry, EventStreamEntrySequence>(
                (currentIndex, entry) =>
                {
                    var currentValidSequence = nextSequence++;
                    Assert.NotEqual<uint>(entry.EntrySequence, currentValidSequence);
                    return currentIndex == invalidSequenceIndex ? entry.EntrySequence : currentValidSequence;
                });
            
            entriesToAppend = entriesToAppend
                .Select((entry, i) => new EventStreamEntry(
                    validStreamId,
                    entry.EntryId,
                    getNextSequenceFunc(i, entry),
                    entry.EventDescriptor,
                    entry.EntryMetadata));

            Assert.Throws<InvalidEventStreamEntrySequenceException>(() => stream.AppendEntries(entriesToAppend));
        }

        [Theory]
        [AutoMoqWithInlineData(0)]
        [AutoMoqWithInlineData(1)]
        [AutoMoqWithInlineData(2)]
        public void Throw_InvalidEventStreamEntrySequenceException_When_AppendingEntries_And_AllEntriesHaveValidStreamId_And_AtLeastOneEntryHasInvalidSequence_And_EventStreamIsEmpty(
            int invalidSequenceIndex,
            IEnumerable<EventStreamEntry> entriesToAppend)
        {
            var stream = EventStream.NewEventStream();
            var validStreamId = stream.StreamId;
            var nextSequence = stream.CurrentSequence + 1;
            var getNextSequenceFunc = new Func<int, EventStreamEntry, EventStreamEntrySequence>(
                (currentIndex, entry) =>
                {
                    var currentValidSequence = nextSequence++;
                    Assert.NotEqual<uint>(entry.EntrySequence, currentValidSequence);
                    return currentIndex == invalidSequenceIndex ? entry.EntrySequence : currentValidSequence;
                });
            
            entriesToAppend = entriesToAppend
                .Select((entry, i) => new EventStreamEntry(
                    validStreamId,
                    entry.EntryId,
                    getNextSequenceFunc(i, entry),
                    entry.EventDescriptor,
                    entry.EntryMetadata));

            Assert.Throws<InvalidEventStreamEntrySequenceException>(() => stream.AppendEntries(entriesToAppend));
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_AppendingEntries_And_SingleEntryHasValidStreamIdAndValidSequence(
            EventStream stream,
            EventStreamEntry entryToAppend)
        {
            var validStreamId = stream.StreamId;
            var validSequence = stream.CurrentSequence + 1;
            entryToAppend = new EventStreamEntry(
                validStreamId,
                entryToAppend.EntryId,
                validSequence,
                entryToAppend.EventDescriptor,
                entryToAppend.EntryMetadata);

            stream.AppendEntries(new[] {entryToAppend});
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_AppendingEntries_And_SingleEntryHasValidStreamIdAndValidSequence_And_EventStreamIsEmpty(
            EventStreamEntry entryToAppend)
        {
            var stream = EventStream.NewEventStream();
            var validStreamId = stream.StreamId;
            uint validSequence = 0;
            entryToAppend = new EventStreamEntry(
                validStreamId,
                entryToAppend.EntryId,
                validSequence,
                entryToAppend.EventDescriptor,
                entryToAppend.EntryMetadata);

            stream.AppendEntries(new[] {entryToAppend});
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_AppendingEntries_And_AllEntriesHaveValidStreamIdAndValidSequence(
            EventStream stream,
            IEnumerable<EventStreamEntry> entriesToAppend)
        {
            var validStreamId = stream.StreamId;
            var validSequence = stream.CurrentSequence + 1;
            entriesToAppend = entriesToAppend
                .Select(entry => new EventStreamEntry(
                    validStreamId,
                    entry.EntryId,
                    validSequence++,
                    entry.EventDescriptor,
                    entry.EntryMetadata));

            stream.AppendEntries(entriesToAppend);
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_AppendingEntries_And_AllEntriesHaveValidStreamIdAndValidSequence_And_EventStreamIsEmpty(
            IEnumerable<EventStreamEntry> entriesToAppend)
        {
            var stream = EventStream.NewEventStream();
            var validStreamId = stream.StreamId;
            uint validSequence = 0;
            entriesToAppend = entriesToAppend
                .Select(entry => new EventStreamEntry(
                    validStreamId,
                    entry.EntryId,
                    validSequence++,
                    entry.EventDescriptor,
                    entry.EntryMetadata));

            stream.AppendEntries(entriesToAppend);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnOriginalStreamId_When_GettingStreamIdAfterAppendingSingleEntry(
            EventStream stream,
            EventStreamEntry entryToAppend)
        {
            var originalStreamId = stream.StreamId;
            var validStreamId = stream.StreamId;
            var validSequence = stream.CurrentSequence + 1;
            entryToAppend = new EventStreamEntry(
                validStreamId,
                entryToAppend.EntryId,
                validSequence,
                entryToAppend.EventDescriptor,
                entryToAppend.EntryMetadata);

            stream.AppendEntries(new[] {entryToAppend});
            
            Assert.Equal(originalStreamId, stream.StreamId);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnOriginalStreamId_When_GettingStreamIdAfterAppendingSingleEntry_And_EventStreamWasEmpty(
            EventStreamEntry entryToAppend)
        {
            var stream = EventStream.NewEventStream();
            var originalStreamId = stream.StreamId;
            var validStreamId = stream.StreamId;
            uint validSequence = 0;
            entryToAppend = new EventStreamEntry(
                validStreamId,
                entryToAppend.EntryId,
                validSequence,
                entryToAppend.EventDescriptor,
                entryToAppend.EntryMetadata);

            stream.AppendEntries(new[] {entryToAppend});
            
            Assert.Equal(originalStreamId, stream.StreamId);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnOriginalStreamId_When_GettingStreamIdAfterAppendingEntries(
            EventStream stream,
            IEnumerable<EventStreamEntry> entriesToAppend)
        {
            var originalStreamId = stream.StreamId;
            var validStreamId = stream.StreamId;
            var validSequence = stream.CurrentSequence + 1;
            entriesToAppend = entriesToAppend
                .Select(entry => new EventStreamEntry(
                    validStreamId,
                    entry.EntryId,
                    validSequence++,
                    entry.EventDescriptor,
                    entry.EntryMetadata));

            stream.AppendEntries(entriesToAppend);
            
            Assert.Equal(originalStreamId, stream.StreamId);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnOriginalStreamId_When_GettingStreamIdAfterAppendingEntries_And_EventStreamWasEmpty(
            IEnumerable<EventStreamEntry> entriesToAppend)
        {
            var stream = EventStream.NewEventStream();
            var originalStreamId = stream.StreamId;
            var validStreamId = stream.StreamId;
            uint validSequence = 0;
            entriesToAppend = entriesToAppend
                .Select(entry => new EventStreamEntry(
                    validStreamId,
                    entry.EntryId,
                    validSequence++,
                    entry.EventDescriptor,
                    entry.EntryMetadata));

            stream.AppendEntries(entriesToAppend);
            
            Assert.Equal(originalStreamId, stream.StreamId);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnOriginalEvents_When_GettingEntriesAfterAppendingSingleEntry(
            EventStream stream,
            EventStreamEntry entryToAppend)
        {
            var originalEntries = stream.Entries;
            var validStreamId = stream.StreamId;
            var validSequence = stream.CurrentSequence + 1;
            entryToAppend = new EventStreamEntry(
                validStreamId,
                entryToAppend.EntryId,
                validSequence,
                entryToAppend.EventDescriptor,
                entryToAppend.EntryMetadata);

            stream.AppendEntries(new[] {entryToAppend});
            
            Assert.Equal(originalEntries, stream.Entries);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnOriginalEvents_When_GettingEntriesAfterAppendingSingleEntry_And_EventStreamWasEmpty(
            EventStreamEntry entryToAppend)
        {
            var stream = EventStream.NewEventStream();
            var originalEntries = stream.Entries;
            var validStreamId = stream.StreamId;
            uint validSequence = 0;
            entryToAppend = new EventStreamEntry(
                validStreamId,
                entryToAppend.EntryId,
                validSequence,
                entryToAppend.EventDescriptor,
                entryToAppend.EntryMetadata);

            stream.AppendEntries(new[] {entryToAppend});
            
            Assert.Equal(originalEntries, stream.Entries);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnOriginalEntries_When_GettingEntriesAfterAppendingEntries(
            EventStream stream,
            IEnumerable<EventStreamEntry> entriesToAppend)
        {
            var originalEntries= stream.Entries;
            var validStreamId = stream.StreamId;
            var validSequence = stream.CurrentSequence + 1;
            entriesToAppend = entriesToAppend
                .Select(entry => new EventStreamEntry(
                    validStreamId,
                    entry.EntryId,
                    validSequence++,
                    entry.EventDescriptor,
                    entry.EntryMetadata));

            stream.AppendEntries(entriesToAppend);
            
            Assert.Equal(originalEntries, stream.Entries);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnOriginalEntries_When_GettingEntriesAfterAppendingEntries_And_EventStreamWasEmpty(
            IEnumerable<EventStreamEntry> entriesToAppend)
        {
            var stream = EventStream.NewEventStream();
            var originalEntries = stream.Entries;
            var validStreamId = stream.StreamId;
            uint validSequence = 0;
            entriesToAppend = entriesToAppend
                .Select(entry => new EventStreamEntry(
                    validStreamId,
                    entry.EntryId,
                    validSequence++,
                    entry.EventDescriptor,
                    entry.EntryMetadata));

            stream.AppendEntries(entriesToAppend);
            
            Assert.Equal(originalEntries, stream.Entries);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnAppendedEntrySequenceAsCurrentSequence_When_GettingCurrentSequenceAfterAppendingSingleEntry(
            EventStream stream,
            EventStreamEntry entryToAppend)
        {
            var validStreamId = stream.StreamId;
            var validSequence = stream.CurrentSequence + 1;
            entryToAppend = new EventStreamEntry(
                validStreamId,
                entryToAppend.EntryId,
                validSequence,
                entryToAppend.EventDescriptor,
                entryToAppend.EntryMetadata);

            stream.AppendEntries(new[] {entryToAppend});
            
            Assert.Equal<EventStreamEntrySequence>(validSequence, stream.CurrentSequence);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnAppendedEntrySequenceAsCurrentSequence_When_GettingCurrentSequenceAfterAppendingSingleEntry_And_EventStreamWasEmpty(
            EventStreamEntry entryToAppend)
        {
            var stream = EventStream.NewEventStream();
            var validStreamId = stream.StreamId;
            uint validSequence = 0;
            entryToAppend = new EventStreamEntry(
                validStreamId,
                entryToAppend.EntryId,
                validSequence,
                entryToAppend.EventDescriptor,
                entryToAppend.EntryMetadata);

            stream.AppendEntries(new[] {entryToAppend});
            
            Assert.Equal<EventStreamEntrySequence>(validSequence, stream.CurrentSequence);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnLastAppendedEntrySequenceAsCurrentSequence_When_GettingCurrentSequenceAfterAppendingEntries(
            EventStream stream,
            List<EventStreamEntry> entriesToAppend)
        {
            var validStreamId = stream.StreamId;
            var validSequence = stream.CurrentSequence + 1;
            entriesToAppend = entriesToAppend
                .Select(entry => new EventStreamEntry(
                    validStreamId,
                    entry.EntryId,
                    validSequence++,
                    entry.EventDescriptor,
                    entry.EntryMetadata))
                .ToList();

            stream.AppendEntries(entriesToAppend);
            
            Assert.Equal(entriesToAppend.Last().EntrySequence, stream.CurrentSequence);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnLastAppendedEntrySequenceAsCurrentSequence_When_GettingCurrentSequenceAfterAppendingEntries_And_EventStreamWasEmpty(
            List<EventStreamEntry> entriesToAppend)
        {
            var stream = EventStream.NewEventStream();
            var validStreamId = stream.StreamId;
            uint validSequence = 0;
            entriesToAppend = entriesToAppend
                .Select(entry => new EventStreamEntry(
                    validStreamId,
                    entry.EntryId,
                    validSequence++,
                    entry.EventDescriptor,
                    entry.EntryMetadata))
                .ToList();

            stream.AppendEntries(entriesToAppend);
            
            Assert.Equal(entriesToAppend.Last().EntrySequence, stream.CurrentSequence);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnAppendedEntryAsSingleEntryToAppend_When_GettingEntriesToAppendAfterAppendingSingleEntry(
            EventStream stream,
            EventStreamEntry entryToAppend)
        {
            var validStreamId = stream.StreamId;
            var validSequence = stream.CurrentSequence + 1;
            entryToAppend = new EventStreamEntry(
                validStreamId,
                entryToAppend.EntryId,
                validSequence,
                entryToAppend.EventDescriptor,
                entryToAppend.EntryMetadata);

            stream.AppendEntries(new[] {entryToAppend});

            var singleEntryToAppend = Assert.Single(stream.EntriesToAppend);
            Assert.Equal(entryToAppend, singleEntryToAppend);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnAppendedEntryAsSingleEntryToAppend_When_GettingEntriesToAppendAfterAppendingSingleEntry_And_EventStreamWasEmpty(
            EventStreamEntry entryToAppend)
        {
            var stream = EventStream.NewEventStream();
            var validStreamId = stream.StreamId;
            uint validSequence = 0;
            entryToAppend = new EventStreamEntry(
                validStreamId,
                entryToAppend.EntryId,
                validSequence,
                entryToAppend.EventDescriptor,
                entryToAppend.EntryMetadata);

            stream.AppendEntries(new[] {entryToAppend});

            var singleEntryToAppend = Assert.Single(stream.EntriesToAppend);
            Assert.Equal(entryToAppend, singleEntryToAppend);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnAppendedEntriesAsEntriesToAppend_When_GettingEntriesToAppendAfterAppendingEntries(
            EventStream stream,
            List<EventStreamEntry> entriesToAppend)
        {
            var validStreamId = stream.StreamId;
            var validSequence = stream.CurrentSequence + 1;
            entriesToAppend = entriesToAppend
                .Select(entry => new EventStreamEntry(
                    validStreamId,
                    entry.EntryId,
                    validSequence++,
                    entry.EventDescriptor,
                    entry.EntryMetadata))
                .ToList();

            stream.AppendEntries(entriesToAppend);
            
            Assert.True(stream.EntriesToAppend.SequenceEqual(entriesToAppend));
        }

        [Theory]
        [AutoMoqData]
        public void ReturnAppendedEntriesAsEntriesToAppend_When_GettingEntriesToAppendAfterAppendingEntries_And_EventStreamWasEmpty(
            List<EventStreamEntry> entriesToAppend)
        {
            var stream = EventStream.NewEventStream();
            var validStreamId = stream.StreamId;
            uint validSequence = 0;
            entriesToAppend = entriesToAppend
                .Select(entry => new EventStreamEntry(
                    validStreamId,
                    entry.EntryId,
                    validSequence++,
                    entry.EventDescriptor,
                    entry.EntryMetadata))
                .ToList();

            stream.AppendEntries(entriesToAppend);
            
            Assert.True(stream.EntriesToAppend.SequenceEqual(entriesToAppend));
        }

        [Theory]
        [AutoMoqData]
        public void ReturnTrue_When_ComparingDifferentObjectsWithSameStreamIdAndEmptyCollectionOfEntries(
            EventStreamId streamId)
        {
            var stream1 = new EventStream(
                streamId,
                EventStreamEntries.Empty);

            var stream2 = new EventStream(
                streamId,
                EventStreamEntries.Empty);
            
            Assert.Equal(stream1, stream2);
            Assert.True(stream1 == stream2);
            Assert.False(stream1 != stream2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnTrue_When_ComparingDifferentObjectsWithSameStreamIdAndEntries(
            EventStream eventStream)
        {
            var stream1 = new EventStream(
                eventStream.StreamId,
                eventStream.Entries);

            var stream2 = new EventStream(
                eventStream.StreamId,
                eventStream.Entries);
            
            Assert.Equal(stream1, stream2);
            Assert.True(stream1 == stream2);
            Assert.False(stream1 != stream2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnTrue_When_ComparingDifferentObjectsWithSameStreamIdAndEntries_And_SameAppendedEntries(
            EventStream eventStream,
            IEnumerable<EventStreamEntry> entriesToAppend)
        {
            var validStreamId = eventStream.StreamId;
            var validSequence = eventStream.CurrentSequence + 1;
            entriesToAppend = entriesToAppend
                .Select(entry => new EventStreamEntry(
                    validStreamId,
                    entry.EntryId,
                    validSequence++,
                    entry.EventDescriptor,
                    entry.EntryMetadata))
                .ToList();
            
            var stream1 = new EventStream(
                eventStream.StreamId,
                eventStream.Entries);

            stream1.AppendEntries(entriesToAppend);

            var stream2 = new EventStream(
                eventStream.StreamId,
                eventStream.Entries);
            
            stream2.AppendEntries(entriesToAppend);
            
            Assert.Equal(stream1, stream2);
            Assert.True(stream1 == stream2);
            Assert.False(stream1 != stream2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentStreamIdAndEmptyCollectionOfEntries(
            EventStreamId streamId1,
            EventStreamId streamId2)
        {
            var stream1 = new EventStream(
                streamId1,
                EventStreamEntries.Empty);

            var stream2 = new EventStream(
                streamId2,
                EventStreamEntries.Empty);
            
            Assert.NotEqual(stream1.GetHashCode(), stream2.GetHashCode());
            Assert.NotEqual(stream1, stream2);
            Assert.True(stream1 != stream2);
            Assert.False(stream1 == stream2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentStreamIdAndEntries(
            EventStream eventStream1,
            EventStream eventStream2)
        {
            var stream1 = new EventStream(
                eventStream1.StreamId,
                eventStream1.Entries);

            var stream2 = new EventStream(
                eventStream2.StreamId,
                eventStream2.Entries);
            
            Assert.NotEqual(stream1, stream2);
            Assert.True(stream1 != stream2);
            Assert.False(stream1 == stream2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithSameStreamIdAndEntries_And_OneHasAppendedEntriesWhileTheOtherDoesNot(
            EventStream eventStream,
            IEnumerable<EventStreamEntry> entriesToAppend)
        {
            var validStreamId = eventStream.StreamId;
            var validSequence = eventStream.CurrentSequence + 1;
            entriesToAppend = entriesToAppend
                .Select(entry => new EventStreamEntry(
                    validStreamId,
                    entry.EntryId,
                    validSequence++,
                    entry.EventDescriptor,
                    entry.EntryMetadata))
                .ToList();
            
            var stream1 = new EventStream(
                eventStream.StreamId,
                eventStream.Entries);

            stream1.AppendEntries(entriesToAppend);

            var stream2 = new EventStream(
                eventStream.StreamId,
                eventStream.Entries);
            
            Assert.NotEqual(stream1, stream2);
            Assert.True(stream1 != stream2);
            Assert.False(stream1 == stream2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithSameStreamIdAndEntries_And_BothHaveDifferentNumberAppendedEntries(
            EventStream eventStream,
            IEnumerable<EventStreamEntry> entriesToAppend)
        {
            var validStreamId = eventStream.StreamId;
            var validSequence = eventStream.CurrentSequence + 1;
            entriesToAppend = entriesToAppend
                .Select(entry => new EventStreamEntry(
                    validStreamId,
                    entry.EntryId,
                    validSequence++,
                    entry.EventDescriptor,
                    entry.EntryMetadata))
                .ToList();
            
            var stream1 = new EventStream(
                eventStream.StreamId,
                eventStream.Entries);

            stream1.AppendEntries(entriesToAppend);

            var stream2 = new EventStream(
                eventStream.StreamId,
                eventStream.Entries);

            stream2.AppendEntries(entriesToAppend.Take(new Random().Next(0, entriesToAppend.Count())));
            
            Assert.NotEqual(stream1, stream2);
            Assert.True(stream1 != stream2);
            Assert.False(stream1 == stream2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithSameStreamIdAndEntries_And_BothHaveDifferentAppendedEntries(
            EventStream eventStream,
            IEnumerable<EventStreamEntry> entriesToAppend1,
            IEnumerable<EventStreamEntry> entriesToAppend2)
        {
            var validStreamId = eventStream.StreamId;
            var validSequence = eventStream.CurrentSequence + 1;
            entriesToAppend1 = entriesToAppend1
                .Select(entry => new EventStreamEntry(
                    validStreamId,
                    entry.EntryId,
                    validSequence++,
                    entry.EventDescriptor,
                    entry.EntryMetadata))
                .ToList();
            
            entriesToAppend2 = entriesToAppend2
                .Select(entry => new EventStreamEntry(
                    validStreamId,
                    entry.EntryId,
                    validSequence++,
                    entry.EventDescriptor,
                    entry.EntryMetadata))
                .ToList();
            
            var stream1 = new EventStream(
                eventStream.StreamId,
                eventStream.Entries);

            stream1.AppendEntries(entriesToAppend1);

            var stream2 = new EventStream(
                eventStream.StreamId,
                eventStream.Entries);

            stream1.AppendEntries(entriesToAppend2);
            
            Assert.NotEqual(stream1, stream2);
            Assert.True(stream1 != stream2);
            Assert.False(stream1 == stream2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnExpectedValue_When_CallingToString(EventStream eventStream)
        {
            var expectedValue =
                $"Event Stream ID: {eventStream.StreamId}, {eventStream.Entries}, Current Sequence: {eventStream.CurrentSequence}, {EntriesToAppendString()}";

            string EntriesToAppendString()
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.Append("Entries to append: ");
                foreach (var eventStreamEntry in eventStream.EntriesToAppend)
                {
                    stringBuilder.Append($"\n\t{eventStreamEntry}");
                }

                return stringBuilder.ToString();
            }
            
            Assert.Equal(expectedValue, eventStream.ToString());
        }

        [Theory]
        [AutoMoqData]
        public void ReturnExpectedValue_When_CallingToStringWithAppendedEntries(
            EventStream eventStream,
            IEnumerable<EventStreamEntry> entriesToAppend)
        {
            var validStreamId = eventStream.StreamId;
            var validSequence = eventStream.CurrentSequence + 1;
            entriesToAppend = entriesToAppend
                .Select(entry => new EventStreamEntry(
                    validStreamId,
                    entry.EntryId,
                    validSequence++,
                    entry.EventDescriptor,
                    entry.EntryMetadata))
                .ToList();

            eventStream.AppendEntries(entriesToAppend);
            
            var expectedValue =
                $"Event Stream ID: {eventStream.StreamId}, {eventStream.Entries}, Current Sequence: {eventStream.CurrentSequence}, {EntriesToAppendString()}";

            string EntriesToAppendString()
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.Append("Entries to append: ");
                foreach (var eventStreamEntry in eventStream.EntriesToAppend)
                {
                    stringBuilder.Append($"\n\t{eventStreamEntry}");
                }

                return stringBuilder.ToString();
            }

            Assert.Equal(expectedValue, eventStream.ToString());
        }
    }
}
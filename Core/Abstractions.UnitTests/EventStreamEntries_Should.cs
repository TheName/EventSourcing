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
    public class EventStreamEntries_Should
    {
        [Fact]
        public void ReturnEmptyEventStreamEntries_When_CallingEmpty()
        {
            var events = EventStreamEntries.Empty;
            
            Assert.Equal<uint>(0, events.MinimumSequence);
            Assert.Equal<uint>(0, events.MaximumSequence);
            Assert.Empty(events);
        }
        
        [Fact]
        public void Throw_ArgumentNullException_When_CreatingWithNullValue()
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEntries(null));
        }

        [Theory]
        [AutoMoqWithInlineData(0)]
        [AutoMoqWithInlineData(1)]
        [AutoMoqWithInlineData(2)]
        public void Throw_InvalidEventStreamIdException_When_Creating_And_AtLeastOneOfEntriesHasInvalidStreamId_And_AllEntriesHaveCorrectSequence(
            int invalidStreamIdIndex,
            EventStreamId validStreamId,
            EventStreamEntrySequence startingSequence,
            IEnumerable<EventStreamEntry> entries)
        {
            entries = entries
                .Select((entry, i) => new EventStreamEntry(
                    i == invalidStreamIdIndex ? entry.StreamId : validStreamId,
                    entry.EntryId,
                    startingSequence++,
                    entry.EventDescriptor,
                    entry.CausationId,
                    entry.CreationTime,
                    entry.CorrelationId));

            Assert.Throws<InvalidEventStreamIdException>(() => new EventStreamEntries(entries));
        }

        [Theory]
        [AutoMoqWithInlineData(0)]
        [AutoMoqWithInlineData(1)]
        [AutoMoqWithInlineData(2)]
        public void Throw_InvalidEventStreamEntrySequenceException_When_Creating_And_AllEntriesHaveValidStreamId_And_AtLeastOneOfEntriesHasInvalidSequence(
            int invalidSequenceIndex,
            EventStreamId validStreamId,
            EventStreamEntrySequence startingSequence,
            IEnumerable<EventStreamEntry> entries)
        {
            var nextSequence = startingSequence;
            var getNextSequenceFunc = new Func<int, EventStreamEntry, EventStreamEntrySequence>(
                (currentIndex, _) =>
                {
                    var currentValidSequence = nextSequence++;
                    if (currentIndex != invalidSequenceIndex)
                    {
                        return currentValidSequence;
                    }

                    var random = new Random();
                    var invalidSequence = random.Next(0, int.MaxValue);
                    while (invalidSequence == currentValidSequence)
                    {
                        invalidSequence = random.Next(0, int.MaxValue);
                    }

                    return Convert.ToUInt32(invalidSequence);
                });
            
            entries = entries
                .Select((entry, i) => new EventStreamEntry(
                    validStreamId,
                    entry.EntryId,
                    getNextSequenceFunc(i, entry),
                    entry.EventDescriptor,
                    entry.CausationId,
                    entry.CreationTime,
                    entry.CorrelationId));

            Assert.Throws<InvalidEventStreamEntrySequenceException>(() => new EventStreamEntries(entries));
        }

        [Fact]
        public void NotThrow_When_CreatingWithEmptyCollection()
        {
            _ = new EventStreamEntries(Array.Empty<EventStreamEntry>());
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingWithSingleEventStreamEntry(EventStreamEntry entry)
        {
            _ = new EventStreamEntries(new[] {entry});
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingWithEventsWithSameStreamIdAndCorrectlyIncreasingSequences(
            EventStreamId validStreamId,
            EventStreamEntrySequence startingSequence,
            IEnumerable<EventStreamEntry> entries)
        {
            entries = entries
                .Select(entry => new EventStreamEntry(
                    validStreamId,
                    entry.EntryId,
                    startingSequence++,
                    entry.EventDescriptor,
                    entry.CausationId,
                    entry.CreationTime,
                    entry.CorrelationId));
            
            _ = new EventStreamEntries(entries);
        }

        [Theory]
        [AutoMoqData]
        public void ContainSingleEntry_When_CreatedWithSingleEntry(EventStreamEntry entry)
        {
            var entries = new EventStreamEntries(new[] {entry});

            var singleEntry = Assert.Single(entries);
            Assert.NotNull(singleEntry);
            Assert.Equal(entry, singleEntry);
        }

        [Theory]
        [AutoMoqData]
        public void ContainAllEntriesInSameSequence_When_CreatedWithMultipleEntries(
            EventStreamId validStreamId,
            EventStreamEntrySequence startingSequence,
            List<EventStreamEntry> entriesCollection)
        {
            entriesCollection = entriesCollection
                .Select(entry => new EventStreamEntry(
                    validStreamId,
                    entry.EntryId,
                    startingSequence++,
                    entry.EventDescriptor,
                    entry.CausationId,
                    entry.CreationTime,
                    entry.CorrelationId))
                .ToList();
            
            var entries = new EventStreamEntries(entriesCollection);

            Assert.Equal(entriesCollection.Count, entries.Count);
            Assert.True(entriesCollection.SequenceEqual(entries));
        }

        [Theory]
        [AutoMoqData]
        public void ReturnEntrySequence_When_CallingMinimumSequence_And_CreatedWithSingleEntry(EventStreamEntry entry)
        {
            var entries = new EventStreamEntries(new[] {entry});
            
            Assert.Equal(entry.EntrySequence, entries.MinimumSequence);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFirstEntrySequence_When_CallingMinimumSequence_And_CreatedWithMultipleEntries(
            EventStreamId validStreamId,
            EventStreamEntrySequence startingSequence,
            List<EventStreamEntry> entriesCollection)
        {
            entriesCollection = entriesCollection
                .Select(entry => new EventStreamEntry(
                    validStreamId,
                    entry.EntryId,
                    startingSequence++,
                    entry.EventDescriptor,
                    entry.CausationId,
                    entry.CreationTime,
                    entry.CorrelationId))
                .ToList();
            
            var events = new EventStreamEntries(entriesCollection);
            
            Assert.Equal(entriesCollection.First().EntrySequence, events.MinimumSequence);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnEntrySequence_When_CallingMaximumSequence_And_CreatedWithSingleEntry(EventStreamEntry entry)
        {
            var entries = new EventStreamEntries(new[] {entry});
            
            Assert.Equal(entry.EntrySequence, entries.MaximumSequence);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnLastEntrySequence_When_CallingMaximumSequence_And_CreatedWithMultipleEntries(
            EventStreamId validStreamId,
            EventStreamEntrySequence startingSequence,
            List<EventStreamEntry> entriesCollection)
        {
            entriesCollection = entriesCollection
                .Select(entry => new EventStreamEntry(
                    validStreamId,
                    entry.EntryId,
                    startingSequence++,
                    entry.EventDescriptor,
                    entry.CausationId,
                    entry.CreationTime,
                    entry.CorrelationId))
                .ToList();
            
            var entries = new EventStreamEntries(entriesCollection);
            
            Assert.Equal(entriesCollection.Last().EntrySequence, entries.MaximumSequence);
        }

        [Fact]
        public void ReturnTrue_When_ComparingDifferentObjectsEmptyCollectionOfEntries()
        {
            var entries1 = new EventStreamEntries(new List<EventStreamEntry>());
            var entries2 = EventStreamEntries.Empty;
            
            Assert.Equal(entries1, entries2);
            Assert.True(entries1 == entries2);
            Assert.False(entries1 != entries2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnTrue_When_ComparingDifferentObjectsWithSameEntries(
            EventStreamId validStreamId,
            EventStreamEntrySequence startingSequence,
            List<EventStreamEntry> entriesCollection)
        {
            entriesCollection = entriesCollection
                .Select(entry => new EventStreamEntry(
                    validStreamId,
                    entry.EntryId,
                    startingSequence++,
                    entry.EventDescriptor,
                    entry.CausationId,
                    entry.CreationTime,
                    entry.CorrelationId))
                .ToList();
            
            var entries1 = new EventStreamEntries(entriesCollection.Select(entry => entry));
            var entries2 = new EventStreamEntries(entriesCollection.Select(entry => entry));
            
            Assert.Equal(entries1, entries2);
            Assert.True(entries1 == entries2);
            Assert.False(entries1 != entries2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentEntries(
            EventStreamId validStreamId,
            EventStreamEntrySequence startingSequence,
            List<EventStreamEntry> entriesCollection1,
            List<EventStreamEntry> entriesCollection2)
        {
            entriesCollection1 = entriesCollection1
                .Select(entry => new EventStreamEntry(
                    validStreamId,
                    entry.EntryId,
                    startingSequence++,
                    entry.EventDescriptor,
                    entry.CausationId,
                    entry.CreationTime,
                    entry.CorrelationId))
                .ToList();
            
            entriesCollection2 = entriesCollection2
                .Select(entry => new EventStreamEntry(
                    validStreamId,
                    entry.EntryId,
                    startingSequence++,
                    entry.EventDescriptor,
                    entry.CausationId,
                    entry.CreationTime,
                    entry.CorrelationId))
                .ToList();
            
            var entries1 = new EventStreamEntries(entriesCollection1.Select(entry => entry));
            var entries2 = new EventStreamEntries(entriesCollection2.Select(entry => entry));
            
            Assert.NotEqual(entries1, entries2);
            Assert.True(entries1 != entries2);
            Assert.False(entries1 == entries2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnExpectedValue_When_CallingToString(EventStreamEntries entries)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("Entries: ");
            foreach (var eventStreamEntry in entries)
            {
                stringBuilder.Append($"\n\t{eventStreamEntry}");
            }

            var expectedValue = stringBuilder.ToString();
            
            Assert.Equal(expectedValue, entries.ToString());
        }
    }
}
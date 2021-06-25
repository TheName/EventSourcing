using System;
using System.Collections.Generic;
using System.Text;
using EventSourcing.Abstractions.Persistence.DataTransferObjects;
using EventSourcing.Abstractions.ValueObjects;
using TestHelpers.Attributes;
using Xunit;

namespace Abstractions.UnitTests.Persistence.DataTransferObjects
{
    public class EventStreamEntries_Should
    {
        [Fact]
        public void Throw_ArgumentNullException_When_CreatingWithNullList()
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEntries(null));
        }
        
        [Fact]
        public void NotThrow_When_CreatingWithEmptyList()
        {
            _ = new EventStreamEntries(new List<EventStreamEntry>());
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingWithNonEmptyList(
            EventStreamId streamId,
            EventStreamEntrySequence sequence,
            List<EventStreamEntry> entries)
        {
            // properly setup sequences and stream ids.
            for (var i = 0; i < entries.Count; i++)
            {
                entries[i] = new EventStreamEntry(
                    streamId,
                    sequence++,
                    entries[i].EntryId,
                    entries[i].Content,
                    entries[i].Metadata);
            }
            
            _ = new EventStreamEntries(entries);
        }

        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentException_When_CreatingWithEntries_And_SequenceIsRandom(
            EventStreamId streamId,
            List<EventStreamEntry> entries)
        {
            // properly setup stream ids.
            for (var i = 0; i < entries.Count; i++)
            {
                entries[i] = new EventStreamEntry(
                    streamId,
                    entries[i].Sequence,
                    entries[i].EntryId,
                    entries[i].Content,
                    entries[i].Metadata);
            }

            Assert.Throws<ArgumentException>(() => new EventStreamEntries(entries));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentException_When_CreatingWithEntries_And_SequenceIsIncreasingByMoreThanOne(
            EventStreamId streamId,
            EventStreamEntrySequence sequence,
            List<EventStreamEntry> entries)
        {
            // properly setup stream ids and arrange sequences.
            for (var i = 0; i < entries.Count; i++)
            {
                entries[i] = new EventStreamEntry(
                    streamId,
                    sequence,
                    entries[i].EntryId,
                    entries[i].Content,
                    entries[i].Metadata);

                sequence += 2;
            }

            Assert.Throws<ArgumentException>(() => new EventStreamEntries(entries));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentException_When_CreatingWithEntries_And_SequenceIsSame(
            EventStreamId streamId,
            EventStreamEntrySequence sequence,
            List<EventStreamEntry> entries)
        {
            // properly setup stream ids.
            for (var i = 0; i < entries.Count; i++)
            {
                entries[i] = new EventStreamEntry(
                    streamId,
                    sequence,
                    entries[i].EntryId,
                    entries[i].Content,
                    entries[i].Metadata);
            }

            Assert.Throws<ArgumentException>(() => new EventStreamEntries(entries));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentException_When_CreatingWithEntries_And_SequenceIsDecreasing(
            EventStreamId streamId,
            EventStreamEntrySequence sequence,
            List<EventStreamEntry> entries)
        {
            // properly setup stream ids and arrange sequences.
            for (var i = 0; i < entries.Count; i++)
            {
                entries[i] = new EventStreamEntry(
                    streamId,
                    sequence--,
                    entries[i].EntryId,
                    entries[i].Content,
                    entries[i].Metadata);
            }

            Assert.Throws<ArgumentException>(() => new EventStreamEntries(entries));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentException_When_CreatingWithEntries_And_SequenceIsIncreasingByOne_And_StreamIdIsRandom(
            EventStreamEntrySequence sequence,
            List<EventStreamEntry> entries)
        {
            // properly setup sequences.
            for (var i = 0; i < entries.Count; i++)
            {
                entries[i] = new EventStreamEntry(
                    entries[i].StreamId,
                    sequence++,
                    entries[i].EntryId,
                    entries[i].Content,
                    entries[i].Metadata);
            }

            Assert.Throws<ArgumentException>(() => new EventStreamEntries(entries));
        }

        [Theory]
        [AutoMoqWithInlineData(0)]
        [AutoMoqWithInlineData(1)]
        [AutoMoqWithInlineData(2)]
        public void Throw_ArgumentException_When_CreatingWithEntries_And_SequenceIsIncreasingByOne_And_AtLeastOneStreamIdIsDifferentFromOthers(
            int differentStreamIdIndex,
            EventStreamId streamId,
            EventStreamEntrySequence sequence,
            List<EventStreamEntry> entries)
        {
            // properly setup sequences.
            for (var i = 0; i < entries.Count; i++)
            {
                entries[i] = new EventStreamEntry(
                    i == differentStreamIdIndex ? entries[i].StreamId : streamId,
                    sequence++,
                    entries[i].EntryId,
                    entries[i].Content,
                    entries[i].Metadata);
            }

            Assert.Throws<ArgumentException>(() => new EventStreamEntries(entries));
        }

        [Theory]
        [AutoMoqData]
        public void ReturnTrue_When_ComparingDifferentObjectsWithSameValue(
            EventStreamId streamId,
            EventStreamEntrySequence sequence,
            List<EventStreamEntry> entries)
        {
            // properly setup sequences and stream ids.
            for (var i = 0; i < entries.Count; i++)
            {
                entries[i] = new EventStreamEntry(
                    streamId,
                    sequence++,
                    entries[i].EntryId,
                    entries[i].Content,
                    entries[i].Metadata);
            }
            
            var entries1 = new EventStreamEntries(entries);
            var entries2 = new EventStreamEntries(entries);
            
            Assert.Equal(entries1, entries2);
            Assert.True(entries1 == entries2);
            Assert.False(entries1 != entries2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentValue(
            EventStreamId streamId1,
            EventStreamEntrySequence sequence1,
            List<EventStreamEntry> entries1,
            EventStreamId streamId2,
            EventStreamEntrySequence sequence2,
            List<EventStreamEntry> entries2)
        {
            // properly setup sequences and stream ids.
            for (var i = 0; i < entries1.Count; i++)
            {
                entries1[i] = new EventStreamEntry(
                    streamId1,
                    sequence1++,
                    entries1[i].EntryId,
                    entries1[i].Content,
                    entries1[i].Metadata);
            }
            
            for (var i = 0; i < entries2.Count; i++)
            {
                entries2[i] = new EventStreamEntry(
                    streamId2,
                    sequence2++,
                    entries2[i].EntryId,
                    entries2[i].Content,
                    entries2[i].Metadata);
            }

            var entries1ValueObject = new EventStreamEntries(entries1);
            var entries2ValueObject = new EventStreamEntries(entries2);
            
            Assert.NotEqual(entries1ValueObject, entries2ValueObject);
            Assert.False(entries1ValueObject == entries2ValueObject);
            Assert.True(entries1ValueObject != entries2ValueObject);
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
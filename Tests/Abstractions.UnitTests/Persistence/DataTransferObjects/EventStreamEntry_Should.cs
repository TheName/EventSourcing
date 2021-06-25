using System;
using EventSourcing.Abstractions.Persistence.DataTransferObjects;
using EventSourcing.Abstractions.ValueObjects;
using TestHelpers.Attributes;
using Xunit;

namespace Abstractions.UnitTests.Persistence.DataTransferObjects
{
    public class EventStreamEntry_Should
    {
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullStreamId(
            EventStreamEntrySequence sequence,
            EventStreamEntryId id,
            EventStreamEntryContent content,
            EventStreamEntryMetadata metadata)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEntry(
                null,
                sequence,
                id,
                content,
                metadata));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullSequence(
            EventStreamId streamId,
            EventStreamEntryId id,
            EventStreamEntryContent content,
            EventStreamEntryMetadata metadata)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEntry(
                streamId,
                null,
                id,
                content,
                metadata));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullId(
            EventStreamId streamId,
            EventStreamEntrySequence sequence,
            EventStreamEntryContent content,
            EventStreamEntryMetadata metadata)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEntry(
                streamId,
                sequence,
                null,
                content,
                metadata));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullContent(
            EventStreamId streamId,
            EventStreamEntrySequence sequence,
            EventStreamEntryId id,
            EventStreamEntryMetadata metadata)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEntry(
                streamId,
                sequence,
                id,
                null,
                metadata));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullMetadata(
            EventStreamId streamId,
            EventStreamEntrySequence sequence,
            EventStreamEntryId id,
            EventStreamEntryContent content)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEntry(
                streamId,
                sequence,
                id,
                content,
                null));
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingWithNonNullValues(
            EventStreamId streamId,
            EventStreamEntrySequence sequence,
            EventStreamEntryId id,
            EventStreamEntryContent content,
            EventStreamEntryMetadata metadata)
        {
            _ = new EventStreamEntry(
                streamId,
                sequence,
                id,
                content,
                metadata);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnTrue_When_ComparingDifferentObjectsWithSameValues(
            EventStreamId streamId,
            EventStreamEntrySequence sequence,
            EventStreamEntryId id,
            EventStreamEntryContent content,
            EventStreamEntryMetadata metadata)
        {
            var entry1 = new EventStreamEntry(
                streamId,
                sequence,
                id,
                content,
                metadata);
            
            var entry2 = new EventStreamEntry(
                streamId,
                sequence,
                id,
                content,
                metadata);
            
            Assert.Equal(entry1, entry2);
            Assert.True(entry1 == entry2);
            Assert.False(entry1 != entry2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentStreamId(
            EventStreamId streamId1,
            EventStreamId streamId2,
            EventStreamEntrySequence sequence,
            EventStreamEntryId id,
            EventStreamEntryContent content,
            EventStreamEntryMetadata metadata)
        {
            var entry1 = new EventStreamEntry(
                streamId1,
                sequence,
                id,
                content,
                metadata);
            
            var entry2 = new EventStreamEntry(
                streamId2,
                sequence,
                id,
                content,
                metadata);
            
            Assert.NotEqual(entry1, entry2);
            Assert.False(entry1 == entry2);
            Assert.True(entry1 != entry2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentSequence(
            EventStreamId streamId,
            EventStreamEntrySequence sequence1,
            EventStreamEntrySequence sequence2,
            EventStreamEntryId id,
            EventStreamEntryContent content,
            EventStreamEntryMetadata metadata)
        {
            var entry1 = new EventStreamEntry(
                streamId,
                sequence1,
                id,
                content,
                metadata);
            
            var entry2 = new EventStreamEntry(
                streamId,
                sequence2,
                id,
                content,
                metadata);
            
            Assert.NotEqual(entry1, entry2);
            Assert.False(entry1 == entry2);
            Assert.True(entry1 != entry2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentId(
            EventStreamId streamId,
            EventStreamEntrySequence sequence,
            EventStreamEntryId id1,
            EventStreamEntryId id2,
            EventStreamEntryContent content,
            EventStreamEntryMetadata metadata)
        {
            var entry1 = new EventStreamEntry(
                streamId,
                sequence,
                id1,
                content,
                metadata);
            
            var entry2 = new EventStreamEntry(
                streamId,
                sequence,
                id2,
                content,
                metadata);
            
            Assert.NotEqual(entry1, entry2);
            Assert.False(entry1 == entry2);
            Assert.True(entry1 != entry2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentContent(
            EventStreamId streamId,
            EventStreamEntrySequence sequence,
            EventStreamEntryId id,
            EventStreamEntryContent content1,
            EventStreamEntryContent content2,
            EventStreamEntryMetadata metadata)
        {
            var entry1 = new EventStreamEntry(
                streamId,
                sequence,
                id,
                content1,
                metadata);
            
            var entry2 = new EventStreamEntry(
                streamId,
                sequence,
                id,
                content2,
                metadata);
            
            Assert.NotEqual(entry1, entry2);
            Assert.False(entry1 == entry2);
            Assert.True(entry1 != entry2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentMetadata(
            EventStreamId streamId,
            EventStreamEntrySequence sequence,
            EventStreamEntryId id,
            EventStreamEntryContent content,
            EventStreamEntryMetadata metadata1,
            EventStreamEntryMetadata metadata2)
        {
            var entry1 = new EventStreamEntry(
                streamId,
                sequence,
                id,
                content,
                metadata1);
            
            var entry2 = new EventStreamEntry(
                streamId,
                sequence,
                id,
                content,
                metadata2);
            
            Assert.NotEqual(entry1, entry2);
            Assert.False(entry1 == entry2);
            Assert.True(entry1 != entry2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnExpectedValue_When_CallingToString(EventStreamEntry eventStreamEntry)
        {
            var expectedValue =
                $"Event Stream ID: {eventStreamEntry.StreamId}, Sequence: {eventStreamEntry.Sequence}, Entry ID: {eventStreamEntry.EntryId}, Content: {eventStreamEntry.Content}, Metadata: {eventStreamEntry.Metadata}";
            
            Assert.Equal(expectedValue, eventStreamEntry.ToString());
        }
    }
}
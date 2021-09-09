using System;
using EventSourcing.Abstractions.ValueObjects;
using TestHelpers.Attributes;
using Xunit;

namespace Abstractions.UnitTests.ValueObjects
{
    public class EventStreamEntryId_Should
    {
        [Fact]
        public void ReturnRandomEventStreamEntryId_When_CallingNewEventStreamEntryId()
        {
            var streamId = EventStreamEntryId.NewEventStreamEntryId();
            
            Assert.NotEqual<Guid>(Guid.Empty, streamId);
        }
        
        [Fact]
        public void ReturnDifferentEventStreamEntryIdEachTime_When_CallingNewEventStreamEntryId()
        {
            var streamId1 = EventStreamEntryId.NewEventStreamEntryId();
            var streamId2 = EventStreamEntryId.NewEventStreamEntryId();
            
            Assert.NotEqual(streamId1, streamId2);
        }
        
        [Fact]
        public void Throw_ArgumentException_When_CreatingWithEmptyGuid()
        {
            Assert.Throws<ArgumentException>(() => (EventStreamEntryId) Guid.Empty);
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingWithNonEmptyGuid(Guid id)
        {
            EventStreamEntryId _ = id;
        }

        [Theory]
        [AutoMoqData]
        public void ReturnTrue_When_ComparingDifferentObjectsWithSameValue(Guid value)
        {
            EventStreamEntryId id1 = value;
            EventStreamEntryId id2 = value;
            
            Assert.Equal(id1, id2);
            Assert.True(id1 == id2);
            Assert.False(id1 != id2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentValue(Guid value, Guid otherValue)
        {
            EventStreamEntryId id1 = value;
            EventStreamEntryId id2 = otherValue;
            
            Assert.NotEqual(id1, id2);
            Assert.False(id1 == id2);
            Assert.True(id1 != id2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnValueToString_When_CallingToString(EventStreamEntryId id)
        {
            var idAsGuid = (Guid) id;
            
            Assert.Equal(idAsGuid.ToString(), id.ToString());
        }
    }
}
using System;
using EventSourcing.Abstractions.Persistence.DataTransferObjects;
using TestHelpers.Attributes;
using Xunit;

namespace Abstractions.UnitTests.Persistence.DataTransferObjects
{
    public class EventStreamEntryCreationTime_Should
    {
        [Fact]
        public void Throw_ArgumentException_When_CreatingWithMinValue()
        {
            Assert.Throws<ArgumentException>(() => (EventStreamEntryCreationTime) DateTime.MinValue);
        }
        
        [Fact]
        public void Throw_ArgumentException_When_CreatingWithMaxValue()
        {
            Assert.Throws<ArgumentException>(() => (EventStreamEntryCreationTime) DateTime.MaxValue);
        }

        [Theory]
        [AutoMoqWithInlineData(DateTimeKind.Local)]
        [AutoMoqWithInlineData(DateTimeKind.Unspecified)]
        public void Throw_ArgumentException_When_CreatingWithDifferentKindThanUtc(DateTimeKind dateTimeKind, long ticks)
        {
            Assert.Throws<ArgumentException>(() => (EventStreamEntryCreationTime) new DateTime(ticks, dateTimeKind));
        }

        [Theory]
        [AutoMoqWithInlineData(DateTimeKind.Utc)]
        public void NotThrow_When_CreatingWithKindEqualToUtc(DateTimeKind dateTimeKind, long ticks)
        {
            EventStreamEntryCreationTime _ = new DateTime(ticks, dateTimeKind);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnTrue_When_ComparingDifferentObjectsWithSameValue(long ticks)
        {
            EventStreamEntryCreationTime id1 = new DateTime(ticks, DateTimeKind.Utc);
            EventStreamEntryCreationTime id2 = new DateTime(ticks, DateTimeKind.Utc);
            
            Assert.Equal(id1, id2);
            Assert.True(id1 == id2);
            Assert.False(id1 != id2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentValue(long ticks, long otherTicks)
        {
            EventStreamEntryCreationTime id1 = new DateTime(ticks, DateTimeKind.Utc);
            EventStreamEntryCreationTime id2 = new DateTime(otherTicks, DateTimeKind.Utc);
            
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
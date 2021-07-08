using System;
using System.Globalization;
using EventSourcing.Abstractions;
using TestHelpers.Attributes;
using Xunit;

namespace Abstractions.UnitTests
{
    public class EventStreamEventCreationTime_Should
    {
        [Fact]
        public void Throw_ArgumentException_When_CreatingWithMinDateTimeValue()
        {
            Assert.Throws<ArgumentException>(() => (EventStreamEventCreationTime) DateTime.MinValue);
        }
        
        [Fact]
        public void Throw_ArgumentException_When_CreatingWithMaxDateTimeValue()
        {
            Assert.Throws<ArgumentException>(() => (EventStreamEventCreationTime) DateTime.MaxValue);
        }
        
        [Theory]
        [AutoMoqWithInlineData(DateTimeKind.Local)]
        [AutoMoqWithInlineData(DateTimeKind.Unspecified)]
        public void Throw_ArgumentException_When_CreatingWithInvalidDateTimeKind(
            DateTimeKind dateTimeKind,
            uint ticks)
        {
            Assert.Throws<ArgumentException>(() => (EventStreamEventCreationTime) new DateTime(ticks, dateTimeKind));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentException_When_CreatingWithUtcDateTimeKind(uint ticks)
        {
            EventStreamEventCreationTime _ = new DateTime(ticks, DateTimeKind.Utc);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnTrue_When_ComparingDifferentObjectsWithSameValue(uint ticks)
        {
            EventStreamEventCreationTime creationTime1 = new DateTime(ticks, DateTimeKind.Utc);
            EventStreamEventCreationTime creationTime2 = new DateTime(ticks, DateTimeKind.Utc);
            
            Assert.Equal(creationTime1, creationTime2);
            Assert.True(creationTime1 == creationTime2);
            Assert.False(creationTime1 != creationTime2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentValue(uint ticks, uint otherTicks)
        {
            EventStreamEventCreationTime creationTime1 = new DateTime(ticks, DateTimeKind.Utc);
            EventStreamEventCreationTime creationTime2 = new DateTime(otherTicks, DateTimeKind.Utc);
            
            Assert.NotEqual(creationTime1, creationTime2);
            Assert.False(creationTime1 == creationTime2);
            Assert.True(creationTime1 != creationTime2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnValueToString_When_CallingToString(EventStreamEventCreationTime creationTime)
        {
            var creationTimeAsDateTime = (DateTime) creationTime;
            
            Assert.Equal(creationTimeAsDateTime.ToString(CultureInfo.InvariantCulture), creationTime.ToString());
        }
    }
}
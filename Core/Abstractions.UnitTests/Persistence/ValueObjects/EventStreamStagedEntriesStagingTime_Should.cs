using System;
using EventSourcing.Persistence.ValueObjects;
using TestHelpers.Attributes;
using Xunit;

namespace Abstractions.UnitTests.Persistence.ValueObjects
{
    public class EventStreamStagedEntriesStagingTime_Should
    {
        [Fact]
        public void Throw_ArgumentException_When_CreatingWithMinDateTimeValue()
        {
            Assert.Throws<ArgumentException>(() => (EventStreamStagedEntriesStagingTime) DateTime.MinValue);
        }

        [Fact]
        public void Throw_ArgumentException_When_CreatingWithMaxDateTimeValue()
        {
            Assert.Throws<ArgumentException>(() => (EventStreamStagedEntriesStagingTime) DateTime.MaxValue);
        }

        [Theory]
        [AutoMoqWithInlineData(DateTimeKind.Local)]
        [AutoMoqWithInlineData(DateTimeKind.Unspecified)]
        public void Throw_ArgumentException_When_CreatingWithInvalidDateTimeKind(
            DateTimeKind dateTimeKind,
            uint ticks)
        {
            Assert.Throws<ArgumentException>(() => (EventStreamStagedEntriesStagingTime) new DateTime(ticks, dateTimeKind));
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingWithUtcDateTimeKind(uint ticks)
        {
            EventStreamStagedEntriesStagingTime _ = new DateTime(ticks, DateTimeKind.Utc);
        }

        [Fact]
        public void ReturnUtcNow_When_CallingNow()
        {
            var result = EventStreamStagedEntriesStagingTime.Now();

            Assert.True((DateTime.UtcNow - result).TotalMilliseconds <= 1);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnTrue_When_ComparingDifferentObjectsWithSameValue(uint ticks)
        {
            EventStreamStagedEntriesStagingTime creationTime1 = new DateTime(ticks, DateTimeKind.Utc);
            EventStreamStagedEntriesStagingTime creationTime2 = new DateTime(ticks, DateTimeKind.Utc);

            Assert.Equal(creationTime1, creationTime2);
            Assert.True(creationTime1 == creationTime2);
            Assert.False(creationTime1 != creationTime2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentValue(uint ticks, uint otherTicks)
        {
            EventStreamStagedEntriesStagingTime creationTime1 = new DateTime(ticks, DateTimeKind.Utc);
            EventStreamStagedEntriesStagingTime creationTime2 = new DateTime(otherTicks, DateTimeKind.Utc);

            Assert.NotEqual(creationTime1, creationTime2);
            Assert.False(creationTime1 == creationTime2);
            Assert.True(creationTime1 != creationTime2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnValueToString_When_CallingToString(EventStreamStagedEntriesStagingTime creationTime)
        {
            var creationTimeAsDateTime = (DateTime) creationTime;

            Assert.Equal(creationTimeAsDateTime.ToString("O"), creationTime.ToString());
        }
    }
}

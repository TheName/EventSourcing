using System;
using EventSourcing.ForgettablePayloads.Abstractions.ValueObjects;
using TestHelpers.Attributes;
using Xunit;

namespace ForgettablePayloads.Abstractions.UnitTests.ValueObjects
{
    public class ForgettablePayloadLastModifiedTime_Should
    {
        [Fact]
        public void Throw_ArgumentException_When_CreatingWithMinDateTimeValue()
        {
            Assert.Throws<ArgumentException>(() => (ForgettablePayloadLastModifiedTime) DateTime.MinValue);
        }
        
        [Fact]
        public void Throw_ArgumentException_When_CreatingWithMaxDateTimeValue()
        {
            Assert.Throws<ArgumentException>(() => (ForgettablePayloadLastModifiedTime) DateTime.MaxValue);
        }
        
        [Theory]
        [AutoMoqWithInlineData(DateTimeKind.Local)]
        [AutoMoqWithInlineData(DateTimeKind.Unspecified)]
        public void Throw_ArgumentException_When_CreatingWithInvalidDateTimeKind(
            DateTimeKind dateTimeKind,
            uint ticks)
        {
            Assert.Throws<ArgumentException>(() => (ForgettablePayloadLastModifiedTime) new DateTime(ticks, dateTimeKind));
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingWithUtcDateTimeKind(uint ticks)
        {
            ForgettablePayloadLastModifiedTime _ = new DateTime(ticks, DateTimeKind.Utc);
        }

        [Fact]
        public void ReturnUtcNow_When_CallingNow()
        {
            var result = ForgettablePayloadLastModifiedTime.Now();

            Assert.True((DateTime.UtcNow - result).TotalMilliseconds <= 1);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnTrue_When_ComparingDifferentObjectsWithSameValue(uint ticks)
        {
            ForgettablePayloadLastModifiedTime time1 = new DateTime(ticks, DateTimeKind.Utc);
            ForgettablePayloadLastModifiedTime time2 = new DateTime(ticks, DateTimeKind.Utc);
            
            Assert.Equal(time1, time2);
            Assert.True(time1 == time2);
            Assert.False(time1 != time2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentValue(uint ticks, uint otherTicks)
        {
            ForgettablePayloadLastModifiedTime time1 = new DateTime(ticks, DateTimeKind.Utc);
            ForgettablePayloadLastModifiedTime time2 = new DateTime(otherTicks, DateTimeKind.Utc);
            
            Assert.NotEqual(time1, time2);
            Assert.False(time1 == time2);
            Assert.True(time1 != time2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnValueToString_When_CallingToString(ForgettablePayloadLastModifiedTime time)
        {
            var timeAsDateTime = (DateTime) time;
            
            Assert.Equal(timeAsDateTime.ToString("O"), time.ToString());
        }
    }
}
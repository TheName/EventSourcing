using System;
using EventSourcing.ForgettablePayloads.ValueObjects;
using TestHelpers.Attributes;
using Xunit;

namespace ForgettablePayloads.Abstractions.UnitTests.ValueObjects
{
    public class ForgettingPayloadTime_Should
    {
        [Fact]
        public void Throw_ArgumentException_When_CreatingWithMinDateTimeValue()
        {
            Assert.Throws<ArgumentException>(() => (ForgettingPayloadTime) DateTime.MinValue);
        }
        
        [Fact]
        public void Throw_ArgumentException_When_CreatingWithMaxDateTimeValue()
        {
            Assert.Throws<ArgumentException>(() => (ForgettingPayloadTime) DateTime.MaxValue);
        }
        
        [Theory]
        [AutoMoqWithInlineData(DateTimeKind.Local)]
        [AutoMoqWithInlineData(DateTimeKind.Unspecified)]
        public void Throw_ArgumentException_When_CreatingWithInvalidDateTimeKind(
            DateTimeKind dateTimeKind,
            uint ticks)
        {
            Assert.Throws<ArgumentException>(() => (ForgettingPayloadTime) new DateTime(ticks, dateTimeKind));
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingWithUtcDateTimeKind(uint ticks)
        {
            ForgettingPayloadTime _ = new DateTime(ticks, DateTimeKind.Utc);
        }

        [Fact]
        public void ReturnUtcNow_When_CallingNow()
        {
            var result = ForgettingPayloadTime.Now();

            Assert.True((DateTime.UtcNow - result).TotalMilliseconds <= 1);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnTrue_When_ComparingDifferentObjectsWithSameValue(uint ticks)
        {
            ForgettingPayloadTime time1 = new DateTime(ticks, DateTimeKind.Utc);
            ForgettingPayloadTime time2 = new DateTime(ticks, DateTimeKind.Utc);
            
            Assert.Equal(time1, time2);
            Assert.True(time1 == time2);
            Assert.False(time1 != time2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentValue(uint ticks, uint otherTicks)
        {
            ForgettingPayloadTime time1 = new DateTime(ticks, DateTimeKind.Utc);
            ForgettingPayloadTime time2 = new DateTime(otherTicks, DateTimeKind.Utc);
            
            Assert.NotEqual(time1, time2);
            Assert.False(time1 == time2);
            Assert.True(time1 != time2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnValueToString_When_CallingToString(ForgettingPayloadTime time)
        {
            var timeAsDateTime = (DateTime) time;
            
            Assert.Equal(timeAsDateTime.ToString("O"), time.ToString());
        }
    }
}
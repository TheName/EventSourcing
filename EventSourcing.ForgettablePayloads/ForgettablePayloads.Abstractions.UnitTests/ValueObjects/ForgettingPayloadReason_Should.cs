using System;
using EventSourcing.ForgettablePayloads.ValueObjects;
using TestHelpers.Attributes;
using Xunit;

namespace ForgettablePayloads.Abstractions.UnitTests.ValueObjects
{
    public class ForgettingPayloadReason_Should
    {
        [Theory]
        [AutoMoqWithInlineData((string)null)]
        [AutoMoqWithInlineData("")]
        [AutoMoqWithInlineData(" ")]
        [AutoMoqWithInlineData("\n")]
        [AutoMoqWithInlineData("\t")]
        public void Throw_ArgumentException_When_CreatingWithNullOrWhitespaceString(string value)
        {
            Assert.Throws<ArgumentException>(() => (ForgettingPayloadReason) value);
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingWithNonEmptyString(string content)
        {
            ForgettingPayloadReason _ = content;
        }

        [Fact]
        public void NotThrow_When_UsingRequestedByDataOwnerStatic()
        {
            _ = ForgettingPayloadReason.RequestedByDataOwner;
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingWithGetDueToBeingUnclaimedForLongerThan(TimeSpan timeSpan)
        {
            _ = ForgettingPayloadReason.GetDueToBeingUnclaimedForLongerThan(timeSpan);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnTrue_When_ComparingDifferentObjectsWithSameValue(string value)
        {
            ForgettingPayloadReason content1 = value;
            ForgettingPayloadReason content2 = value;
            
            Assert.Equal(content1, content2);
            Assert.True(content1 == content2);
            Assert.False(content1 != content2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentValue(string value, string otherValue)
        {
            ForgettingPayloadReason content1 = value;
            ForgettingPayloadReason content2 = otherValue;
            
            Assert.NotEqual(content1, content2);
            Assert.False(content1 == content2);
            Assert.True(content1 != content2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnValueToString_When_CallingToString(string value)
        {
            ForgettingPayloadReason content = value;
            
            Assert.Equal(value, content.ToString());
        }
    }
}
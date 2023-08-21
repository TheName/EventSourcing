using System;
using EventSourcing.ForgettablePayloads.ValueObjects;
using TestHelpers.Attributes;
using Xunit;

namespace ForgettablePayloads.Abstractions.UnitTests.ValueObjects
{
    public class ForgettablePayloadState_Should
    {
        [Theory]
        [AutoMoqWithInlineData((string)null)]
        [AutoMoqWithInlineData("")]
        [AutoMoqWithInlineData(" ")]
        [AutoMoqWithInlineData("\n")]
        [AutoMoqWithInlineData("\t")]
        public void Throw_ArgumentException_When_CreatingWithNullOrWhitespaceString(string value)
        {
            Assert.Throws<ArgumentException>(() => (ForgettablePayloadState) value);
        }

        [Theory]
        [AutoMoqData]
        public void Throw_When_CreatingWithInvalidNonEmptyString(string value)
        {
            Assert.Throws<ArgumentException>(() => (ForgettablePayloadState) value);
        }

        [Theory]
        [InlineData(nameof(ForgettablePayloadState.Created))]
        [InlineData(nameof(ForgettablePayloadState.CreatedAndClaimed))]
        [InlineData(nameof(ForgettablePayloadState.Forgotten))]
        public void NotThrow_When_CreatingWithValidString(string value)
        {
            ForgettablePayloadState _ = value;
        }

        [Theory]
        [InlineData(nameof(ForgettablePayloadState.Created))]
        [InlineData(nameof(ForgettablePayloadState.CreatedAndClaimed))]
        [InlineData(nameof(ForgettablePayloadState.Forgotten))]
        public void ReturnTrue_When_ComparingDifferentObjectsWithSameValue(string value)
        {
            ForgettablePayloadState content1 = value;
            ForgettablePayloadState content2 = value;
            
            Assert.Equal(content1, content2);
            Assert.True(content1 == content2);
            Assert.False(content1 != content2);
        }

        [Theory]
        [InlineData(nameof(ForgettablePayloadState.Created), nameof(ForgettablePayloadState.CreatedAndClaimed))]
        [InlineData(nameof(ForgettablePayloadState.Created), nameof(ForgettablePayloadState.Forgotten))]
        [InlineData(nameof(ForgettablePayloadState.CreatedAndClaimed), nameof(ForgettablePayloadState.Created))]
        [InlineData(nameof(ForgettablePayloadState.CreatedAndClaimed), nameof(ForgettablePayloadState.Forgotten))]
        [InlineData(nameof(ForgettablePayloadState.Forgotten), nameof(ForgettablePayloadState.Created))]
        [InlineData(nameof(ForgettablePayloadState.Forgotten), nameof(ForgettablePayloadState.CreatedAndClaimed))]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentValue(string value, string otherValue)
        {
            ForgettablePayloadState content1 = value;
            ForgettablePayloadState content2 = otherValue;
            
            Assert.NotEqual(content1, content2);
            Assert.False(content1 == content2);
            Assert.True(content1 != content2);
        }

        [Theory]
        [InlineData(nameof(ForgettablePayloadState.Created))]
        [InlineData(nameof(ForgettablePayloadState.CreatedAndClaimed))]
        [InlineData(nameof(ForgettablePayloadState.Forgotten))]
        public void ReturnValueToString_When_CallingToString(string value)
        {
            ForgettablePayloadState content = value;
            
            Assert.Equal(value, content.ToString());
        }
    }
}
using System;
using EventSourcing.ForgettablePayloads.ValueObjects;
using TestHelpers.Attributes;
using Xunit;

namespace ForgettablePayloads.Abstractions.UnitTests.ValueObjects
{
    public class ForgettablePayloadContent_Should
    {
        [Theory]
        [AutoMoqWithInlineData((string)null)]
        [AutoMoqWithInlineData("")]
        [AutoMoqWithInlineData(" ")]
        [AutoMoqWithInlineData("\n")]
        [AutoMoqWithInlineData("\t")]
        public void Throw_ArgumentException_When_CreatingWithNullOrWhitespaceString(string value)
        {
            Assert.Throws<ArgumentException>(() => (ForgettablePayloadContent) value);
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingWithNonEmptyString(string content)
        {
            ForgettablePayloadContent _ = content;
        }

        [Theory]
        [AutoMoqData]
        public void ReturnTrue_When_ComparingDifferentObjectsWithSameValue(string value)
        {
            ForgettablePayloadContent content1 = value;
            ForgettablePayloadContent content2 = value;
            
            Assert.Equal(content1, content2);
            Assert.True(content1 == content2);
            Assert.False(content1 != content2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentValue(string value, string otherValue)
        {
            ForgettablePayloadContent content1 = value;
            ForgettablePayloadContent content2 = otherValue;
            
            Assert.NotEqual(content1, content2);
            Assert.False(content1 == content2);
            Assert.True(content1 != content2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnValueToString_When_CallingToString(string value)
        {
            ForgettablePayloadContent content = value;
            
            Assert.Equal(value, content.ToString());
        }
    }
}
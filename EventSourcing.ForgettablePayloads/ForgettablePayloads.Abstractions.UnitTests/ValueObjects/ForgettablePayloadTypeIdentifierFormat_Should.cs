using System;
using EventSourcing.ForgettablePayloads.Abstractions.ValueObjects;
using TestHelpers.Attributes;
using Xunit;

namespace ForgettablePayloads.Abstractions.UnitTests.ValueObjects
{
    public class ForgettablePayloadTypeIdentifierFormat_Should
    {
        [Theory]
        [AutoMoqWithInlineData((string)null)]
        [AutoMoqWithInlineData("")]
        [AutoMoqWithInlineData(" ")]
        [AutoMoqWithInlineData("\n")]
        [AutoMoqWithInlineData("\t")]
        public void Throw_ArgumentException_When_CreatingWithNullOrWhitespaceString(string value)
        {
            Assert.Throws<ArgumentException>(() => (ForgettablePayloadTypeIdentifierFormat) value);
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingWithNonEmptyString(string content)
        {
            ForgettablePayloadTypeIdentifierFormat _ = content;
        }

        [Theory]
        [AutoMoqData]
        public void ReturnTrue_When_ComparingDifferentObjectsWithSameValue(string value)
        {
            ForgettablePayloadTypeIdentifierFormat content1 = value;
            ForgettablePayloadTypeIdentifierFormat content2 = value;
            
            Assert.Equal(content1, content2);
            Assert.True(content1 == content2);
            Assert.False(content1 != content2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentValue(string value, string otherValue)
        {
            ForgettablePayloadTypeIdentifierFormat content1 = value;
            ForgettablePayloadTypeIdentifierFormat content2 = otherValue;
            
            Assert.NotEqual(content1, content2);
            Assert.False(content1 == content2);
            Assert.True(content1 != content2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnValueToString_When_CallingToString(string value)
        {
            ForgettablePayloadTypeIdentifierFormat content = value;
            
            Assert.Equal(value, content.ToString());
        }
    }
}
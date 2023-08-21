using System;
using EventSourcing.ForgettablePayloads.ValueObjects;
using TestHelpers.Attributes;
using Xunit;

namespace ForgettablePayloads.Abstractions.UnitTests.ValueObjects
{
    public class ForgettablePayloadTypeIdentifier_Should
    {
        [Theory]
        [AutoMoqWithInlineData((string)null)]
        [AutoMoqWithInlineData("")]
        [AutoMoqWithInlineData(" ")]
        [AutoMoqWithInlineData("\n")]
        [AutoMoqWithInlineData("\t")]
        public void Throw_ArgumentException_When_CreatingWithNullOrWhitespaceString(string value)
        {
            Assert.Throws<ArgumentException>(() => (ForgettablePayloadTypeIdentifier) value);
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingWithNonEmptyString(string content)
        {
            ForgettablePayloadTypeIdentifier _ = content;
        }

        [Theory]
        [AutoMoqData]
        public void ReturnTrue_When_ComparingDifferentObjectsWithSameValue(string value)
        {
            ForgettablePayloadTypeIdentifier typeIdentifier1 = value;
            ForgettablePayloadTypeIdentifier typeIdentifier2 = value;
            
            Assert.Equal(typeIdentifier1, typeIdentifier2);
            Assert.True(typeIdentifier1 == typeIdentifier2);
            Assert.False(typeIdentifier1 != typeIdentifier2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentValue(string value, string otherValue)
        {
            ForgettablePayloadTypeIdentifier typeIdentifier1 = value;
            ForgettablePayloadTypeIdentifier typeIdentifier2 = otherValue;
            
            Assert.NotEqual(typeIdentifier1, typeIdentifier2);
            Assert.False(typeIdentifier1 == typeIdentifier2);
            Assert.True(typeIdentifier1 != typeIdentifier2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnValueToString_When_CallingToString(string value)
        {
            ForgettablePayloadTypeIdentifier typeIdentifier = value;
            
            Assert.Equal(value, typeIdentifier.ToString());
        }
    }
}
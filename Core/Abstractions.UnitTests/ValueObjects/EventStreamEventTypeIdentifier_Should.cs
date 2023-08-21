using System;
using EventSourcing.ValueObjects;
using TestHelpers.Attributes;
using Xunit;

namespace Abstractions.UnitTests.ValueObjects
{
    public class EventStreamEventTypeIdentifier_Should
    {
        [Theory]
        [AutoMoqWithInlineData((string)null)]
        [AutoMoqWithInlineData("")]
        [AutoMoqWithInlineData(" ")]
        [AutoMoqWithInlineData("\n")]
        [AutoMoqWithInlineData("\t")]
        public void Throw_ArgumentException_When_CreatingWithNullOrWhitespaceString(string value)
        {
            Assert.Throws<ArgumentException>(() => (EventStreamEventTypeIdentifier) value);
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingWithNonEmptyString(string content)
        {
            EventStreamEventTypeIdentifier _ = content;
        }

        [Theory]
        [AutoMoqData]
        public void ReturnTrue_When_ComparingDifferentObjectsWithSameValue(string value)
        {
            EventStreamEventTypeIdentifier typeIdentifier1 = value;
            EventStreamEventTypeIdentifier typeIdentifier2 = value;
            
            Assert.Equal(typeIdentifier1, typeIdentifier2);
            Assert.True(typeIdentifier1 == typeIdentifier2);
            Assert.False(typeIdentifier1 != typeIdentifier2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentValue(string value, string otherValue)
        {
            EventStreamEventTypeIdentifier typeIdentifier1 = value;
            EventStreamEventTypeIdentifier typeIdentifier2 = otherValue;
            
            Assert.NotEqual(typeIdentifier1, typeIdentifier2);
            Assert.False(typeIdentifier1 == typeIdentifier2);
            Assert.True(typeIdentifier1 != typeIdentifier2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnValueToString_When_CallingToString(string value)
        {
            EventStreamEventTypeIdentifier typeIdentifier = value;
            
            Assert.Equal(value, typeIdentifier.ToString());
        }
    }
}

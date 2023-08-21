using System;
using EventSourcing.ForgettablePayloads.ValueObjects;
using EventSourcing.ValueObjects;
using TestHelpers.Attributes;
using Xunit;

namespace ForgettablePayloads.Abstractions.UnitTests.ValueObjects
{
    public class ForgettablePayloadContentDescriptor_Should
    {
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullForgettablePayloadContent(
            SerializationFormat payloadContentSerializationFormat,
            ForgettablePayloadTypeIdentifier payloadTypeIdentifier,
            ForgettablePayloadTypeIdentifierFormat payloadTypeIdentifierFormat)
        {
            Assert.Throws<ArgumentNullException>(() => new ForgettablePayloadContentDescriptor(
                null,
                payloadContentSerializationFormat,
                payloadTypeIdentifier,
                payloadTypeIdentifierFormat));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullSerializationFormat(
            ForgettablePayloadContent payloadContent,
            ForgettablePayloadTypeIdentifier payloadTypeIdentifier,
            ForgettablePayloadTypeIdentifierFormat payloadTypeIdentifierFormat)
        {
            Assert.Throws<ArgumentNullException>(() => new ForgettablePayloadContentDescriptor(
                payloadContent,
                null,
                payloadTypeIdentifier,
                payloadTypeIdentifierFormat));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullForgettablePayloadTypeIdentifier(
            ForgettablePayloadContent payloadContent,
            SerializationFormat payloadContentSerializationFormat,
            ForgettablePayloadTypeIdentifierFormat payloadTypeIdentifierFormat)
        {
            Assert.Throws<ArgumentNullException>(() => new ForgettablePayloadContentDescriptor(
                payloadContent,
                payloadContentSerializationFormat,
                null,
                payloadTypeIdentifierFormat));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullForgettablePayloadTypeIdentifierFormat(
            ForgettablePayloadContent payloadContent,
            SerializationFormat payloadContentSerializationFormat,
            ForgettablePayloadTypeIdentifier payloadTypeIdentifier)
        {
            Assert.Throws<ArgumentNullException>(() => new ForgettablePayloadContentDescriptor(
                payloadContent,
                payloadContentSerializationFormat,
                payloadTypeIdentifier,
                null));
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingWithNonNullValues(
            ForgettablePayloadContent payloadContent,
            SerializationFormat payloadContentSerializationFormat,
            ForgettablePayloadTypeIdentifier payloadTypeIdentifier,
            ForgettablePayloadTypeIdentifierFormat payloadTypeIdentifierFormat)
        {
            _ = new ForgettablePayloadContentDescriptor(
                payloadContent,
                payloadContentSerializationFormat,
                payloadTypeIdentifier,
                payloadTypeIdentifierFormat);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnValuesProvidedDuringCreation_When_GettingPropertiesValues(
            ForgettablePayloadContent payloadContent,
            SerializationFormat payloadContentSerializationFormat,
            ForgettablePayloadTypeIdentifier payloadTypeIdentifier,
            ForgettablePayloadTypeIdentifierFormat payloadTypeIdentifierFormat)
        {
            var payloadDescriptor = new ForgettablePayloadContentDescriptor(
                payloadContent,
                payloadContentSerializationFormat,
                payloadTypeIdentifier,
                payloadTypeIdentifierFormat);
            
            Assert.Equal(payloadContent, payloadDescriptor.PayloadContent);
            Assert.Equal(payloadContentSerializationFormat, payloadDescriptor.PayloadContentSerializationFormat);
            Assert.Equal(payloadTypeIdentifier, payloadDescriptor.PayloadTypeIdentifier);
            Assert.Equal(payloadTypeIdentifierFormat, payloadDescriptor.PayloadTypeIdentifierFormat);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnTrue_When_ComparingDifferentObjectsWithSameValues(
            ForgettablePayloadContent payloadContent,
            SerializationFormat payloadContentSerializationFormat,
            ForgettablePayloadTypeIdentifier payloadTypeIdentifier,
            ForgettablePayloadTypeIdentifierFormat payloadTypeIdentifierFormat)
        {
            var descriptor1 = new ForgettablePayloadContentDescriptor(
                payloadContent,
                payloadContentSerializationFormat,
                payloadTypeIdentifier,
                payloadTypeIdentifierFormat);
            
            var descriptor2 = new ForgettablePayloadContentDescriptor(
                payloadContent,
                payloadContentSerializationFormat,
                payloadTypeIdentifier,
                payloadTypeIdentifierFormat);
            
            Assert.Equal(descriptor1, descriptor2);
            Assert.True(descriptor1 == descriptor2);
            Assert.False(descriptor1 != descriptor2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentPayloadContent(
            ForgettablePayloadContentDescriptor descriptor,
            ForgettablePayloadContent differentPayloadContent)
        {
            var descriptor1 = new ForgettablePayloadContentDescriptor(
                descriptor.PayloadContent,
                descriptor.PayloadContentSerializationFormat,
                descriptor.PayloadTypeIdentifier,
                descriptor.PayloadTypeIdentifierFormat);
            
            var descriptor2 = new ForgettablePayloadContentDescriptor(
                differentPayloadContent,
                descriptor.PayloadContentSerializationFormat,
                descriptor.PayloadTypeIdentifier,
                descriptor.PayloadTypeIdentifierFormat);
            
            Assert.NotEqual(descriptor1, descriptor2);
            Assert.False(descriptor1 == descriptor2);
            Assert.True(descriptor1 != descriptor2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentPayloadContentSerializationFormat(
            ForgettablePayloadContentDescriptor descriptor,
            SerializationFormat differentPayloadContentSerializationFormat)
        {
            var descriptor1 = new ForgettablePayloadContentDescriptor(
                descriptor.PayloadContent,
                descriptor.PayloadContentSerializationFormat,
                descriptor.PayloadTypeIdentifier,
                descriptor.PayloadTypeIdentifierFormat);
            
            var descriptor2 = new ForgettablePayloadContentDescriptor(
                descriptor.PayloadContent,
                differentPayloadContentSerializationFormat,
                descriptor.PayloadTypeIdentifier,
                descriptor.PayloadTypeIdentifierFormat);
            
            Assert.NotEqual(descriptor1, descriptor2);
            Assert.False(descriptor1 == descriptor2);
            Assert.True(descriptor1 != descriptor2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentPayloadTypeIdentifier(
            ForgettablePayloadContentDescriptor descriptor,
            ForgettablePayloadTypeIdentifier differentPayloadTypeIdentifier)
        {
            var descriptor1 = new ForgettablePayloadContentDescriptor(
                descriptor.PayloadContent,
                descriptor.PayloadContentSerializationFormat,
                descriptor.PayloadTypeIdentifier,
                descriptor.PayloadTypeIdentifierFormat);
            
            var descriptor2 = new ForgettablePayloadContentDescriptor(
                descriptor.PayloadContent,
                descriptor.PayloadContentSerializationFormat,
                differentPayloadTypeIdentifier,
                descriptor.PayloadTypeIdentifierFormat);
            
            Assert.NotEqual(descriptor1, descriptor2);
            Assert.False(descriptor1 == descriptor2);
            Assert.True(descriptor1 != descriptor2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentPayloadTypeIdentifierFormat(
            ForgettablePayloadContentDescriptor descriptor,
            ForgettablePayloadTypeIdentifierFormat differentPayloadTypeIdentifierFormat)
        {
            var descriptor1 = new ForgettablePayloadContentDescriptor(
                descriptor.PayloadContent,
                descriptor.PayloadContentSerializationFormat,
                descriptor.PayloadTypeIdentifier,
                descriptor.PayloadTypeIdentifierFormat);
            
            var descriptor2 = new ForgettablePayloadContentDescriptor(
                descriptor.PayloadContent,
                descriptor.PayloadContentSerializationFormat,
                descriptor.PayloadTypeIdentifier,
                differentPayloadTypeIdentifierFormat);
            
            Assert.NotEqual(descriptor1, descriptor2);
            Assert.False(descriptor1 == descriptor2);
            Assert.True(descriptor1 != descriptor2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnExpectedValue_When_CallingToString(ForgettablePayloadContentDescriptor descriptor)
        {
            var expectedValue =
                $"Forgettable Payload Content: {descriptor.PayloadContent}, Payload Content Serialization Format: {descriptor.PayloadContentSerializationFormat}, Payload Type Identifier: {descriptor.PayloadTypeIdentifier}, Payload Type Identifier Format: {descriptor.PayloadTypeIdentifierFormat}";
            
            Assert.Equal(expectedValue, descriptor.ToString());
        }
    }
}

using System;
using AutoFixture.Xunit2;
using EventSourcing.ForgettablePayloads.Conversion;
using EventSourcing.ForgettablePayloads.ValueObjects;
using EventSourcing.Serialization;
using EventSourcing.ValueObjects;
using Moq;
using TestHelpers.Attributes;
using Xunit;

namespace ForgettablePayloads.UnitTests.Conversion
{
    public class ForgettablePayloadContentConverter_Should
    {
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_Creating_And_SerializerProviderIsNull(
            IForgettablePayloadTypeIdentifierConverterProvider typeIdentifierConverterProvider)
        {
            Assert.Throws<ArgumentNullException>(() =>
                new ForgettablePayloadContentConverter(null, typeIdentifierConverterProvider));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_Creating_And_TypeIdentifierConverterProviderIsNull(
            ISerializerProvider serializerProvider)
        {
            Assert.Throws<ArgumentNullException>(() =>
                new ForgettablePayloadContentConverter(serializerProvider, null));
        }
        
        [Theory]
        [AutoMoqData]
        public void NotThrow_When_Creating_And_AllParametersAreNotNull(
            ISerializerProvider serializerProvider,
            IForgettablePayloadTypeIdentifierConverterProvider typeIdentifierConverterProvider)
        {
            _ = new ForgettablePayloadContentConverter(serializerProvider, typeIdentifierConverterProvider);
        }

        [Theory]
        [AutoMoqData]
        internal void Throw_ArgumentNullException_When_CallingToPayloadContentDescriptor_And_ProvidingNullPayloadParameter(
            ForgettablePayloadContentConverter contentConverter)
        {
            Assert.Throws<ArgumentNullException>(() => contentConverter.ToPayloadContentDescriptor(null));
        }

        [Theory]
        [AutoMoqData]
        internal void Throw_InvalidOperationException_When_CallingToPayloadContentDescriptor_And_TypeIdentifierConverterProviderReturnsNullWhenGetForgettablePayloadTypeIdentifierConverterIsCalled(
            object payload,
            [Frozen] Mock<IForgettablePayloadTypeIdentifierConverterProvider> typeIdentifierConverterProviderMock,
            ForgettablePayloadContentConverter contentConverter)
        {
            typeIdentifierConverterProviderMock
                .Setup(provider => provider.GetForgettablePayloadTypeIdentifierConverter())
                .Returns(null as IForgettablePayloadTypeIdentifierConverter);
            
            Assert.Throws<InvalidOperationException>(() => contentConverter.ToPayloadContentDescriptor(payload));
        }

        [Theory]
        [AutoMoqData]
        internal void Throw_InvalidOperationException_When_CallingToPayloadContentDescriptor_And_SerializerProviderReturnsNullWhenGetForgettablePayloadSerializerIsCalled(
            object payload,
            ForgettablePayloadTypeIdentifier typeIdentifier,
            Mock<IForgettablePayloadTypeIdentifierConverter> typeIdentifierConverterMock,
            [Frozen] Mock<IForgettablePayloadTypeIdentifierConverterProvider> typeIdentifierConverterProviderMock,
            [Frozen] Mock<ISerializerProvider> serializerProviderMock,
            ForgettablePayloadContentConverter contentConverter)
        {
            typeIdentifierConverterProviderMock
                .Setup(provider => provider.GetForgettablePayloadTypeIdentifierConverter())
                .Returns(typeIdentifierConverterMock.Object);

            typeIdentifierConverterMock
                .Setup(converter => converter.ToTypeIdentifier(payload.GetType()))
                .Returns(typeIdentifier);

            serializerProviderMock
                .Setup(provider => provider.GetForgettablePayloadSerializer())
                .Returns(null as ISerializer);
            
            Assert.Throws<InvalidOperationException>(() => contentConverter.ToPayloadContentDescriptor(payload));
        }

        [Theory]
        [AutoMoqData]
        internal void ReturnForgettablePayloadContentDescriptor_When_CallingToPayloadContentDescriptor(
            object payload,
            string serializedPayload,
            ForgettablePayloadTypeIdentifier typeIdentifier,
            ForgettablePayloadTypeIdentifierFormat typeIdentifierFormat,
            Mock<IForgettablePayloadTypeIdentifierConverter> typeIdentifierConverterMock,
            SerializationFormat serializationFormat,
            Mock<ISerializer> serializerMock,
            [Frozen] Mock<IForgettablePayloadTypeIdentifierConverterProvider> typeIdentifierConverterProviderMock,
            [Frozen] Mock<ISerializerProvider> serializerProviderMock,
            ForgettablePayloadContentConverter contentConverter)
        {
            typeIdentifierConverterProviderMock
                .Setup(provider => provider.GetForgettablePayloadTypeIdentifierConverter())
                .Returns(typeIdentifierConverterMock.Object);

            typeIdentifierConverterMock
                .Setup(converter => converter.ToTypeIdentifier(payload.GetType()))
                .Returns(typeIdentifier);

            typeIdentifierConverterMock
                .SetupGet(converter => converter.TypeIdentifierFormat)
                .Returns(typeIdentifierFormat);

            serializerProviderMock
                .Setup(provider => provider.GetForgettablePayloadSerializer())
                .Returns(serializerMock.Object);

            serializerMock
                .Setup(serializer => serializer.Serialize(payload))
                .Returns(serializedPayload);

            serializerMock
                .SetupGet(serializer => serializer.SerializationFormat)
                .Returns(serializationFormat);

            var result = contentConverter.ToPayloadContentDescriptor(payload);

            Assert.NotNull(result);
            Assert.Equal(serializedPayload, result.PayloadContent);
            Assert.Equal(serializationFormat, result.PayloadContentSerializationFormat);
            Assert.Equal(typeIdentifier, result.PayloadTypeIdentifier);
            Assert.Equal(typeIdentifierFormat, result.PayloadTypeIdentifierFormat);
        }

        [Theory]
        [AutoMoqData]
        internal void Throw_ArgumentNullException_When_CallingFromPayloadContentDescriptor_And_ProvidingNullDescriptorParameter(
            ForgettablePayloadContentConverter contentConverter)
        {
            Assert.Throws<ArgumentNullException>(() => contentConverter.FromPayloadContentDescriptor(null));
        }

        [Theory]
        [AutoMoqData]
        internal void Throw_InvalidOperationException_When_CallingFromPayloadContentDescriptor_And_TypeIdentifierConverterProviderReturnsNullWhenGetConverterIsCalled(
            ForgettablePayloadContentDescriptor payloadContentDescriptor,
            [Frozen] Mock<IForgettablePayloadTypeIdentifierConverterProvider> typeIdentifierConverterProviderMock,
            ForgettablePayloadContentConverter contentConverter)
        {
            typeIdentifierConverterProviderMock
                .Setup(provider => provider.GetConverter(payloadContentDescriptor.PayloadTypeIdentifierFormat))
                .Returns(null as IForgettablePayloadTypeIdentifierConverter);
            
            Assert.Throws<InvalidOperationException>(() => contentConverter.FromPayloadContentDescriptor(payloadContentDescriptor));
        }

        [Theory]
        [AutoMoqData]
        internal void Throw_InvalidOperationException_When_CallingFromPayloadContentDescriptor_And_SerializerProviderReturnsNullWhenGetSerializerIsCalled(
            ForgettablePayloadContentDescriptor payloadContentDescriptor,
            Type returnedPayloadType,
            Mock<IForgettablePayloadTypeIdentifierConverter> typeIdentifierConverterMock,
            [Frozen] Mock<IForgettablePayloadTypeIdentifierConverterProvider> typeIdentifierConverterProviderMock,
            [Frozen] Mock<ISerializerProvider> serializerProviderMock,
            ForgettablePayloadContentConverter contentConverter)
        {
            typeIdentifierConverterProviderMock
                .Setup(provider => provider.GetConverter(payloadContentDescriptor.PayloadTypeIdentifierFormat))
                .Returns(typeIdentifierConverterMock.Object);

            typeIdentifierConverterMock
                .Setup(converter => converter.FromTypeIdentifier(payloadContentDescriptor.PayloadTypeIdentifier))
                .Returns(returnedPayloadType);

            serializerProviderMock
                .Setup(provider => provider.GetSerializer(payloadContentDescriptor.PayloadContentSerializationFormat))
                .Returns(null as ISerializer);
            
            Assert.Throws<InvalidOperationException>(() => contentConverter.FromPayloadContentDescriptor(payloadContentDescriptor));
        }

        [Theory]
        [AutoMoqData]
        internal void ReturnDeserializedPayloadObject_When_CallingFromPayloadContentDescriptor(
            ForgettablePayloadContentDescriptor payloadContentDescriptor,
            Type returnedPayloadType,
            object returnedPayload,
            Mock<IForgettablePayloadTypeIdentifierConverter> typeIdentifierConverterMock,
            Mock<ISerializer> serializerMock,
            [Frozen] Mock<IForgettablePayloadTypeIdentifierConverterProvider> typeIdentifierConverterProviderMock,
            [Frozen] Mock<ISerializerProvider> serializerProviderMock,
            ForgettablePayloadContentConverter contentConverter)
        {
            typeIdentifierConverterProviderMock
                .Setup(provider => provider.GetConverter(payloadContentDescriptor.PayloadTypeIdentifierFormat))
                .Returns(typeIdentifierConverterMock.Object);

            typeIdentifierConverterMock
                .Setup(converter => converter.FromTypeIdentifier(payloadContentDescriptor.PayloadTypeIdentifier))
                .Returns(returnedPayloadType);

            serializerProviderMock
                .Setup(provider => provider.GetSerializer(payloadContentDescriptor.PayloadContentSerializationFormat))
                .Returns(serializerMock.Object);

            serializerMock
                .Setup(serializer => serializer.Deserialize(payloadContentDescriptor.PayloadContent, returnedPayloadType))
                .Returns(returnedPayload);

            var result = contentConverter.FromPayloadContentDescriptor(payloadContentDescriptor);

            Assert.Equal(returnedPayload, result);
        }
    }
}
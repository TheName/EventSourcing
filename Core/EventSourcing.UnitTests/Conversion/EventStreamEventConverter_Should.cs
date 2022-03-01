using System;
using AutoFixture.Xunit2;
using EventSourcing.Abstractions.Conversion;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Conversion;
using EventSourcing.Serialization.Abstractions;
using Moq;
using TestHelpers.Attributes;
using Xunit;

namespace EventSourcing.UnitTests.Conversion
{
    public class EventStreamEventConverter_Should
    {
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_Creating_And_SerializerProviderIsNull(
            IEventStreamEventTypeIdentifierConverterProvider typeIdentifierConverterProvider)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEventConverter(
                null,
                typeIdentifierConverterProvider));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_Creating_And_TypeIdentifierConverterProviderIsNull(
            ISerializerProvider serializerProvider)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEventConverter(
                serializerProvider,
                null));
        }
        
        [Theory]
        [AutoMoqData]
        public void NotThrow_When_Creating_And_AllParametersAreNotNull(
            ISerializerProvider serializerProvider,
            IEventStreamEventTypeIdentifierConverterProvider typeIdentifierConverterProvider)
        {
            _ = new EventStreamEventConverter(serializerProvider, typeIdentifierConverterProvider);
        }
        
        [Theory]
        [AutoMoqData]
        internal void Throw_When_TryingToConvertToEventDescriptor_And_ProvidedEventIsNull(
            EventStreamEventConverter converter)
        {
            Assert.Throws<ArgumentNullException>(() => converter.ToEventDescriptor(null));
        }

        [Theory]
        [AutoMoqData]
        internal void ReturnEventDescriptorWithSerializedContentAndCorrectTypeIdentifier_When_TryingToConvertToEventDescriptor(
            object @event,
            string serializedEvent,
            EventStreamEventTypeIdentifier typeIdentifier,
            EventStreamEventTypeIdentifierFormat typeIdentifierFormat,
            SerializationFormat serializationFormat,
            Mock<ISerializer> serializerMock,
            Mock<IEventStreamEventTypeIdentifierConverter> typeIdentifierConverterMock,
            [Frozen] Mock<ISerializerProvider> serializerProviderMock,
            [Frozen] Mock<IEventStreamEventTypeIdentifierConverterProvider> typeIdentifierConverterProviderMock,
            EventStreamEventConverter converter)
        {
            serializerProviderMock
                .Setup(provider => provider.GetEventContentSerializer())
                .Returns(serializerMock.Object);
            
            serializerMock
                .Setup(serializer => serializer.Serialize(@event))
                .Returns(serializedEvent);

            serializerMock
                .SetupGet(serializer => serializer.SerializationFormat)
                .Returns(serializationFormat);

            typeIdentifierConverterProviderMock
                .Setup(provider => provider.GetEventTypeIdentifierConverter())
                .Returns(typeIdentifierConverterMock.Object);

            typeIdentifierConverterMock
                .Setup(identifierConverter => identifierConverter.ToTypeIdentifier(@event.GetType()))
                .Returns(typeIdentifier);

            typeIdentifierConverterMock
                .SetupGet(identifierConverter => identifierConverter.TypeIdentifierFormat)
                .Returns(typeIdentifierFormat);

            var result = converter.ToEventDescriptor(@event);

            Assert.Equal(serializedEvent, result.EventContent);
            Assert.Equal(serializationFormat, result.EventContentSerializationFormat);
            Assert.Equal(typeIdentifier, result.EventTypeIdentifier);
            Assert.Equal(typeIdentifierFormat, result.EventTypeIdentifierFormat);
        }
        
        [Theory]
        [AutoMoqData]
        internal void Throw_When_TryingToConvertFromEventDescriptor_And_ProvidedEventIsNull(
            EventStreamEventConverter converter)
        {
            Assert.Throws<ArgumentNullException>(() => converter.FromEventDescriptor(null));
        }

        [Theory]
        [AutoMoqData]
        internal void ReturnDeserializedObject_When_TryingToConvertFromEventDescriptor(
            EventStreamEventDescriptor eventDescriptor,
            object deserializedEvent,
            Type eventType,
            Mock<ISerializer> serializerMock,
            Mock<IEventStreamEventTypeIdentifierConverter> typeIdentifierConverterMock,
            [Frozen] Mock<ISerializerProvider> serializerProviderMock,
            [Frozen] Mock<IEventStreamEventTypeIdentifierConverterProvider> typeIdentifierConverterProviderMock,
            EventStreamEventConverter converter)
        {
            serializerProviderMock
                .Setup(provider => provider.GetSerializer(eventDescriptor.EventContentSerializationFormat))
                .Returns(serializerMock.Object);
            
            serializerMock
                .Setup(serializer => serializer.Deserialize(eventDescriptor.EventContent, eventType))
                .Returns(deserializedEvent);

            typeIdentifierConverterProviderMock
                .Setup(provider => provider.GetConverter(eventDescriptor.EventTypeIdentifierFormat))
                .Returns(typeIdentifierConverterMock.Object);

            typeIdentifierConverterMock
                .Setup(identifierConverter => identifierConverter.FromTypeIdentifier(eventDescriptor.EventTypeIdentifier))
                .Returns(eventType);

            var result = converter.FromEventDescriptor(eventDescriptor);

            Assert.Equal(deserializedEvent, result);
        }
    }
}
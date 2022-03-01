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
            IEventStreamEventTypeIdentifierConverter typeIdentifierConverter)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEventConverter(
                null,
                typeIdentifierConverter));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_Creating_And_TypeIdentifierConverterIsNull(
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
            IEventStreamEventTypeIdentifierConverter typeIdentifierConverter)
        {
            _ = new EventStreamEventConverter(serializerProvider, typeIdentifierConverter);
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
            SerializationFormat serializationFormat,
            Mock<ISerializer> serializerMock,
            [Frozen] Mock<ISerializerProvider> serializerProviderMock,
            [Frozen] Mock<IEventStreamEventTypeIdentifierConverter> typeIdentifierConverterMock,
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

            typeIdentifierConverterMock
                .Setup(identifierConverter => identifierConverter.ToTypeIdentifier(@event.GetType()))
                .Returns(typeIdentifier);

            var result = converter.ToEventDescriptor(@event);

            Assert.Equal(serializedEvent, result.EventContent);
            Assert.Equal(serializationFormat, result.EventContentSerializationFormat);
            Assert.Equal(typeIdentifier, result.EventTypeIdentifier);
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
            [Frozen] Mock<ISerializerProvider> serializerProviderMock,
            [Frozen] Mock<IEventStreamEventTypeIdentifierConverter> typeIdentifierConverterMock,
            EventStreamEventConverter converter)
        {
            serializerProviderMock
                .Setup(provider => provider.GetSerializer(eventDescriptor.EventContentSerializationFormat))
                .Returns(serializerMock.Object);
            
            serializerMock
                .Setup(serializer => serializer.Deserialize(eventDescriptor.EventContent, eventType))
                .Returns(deserializedEvent);

            typeIdentifierConverterMock
                .Setup(identifierConverter => identifierConverter.FromTypeIdentifier(eventDescriptor.EventTypeIdentifier))
                .Returns(eventType);

            var result = converter.FromEventDescriptor(eventDescriptor);

            Assert.Equal(deserializedEvent, result);
        }
    }
}
using System;
using AutoFixture.Xunit2;
using EventSourcing.Abstractions;
using EventSourcing.Abstractions.Conversion;
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
        public void Throw_ArgumentNullException_When_Creating_And_SerializerIsNull(
            IEventStreamEventTypeIdentifierConverter typeIdentifierConverter)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEventConverter(
                null,
                typeIdentifierConverter));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_Creating_And_TypeIdentifierConverterIsNull(
            ISerializer serializer)
        {
            Assert.Throws<ArgumentNullException>(() => new EventStreamEventConverter(
                serializer,
                null));
        }
        
        [Theory]
        [AutoMoqData]
        public void NotThrow_When_Creating_And_AllParametersAreNotNull(
            ISerializer serializer,
            IEventStreamEventTypeIdentifierConverter typeIdentifierConverter)
        {
            _ = new EventStreamEventConverter(serializer, typeIdentifierConverter);
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
            [Frozen] Mock<ISerializer> serializerMock,
            [Frozen] Mock<IEventStreamEventTypeIdentifierConverter> typeIdentifierConverterMock,
            EventStreamEventConverter converter)
        {
            serializerMock
                .Setup(serializer => serializer.Serialize(@event))
                .Returns(serializedEvent);

            typeIdentifierConverterMock
                .Setup(identifierConverter => identifierConverter.ToTypeIdentifier(@event.GetType()))
                .Returns(typeIdentifier);

            var result = converter.ToEventDescriptor(@event);

            Assert.Equal(serializedEvent, result.EventContent);
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
            [Frozen] Mock<ISerializer> serializerMock,
            [Frozen] Mock<IEventStreamEventTypeIdentifierConverter> typeIdentifierConverterMock,
            EventStreamEventConverter converter)
        {
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
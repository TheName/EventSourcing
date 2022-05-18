using System;
using EventSourcing.ForgettablePayloads.Abstractions.Conversion;
using EventSourcing.ForgettablePayloads.Abstractions.ValueObjects;
using EventSourcing.Serialization.Abstractions;

namespace EventSourcing.ForgettablePayloads.Conversion
{
    internal class ForgettablePayloadContentConverter : IForgettablePayloadContentConverter
    {
        private readonly ISerializerProvider _serializerProvider;
        private readonly IForgettablePayloadTypeIdentifierConverterProvider _typeIdentifierConverterProvider;

        public ForgettablePayloadContentConverter(
            ISerializerProvider serializerProvider,
            IForgettablePayloadTypeIdentifierConverterProvider typeIdentifierConverterProvider)
        {
            _serializerProvider = serializerProvider ?? throw new ArgumentNullException(nameof(serializerProvider));
            _typeIdentifierConverterProvider = typeIdentifierConverterProvider ?? throw new ArgumentNullException(nameof(typeIdentifierConverterProvider));
        }

        public ForgettablePayloadContentDescriptor ToPayloadContentDescriptor(object payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }
            
            var typeIdentifierConverter = _typeIdentifierConverterProvider.GetForgettablePayloadTypeIdentifierConverter();
            if (typeIdentifierConverter == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(IForgettablePayloadTypeIdentifierConverterProvider)} of type {_typeIdentifierConverterProvider.GetType()} has returned null when called {nameof(IForgettablePayloadTypeIdentifierConverterProvider.GetForgettablePayloadTypeIdentifierConverter)}");
            }
            
            var serializer = _serializerProvider.GetForgettablePayloadSerializer();
            if (serializer == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(ISerializerProvider)} of type {_serializerProvider.GetType()} has returned null when called {nameof(ISerializerProvider.GetForgettablePayloadSerializer)}");
            }
            
            return new ForgettablePayloadContentDescriptor(
                serializer.Serialize(payload),
                serializer.SerializationFormat,
                typeIdentifierConverter.ToTypeIdentifier(payload.GetType()),
                typeIdentifierConverter.TypeIdentifierFormat);
        }

        public object FromPayloadContentDescriptor(ForgettablePayloadContentDescriptor payloadContentDescriptor)
        {
            if (payloadContentDescriptor == null)
            {
                throw new ArgumentNullException(nameof(payloadContentDescriptor));
            }
            
            var typeIdentifierConverter = _typeIdentifierConverterProvider.GetConverter(payloadContentDescriptor.PayloadTypeIdentifierFormat);
            if (typeIdentifierConverter == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(IForgettablePayloadTypeIdentifierConverterProvider)} of type {_typeIdentifierConverterProvider.GetType()} has returned null when called {nameof(IForgettablePayloadTypeIdentifierConverterProvider.GetConverter)} for {payloadContentDescriptor.PayloadTypeIdentifierFormat}");
            }
            
            var payloadType = typeIdentifierConverter.FromTypeIdentifier(payloadContentDescriptor.PayloadTypeIdentifier);
            var serializer = _serializerProvider.GetSerializer(payloadContentDescriptor.PayloadContentSerializationFormat);
            if (serializer == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(ISerializerProvider)} of type {_serializerProvider.GetType()} has returned null when called {nameof(ISerializerProvider.GetSerializer)} for format {payloadContentDescriptor.PayloadContentSerializationFormat}");
            }

            return serializer.Deserialize(payloadContentDescriptor.PayloadContent, payloadType);
        }
    }
}
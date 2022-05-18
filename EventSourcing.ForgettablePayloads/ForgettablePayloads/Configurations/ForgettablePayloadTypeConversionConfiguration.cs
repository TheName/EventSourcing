using EventSourcing.ForgettablePayloads.Abstractions.Configurations;
using EventSourcing.ForgettablePayloads.Abstractions.ValueObjects;

namespace EventSourcing.ForgettablePayloads.Configurations
{
    internal class ForgettablePayloadTypeConversionConfiguration : IForgettablePayloadTypeConversionConfiguration
    {
        public ForgettablePayloadTypeIdentifierFormat ForgettablePayloadTypeIdentifierFormat { get; set; }
    }
}
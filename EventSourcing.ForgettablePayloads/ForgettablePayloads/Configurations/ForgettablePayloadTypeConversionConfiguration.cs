using EventSourcing.ForgettablePayloads.ValueObjects;

namespace EventSourcing.ForgettablePayloads.Configurations
{
    internal class ForgettablePayloadTypeConversionConfiguration : IForgettablePayloadTypeConversionConfiguration
    {
        public ForgettablePayloadTypeIdentifierFormat ForgettablePayloadTypeIdentifierFormat { get; set; }
    }
}

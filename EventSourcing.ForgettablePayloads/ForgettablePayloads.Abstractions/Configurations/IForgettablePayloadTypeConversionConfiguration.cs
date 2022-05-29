using EventSourcing.ForgettablePayloads.Abstractions.ValueObjects;

namespace EventSourcing.ForgettablePayloads.Abstractions.Configurations
{
    /// <summary>
    /// Configuration for type conversion of <see cref="ForgettablePayload{T}"/>
    /// </summary>
    public interface IForgettablePayloadTypeConversionConfiguration
    {
        /// <summary>
        /// The <see cref="ForgettablePayloadTypeIdentifierFormat"/> that should be used when converting event's type to <see cref="ForgettablePayloadTypeIdentifier"/>.
        /// </summary>
        ForgettablePayloadTypeIdentifierFormat ForgettablePayloadTypeIdentifierFormat { get; }
    }
}
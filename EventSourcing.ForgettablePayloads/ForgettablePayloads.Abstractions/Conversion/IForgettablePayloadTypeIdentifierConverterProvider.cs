using EventSourcing.ForgettablePayloads.ValueObjects;

namespace EventSourcing.ForgettablePayloads.Conversion
{
    /// <summary>
    /// Provides instances of <see cref="IForgettablePayloadTypeIdentifierConverter"/>.
    /// </summary>
    public interface IForgettablePayloadTypeIdentifierConverterProvider
    {
        /// <summary>
        /// Gets <see cref="IForgettablePayloadTypeIdentifierConverter"/> configured to be used for conversion of event type to <see cref="ForgettablePayloadTypeIdentifier"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="IForgettablePayloadTypeIdentifierConverter"/>.
        /// </returns>
        IForgettablePayloadTypeIdentifierConverter GetForgettablePayloadTypeIdentifierConverter();

        /// <summary>
        /// Gets <see cref="IForgettablePayloadTypeIdentifierConverter"/> registered for provided <paramref name="forgettablePayloadTypeIdentifierFormat"/>.
        /// </summary>
        /// <param name="forgettablePayloadTypeIdentifierFormat">
        /// The <see cref="ForgettablePayloadTypeIdentifierFormat"/> that returned <see cref="IForgettablePayloadTypeIdentifierConverter"/> should be handling.
        /// </param>
        /// <returns>
        /// The <see cref="IForgettablePayloadTypeIdentifierConverter"/>.
        /// </returns>
        IForgettablePayloadTypeIdentifierConverter GetConverter(ForgettablePayloadTypeIdentifierFormat forgettablePayloadTypeIdentifierFormat);
    }
}

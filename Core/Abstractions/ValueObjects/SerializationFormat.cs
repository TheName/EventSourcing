using System;

namespace EventSourcing.Abstractions.ValueObjects
{
    /// <summary>
    /// The serialization format value object.
    /// </summary>
    public class SerializationFormat
    {
        /// <summary>
        /// The JSON serialization format
        /// </summary>
        public static SerializationFormat Json = new SerializationFormat(nameof(Json));
        
        /// <summary>
        /// The actual value of serialization format
        /// </summary>
        public string Value { get; }

        private SerializationFormat(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"{nameof(SerializationFormat)} cannot be null or whitespace.", nameof(value));
            }
            
            Value = value;
        }

        #region Operators

        /// <summary>
        /// Implicit operator that converts the <see cref="SerializationFormat"/> to <see cref="string"/>.
        /// </summary>
        /// <param name="serializationFormat">
        /// The <see cref="SerializationFormat"/>.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static implicit operator string(SerializationFormat serializationFormat) => serializationFormat.Value;
        
        /// <summary>
        /// Implicit operator that converts the <see cref="string"/> to <see cref="SerializationFormat"/>.
        /// </summary>
        /// <param name="serializationFormat">
        /// The <see cref="string"/>.
        /// </param>
        /// <returns>
        /// The <see cref="SerializationFormat"/>.
        /// </returns>
        public static implicit operator SerializationFormat(string serializationFormat) => new SerializationFormat(serializationFormat);

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="serializationFormat">
        /// The <see cref="SerializationFormat"/>.
        /// </param>
        /// <param name="otherSerializationFormat">
        /// The <see cref="SerializationFormat"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="serializationFormat"/> and <paramref name="otherSerializationFormat"/> are equal, false otherwise.
        /// </returns>
        public static bool operator ==(SerializationFormat serializationFormat, SerializationFormat otherSerializationFormat) =>
            Equals(serializationFormat, otherSerializationFormat);

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="serializationFormat">
        /// The <see cref="SerializationFormat"/>.
        /// </param>
        /// <param name="otherSerializationFormat">
        /// The <see cref="SerializationFormat"/>.
        /// </param>
        /// <returns>
        /// True if <paramref name="serializationFormat"/> and <paramref name="otherSerializationFormat"/> are not equal, false otherwise.
        /// </returns>
        public static bool operator !=(SerializationFormat serializationFormat, SerializationFormat otherSerializationFormat) =>
            !(serializationFormat == otherSerializationFormat);

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is SerializationFormat other &&
            other.Value == Value;

        /// <inheritdoc />
        public override int GetHashCode() =>
            Value.GetHashCode();

        /// <inheritdoc />
        public override string ToString() =>
            Value;
    }
}
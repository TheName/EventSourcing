using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EventSourcing.Serialization.Json.Converters
{
    internal class TypeConverter : JsonConverter<Type>
    {
        public override Type Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            if (value == null)
            {
                return null;
            }

            return Type.GetType(value);
        }

        public override void Write(Utf8JsonWriter writer, Type value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNullValue();
                return;
            }
            
            writer.WriteStringValue(value.FullName);
        }
    }
}
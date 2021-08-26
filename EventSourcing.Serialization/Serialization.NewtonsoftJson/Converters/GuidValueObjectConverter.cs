using System;
using Newtonsoft.Json;

namespace EventSourcing.Serialization.NewtonsoftJson.Converters
{
    internal abstract class GuidValueObjectConverter<T> : JsonConverter<T>
    {
        public override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override T ReadJson(JsonReader reader, Type objectType, T existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var stringValue = (string) reader.Value;
            var guidValue = Guid.Parse(stringValue);

            return Cast(guidValue);
        }

        protected abstract T Cast(Guid value);
    }
}
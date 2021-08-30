using System;
using EventSourcing.Abstractions;
using EventSourcing.Abstractions.ValueObjects;
using Newtonsoft.Json;

namespace EventSourcing.Serialization.NewtonsoftJson.Converters
{
    internal class EventStreamEventContentConverter : JsonConverter<EventStreamEventContent>
    {
        public override void WriteJson(JsonWriter writer, EventStreamEventContent value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override EventStreamEventContent ReadJson(
            JsonReader reader,
            Type objectType,
            EventStreamEventContent existingValue,
            bool hasExistingValue, 
            JsonSerializer serializer)
        {
            var stringValue = (string) reader.Value;
            return stringValue;
        }
    }
}
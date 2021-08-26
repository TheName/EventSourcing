using System;
using EventSourcing.Abstractions;
using Newtonsoft.Json;

namespace EventSourcing.Serialization.NewtonsoftJson.Converters
{
    internal class EventStreamEventTypeIdentifierConverter : JsonConverter<EventStreamEventTypeIdentifier>
    {
        public override void WriteJson(JsonWriter writer, EventStreamEventTypeIdentifier value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override EventStreamEventTypeIdentifier ReadJson(
            JsonReader reader,
            Type objectType,
            EventStreamEventTypeIdentifier existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            var stringValue = (string) reader.Value;
            return stringValue;
        }
    }
}
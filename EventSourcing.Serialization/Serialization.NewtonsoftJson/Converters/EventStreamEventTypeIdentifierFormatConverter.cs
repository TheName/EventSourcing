using System;
using EventSourcing.Abstractions;
using EventSourcing.Abstractions.ValueObjects;
using Newtonsoft.Json;

namespace EventSourcing.Serialization.NewtonsoftJson.Converters
{
    internal class EventStreamEventTypeIdentifierFormatConverter : JsonConverter<EventStreamEventTypeIdentifierFormat>
    {
        public override void WriteJson(JsonWriter writer, EventStreamEventTypeIdentifierFormat value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override EventStreamEventTypeIdentifierFormat ReadJson(
            JsonReader reader,
            Type objectType,
            EventStreamEventTypeIdentifierFormat existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            var stringValue = (string) reader.Value;
            return stringValue;
        }
    }
}
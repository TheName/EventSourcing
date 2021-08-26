using System;
using EventSourcing.Abstractions;
using Newtonsoft.Json;

namespace EventSourcing.Serialization.NewtonsoftJson.Converters
{
    internal class EventStreamEntrySequenceConverter : JsonConverter<EventStreamEntrySequence>
    {
        public override void WriteJson(JsonWriter writer, EventStreamEntrySequence value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }

        public override EventStreamEntrySequence ReadJson(
            JsonReader reader,
            Type objectType,
            EventStreamEntrySequence existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            var uintValue = (uint) reader.Value;
            return uintValue;
        }
    }
}
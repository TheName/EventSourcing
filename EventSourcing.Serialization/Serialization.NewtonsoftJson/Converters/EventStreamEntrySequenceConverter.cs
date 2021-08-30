using System;
using EventSourcing.Abstractions;
using EventSourcing.Abstractions.ValueObjects;
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
            var uintValue = Convert.ToUInt32(reader.Value);
            return uintValue;
        }
    }
}
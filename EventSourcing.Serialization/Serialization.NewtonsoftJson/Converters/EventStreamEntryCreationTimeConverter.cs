using System;
using System.Globalization;
using EventSourcing.Abstractions;
using Newtonsoft.Json;

namespace EventSourcing.Serialization.NewtonsoftJson.Converters
{
    internal class EventStreamEntryCreationTimeConverter : JsonConverter<EventStreamEntryCreationTime>
    {
        public override void WriteJson(JsonWriter writer, EventStreamEntryCreationTime value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override EventStreamEntryCreationTime ReadJson(
            JsonReader reader,
            Type objectType,
            EventStreamEntryCreationTime existingValue,
            bool hasExistingValue, 
            JsonSerializer serializer)
        {
            var stringValue = (string) reader.Value;
            var dateTime = DateTime.ParseExact(stringValue, "u", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
            return dateTime;
        }
    }
}
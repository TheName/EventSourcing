using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using EventSourcing.Abstractions;
using EventSourcing.Abstractions.ValueObjects;

namespace EventSourcing.Serialization.Json.Converters
{
    internal class EventStreamEntryCreationTimeConverter : JsonConverter<EventStreamEntryCreationTime>
    {
        public override EventStreamEntryCreationTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            return DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
        }

        public override void Write(Utf8JsonWriter writer, EventStreamEntryCreationTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
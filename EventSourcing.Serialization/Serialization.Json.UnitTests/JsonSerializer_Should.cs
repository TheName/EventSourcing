using System.Linq;
using System.Text;
using AutoFixture;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Serialization.Json;
using TestHelpers.Attributes;
using Xunit;

namespace Serialization.Json.UnitTests
{
    public class JsonSerializer_Should
    {
        private readonly JsonSerializer _serializer = new(JsonSerializer.DefaultJsonSerializerOptions);
        
        [Theory]
        [AutoMoqData]
        internal void SerializeEventStreamEntryCorrectly(EventStreamEntry entry)
        {
            var result = _serializer.Serialize(entry);

            var expectedString = GetExpectedSerializedString(entry);
            Assert.Equal(expectedString, result);
        }
        
        [Theory]
        [AutoMoqData]
        internal void SerializeEventStreamEntryToUtf8BytesCorrectly(EventStreamEntry entry)
        {
            var result = _serializer.SerializeToUtf8Bytes(entry);

            var expectedString = GetExpectedSerializedString(entry);
            var expectedBytes = Encoding.UTF8.GetBytes(expectedString);
            
            Assert.True(expectedBytes.SequenceEqual(result));
        }
        
        [Theory]
        [AutoMoqData]
        internal void DeserializeEventStreamEntryCorrectly(EventStreamEntry entry)
        {
            var serializedString = GetExpectedSerializedString(entry);

            var result = _serializer.Deserialize(serializedString, typeof(EventStreamEntry));

            Assert.Equal(entry, result);
        }
        
        [Theory]
        [AutoMoqData]
        internal void DeserializeEventStreamEntryFromUtf8BytesCorrectly(EventStreamEntry entry)
        {
            var serializedString = GetExpectedSerializedString(entry);
            var serializedBytes = Encoding.UTF8.GetBytes(serializedString);

            var result = _serializer.DeserializeFromUtf8Bytes(serializedBytes, typeof(EventStreamEntry));

            Assert.Equal(entry, result);
        }
        
        [Theory]
        [AutoMoqData]
        internal void SerializeClassWithEnumsCorrectly(IFixture fixture)
        {
            var objectToSerialize = fixture.Create<ClassWithEnums>();
            var result = _serializer.Serialize(objectToSerialize);

            var expectedString = GetExpectedSerializedString(objectToSerialize);
            Assert.Equal(expectedString, result);
        }
        
        [Theory]
        [AutoMoqData]
        internal void SerializeClassWithEnumsToUtf8BytesCorrectly(IFixture fixture)
        {
            var objectToSerialize = fixture.Create<ClassWithEnums>();
            var result = _serializer.SerializeToUtf8Bytes(objectToSerialize);

            var expectedString = GetExpectedSerializedString(objectToSerialize);
            var expectedBytes = Encoding.UTF8.GetBytes(expectedString);
            
            Assert.True(expectedBytes.SequenceEqual(result));
        }
        
        [Theory]
        [AutoMoqData]
        internal void DeserializeClassWithEnumsCorrectly(IFixture fixture)
        {
            var objectToSerialize = fixture.Create<ClassWithEnums>();
            var serializedString = GetExpectedSerializedString(objectToSerialize);

            var result = _serializer.Deserialize(serializedString, typeof(ClassWithEnums));

            var resultAsClassWithEnums = Assert.IsType<ClassWithEnums>(result);
            Assert.Equal(objectToSerialize.StringValue, resultAsClassWithEnums.StringValue);
            Assert.Equal(objectToSerialize.IntValue, resultAsClassWithEnums.IntValue);
            Assert.Equal(objectToSerialize.BoolValue, resultAsClassWithEnums.BoolValue);
            Assert.Equal(objectToSerialize.EnumValue, resultAsClassWithEnums.EnumValue);
        }
        
        [Theory]
        [AutoMoqData]
        internal void DeserializeClassWithEnumsFromUtf8BytesCorrectly(IFixture fixture)
        {
            var objectToSerialize = fixture.Create<ClassWithEnums>();
            var serializedString = GetExpectedSerializedString(objectToSerialize);
            var serializedBytes = Encoding.UTF8.GetBytes(serializedString);

            var result = _serializer.DeserializeFromUtf8Bytes(serializedBytes, typeof(ClassWithEnums));

            var resultAsClassWithEnums = Assert.IsType<ClassWithEnums>(result);
            Assert.Equal(objectToSerialize.StringValue, resultAsClassWithEnums.StringValue);
            Assert.Equal(objectToSerialize.IntValue, resultAsClassWithEnums.IntValue);
            Assert.Equal(objectToSerialize.BoolValue, resultAsClassWithEnums.BoolValue);
            Assert.Equal(objectToSerialize.EnumValue, resultAsClassWithEnums.EnumValue);
        }

        private static string GetExpectedSerializedString(EventStreamEntry entry)
        {
            return
                $"{{\"StreamId\":{{\"Value\":\"{entry.StreamId}\"}},\"EntryId\":{{\"Value\":\"{entry.EntryId}\"}},\"EntrySequence\":{{\"Value\":{entry.EntrySequence}}},\"EventDescriptor\":{{\"EventContent\":{{\"Value\":\"{entry.EventDescriptor.EventContent}\"}},\"EventContentSerializationFormat\":{{\"Value\":\"{entry.EventDescriptor.EventContentSerializationFormat}\"}},\"EventTypeIdentifier\":{{\"Value\":\"{entry.EventDescriptor.EventTypeIdentifier}\"}},\"EventTypeIdentifierFormat\":{{\"Value\":\"{entry.EventDescriptor.EventTypeIdentifierFormat}\"}}}},\"CausationId\":{{\"Value\":\"{entry.CausationId}\"}},\"CreationTime\":{{\"Value\":\"{entry.CreationTime}\"}},\"CorrelationId\":{{\"Value\":\"{entry.CorrelationId}\"}}}}";
        }

        private static string GetExpectedSerializedString(ClassWithEnums classWithEnums)
        {
            return
                $"{{\"StringValue\":\"{classWithEnums.StringValue}\",\"IntValue\":{classWithEnums.IntValue},\"BoolValue\":{classWithEnums.BoolValue.ToString().ToLower()},\"EnumValue\":\"{classWithEnums.EnumValue.ToString()}\"}}";
        }
        
        private class ClassWithEnums
        {
            public string StringValue { get; set; }
            public int IntValue { get; set; }
            public bool BoolValue { get; set; }
            public SomeEnum EnumValue { get; set; }
        }
        
        private enum SomeEnum
        {
            Undefined,
            FirstLegalValue,
            SecondLegalValue
        }
    }
}
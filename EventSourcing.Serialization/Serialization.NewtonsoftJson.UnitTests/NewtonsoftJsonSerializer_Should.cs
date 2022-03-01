﻿using System.Linq;
using System.Text;
using AutoFixture;
using EventSourcing.Abstractions;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.Serialization.NewtonsoftJson;
using TestHelpers.Attributes;
using Xunit;

namespace Serialization.NewtonsoftJson.UnitTests
{
    public class NewtonsoftJsonSerializer_Should
    {
        [Theory]
        [AutoMoqData]
        internal void SerializeEventStreamEntryCorrectly(EventStreamEntry entry, NewtonsoftJsonSerializer serializer)
        {
            var result = serializer.Serialize(entry);

            var expectedString = GetExpectedSerializedString(entry);
            Assert.Equal(expectedString, result);
        }
        
        [Theory]
        [AutoMoqData]
        internal void SerializeEventStreamEntryToUtf8BytesCorrectly(EventStreamEntry entry, NewtonsoftJsonSerializer serializer)
        {
            var result = serializer.SerializeToUtf8Bytes(entry);

            var expectedString = GetExpectedSerializedString(entry);
            var expectedBytes = Encoding.UTF8.GetBytes(expectedString);
            
            Assert.True(expectedBytes.SequenceEqual(result));
        }
        
        [Theory]
        [AutoMoqData]
        internal void DeserializeEventStreamEntryCorrectly(EventStreamEntry entry, NewtonsoftJsonSerializer serializer)
        {
            var serializedString = GetExpectedSerializedString(entry);

            var result = serializer.Deserialize(serializedString, typeof(EventStreamEntry));

            Assert.Equal(entry, result);
        }
        
        [Theory]
        [AutoMoqData]
        internal void DeserializeEventStreamEntryFromUtf8BytesCorrectly(EventStreamEntry entry, NewtonsoftJsonSerializer serializer)
        {
            var serializedString = GetExpectedSerializedString(entry);
            var serializedBytes = Encoding.UTF8.GetBytes(serializedString);

            var result = serializer.DeserializeFromUtf8Bytes(serializedBytes, typeof(EventStreamEntry));

            Assert.Equal(entry, result);
        }
        
        [Theory]
        [AutoMoqData]
        internal void SerializeClassWithEnumsCorrectly(IFixture fixture, NewtonsoftJsonSerializer serializer)
        {
            var objectToSerialize = fixture.Create<ClassWithEnums>();
            var result = serializer.Serialize(objectToSerialize);

            var expectedString = GetExpectedSerializedString(objectToSerialize);
            Assert.Equal(expectedString, result);
        }
        
        [Theory]
        [AutoMoqData]
        internal void SerializeClassWithEnumsToUtf8BytesCorrectly(IFixture fixture, NewtonsoftJsonSerializer serializer)
        {
            var objectToSerialize = fixture.Create<ClassWithEnums>();
            var result = serializer.SerializeToUtf8Bytes(objectToSerialize);

            var expectedString = GetExpectedSerializedString(objectToSerialize);
            var expectedBytes = Encoding.UTF8.GetBytes(expectedString);
            
            Assert.True(expectedBytes.SequenceEqual(result));
        }
        
        [Theory]
        [AutoMoqData]
        internal void DeserializeClassWithEnumsCorrectly(IFixture fixture, NewtonsoftJsonSerializer serializer)
        {
            var objectToSerialize = fixture.Create<ClassWithEnums>();
            var serializedString = GetExpectedSerializedString(objectToSerialize);

            var result = serializer.Deserialize(serializedString, typeof(ClassWithEnums));

            var resultAsClassWithEnums = Assert.IsType<ClassWithEnums>(result);
            Assert.Equal(objectToSerialize.StringValue, resultAsClassWithEnums.StringValue);
            Assert.Equal(objectToSerialize.IntValue, resultAsClassWithEnums.IntValue);
            Assert.Equal(objectToSerialize.BoolValue, resultAsClassWithEnums.BoolValue);
            Assert.Equal(objectToSerialize.EnumValue, resultAsClassWithEnums.EnumValue);
        }
        
        [Theory]
        [AutoMoqData]
        internal void DeserializeClassWithEnumsFromUtf8BytesCorrectly(IFixture fixture, NewtonsoftJsonSerializer serializer)
        {
            var objectToSerialize = fixture.Create<ClassWithEnums>();
            var serializedString = GetExpectedSerializedString(objectToSerialize);
            var serializedBytes = Encoding.UTF8.GetBytes(serializedString);

            var result = serializer.DeserializeFromUtf8Bytes(serializedBytes, typeof(ClassWithEnums));

            var resultAsClassWithEnums = Assert.IsType<ClassWithEnums>(result);
            Assert.Equal(objectToSerialize.StringValue, resultAsClassWithEnums.StringValue);
            Assert.Equal(objectToSerialize.IntValue, resultAsClassWithEnums.IntValue);
            Assert.Equal(objectToSerialize.BoolValue, resultAsClassWithEnums.BoolValue);
            Assert.Equal(objectToSerialize.EnumValue, resultAsClassWithEnums.EnumValue);
        }

        private static string GetExpectedSerializedString(EventStreamEntry entry)
        {
            return
                $"{{\"StreamId\":\"{entry.StreamId}\",\"EntryId\":\"{entry.EntryId}\",\"EntrySequence\":{entry.EntrySequence},\"EventDescriptor\":{{\"EventContent\":\"{entry.EventDescriptor.EventContent}\",\"EventContentSerializationFormat\":\"{entry.EventDescriptor.EventContentSerializationFormat}\",\"EventTypeIdentifier\":\"{entry.EventDescriptor.EventTypeIdentifier}\"}},\"CausationId\":\"{entry.CausationId}\",\"CreationTime\":\"{entry.CreationTime}\",\"CorrelationId\":\"{entry.CorrelationId}\"}}";
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
using System.Linq;
using System.Text;
using AutoFixture;
using EventSourcing.Abstractions;
using EventSourcing.Serialization.NewtonsoftJson;
using TestHelpers.Attributes;
using Xunit;

namespace Serialization.NewtonsoftJson.UnitTests
{
    public class NewtonsoftJsonSerializer_Should
    {
        [Theory]
        [AutoMoqData]
        internal void SerializeEventStreamEntryToUtf8BytesCorrectly(EventStreamEntry entry, NewtonsoftJsonSerializer serializer)
        {
            var result = serializer.SerializeToUtf8Bytes(entry);

            var expectedString =
                $"{{\"StreamId\":\"{entry.StreamId}\",\"EntryId\":\"{entry.EntryId}\",\"EntrySequence\":{entry.EntrySequence},\"EventDescriptor\":{{\"EventContent\":\"{entry.EventDescriptor.EventContent}\",\"EventTypeIdentifier\":\"{entry.EventDescriptor.EventTypeIdentifier}\"}},\"EntryMetadata\":{{\"CausationId\":\"{entry.EntryMetadata.CausationId}\",\"CreationTime\":\"{entry.EntryMetadata.CreationTime}\",\"CorrelationId\":\"{entry.EntryMetadata.CorrelationId}\"}}}}";
            var expectedBytes = Encoding.UTF8.GetBytes(expectedString);
            
            Assert.True(expectedBytes.SequenceEqual(result));
        }
        
        [Theory]
        [AutoMoqData]
        internal void SerializeClassWithEnumsToUtf8BytesCorrectly(IFixture fixture, NewtonsoftJsonSerializer serializer)
        {
            var objectToSerialize = fixture.Create<ClassWithEnums>();
            var result = serializer.SerializeToUtf8Bytes(objectToSerialize);

            var expectedString =
                $"{{\"StringValue\":\"{objectToSerialize.StringValue}\",\"IntValue\":{objectToSerialize.IntValue},\"BoolValue\":{objectToSerialize.BoolValue.ToString().ToLower()},\"EnumValue\":\"{objectToSerialize.EnumValue.ToString()}\"}}";
            var expectedBytes = Encoding.UTF8.GetBytes(expectedString);
            
            Assert.True(expectedBytes.SequenceEqual(result));
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
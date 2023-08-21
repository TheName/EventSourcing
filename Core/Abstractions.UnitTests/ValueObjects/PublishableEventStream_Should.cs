using System;
using System.Collections.Generic;
using System.Text;
using EventSourcing;
using EventSourcing.ValueObjects;
using TestHelpers.Attributes;
using Xunit;

namespace Abstractions.UnitTests.ValueObjects
{
    public class PublishableEventStream_Should
    {
        [Fact]
        public void Throw_ArgumentNullException_When_CreatingWithNullAppendableEventStream()
        {
            Assert.Throws<ArgumentNullException>(() => new PublishableEventStream(null));
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingWithNonNullParameters(AppendableEventStream appendableEventStream)
        {
            _ = new PublishableEventStream(appendableEventStream);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnStreamIdProvidedDuringCreation_When_GettingStreamId(AppendableEventStream appendableEventStream)
        {
            var stream = new PublishableEventStream(appendableEventStream);

            Assert.Equal(appendableEventStream.StreamId, stream.StreamId);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnEventsWithMetadataProvidedDuringCreation_When_GettingEventsWithMetadata(AppendableEventStream appendableEventStream)
        {
            var stream = new PublishableEventStream(appendableEventStream);

            Assert.NotSame(appendableEventStream.EventsWithMetadata, stream.EventsWithMetadata);
            Assert.Null(stream.EventsWithMetadata as List<EventStreamEventWithMetadata>);
            Assert.False(ReferenceEquals(appendableEventStream.EventsWithMetadata, stream.EventsWithMetadata));
            Assert.Equal(appendableEventStream.EventsWithMetadata, stream.EventsWithMetadata);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnEventsWithMetadataToPublishProvidedDuringCreationAsEventsToAppend_When_GettingEventsWithMetadataToPublish(AppendableEventStream appendableEventStream)
        {
            var stream = new PublishableEventStream(appendableEventStream);

            Assert.NotSame(appendableEventStream.EventsWithMetadataToAppend, stream.EventsWithMetadataToPublish);
            Assert.Null(stream.EventsWithMetadataToPublish as List<EventStreamEventWithMetadata>);
            Assert.False(ReferenceEquals(appendableEventStream.EventsWithMetadataToAppend, stream.EventsWithMetadataToPublish));
            Assert.Equal(appendableEventStream.EventsWithMetadataToAppend, stream.EventsWithMetadataToPublish);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnTrue_When_ComparingDifferentObjectsWithSameAppendableEventStream(AppendableEventStream appendableEventStream)
        {
            var stream1 = new PublishableEventStream(appendableEventStream);
            var stream2 = new PublishableEventStream(appendableEventStream);

            Assert.Equal(stream1, stream2);
            Assert.True(stream1 == stream2);
            Assert.False(stream1 != stream2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentAppendableEventStream(
            AppendableEventStream appendableEventStream1,
            AppendableEventStream appendableEventStream2)
        {
            var stream1 = new PublishableEventStream(appendableEventStream1);
            var stream2 = new PublishableEventStream(appendableEventStream2);

            Assert.NotEqual(stream1.GetHashCode(), stream2.GetHashCode());
            Assert.NotEqual(stream1, stream2);
            Assert.True(stream1 != stream2);
            Assert.False(stream1 == stream2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnExpectedValue_When_CallingToString(PublishableEventStream publishableEventStream)
        {
            var expectedValue =
                $"Event Stream ID: {publishableEventStream.StreamId}, {EventsWithMetadataString()}, {EventsWithMetadataToPublishString()}";

            string EventsWithMetadataString()
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.Append("EventsWithMetadata: ");
                foreach (var eventWithMetadata in publishableEventStream.EventsWithMetadata)
                {
                    stringBuilder.Append($"\n\t{eventWithMetadata}");
                }

                return stringBuilder.ToString();
            }

            string EventsWithMetadataToPublishString()
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.Append("EventsWithMetadata to publish: ");
                foreach (var eventWithMetadata in publishableEventStream.EventsWithMetadataToPublish)
                {
                    stringBuilder.Append($"\n\t{eventWithMetadata}");
                }

                return stringBuilder.ToString();
            }

            Assert.Equal(expectedValue, publishableEventStream.ToString());
        }
    }
}

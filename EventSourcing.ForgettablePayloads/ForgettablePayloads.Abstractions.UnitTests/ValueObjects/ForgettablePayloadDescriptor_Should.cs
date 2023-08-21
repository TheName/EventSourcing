using System;
using EventSourcing.ForgettablePayloads.ValueObjects;
using EventSourcing.ValueObjects;
using TestHelpers.Attributes;
using Xunit;

namespace ForgettablePayloads.Abstractions.UnitTests.ValueObjects
{
    public class ForgettablePayloadDescriptor_Should
    {
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullEventStreamId(
            EventStreamEntryId eventStreamEntryId,
            ForgettablePayloadId payloadId,
            ForgettablePayloadState payloadState,
            ForgettablePayloadCreationTime payloadCreationTime,
            ForgettablePayloadLastModifiedTime payloadLastModifiedTime,
            ForgettablePayloadSequence payloadSequence,
            ForgettablePayloadContent payloadContent,
            SerializationFormat payloadContentSerializationFormat,
            ForgettablePayloadTypeIdentifier payloadTypeIdentifier,
            ForgettablePayloadTypeIdentifierFormat payloadTypeIdentifierFormat)
        {
            Assert.Throws<ArgumentNullException>(() => new ForgettablePayloadDescriptor(
                null,
                eventStreamEntryId,
                payloadId,
                payloadState,
                payloadCreationTime,
                payloadLastModifiedTime,
                payloadSequence,
                payloadContent,
                payloadContentSerializationFormat,
                payloadTypeIdentifier,
                payloadTypeIdentifierFormat));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullEventStreamEntryId(
            EventStreamId eventStreamId,
            ForgettablePayloadId payloadId,
            ForgettablePayloadState payloadState,
            ForgettablePayloadCreationTime payloadCreationTime,
            ForgettablePayloadLastModifiedTime payloadLastModifiedTime,
            ForgettablePayloadSequence payloadSequence,
            ForgettablePayloadContent payloadContent,
            SerializationFormat payloadContentSerializationFormat,
            ForgettablePayloadTypeIdentifier payloadTypeIdentifier,
            ForgettablePayloadTypeIdentifierFormat payloadTypeIdentifierFormat)
        {
            Assert.Throws<ArgumentNullException>(() => new ForgettablePayloadDescriptor(
                eventStreamId,
                null,
                payloadId,
                payloadState,
                payloadCreationTime,
                payloadLastModifiedTime,
                payloadSequence,
                payloadContent,
                payloadContentSerializationFormat,
                payloadTypeIdentifier,
                payloadTypeIdentifierFormat));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullForgettablePayloadId(
            EventStreamId eventStreamId,
            EventStreamEntryId eventStreamEntryId,
            ForgettablePayloadState payloadState,
            ForgettablePayloadCreationTime payloadCreationTime,
            ForgettablePayloadLastModifiedTime payloadLastModifiedTime,
            ForgettablePayloadSequence payloadSequence,
            ForgettablePayloadContent payloadContent,
            SerializationFormat payloadContentSerializationFormat,
            ForgettablePayloadTypeIdentifier payloadTypeIdentifier,
            ForgettablePayloadTypeIdentifierFormat payloadTypeIdentifierFormat)
        {
            Assert.Throws<ArgumentNullException>(() => new ForgettablePayloadDescriptor(
                eventStreamId,
                eventStreamEntryId,
                null,
                payloadState,
                payloadCreationTime,
                payloadLastModifiedTime,
                payloadSequence,
                payloadContent,
                payloadContentSerializationFormat,
                payloadTypeIdentifier,
                payloadTypeIdentifierFormat));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullForgettablePayloadState(
            EventStreamId eventStreamId,
            EventStreamEntryId eventStreamEntryId,
            ForgettablePayloadId payloadId,
            ForgettablePayloadCreationTime payloadCreationTime,
            ForgettablePayloadLastModifiedTime payloadLastModifiedTime,
            ForgettablePayloadSequence payloadSequence,
            ForgettablePayloadContent payloadContent,
            SerializationFormat payloadContentSerializationFormat,
            ForgettablePayloadTypeIdentifier payloadTypeIdentifier,
            ForgettablePayloadTypeIdentifierFormat payloadTypeIdentifierFormat)
        {
            Assert.Throws<ArgumentNullException>(() => new ForgettablePayloadDescriptor(
                eventStreamId,
                eventStreamEntryId,
                payloadId,
                null,
                payloadCreationTime,
                payloadLastModifiedTime,
                payloadSequence,
                payloadContent,
                payloadContentSerializationFormat,
                payloadTypeIdentifier,
                payloadTypeIdentifierFormat));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullForgettablePayloadCreationTime(
            EventStreamId eventStreamId,
            EventStreamEntryId eventStreamEntryId,
            ForgettablePayloadId payloadId,
            ForgettablePayloadState payloadState,
            ForgettablePayloadLastModifiedTime payloadLastModifiedTime,
            ForgettablePayloadSequence payloadSequence,
            ForgettablePayloadContent payloadContent,
            SerializationFormat payloadContentSerializationFormat,
            ForgettablePayloadTypeIdentifier payloadTypeIdentifier,
            ForgettablePayloadTypeIdentifierFormat payloadTypeIdentifierFormat)
        {
            Assert.Throws<ArgumentNullException>(() => new ForgettablePayloadDescriptor(
                eventStreamId,
                eventStreamEntryId,
                payloadId,
                payloadState,
                null,
                payloadLastModifiedTime,
                payloadSequence,
                payloadContent,
                payloadContentSerializationFormat,
                payloadTypeIdentifier,
                payloadTypeIdentifierFormat));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullForgettablePayloadLastModifiedTime(
            EventStreamId eventStreamId,
            EventStreamEntryId eventStreamEntryId,
            ForgettablePayloadId payloadId,
            ForgettablePayloadState payloadState,
            ForgettablePayloadCreationTime payloadCreationTime,
            ForgettablePayloadSequence payloadSequence,
            ForgettablePayloadContent payloadContent,
            SerializationFormat payloadContentSerializationFormat,
            ForgettablePayloadTypeIdentifier payloadTypeIdentifier,
            ForgettablePayloadTypeIdentifierFormat payloadTypeIdentifierFormat)
        {
            Assert.Throws<ArgumentNullException>(() => new ForgettablePayloadDescriptor(
                eventStreamId,
                eventStreamEntryId,
                payloadId,
                payloadState,
                payloadCreationTime,
                null,
                payloadSequence,
                payloadContent,
                payloadContentSerializationFormat,
                payloadTypeIdentifier,
                payloadTypeIdentifierFormat));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullForgettablePayloadSequence(
            EventStreamId eventStreamId,
            EventStreamEntryId eventStreamEntryId,
            ForgettablePayloadId payloadId,
            ForgettablePayloadState payloadState,
            ForgettablePayloadCreationTime payloadCreationTime,
            ForgettablePayloadLastModifiedTime payloadLastModifiedTime,
            ForgettablePayloadContent payloadContent,
            SerializationFormat payloadContentSerializationFormat,
            ForgettablePayloadTypeIdentifier payloadTypeIdentifier,
            ForgettablePayloadTypeIdentifierFormat payloadTypeIdentifierFormat)
        {
            Assert.Throws<ArgumentNullException>(() => new ForgettablePayloadDescriptor(
                eventStreamId,
                eventStreamEntryId,
                payloadId,
                payloadState,
                payloadCreationTime,
                payloadLastModifiedTime,
                null,
                payloadContent,
                payloadContentSerializationFormat,
                payloadTypeIdentifier,
                payloadTypeIdentifierFormat));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullForgettablePayloadContent(
            EventStreamId eventStreamId,
            EventStreamEntryId eventStreamEntryId,
            ForgettablePayloadId payloadId,
            ForgettablePayloadState payloadState,
            ForgettablePayloadCreationTime payloadCreationTime,
            ForgettablePayloadLastModifiedTime payloadLastModifiedTime,
            ForgettablePayloadSequence payloadSequence,
            SerializationFormat payloadContentSerializationFormat,
            ForgettablePayloadTypeIdentifier payloadTypeIdentifier,
            ForgettablePayloadTypeIdentifierFormat payloadTypeIdentifierFormat)
        {
            Assert.Throws<ArgumentNullException>(() => new ForgettablePayloadDescriptor(
                eventStreamId,
                eventStreamEntryId,
                payloadId,
                payloadState,
                payloadCreationTime,
                payloadLastModifiedTime,
                payloadSequence,
                null,
                payloadContentSerializationFormat,
                payloadTypeIdentifier,
                payloadTypeIdentifierFormat));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullSerializationFormat(
            EventStreamId eventStreamId,
            EventStreamEntryId eventStreamEntryId,
            ForgettablePayloadId payloadId,
            ForgettablePayloadState payloadState,
            ForgettablePayloadCreationTime payloadCreationTime,
            ForgettablePayloadLastModifiedTime payloadLastModifiedTime,
            ForgettablePayloadSequence payloadSequence,
            ForgettablePayloadContent payloadContent,
            ForgettablePayloadTypeIdentifier payloadTypeIdentifier,
            ForgettablePayloadTypeIdentifierFormat payloadTypeIdentifierFormat)
        {
            Assert.Throws<ArgumentNullException>(() => new ForgettablePayloadDescriptor(
                eventStreamId,
                eventStreamEntryId,
                payloadId,
                payloadState,
                payloadCreationTime,
                payloadLastModifiedTime,
                payloadSequence,
                payloadContent,
                null,
                payloadTypeIdentifier,
                payloadTypeIdentifierFormat));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullForgettablePayloadTypeIdentifier(
            EventStreamId eventStreamId,
            EventStreamEntryId eventStreamEntryId,
            ForgettablePayloadId payloadId,
            ForgettablePayloadState payloadState,
            ForgettablePayloadCreationTime payloadCreationTime,
            ForgettablePayloadLastModifiedTime payloadLastModifiedTime,
            ForgettablePayloadSequence payloadSequence,
            ForgettablePayloadContent payloadContent,
            SerializationFormat payloadContentSerializationFormat,
            ForgettablePayloadTypeIdentifierFormat payloadTypeIdentifierFormat)
        {
            Assert.Throws<ArgumentNullException>(() => new ForgettablePayloadDescriptor(
                eventStreamId,
                eventStreamEntryId,
                payloadId,
                payloadState,
                payloadCreationTime,
                payloadLastModifiedTime,
                payloadSequence,
                payloadContent,
                payloadContentSerializationFormat,
                null,
                payloadTypeIdentifierFormat));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingWithNullForgettablePayloadTypeIdentifierFormat(
            EventStreamId eventStreamId,
            EventStreamEntryId eventStreamEntryId,
            ForgettablePayloadId payloadId,
            ForgettablePayloadState payloadState,
            ForgettablePayloadCreationTime payloadCreationTime,
            ForgettablePayloadLastModifiedTime payloadLastModifiedTime,
            ForgettablePayloadSequence payloadSequence,
            ForgettablePayloadContent payloadContent,
            SerializationFormat payloadContentSerializationFormat,
            ForgettablePayloadTypeIdentifier payloadTypeIdentifier)
        {
            Assert.Throws<ArgumentNullException>(() => new ForgettablePayloadDescriptor(
                eventStreamId,
                eventStreamEntryId,
                payloadId,
                payloadState,
                payloadCreationTime,
                payloadLastModifiedTime,
                payloadSequence,
                payloadContent,
                payloadContentSerializationFormat,
                payloadTypeIdentifier,
                null));
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingWithNonNullValues(
            EventStreamId eventStreamId,
            EventStreamEntryId eventStreamEntryId,
            ForgettablePayloadId payloadId,
            ForgettablePayloadState payloadState,
            ForgettablePayloadCreationTime payloadCreationTime,
            ForgettablePayloadLastModifiedTime payloadLastModifiedTime,
            ForgettablePayloadSequence payloadSequence,
            ForgettablePayloadContent payloadContent,
            SerializationFormat payloadContentSerializationFormat,
            ForgettablePayloadTypeIdentifier payloadTypeIdentifier,
            ForgettablePayloadTypeIdentifierFormat payloadTypeIdentifierFormat)
        {
            _ = new ForgettablePayloadDescriptor(
                eventStreamId,
                eventStreamEntryId,
                payloadId,
                payloadState,
                payloadCreationTime,
                payloadLastModifiedTime,
                payloadSequence,
                payloadContent,
                payloadContentSerializationFormat,
                payloadTypeIdentifier,
                payloadTypeIdentifierFormat);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnValuesProvidedDuringCreation_When_GettingPropertiesValues(
            EventStreamId eventStreamId,
            EventStreamEntryId eventStreamEntryId,
            ForgettablePayloadId payloadId,
            ForgettablePayloadState payloadState,
            ForgettablePayloadCreationTime payloadCreationTime,
            ForgettablePayloadLastModifiedTime payloadLastModifiedTime,
            ForgettablePayloadSequence payloadSequence,
            ForgettablePayloadContent payloadContent,
            SerializationFormat payloadContentSerializationFormat,
            ForgettablePayloadTypeIdentifier payloadTypeIdentifier,
            ForgettablePayloadTypeIdentifierFormat payloadTypeIdentifierFormat)
        {
            var payloadDescriptor = new ForgettablePayloadDescriptor(
                eventStreamId,
                eventStreamEntryId,
                payloadId,
                payloadState,
                payloadCreationTime,
                payloadLastModifiedTime,
                payloadSequence,
                payloadContent,
                payloadContentSerializationFormat,
                payloadTypeIdentifier,
                payloadTypeIdentifierFormat);

            Assert.Equal(eventStreamId, payloadDescriptor.EventStreamId);
            Assert.Equal(eventStreamEntryId, payloadDescriptor.EventStreamEntryId);
            Assert.Equal(payloadId, payloadDescriptor.PayloadId);
            Assert.Equal(payloadState, payloadDescriptor.PayloadState);
            Assert.Equal(payloadCreationTime, payloadDescriptor.PayloadCreationTime);
            Assert.Equal(payloadLastModifiedTime, payloadDescriptor.PayloadLastModifiedTime);
            Assert.Equal(payloadSequence, payloadDescriptor.PayloadSequence);
            Assert.Equal(payloadContent, payloadDescriptor.PayloadContent);
            Assert.Equal(payloadContentSerializationFormat, payloadDescriptor.PayloadContentSerializationFormat);
            Assert.Equal(payloadTypeIdentifier, payloadDescriptor.PayloadTypeIdentifier);
            Assert.Equal(payloadTypeIdentifierFormat, payloadDescriptor.PayloadTypeIdentifierFormat);
        }

        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingFromMetadataAndContentDescriptorWithNullMetadata(
            ForgettablePayloadContentDescriptor contentDescriptor)
        {
            Assert.Throws<ArgumentNullException>(() =>
                ForgettablePayloadDescriptor.CreateFromMetadataAndContentDescriptor(
                    null,
                    contentDescriptor));
        }

        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_CreatingFromMetadataAndContentDescriptorWithNullContentDescriptor(
            ForgettablePayloadMetadata metadata)
        {
            Assert.Throws<ArgumentNullException>(() =>
                ForgettablePayloadDescriptor.CreateFromMetadataAndContentDescriptor(
                    metadata,
                    null));
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingFromMetadataAndContentDescriptorWithNonNullValues(
            ForgettablePayloadMetadata metadata,
            ForgettablePayloadContentDescriptor contentDescriptor)
        {
            _ = ForgettablePayloadDescriptor.CreateFromMetadataAndContentDescriptor(
                metadata,
                contentDescriptor);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnValuesProvidedDuringCreationFromMetadataAndContentDescriptor_When_GettingPropertiesValues(
            ForgettablePayloadMetadata metadata,
            ForgettablePayloadContentDescriptor contentDescriptor)
        {
            var payloadDescriptor = ForgettablePayloadDescriptor.CreateFromMetadataAndContentDescriptor(
                metadata,
                contentDescriptor);

            Assert.Equal(metadata.EventStreamId, payloadDescriptor.EventStreamId);
            Assert.Equal(metadata.EventStreamEntryId, payloadDescriptor.EventStreamEntryId);
            Assert.Equal(metadata.PayloadId, payloadDescriptor.PayloadId);
            Assert.Equal(metadata.PayloadState, payloadDescriptor.PayloadState);
            Assert.Equal(metadata.PayloadCreationTime, payloadDescriptor.PayloadCreationTime);
            Assert.Equal(metadata.PayloadLastModifiedTime, payloadDescriptor.PayloadLastModifiedTime);
            Assert.Equal(metadata.PayloadSequence, payloadDescriptor.PayloadSequence);
            Assert.Equal(contentDescriptor.PayloadContent, payloadDescriptor.PayloadContent);
            Assert.Equal(contentDescriptor.PayloadContentSerializationFormat, payloadDescriptor.PayloadContentSerializationFormat);
            Assert.Equal(contentDescriptor.PayloadTypeIdentifier, payloadDescriptor.PayloadTypeIdentifier);
            Assert.Equal(contentDescriptor.PayloadTypeIdentifierFormat, payloadDescriptor.PayloadTypeIdentifierFormat);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnTrue_When_ComparingDifferentObjectsWithSameValues(
            EventStreamId eventStreamId,
            EventStreamEntryId eventStreamEntryId,
            ForgettablePayloadId payloadId,
            ForgettablePayloadState payloadState,
            ForgettablePayloadCreationTime payloadCreationTime,
            ForgettablePayloadLastModifiedTime payloadLastModifiedTime,
            ForgettablePayloadSequence payloadSequence,
            ForgettablePayloadContent payloadContent,
            SerializationFormat payloadContentSerializationFormat,
            ForgettablePayloadTypeIdentifier payloadTypeIdentifier,
            ForgettablePayloadTypeIdentifierFormat payloadTypeIdentifierFormat)
        {
            var descriptor1 = new ForgettablePayloadDescriptor(
                eventStreamId,
                eventStreamEntryId,
                payloadId,
                payloadState,
                payloadCreationTime,
                payloadLastModifiedTime,
                payloadSequence,
                payloadContent,
                payloadContentSerializationFormat,
                payloadTypeIdentifier,
                payloadTypeIdentifierFormat);

            var descriptor2 = new ForgettablePayloadDescriptor(
                eventStreamId,
                eventStreamEntryId,
                payloadId,
                payloadState,
                payloadCreationTime,
                payloadLastModifiedTime,
                payloadSequence,
                payloadContent,
                payloadContentSerializationFormat,
                payloadTypeIdentifier,
                payloadTypeIdentifierFormat);

            Assert.Equal(descriptor1, descriptor2);
            Assert.True(descriptor1 == descriptor2);
            Assert.False(descriptor1 != descriptor2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentEventStreamId(
            ForgettablePayloadDescriptor descriptor,
            EventStreamId differentEventStreamId)
        {
            var descriptor1 = new ForgettablePayloadDescriptor(
                descriptor.EventStreamId,
                descriptor.EventStreamEntryId,
                descriptor.PayloadId,
                descriptor.PayloadState,
                descriptor.PayloadCreationTime,
                descriptor.PayloadLastModifiedTime,
                descriptor.PayloadSequence,
                descriptor.PayloadContent,
                descriptor.PayloadContentSerializationFormat,
                descriptor.PayloadTypeIdentifier,
                descriptor.PayloadTypeIdentifierFormat);

            var descriptor2 = new ForgettablePayloadDescriptor(
                differentEventStreamId,
                descriptor.EventStreamEntryId,
                descriptor.PayloadId,
                descriptor.PayloadState,
                descriptor.PayloadCreationTime,
                descriptor.PayloadLastModifiedTime,
                descriptor.PayloadSequence,
                descriptor.PayloadContent,
                descriptor.PayloadContentSerializationFormat,
                descriptor.PayloadTypeIdentifier,
                descriptor.PayloadTypeIdentifierFormat);

            Assert.NotEqual(descriptor1, descriptor2);
            Assert.False(descriptor1 == descriptor2);
            Assert.True(descriptor1 != descriptor2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentEventStreamEntryId(
            ForgettablePayloadDescriptor descriptor,
            EventStreamEntryId differentEventStreamEntryId)
        {
            var descriptor1 = new ForgettablePayloadDescriptor(
                descriptor.EventStreamId,
                descriptor.EventStreamEntryId,
                descriptor.PayloadId,
                descriptor.PayloadState,
                descriptor.PayloadCreationTime,
                descriptor.PayloadLastModifiedTime,
                descriptor.PayloadSequence,
                descriptor.PayloadContent,
                descriptor.PayloadContentSerializationFormat,
                descriptor.PayloadTypeIdentifier,
                descriptor.PayloadTypeIdentifierFormat);

            var descriptor2 = new ForgettablePayloadDescriptor(
                descriptor.EventStreamId,
                differentEventStreamEntryId,
                descriptor.PayloadId,
                descriptor.PayloadState,
                descriptor.PayloadCreationTime,
                descriptor.PayloadLastModifiedTime,
                descriptor.PayloadSequence,
                descriptor.PayloadContent,
                descriptor.PayloadContentSerializationFormat,
                descriptor.PayloadTypeIdentifier,
                descriptor.PayloadTypeIdentifierFormat);

            Assert.NotEqual(descriptor1, descriptor2);
            Assert.False(descriptor1 == descriptor2);
            Assert.True(descriptor1 != descriptor2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentPayloadId(
            ForgettablePayloadDescriptor descriptor,
            ForgettablePayloadId differentPayloadId)
        {
            var descriptor1 = new ForgettablePayloadDescriptor(
                descriptor.EventStreamId,
                descriptor.EventStreamEntryId,
                descriptor.PayloadId,
                descriptor.PayloadState,
                descriptor.PayloadCreationTime,
                descriptor.PayloadLastModifiedTime,
                descriptor.PayloadSequence,
                descriptor.PayloadContent,
                descriptor.PayloadContentSerializationFormat,
                descriptor.PayloadTypeIdentifier,
                descriptor.PayloadTypeIdentifierFormat);

            var descriptor2 = new ForgettablePayloadDescriptor(
                descriptor.EventStreamId,
                descriptor.EventStreamEntryId,
                differentPayloadId,
                descriptor.PayloadState,
                descriptor.PayloadCreationTime,
                descriptor.PayloadLastModifiedTime,
                descriptor.PayloadSequence,
                descriptor.PayloadContent,
                descriptor.PayloadContentSerializationFormat,
                descriptor.PayloadTypeIdentifier,
                descriptor.PayloadTypeIdentifierFormat);

            Assert.NotEqual(descriptor1, descriptor2);
            Assert.False(descriptor1 == descriptor2);
            Assert.True(descriptor1 != descriptor2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentPayloadState(
            ForgettablePayloadDescriptor descriptor)
        {
            var descriptor1 = new ForgettablePayloadDescriptor(
                descriptor.EventStreamId,
                descriptor.EventStreamEntryId,
                descriptor.PayloadId,
                descriptor.PayloadState,
                descriptor.PayloadCreationTime,
                descriptor.PayloadLastModifiedTime,
                descriptor.PayloadSequence,
                descriptor.PayloadContent,
                descriptor.PayloadContentSerializationFormat,
                descriptor.PayloadTypeIdentifier,
                descriptor.PayloadTypeIdentifierFormat);

            ForgettablePayloadState differentPayloadState;
            if (descriptor.PayloadState == ForgettablePayloadState.Created)
            {
                differentPayloadState = new Random().Next(0, 2) == 0
                    ? ForgettablePayloadState.CreatedAndClaimed
                    : ForgettablePayloadState.Forgotten;
            }
            else if (descriptor.PayloadState == ForgettablePayloadState.CreatedAndClaimed)
            {
                differentPayloadState = new Random().Next(0, 2) == 0
                    ? ForgettablePayloadState.Created
                    : ForgettablePayloadState.Forgotten;
            }
            else
            {
                differentPayloadState = new Random().Next(0, 2) == 0
                    ? ForgettablePayloadState.Created
                    : ForgettablePayloadState.CreatedAndClaimed;
            }

            var descriptor2 = new ForgettablePayloadDescriptor(
                descriptor.EventStreamId,
                descriptor.EventStreamEntryId,
                descriptor.PayloadId,
                differentPayloadState,
                descriptor.PayloadCreationTime,
                descriptor.PayloadLastModifiedTime,
                descriptor.PayloadSequence,
                descriptor.PayloadContent,
                descriptor.PayloadContentSerializationFormat,
                descriptor.PayloadTypeIdentifier,
                descriptor.PayloadTypeIdentifierFormat);

            Assert.NotEqual(descriptor1, descriptor2);
            Assert.False(descriptor1 == descriptor2);
            Assert.True(descriptor1 != descriptor2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentPayloadCreationTime(
            ForgettablePayloadDescriptor descriptor,
            ForgettablePayloadCreationTime differentPayloadCreationTime)
        {
            var descriptor1 = new ForgettablePayloadDescriptor(
                descriptor.EventStreamId,
                descriptor.EventStreamEntryId,
                descriptor.PayloadId,
                descriptor.PayloadState,
                descriptor.PayloadCreationTime,
                descriptor.PayloadLastModifiedTime,
                descriptor.PayloadSequence,
                descriptor.PayloadContent,
                descriptor.PayloadContentSerializationFormat,
                descriptor.PayloadTypeIdentifier,
                descriptor.PayloadTypeIdentifierFormat);

            var descriptor2 = new ForgettablePayloadDescriptor(
                descriptor.EventStreamId,
                descriptor.EventStreamEntryId,
                descriptor.PayloadId,
                descriptor.PayloadState,
                differentPayloadCreationTime,
                descriptor.PayloadLastModifiedTime,
                descriptor.PayloadSequence,
                descriptor.PayloadContent,
                descriptor.PayloadContentSerializationFormat,
                descriptor.PayloadTypeIdentifier,
                descriptor.PayloadTypeIdentifierFormat);

            Assert.NotEqual(descriptor1, descriptor2);
            Assert.False(descriptor1 == descriptor2);
            Assert.True(descriptor1 != descriptor2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentPayloadLastModifiedTime(
            ForgettablePayloadDescriptor descriptor,
            ForgettablePayloadLastModifiedTime differentPayloadLastModifiedTime)
        {
            var descriptor1 = new ForgettablePayloadDescriptor(
                descriptor.EventStreamId,
                descriptor.EventStreamEntryId,
                descriptor.PayloadId,
                descriptor.PayloadState,
                descriptor.PayloadCreationTime,
                descriptor.PayloadLastModifiedTime,
                descriptor.PayloadSequence,
                descriptor.PayloadContent,
                descriptor.PayloadContentSerializationFormat,
                descriptor.PayloadTypeIdentifier,
                descriptor.PayloadTypeIdentifierFormat);

            var descriptor2 = new ForgettablePayloadDescriptor(
                descriptor.EventStreamId,
                descriptor.EventStreamEntryId,
                descriptor.PayloadId,
                descriptor.PayloadState,
                descriptor.PayloadCreationTime,
                differentPayloadLastModifiedTime,
                descriptor.PayloadSequence,
                descriptor.PayloadContent,
                descriptor.PayloadContentSerializationFormat,
                descriptor.PayloadTypeIdentifier,
                descriptor.PayloadTypeIdentifierFormat);

            Assert.NotEqual(descriptor1, descriptor2);
            Assert.False(descriptor1 == descriptor2);
            Assert.True(descriptor1 != descriptor2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentPayloadSequence(
            ForgettablePayloadDescriptor descriptor,
            ForgettablePayloadSequence differentPayloadSequence)
        {
            var descriptor1 = new ForgettablePayloadDescriptor(
                descriptor.EventStreamId,
                descriptor.EventStreamEntryId,
                descriptor.PayloadId,
                descriptor.PayloadState,
                descriptor.PayloadCreationTime,
                descriptor.PayloadLastModifiedTime,
                descriptor.PayloadSequence,
                descriptor.PayloadContent,
                descriptor.PayloadContentSerializationFormat,
                descriptor.PayloadTypeIdentifier,
                descriptor.PayloadTypeIdentifierFormat);

            var descriptor2 = new ForgettablePayloadDescriptor(
                descriptor.EventStreamId,
                descriptor.EventStreamEntryId,
                descriptor.PayloadId,
                descriptor.PayloadState,
                descriptor.PayloadCreationTime,
                descriptor.PayloadLastModifiedTime,
                differentPayloadSequence,
                descriptor.PayloadContent,
                descriptor.PayloadContentSerializationFormat,
                descriptor.PayloadTypeIdentifier,
                descriptor.PayloadTypeIdentifierFormat);

            Assert.NotEqual(descriptor1, descriptor2);
            Assert.False(descriptor1 == descriptor2);
            Assert.True(descriptor1 != descriptor2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentPayloadContent(
            ForgettablePayloadDescriptor descriptor,
            ForgettablePayloadContent differentPayloadContent)
        {
            var descriptor1 = new ForgettablePayloadDescriptor(
                descriptor.EventStreamId,
                descriptor.EventStreamEntryId,
                descriptor.PayloadId,
                descriptor.PayloadState,
                descriptor.PayloadCreationTime,
                descriptor.PayloadLastModifiedTime,
                descriptor.PayloadSequence,
                descriptor.PayloadContent,
                descriptor.PayloadContentSerializationFormat,
                descriptor.PayloadTypeIdentifier,
                descriptor.PayloadTypeIdentifierFormat);

            var descriptor2 = new ForgettablePayloadDescriptor(
                descriptor.EventStreamId,
                descriptor.EventStreamEntryId,
                descriptor.PayloadId,
                descriptor.PayloadState,
                descriptor.PayloadCreationTime,
                descriptor.PayloadLastModifiedTime,
                descriptor.PayloadSequence,
                differentPayloadContent,
                descriptor.PayloadContentSerializationFormat,
                descriptor.PayloadTypeIdentifier,
                descriptor.PayloadTypeIdentifierFormat);

            Assert.NotEqual(descriptor1, descriptor2);
            Assert.False(descriptor1 == descriptor2);
            Assert.True(descriptor1 != descriptor2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentPayloadContentSerializationFormat(
            ForgettablePayloadDescriptor descriptor,
            SerializationFormat differentPayloadContentSerializationFormat)
        {
            var descriptor1 = new ForgettablePayloadDescriptor(
                descriptor.EventStreamId,
                descriptor.EventStreamEntryId,
                descriptor.PayloadId,
                descriptor.PayloadState,
                descriptor.PayloadCreationTime,
                descriptor.PayloadLastModifiedTime,
                descriptor.PayloadSequence,
                descriptor.PayloadContent,
                descriptor.PayloadContentSerializationFormat,
                descriptor.PayloadTypeIdentifier,
                descriptor.PayloadTypeIdentifierFormat);

            var descriptor2 = new ForgettablePayloadDescriptor(
                descriptor.EventStreamId,
                descriptor.EventStreamEntryId,
                descriptor.PayloadId,
                descriptor.PayloadState,
                descriptor.PayloadCreationTime,
                descriptor.PayloadLastModifiedTime,
                descriptor.PayloadSequence,
                descriptor.PayloadContent,
                differentPayloadContentSerializationFormat,
                descriptor.PayloadTypeIdentifier,
                descriptor.PayloadTypeIdentifierFormat);

            Assert.NotEqual(descriptor1, descriptor2);
            Assert.False(descriptor1 == descriptor2);
            Assert.True(descriptor1 != descriptor2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentPayloadTypeIdentifier(
            ForgettablePayloadDescriptor descriptor,
            ForgettablePayloadTypeIdentifier differentPayloadTypeIdentifier)
        {
            var descriptor1 = new ForgettablePayloadDescriptor(
                descriptor.EventStreamId,
                descriptor.EventStreamEntryId,
                descriptor.PayloadId,
                descriptor.PayloadState,
                descriptor.PayloadCreationTime,
                descriptor.PayloadLastModifiedTime,
                descriptor.PayloadSequence,
                descriptor.PayloadContent,
                descriptor.PayloadContentSerializationFormat,
                descriptor.PayloadTypeIdentifier,
                descriptor.PayloadTypeIdentifierFormat);

            var descriptor2 = new ForgettablePayloadDescriptor(
                descriptor.EventStreamId,
                descriptor.EventStreamEntryId,
                descriptor.PayloadId,
                descriptor.PayloadState,
                descriptor.PayloadCreationTime,
                descriptor.PayloadLastModifiedTime,
                descriptor.PayloadSequence,
                descriptor.PayloadContent,
                descriptor.PayloadContentSerializationFormat,
                differentPayloadTypeIdentifier,
                descriptor.PayloadTypeIdentifierFormat);

            Assert.NotEqual(descriptor1, descriptor2);
            Assert.False(descriptor1 == descriptor2);
            Assert.True(descriptor1 != descriptor2);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnFalse_When_ComparingDifferentObjectsWithDifferentPayloadTypeIdentifierFormat(
            ForgettablePayloadDescriptor descriptor,
            ForgettablePayloadTypeIdentifierFormat differentPayloadTypeIdentifierFormat)
        {
            var descriptor1 = new ForgettablePayloadDescriptor(
                descriptor.EventStreamId,
                descriptor.EventStreamEntryId,
                descriptor.PayloadId,
                descriptor.PayloadState,
                descriptor.PayloadCreationTime,
                descriptor.PayloadLastModifiedTime,
                descriptor.PayloadSequence,
                descriptor.PayloadContent,
                descriptor.PayloadContentSerializationFormat,
                descriptor.PayloadTypeIdentifier,
                descriptor.PayloadTypeIdentifierFormat);

            var descriptor2 = new ForgettablePayloadDescriptor(
                descriptor.EventStreamId,
                descriptor.EventStreamEntryId,
                descriptor.PayloadId,
                descriptor.PayloadState,
                descriptor.PayloadCreationTime,
                descriptor.PayloadLastModifiedTime,
                descriptor.PayloadSequence,
                descriptor.PayloadContent,
                descriptor.PayloadContentSerializationFormat,
                descriptor.PayloadTypeIdentifier,
                differentPayloadTypeIdentifierFormat);

            Assert.NotEqual(descriptor1, descriptor2);
            Assert.False(descriptor1 == descriptor2);
            Assert.True(descriptor1 != descriptor2);
        }

        [Theory]
        [AutoMoqData]
        public void CreateMetadataObjectWithSameProperties_When_CallingToMetadata(
            ForgettablePayloadDescriptor descriptor)
        {
            var metadata = descriptor.ToMetadata();

            Assert.Equal(descriptor.EventStreamId, metadata.EventStreamId);
            Assert.Equal(descriptor.EventStreamEntryId, metadata.EventStreamEntryId);
            Assert.Equal(descriptor.PayloadId, metadata.PayloadId);
            Assert.Equal(descriptor.PayloadSequence, metadata.PayloadSequence);
            Assert.Equal(descriptor.PayloadState, metadata.PayloadState);
            Assert.Equal(descriptor.PayloadCreationTime, metadata.PayloadCreationTime);
            Assert.Equal(descriptor.PayloadLastModifiedTime, metadata.PayloadLastModifiedTime);
        }

        [Theory]
        [AutoMoqData]
        public void CreateContentDescriptorObjectWithSameProperties_When_CallingToContentDescriptor(
            ForgettablePayloadDescriptor descriptor)
        {
            var contentDescriptor = descriptor.ToContentDescriptor();

            Assert.Equal(descriptor.PayloadContent, contentDescriptor.PayloadContent);
            Assert.Equal(descriptor.PayloadContentSerializationFormat, contentDescriptor.PayloadContentSerializationFormat);
            Assert.Equal(descriptor.PayloadTypeIdentifier, contentDescriptor.PayloadTypeIdentifier);
            Assert.Equal(descriptor.PayloadTypeIdentifierFormat, contentDescriptor.PayloadTypeIdentifierFormat);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnExpectedValue_When_CallingToString(ForgettablePayloadDescriptor descriptor)
        {
            var expectedValue =
                $"Forgettable payload attached to event stream id {descriptor.EventStreamId} and entry id {descriptor.EventStreamEntryId}, Payload Id {descriptor.PayloadId}, Payload State: {descriptor.PayloadState}, Payload Creation Time: {descriptor.PayloadCreationTime}, Payload Last Modified Time: {descriptor.PayloadLastModifiedTime}, Payload Sequence: {descriptor.PayloadSequence}, Payload Content: {descriptor.PayloadContent}, Payload Content Serialization Format: {descriptor.PayloadContentSerializationFormat}, Payload Type Identifier: {descriptor.PayloadTypeIdentifier}, Payload Type Identifier Format: {descriptor.PayloadTypeIdentifierFormat}";

            Assert.Equal(expectedValue, descriptor.ToString());
        }
    }
}

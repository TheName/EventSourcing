using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.ForgettablePayloads.Abstractions;
using EventSourcing.ForgettablePayloads.Abstractions.Conversion;
using EventSourcing.ForgettablePayloads.Abstractions.Services;
using EventSourcing.ForgettablePayloads.Abstractions.ValueObjects;
using EventSourcing.ForgettablePayloads.Hooks;
using EventSourcing.ForgettablePayloads.Persistence.Abstractions;
using Moq;
using TestHelpers.Attributes;
using Xunit;

namespace ForgettablePayloads.UnitTests.Hooks
{
    public class StoreForgettablePayloadsPrePublishingHook_Should
    {
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_Creating_With_NullFinder(
            IForgettablePayloadStorageWriter forgettablePayloadStorageWriter,
            IForgettablePayloadContentConverter forgettablePayloadConverter)
        {
            Assert.Throws<ArgumentNullException>(() => new StoreForgettablePayloadsPrePublishingHook(
                null,
                forgettablePayloadStorageWriter,
                forgettablePayloadConverter));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_Creating_With_NullStorageWriter(
            IForgettablePayloadFinder forgettablePayloadFinder,
            IForgettablePayloadContentConverter forgettablePayloadConverter)
        {
            Assert.Throws<ArgumentNullException>(() => new StoreForgettablePayloadsPrePublishingHook(
                forgettablePayloadFinder,
                null,
                forgettablePayloadConverter));
        }
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_Creating_With_NullContentConverter(
            IForgettablePayloadFinder forgettablePayloadFinder,
            IForgettablePayloadStorageWriter forgettablePayloadStorageWriter)
        {
            Assert.Throws<ArgumentNullException>(() => new StoreForgettablePayloadsPrePublishingHook(
                forgettablePayloadFinder,
                forgettablePayloadStorageWriter,
                null));
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_Creating_With_NonNullArguments(
            IForgettablePayloadFinder forgettablePayloadFinder,
            IForgettablePayloadStorageWriter forgettablePayloadStorageWriter,
            IForgettablePayloadContentConverter forgettablePayloadConverter)
        {
            _ = new StoreForgettablePayloadsPrePublishingHook(
                forgettablePayloadFinder,
                forgettablePayloadStorageWriter,
                forgettablePayloadConverter);
        }

        [Theory]
        [AutoMoqData]
        internal async Task Throw_ArgumentNullException_When_PreEventStreamEventWithMetadataPublishHookAsyncIsCalled_With_NullEventWithMetadata(
            StoreForgettablePayloadsPrePublishingHook hook)
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                hook.PreEventStreamEventWithMetadataPublishHookAsync(null, CancellationToken.None));
        }

        [Theory]
        [AutoMoqData]
        internal async Task Throw_InvalidOperationException_When_PreEventStreamEventWithMetadataPublishHookAsyncIsCalled_And_FinderReturnsNull(
            EventStreamEventWithMetadata eventWithMetadata,
            [Frozen] Mock<IForgettablePayloadFinder> finderMock,
            StoreForgettablePayloadsPrePublishingHook hook)
        {
            finderMock
                .Setup(finder => finder.Find(eventWithMetadata.Event))
                .Returns(null as IReadOnlyCollection<ForgettablePayload>)
                .Verifiable();
            
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                hook.PreEventStreamEventWithMetadataPublishHookAsync(eventWithMetadata, CancellationToken.None));
            
            finderMock.Verify();
        }

        [Theory]
        [AutoMoqWithInlineData(0)]
        [AutoMoqWithInlineData(1)]
        [AutoMoqWithInlineData(2)]
        internal async Task Throw_InvalidOperationException_When_PreEventStreamEventWithMetadataPublishHookAsyncIsCalled_And_FinderReturnsCollectionWithAtLeastOneNull(
            int indexAtWhichNullShouldBeInserted,
            EventStreamEventWithMetadata eventWithMetadata,
            List<ForgettablePayload> forgettablePayloads,
            [Frozen] Mock<IForgettablePayloadFinder> finderMock,
            StoreForgettablePayloadsPrePublishingHook hook)
        {
            forgettablePayloads.Insert(indexAtWhichNullShouldBeInserted, null);
            
            finderMock
                .Setup(finder => finder.Find(eventWithMetadata.Event))
                .Returns(forgettablePayloads)
                .Verifiable();

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                hook.PreEventStreamEventWithMetadataPublishHookAsync(eventWithMetadata, CancellationToken.None));

            finderMock.Verify();
        }

        [Theory]
        [AutoMoqWithInlineData(0)]
        [AutoMoqWithInlineData(1)]
        [AutoMoqWithInlineData(2)]
        internal async Task InsertAllPayloadsThatWereCreatedAndNotThrow_When_PreEventStreamEventWithMetadataPublishHookAsyncIsCalled_And_FinderReturnsCollectionWithAtLeastOneForgettablePayloadThatWasNotCreated(
            int indexAtWhichNotCreatedForgettablePayloadShouldBeInserted,
            EventStreamEventWithMetadata eventWithMetadata,
            List<object> payloads,
            ForgettablePayload notCreatedForgettablePayload,
            List<ForgettablePayloadContentDescriptor> contentDescriptors,
            [Frozen] Mock<IForgettablePayloadFinder> finderMock,
            [Frozen] Mock<IForgettablePayloadContentConverter> contentConverterMock,
            [Frozen] Mock<IForgettablePayloadStorageWriter> storageWriterMock,
            StoreForgettablePayloadsPrePublishingHook hook)
        {
            // Arrange
            var forgettablePayloads = payloads
                .Select(ForgettablePayload.CreateNew)
                .ToList();
            
            forgettablePayloads.Insert(indexAtWhichNotCreatedForgettablePayloadShouldBeInserted, notCreatedForgettablePayload);
            
            finderMock
                .Setup(finder => finder.Find(eventWithMetadata.Event))
                .Returns(forgettablePayloads)
                .Verifiable();

            for (var i = 0; i < payloads.Count; i++)
            {
                contentConverterMock
                    .Setup(converter => converter.ToPayloadContentDescriptor(payloads[i]))
                    .Returns(contentDescriptors[i])
                    .Verifiable();
            }

            // Act
            await hook.PreEventStreamEventWithMetadataPublishHookAsync(eventWithMetadata, CancellationToken.None);

            // Assert
            var contentDescriptorsIndex = 0;
            for (var i = 0; i < forgettablePayloads.Count; i++)
            {
                if (i == indexAtWhichNotCreatedForgettablePayloadShouldBeInserted)
                {
                    var notCreatedResult = forgettablePayloads[i].TryCreateMetadataForEventStreamIdAndEntryId(
                        eventWithMetadata.EventMetadata.StreamId,
                        eventWithMetadata.EventMetadata.EntryId,
                        out var notCreatedMetadata);
                    
                    Assert.False(notCreatedResult);
                    Assert.Null(notCreatedMetadata);
                    
                    continue;
                }
                
                var result = forgettablePayloads[i].TryCreateMetadataForEventStreamIdAndEntryId(
                    eventWithMetadata.EventMetadata.StreamId,
                    eventWithMetadata.EventMetadata.EntryId,
                    out var metadata);
                
                Assert.True(result);
                var contentDescriptor = contentDescriptors[contentDescriptorsIndex++];
                var descriptor = ForgettablePayloadDescriptor.CreateFromMetadataAndContentDescriptor(metadata, contentDescriptor);

                storageWriterMock
                    .Verify(writer => writer.InsertAsync(descriptor, It.IsAny<CancellationToken>()), Times.Once);
            }
            
            storageWriterMock.VerifyNoOtherCalls();
            finderMock.Verify();
            contentConverterMock.Verify();
        }

        [Theory]
        [AutoMoqWithInlineData(0)]
        [AutoMoqWithInlineData(1)]
        [AutoMoqWithInlineData(2)]
        internal async Task Throw_AggregateException_When_PreEventStreamEventWithMetadataPublishHookAsyncIsCalled_And_ConverterReturnsNullForAtLeastOneForgettablePayload(
            int indexOfPayloadWhichConversionShouldReturnNull,
            EventStreamEventWithMetadata eventWithMetadata,
            List<object> payloads,
            [Frozen] Mock<IForgettablePayloadFinder> finderMock,
            [Frozen] Mock<IForgettablePayloadContentConverter> contentConverterMock,
            StoreForgettablePayloadsPrePublishingHook hook)
        {
            // Arrange
            var forgettablePayloads = payloads
                .Select(ForgettablePayload.CreateNew)
                .ToList();
            
            finderMock
                .Setup(finder => finder.Find(eventWithMetadata.Event))
                .Returns(forgettablePayloads)
                .Verifiable();

            contentConverterMock
                .Setup(converter => converter.ToPayloadContentDescriptor(payloads[indexOfPayloadWhichConversionShouldReturnNull]))
                .Returns(null as ForgettablePayloadContentDescriptor);

            // Act
            var aggregateException = await Assert.ThrowsAsync<AggregateException>(() =>
                hook.PreEventStreamEventWithMetadataPublishHookAsync(eventWithMetadata, CancellationToken.None));

            // Assert
            var singleInnerException = Assert.Single(aggregateException.InnerExceptions);
            Assert.IsType<InvalidOperationException>(singleInnerException);

            finderMock.Verify();
        }

        [Theory]
        [AutoMoqWithInlineData(0)]
        [AutoMoqWithInlineData(1)]
        [AutoMoqWithInlineData(2)]
        internal async Task TryToInsertAllForgettablePayloadDescriptors_When_PreEventStreamEventWithMetadataPublishHookAsyncIsCalled_And_OneInsertionThrows(
            int indexThatThrows,
            Exception expectedException,
            EventStreamEventWithMetadata eventWithMetadata,
            List<object> payloads,
            List<ForgettablePayloadContentDescriptor> contentDescriptors,
            [Frozen] Mock<IForgettablePayloadFinder> finderMock,
            [Frozen] Mock<IForgettablePayloadContentConverter> contentConverterMock,
            [Frozen] Mock<IForgettablePayloadStorageWriter> storageWriterMock,
            StoreForgettablePayloadsPrePublishingHook hook)
        {
            // Arrange
            var forgettablePayloads = payloads
                .Select(ForgettablePayload.CreateNew)
                .ToList();
            
            finderMock
                .Setup(finder => finder.Find(eventWithMetadata.Event))
                .Returns(forgettablePayloads)
                .Verifiable();

            for (var i = 0; i < payloads.Count; i++)
            {
                contentConverterMock
                    .Setup(converter => converter.ToPayloadContentDescriptor(payloads[i]))
                    .Returns(contentDescriptors[i])
                    .Verifiable();
                
                var result = forgettablePayloads[i].TryCreateMetadataForEventStreamIdAndEntryId(
                    eventWithMetadata.EventMetadata.StreamId,
                    eventWithMetadata.EventMetadata.EntryId,
                    out var metadata);
                
                Assert.True(result);
                var contentDescriptor = contentDescriptors[i];
                var descriptor = ForgettablePayloadDescriptor.CreateFromMetadataAndContentDescriptor(metadata, contentDescriptor);

                var setup = storageWriterMock.Setup(writer => writer.InsertAsync(descriptor, It.IsAny<CancellationToken>()));
                if (indexThatThrows == i)
                {
                    setup.Throws(expectedException);
                }

                setup.Verifiable();
            }

            // Act
            var aggregateException = await Assert.ThrowsAsync<AggregateException>(() => hook.PreEventStreamEventWithMetadataPublishHookAsync(eventWithMetadata, CancellationToken.None));

            // Assert
            var singleInnerException = Assert.Single(aggregateException.InnerExceptions);
            Assert.Equal(expectedException, singleInnerException);
            
            storageWriterMock.Verify();
            storageWriterMock.VerifyNoOtherCalls();
            finderMock.Verify();
            contentConverterMock.Verify();
        }

        [Theory]
        [AutoMoqData]
        internal async Task InsertForgettablePayloadDescriptors_When_PreEventStreamEventWithMetadataPublishHookAsyncIsCalled(
            EventStreamEventWithMetadata eventWithMetadata,
            List<object> payloads,
            List<ForgettablePayloadContentDescriptor> contentDescriptors,
            [Frozen] Mock<IForgettablePayloadFinder> finderMock,
            [Frozen] Mock<IForgettablePayloadContentConverter> contentConverterMock,
            [Frozen] Mock<IForgettablePayloadStorageWriter> storageWriterMock,
            StoreForgettablePayloadsPrePublishingHook hook)
        {
            // Arrange
            var forgettablePayloads = payloads
                .Select(ForgettablePayload.CreateNew)
                .ToList();
            
            finderMock
                .Setup(finder => finder.Find(eventWithMetadata.Event))
                .Returns(forgettablePayloads)
                .Verifiable();

            for (var i = 0; i < payloads.Count; i++)
            {
                contentConverterMock
                    .Setup(converter => converter.ToPayloadContentDescriptor(payloads[i]))
                    .Returns(contentDescriptors[i])
                    .Verifiable();
            }

            // Act
            await hook.PreEventStreamEventWithMetadataPublishHookAsync(eventWithMetadata, CancellationToken.None);

            // Assert
            for (var i = 0; i < payloads.Count; i++)
            {
                var result = forgettablePayloads[i].TryCreateMetadataForEventStreamIdAndEntryId(
                    eventWithMetadata.EventMetadata.StreamId,
                    eventWithMetadata.EventMetadata.EntryId,
                    out var metadata);

                Assert.True(result);
                var contentDescriptor = contentDescriptors[i];
                var descriptor = ForgettablePayloadDescriptor.CreateFromMetadataAndContentDescriptor(metadata, contentDescriptor);

                storageWriterMock
                    .Verify(writer => writer.InsertAsync(descriptor, It.IsAny<CancellationToken>()), Times.Once);
            }
            
            storageWriterMock.VerifyNoOtherCalls();
            finderMock.Verify();
            contentConverterMock.Verify();
        }
    }
}
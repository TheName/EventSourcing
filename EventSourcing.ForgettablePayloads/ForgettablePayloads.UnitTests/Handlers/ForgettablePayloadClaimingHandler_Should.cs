using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using EventSourcing.ForgettablePayloads;
using EventSourcing.ForgettablePayloads.Conversion;
using EventSourcing.ForgettablePayloads.Handlers;
using EventSourcing.ForgettablePayloads.Services;
using EventSourcing.ForgettablePayloads.ValueObjects;
using EventSourcing.ValueObjects;
using Moq;
using TestHelpers.Attributes;
using Xunit;

namespace ForgettablePayloads.UnitTests.Handlers
{
    public class ForgettablePayloadClaimingHandler_Should
    {
        [Fact]
        public void Throw_ArgumentNullException_When_CreatingWithNullFinder()
        {
            Assert.Throws<ArgumentNullException>(() => new ForgettablePayloadClaimingHandler(null));
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_CreatingWithNotNullParameters(IForgettablePayloadFinder finder)
        {
            _ = new ForgettablePayloadClaimingHandler(finder);
        }

        [Theory]
        [AutoMoqData]
        internal async Task Throw_ArgumentNullException_When_Handling_And_EventIsNull(
            EventStreamEventMetadata metadata,
            ForgettablePayloadClaimingHandler handler)
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                handler.HandleAsync(null, metadata, CancellationToken.None));
        }

        [Theory]
        [AutoMoqData]
        internal async Task Throw_ArgumentNullException_When_Handling_And_EventMetadataIsNull(
            object @event,
            ForgettablePayloadClaimingHandler handler)
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                handler.HandleAsync(@event, null, CancellationToken.None));
        }

        [Theory]
        [AutoMoqData]
        internal async Task Throw_InvalidOperationException_When_Handling_And_FinderReturnsNull(
            object @event,
            EventStreamEventMetadata eventMetadata,
            [Frozen] Mock<IForgettablePayloadFinder> finderMock,
            ForgettablePayloadClaimingHandler handler)
        {
            finderMock
                .Setup(finder => finder.Find(@event))
                .Returns(null as IReadOnlyCollection<ForgettablePayload>);
            
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                handler.HandleAsync(@event, eventMetadata, CancellationToken.None));
        }

        [Theory]
        [AutoMoqData]
        internal async Task ClaimAllFoundPayloads_When_Handling(
            object @event,
            EventStreamEventMetadata eventMetadata,
            List<ForgettablePayload> forgettablePayloads,
            List<ForgettablePayloadDescriptor> forgettablePayloadDescriptors,
            Mock<IForgettablePayloadDescriptorLoader> forgettablePayloadDescriptorLoaderMock,
            Mock<IForgettablePayloadClaimingService> claimingServiceMock,
            IForgettablePayloadContentConverter contentConverter,
            [Frozen] Mock<IForgettablePayloadFinder> finderMock,
            ForgettablePayloadClaimingHandler handler)
        {
            finderMock
                .Setup(finder => finder.Find(@event))
                .Returns(forgettablePayloads);

            foreach (var forgettablePayload in forgettablePayloads)
            {
                forgettablePayload.AssignPayloadDescriptorLoaderService(forgettablePayloadDescriptorLoaderMock.Object);

                var forgettablePayloadDescriptor = forgettablePayloadDescriptors[forgettablePayloads.IndexOf(forgettablePayload)];
                forgettablePayloadDescriptor = CreateWithIdAndState(
                    forgettablePayloadDescriptor,
                    forgettablePayload.PayloadId,
                    ForgettablePayloadState.Created);

                forgettablePayloadDescriptorLoaderMock
                    .Setup(loader => loader.LoadAsync(forgettablePayload.PayloadId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(forgettablePayloadDescriptor);

                forgettablePayload.AssignClaimingService(claimingServiceMock.Object);
                claimingServiceMock
                    .Setup(service => service.ClaimAsync(forgettablePayloadDescriptor, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(
                        CreateWithIdAndState(
                            forgettablePayloadDescriptor,
                            forgettablePayloadDescriptor.PayloadId,
                            ForgettablePayloadState.CreatedAndClaimed))
                    .Verifiable();
                
                forgettablePayload.AssignContentConverter(contentConverter);
            }

            await handler.HandleAsync(@event, eventMetadata, CancellationToken.None);
            
            claimingServiceMock.Verify();
            claimingServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task TryClaimingAllFoundPayloads_When_Handling_And_ClaimingServiceThrowsException(
            object @event,
            EventStreamEventMetadata eventMetadata,
            List<ForgettablePayload> forgettablePayloads,
            List<ForgettablePayloadDescriptor> forgettablePayloadDescriptors,
            List<Exception> claimingExceptions,
            Mock<IForgettablePayloadDescriptorLoader> forgettablePayloadDescriptorLoaderMock,
            Mock<IForgettablePayloadClaimingService> claimingServiceMock,
            [Frozen] Mock<IForgettablePayloadFinder> finderMock,
            ForgettablePayloadClaimingHandler handler)
        {
            finderMock
                .Setup(finder => finder.Find(@event))
                .Returns(forgettablePayloads);

            foreach (var forgettablePayload in forgettablePayloads)
            {
                forgettablePayload.AssignPayloadDescriptorLoaderService(forgettablePayloadDescriptorLoaderMock.Object);

                var forgettablePayloadDescriptor = forgettablePayloadDescriptors[forgettablePayloads.IndexOf(forgettablePayload)];
                forgettablePayloadDescriptor = CreateWithIdAndState(
                    forgettablePayloadDescriptor,
                    forgettablePayload.PayloadId,
                    ForgettablePayloadState.Created);

                forgettablePayloadDescriptorLoaderMock
                    .Setup(loader => loader.LoadAsync(forgettablePayload.PayloadId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(forgettablePayloadDescriptor);

                forgettablePayload.AssignClaimingService(claimingServiceMock.Object);
                claimingServiceMock
                    .Setup(service => service.ClaimAsync(forgettablePayloadDescriptor, It.IsAny<CancellationToken>()))
                    .ThrowsAsync(claimingExceptions[forgettablePayloads.IndexOf(forgettablePayload)])
                    .Verifiable();
            }

            var thrownException = await Assert.ThrowsAsync<AggregateException>(() =>
                handler.HandleAsync(@event, eventMetadata, CancellationToken.None));
            
            claimingServiceMock.Verify();
            claimingServiceMock.VerifyNoOtherCalls();
            Assert.Equal(forgettablePayloads.Count, thrownException.InnerExceptions.Count);
            Assert.All(claimingExceptions, exception => Assert.Contains(exception, thrownException.InnerExceptions));
        }

        private static ForgettablePayloadDescriptor CreateWithIdAndState(
            ForgettablePayloadDescriptor descriptor,
            ForgettablePayloadId id,
            ForgettablePayloadState state)
        {
            return new ForgettablePayloadDescriptor(
                descriptor.EventStreamId,
                descriptor.EventStreamEntryId,
                id,
                state,
                descriptor.PayloadCreationTime,
                descriptor.PayloadLastModifiedTime,
                descriptor.PayloadSequence,
                descriptor.PayloadContent,
                descriptor.PayloadContentSerializationFormat,
                descriptor.PayloadTypeIdentifier,
                descriptor.PayloadTypeIdentifierFormat);
        }
    }
}
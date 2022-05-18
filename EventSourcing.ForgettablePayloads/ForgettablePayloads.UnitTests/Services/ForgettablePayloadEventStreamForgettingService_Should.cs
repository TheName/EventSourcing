using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using EventSourcing.Abstractions.ValueObjects;
using EventSourcing.ForgettablePayloads.Abstractions.Services;
using EventSourcing.ForgettablePayloads.Abstractions.ValueObjects;
using EventSourcing.ForgettablePayloads.Persistence.Abstractions;
using EventSourcing.ForgettablePayloads.Services;
using Moq;
using TestHelpers.Attributes;
using Xunit;

namespace ForgettablePayloads.UnitTests.Services
{
    public class ForgettablePayloadEventStreamForgettingService_Should
    {
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_Creating_With_NullStorageReader(
            IForgettablePayloadForgettingService forgettingService)
        {
            Assert.Throws<ArgumentNullException>(() =>
                new ForgettablePayloadEventStreamForgettingService(null, forgettingService));
        }
        
        [Theory]
        [AutoMoqData]
        public void Throw_ArgumentNullException_When_Creating_With_NullForgettingService(
            IForgettablePayloadStorageReader forgettablePayloadStorageReader)
        {
            Assert.Throws<ArgumentNullException>(() =>
                new ForgettablePayloadEventStreamForgettingService(forgettablePayloadStorageReader, null));
        }
        
        [Theory]
        [AutoMoqData]
        public void NotThrow_When_Creating_With_NotNullParameters(
            IForgettablePayloadStorageReader forgettablePayloadStorageReader,
            IForgettablePayloadForgettingService forgettingService)
        {
            _ = new ForgettablePayloadEventStreamForgettingService(forgettablePayloadStorageReader, forgettingService);
        }

        [Theory]
        [AutoMoqData]
        internal async Task Throw_ArgumentNullException_When_CallingForgetAllForgettablePayloadsFromStreamAsync_And_EventStreamIdIsNull(
            ForgettingPayloadReason reason,
            ForgettingPayloadRequestedBy requestedBy,
            ForgettablePayloadEventStreamForgettingService forgettingService)
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                forgettingService.ForgetAllForgettablePayloadsFromStreamAsync(
                    null,
                    reason,
                    requestedBy,
                    CancellationToken.None));
        }

        [Theory]
        [AutoMoqData]
        internal async Task Throw_ArgumentNullException_When_CallingForgetAllForgettablePayloadsFromStreamAsync_And_ReasonIsNull(
            EventStreamId eventStreamId,
            ForgettingPayloadRequestedBy requestedBy,
            ForgettablePayloadEventStreamForgettingService forgettingService)
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                forgettingService.ForgetAllForgettablePayloadsFromStreamAsync(
                    eventStreamId,
                    null,
                    requestedBy,
                    CancellationToken.None));
        }

        [Theory]
        [AutoMoqData]
        internal async Task Throw_ArgumentNullException_When_CallingForgetAllForgettablePayloadsFromStreamAsync_And_RequestedByIsNull(
            EventStreamId eventStreamId,
            ForgettingPayloadReason reason,
            ForgettablePayloadEventStreamForgettingService forgettingService)
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                forgettingService.ForgetAllForgettablePayloadsFromStreamAsync(
                    eventStreamId,
                    reason,
                    null,
                    CancellationToken.None));
        }

        [Theory]
        [AutoMoqData]
        internal async Task Throw_InvalidOperationException_When_CallingForgetAllForgettablePayloadsFromStreamAsync_And_StorageReaderReturnsNull(
            EventStreamId eventStreamId,
            ForgettingPayloadReason reason,
            ForgettingPayloadRequestedBy requestedBy,
            [Frozen] Mock<IForgettablePayloadStorageReader> storageReaderMock,
            ForgettablePayloadEventStreamForgettingService forgettingService)
        {
            storageReaderMock
                .Setup(reader => reader.ReadAsync(eventStreamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as IReadOnlyCollection<ForgettablePayloadDescriptor>)
                .Verifiable();
            
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                forgettingService.ForgetAllForgettablePayloadsFromStreamAsync(
                    eventStreamId,
                    reason,
                    requestedBy,
                    CancellationToken.None));
            
            storageReaderMock.Verify();
        }

        [Theory]
        [AutoMoqData]
        internal async Task DoNothing_When_CallingForgetAllForgettablePayloadsFromStreamAsync_And_StorageReaderReturnsEmptyCollection(
            EventStreamId eventStreamId,
            ForgettingPayloadReason reason,
            ForgettingPayloadRequestedBy requestedBy,
            [Frozen] Mock<IForgettablePayloadForgettingService> forgettingServiceMock,
            [Frozen] Mock<IForgettablePayloadStorageReader> storageReaderMock,
            ForgettablePayloadEventStreamForgettingService forgettingService)
        {
            storageReaderMock
                .Setup(reader => reader.ReadAsync(eventStreamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ForgettablePayloadDescriptor>())
                .Verifiable();

            await forgettingService.ForgetAllForgettablePayloadsFromStreamAsync(
                eventStreamId,
                reason,
                requestedBy,
                CancellationToken.None);
            
            storageReaderMock.Verify();
            forgettingServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqWithInlineData(0)]
        [AutoMoqWithInlineData(1)]
        [AutoMoqWithInlineData(2)]
        internal async Task TryToForgetAllForgettablePayloadDescriptors_When_CallingForgetAllForgettablePayloadsFromStreamAsync_And_OneForgettingThrows(
            int indexThatThrows,
            Exception expectedException,
            EventStreamId eventStreamId,
            ForgettingPayloadReason reason,
            ForgettingPayloadRequestedBy requestedBy,
            List<ForgettablePayloadDescriptor> forgettablePayloadDescriptors,
            [Frozen] Mock<IForgettablePayloadForgettingService> forgettingServiceMock,
            [Frozen] Mock<IForgettablePayloadStorageReader> storageReaderMock,
            ForgettablePayloadEventStreamForgettingService forgettingService)
        {
            // Arrange
            storageReaderMock
                .Setup(reader => reader.ReadAsync(eventStreamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(forgettablePayloadDescriptors);

            foreach (var forgettablePayloadDescriptor in forgettablePayloadDescriptors)
            {
                var setup = forgettingServiceMock
                    .Setup(service => service.ForgetAsync(
                        forgettablePayloadDescriptor,
                        reason,
                        requestedBy,
                        It.IsAny<CancellationToken>()));

                if (forgettablePayloadDescriptors.IndexOf(forgettablePayloadDescriptor) == indexThatThrows)
                {
                    setup.ThrowsAsync(expectedException);
                }
                else
                {
                    setup.ReturnsAsync(forgettablePayloadDescriptor);
                }
                
                setup.Verifiable();
            }

            // Act
            var aggregateException = await Assert.ThrowsAsync<AggregateException>(() =>
                forgettingService.ForgetAllForgettablePayloadsFromStreamAsync(
                    eventStreamId,
                    reason,
                    requestedBy,
                    CancellationToken.None));

            // Assert
            var singleInnerException = Assert.Single(aggregateException.InnerExceptions);
            Assert.Equal(expectedException, singleInnerException);
            forgettingServiceMock.Verify();
            forgettingServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal async Task ForgetAllForgettablePayloadDescriptors_When_CallingForgetAllForgettablePayloadsFromStreamAsync(
            EventStreamId eventStreamId,
            ForgettingPayloadReason reason,
            ForgettingPayloadRequestedBy requestedBy,
            List<ForgettablePayloadDescriptor> forgettablePayloadDescriptors,
            [Frozen] Mock<IForgettablePayloadForgettingService> forgettingServiceMock,
            [Frozen] Mock<IForgettablePayloadStorageReader> storageReaderMock,
            ForgettablePayloadEventStreamForgettingService forgettingService)
        {
            // Arrange
            storageReaderMock
                .Setup(reader => reader.ReadAsync(eventStreamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(forgettablePayloadDescriptors);

            foreach (var forgettablePayloadDescriptor in forgettablePayloadDescriptors)
            {
                forgettingServiceMock
                    .Setup(service => service.ForgetAsync(
                        forgettablePayloadDescriptor,
                        reason,
                        requestedBy,
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(forgettablePayloadDescriptor)
                    .Verifiable();
            }

            // Act
            await forgettingService.ForgetAllForgettablePayloadsFromStreamAsync(
                eventStreamId,
                reason,
                requestedBy,
                CancellationToken.None);

            // Assert
            forgettingServiceMock.Verify();
            forgettingServiceMock.VerifyNoOtherCalls();
        }
    }
}
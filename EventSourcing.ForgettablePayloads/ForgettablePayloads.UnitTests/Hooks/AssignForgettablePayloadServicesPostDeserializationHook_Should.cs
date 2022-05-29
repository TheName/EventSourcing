using System;
using System.Collections.Generic;
using AutoFixture.Xunit2;
using EventSourcing.ForgettablePayloads.Abstractions;
using EventSourcing.ForgettablePayloads.Abstractions.Conversion;
using EventSourcing.ForgettablePayloads.Abstractions.Services;
using EventSourcing.ForgettablePayloads.Hooks;
using ForgettablePayloads.UnitTests.Extensions;
using Moq;
using TestHelpers.Attributes;
using Xunit;

namespace ForgettablePayloads.UnitTests.Hooks
{
    public class AssignForgettablePayloadServicesPostDeserializationHook_Should
    {
        [Theory]
        [AutoMoqData]
        internal void Throw_ArgumentNullException_When_Creating_And_ForgettablePayloadFinderIsNull(
            IForgettablePayloadDescriptorLoader forgettablePayloadLoader,
            IForgettablePayloadForgettingService forgettingService,
            IForgettablePayloadClaimingService claimingService,
            IForgettablePayloadContentConverter converter)
        {
            Assert.Throws<ArgumentNullException>(() => new AssignForgettablePayloadServicesPostDeserializationHook(
                null,
                forgettablePayloadLoader,
                forgettingService,
                claimingService,
                converter));
        }
        
        [Theory]
        [AutoMoqData]
        internal void Throw_ArgumentNullException_When_Creating_And_ForgettablePayloadDescriptorLoaderIsNull(
            IForgettablePayloadFinder forgettablePayloadFinder,
            IForgettablePayloadForgettingService forgettingService,
            IForgettablePayloadClaimingService claimingService,
            IForgettablePayloadContentConverter converter)
        {
            Assert.Throws<ArgumentNullException>(() => new AssignForgettablePayloadServicesPostDeserializationHook(
                forgettablePayloadFinder,
                null,
                forgettingService,
                claimingService,
                converter));
        }
        
        [Theory]
        [AutoMoqData]
        internal void Throw_ArgumentNullException_When_Creating_And_ForgettablePayloadForgettingServiceIsNull(
            IForgettablePayloadFinder forgettablePayloadFinder,
            IForgettablePayloadDescriptorLoader forgettablePayloadLoader,
            IForgettablePayloadClaimingService claimingService,
            IForgettablePayloadContentConverter converter)
        {
            Assert.Throws<ArgumentNullException>(() => new AssignForgettablePayloadServicesPostDeserializationHook(
                forgettablePayloadFinder,
                forgettablePayloadLoader,
                null,
                claimingService,
                converter));
        }
        
        [Theory]
        [AutoMoqData]
        internal void Throw_ArgumentNullException_When_Creating_And_ForgettablePayloadClaimingServiceIsNull(
            IForgettablePayloadFinder forgettablePayloadFinder,
            IForgettablePayloadDescriptorLoader forgettablePayloadLoader,
            IForgettablePayloadForgettingService forgettingService,
            IForgettablePayloadContentConverter converter)
        {
            Assert.Throws<ArgumentNullException>(() => new AssignForgettablePayloadServicesPostDeserializationHook(
                forgettablePayloadFinder,
                forgettablePayloadLoader,
                forgettingService,
                null,
                converter));
        }
        
        [Theory]
        [AutoMoqData]
        internal void Throw_ArgumentNullException_When_Creating_And_ForgettablePayloadContentConverterIsNull(
            IForgettablePayloadFinder forgettablePayloadFinder,
            IForgettablePayloadDescriptorLoader forgettablePayloadLoader,
            IForgettablePayloadForgettingService forgettingService,
            IForgettablePayloadClaimingService claimingService)
        {
            Assert.Throws<ArgumentNullException>(() => new AssignForgettablePayloadServicesPostDeserializationHook(
                forgettablePayloadFinder,
                forgettablePayloadLoader,
                forgettingService,
                claimingService,
                null));
        }
        
        [Theory]
        [AutoMoqData]
        internal void DoNothing_When_Creating_And_AllParametersAreNotNull(
            IForgettablePayloadFinder forgettablePayloadFinder,
            IForgettablePayloadDescriptorLoader forgettablePayloadLoader,
            IForgettablePayloadForgettingService forgettingService,
            IForgettablePayloadClaimingService claimingService,
            IForgettablePayloadContentConverter converter)
        {
            _ = new AssignForgettablePayloadServicesPostDeserializationHook(
                forgettablePayloadFinder,
                forgettablePayloadLoader,
                forgettingService,
                claimingService,
                converter);
        }
        
        [Theory]
        [AutoMoqData]
        internal void Throw_ArgumentNullException_When_EventDeserializedHookIsCalled_And_ProvidedEventIsNull(
            [Frozen] Mock<IForgettablePayloadFinder> forgettablePayloadFinderMock,
            AssignForgettablePayloadServicesPostDeserializationHook modifier)
        {
            Assert.Throws<ArgumentNullException>(() => modifier.PostEventDeserializationHook(null));
            
            forgettablePayloadFinderMock.VerifyNoOtherCalls();
        }
        
        [Theory]
        [AutoMoqData]
        internal void Throw_InvalidOperationException_When_EventDeserializedHookIsCalled_And_ForgettablePayloadFinderReturnsNull(
            object @event,
            [Frozen] Mock<IForgettablePayloadFinder> forgettablePayloadFinderMock,
            AssignForgettablePayloadServicesPostDeserializationHook modifier)
        {
            forgettablePayloadFinderMock
                .Setup(finder => finder.Find(@event))
                .Returns(null as IReadOnlyCollection<ForgettablePayload>)
                .Verifiable();
            
            Assert.Throws<InvalidOperationException>(() => modifier.PostEventDeserializationHook(@event));
            
            forgettablePayloadFinderMock.Verify();
            forgettablePayloadFinderMock.VerifyNoOtherCalls();
        }
        
        [Theory]
        [AutoMoqData]
        internal void DoNothing_When_EventDeserializedHookIsCalled_And_ForgettablePayloadFinderReturnsEmptyCollection(
            object @event,
            [Frozen] Mock<IForgettablePayloadFinder> forgettablePayloadFinderMock,
            AssignForgettablePayloadServicesPostDeserializationHook modifier)
        {
            forgettablePayloadFinderMock
                .Setup(finder => finder.Find(@event))
                .Returns(new List<ForgettablePayload>())
                .Verifiable();

            modifier.PostEventDeserializationHook(@event);
            
            forgettablePayloadFinderMock.Verify();
            forgettablePayloadFinderMock.VerifyNoOtherCalls();
        }

        [Theory]
        [AutoMoqData]
        internal void AssignPayloadDescriptorLoaderServiceToEveryForgettablePayloadFoundByForgettablePayloadFinder_When_EventDeserializedHookIsCalled(
            object @event,
            List<ForgettablePayload> forgettablePayloads,
            [Frozen] IForgettablePayloadDescriptorLoader forgettablePayloadLoader,
            [Frozen] Mock<IForgettablePayloadFinder> forgettablePayloadFinderMock,
            AssignForgettablePayloadServicesPostDeserializationHook modifier)
        {
            forgettablePayloadFinderMock
                .Setup(finder => finder.Find(@event))
                .Returns(forgettablePayloads)
                .Verifiable();

            modifier.PostEventDeserializationHook(@event);

            forgettablePayloadFinderMock.Verify();
            forgettablePayloadFinderMock.VerifyNoOtherCalls();

            foreach (var forgettablePayload in forgettablePayloads)
            {
                Assert.Equal(forgettablePayloadLoader, forgettablePayload.GetForgettablePayloadDescriptorLoader());
            }
        }

        [Theory]
        [AutoMoqData]
        internal void AssignPayloadForgettingServiceToEveryForgettablePayloadFoundByForgettablePayloadFinder_When_EventDeserializedHookIsCalled(
            object @event,
            List<ForgettablePayload> forgettablePayloads,
            [Frozen] IForgettablePayloadForgettingService forgettingService,
            [Frozen] Mock<IForgettablePayloadFinder> forgettablePayloadFinderMock,
            AssignForgettablePayloadServicesPostDeserializationHook modifier)
        {
            forgettablePayloadFinderMock
                .Setup(finder => finder.Find(@event))
                .Returns(forgettablePayloads)
                .Verifiable();

            modifier.PostEventDeserializationHook(@event);

            forgettablePayloadFinderMock.Verify();
            forgettablePayloadFinderMock.VerifyNoOtherCalls();

            foreach (var forgettablePayload in forgettablePayloads)
            {
                Assert.Equal(forgettingService, forgettablePayload.GetForgettablePayloadForgettingService());
            }
        }

        [Theory]
        [AutoMoqData]
        internal void AssignPayloadClaimingServiceToEveryForgettablePayloadFoundByForgettablePayloadFinder_When_EventDeserializedHookIsCalled(
            object @event,
            List<ForgettablePayload> forgettablePayloads,
            [Frozen] IForgettablePayloadClaimingService claimingService,
            [Frozen] Mock<IForgettablePayloadFinder> forgettablePayloadFinderMock,
            AssignForgettablePayloadServicesPostDeserializationHook modifier)
        {
            forgettablePayloadFinderMock
                .Setup(finder => finder.Find(@event))
                .Returns(forgettablePayloads)
                .Verifiable();

            modifier.PostEventDeserializationHook(@event);

            forgettablePayloadFinderMock.Verify();
            forgettablePayloadFinderMock.VerifyNoOtherCalls();

            foreach (var forgettablePayload in forgettablePayloads)
            {
                Assert.Equal(claimingService, forgettablePayload.GetForgettablePayloadClaimingService());
            }
        }

        [Theory]
        [AutoMoqData]
        internal void AssignPayloadContentConverterServiceToEveryForgettablePayloadFoundByForgettablePayloadFinder_When_EventDeserializedHookIsCalled(
            object @event,
            List<ForgettablePayload> forgettablePayloads,
            [Frozen] IForgettablePayloadContentConverter contentConverter,
            [Frozen] Mock<IForgettablePayloadFinder> forgettablePayloadFinderMock,
            AssignForgettablePayloadServicesPostDeserializationHook modifier)
        {
            forgettablePayloadFinderMock
                .Setup(finder => finder.Find(@event))
                .Returns(forgettablePayloads)
                .Verifiable();

            modifier.PostEventDeserializationHook(@event);

            forgettablePayloadFinderMock.Verify();
            forgettablePayloadFinderMock.VerifyNoOtherCalls();

            foreach (var forgettablePayload in forgettablePayloads)
            {
                Assert.Equal(contentConverter, forgettablePayload.GetForgettablePayloadContentConverter());
            }
        }
    }
}
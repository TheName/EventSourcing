using System;
using System.Collections.Generic;
using EventSourcing.Handling;
using Microsoft.Extensions.DependencyInjection;
using TestHelpers.Attributes;
using Xunit;

namespace EventSourcing.UnitTests.Handling
{
    public class EventHandlerProvider_Should
    {
        [Fact]
        public void Throw_When_Creating_And_ServiceProviderIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new EventHandlerProvider(null));
        }

        [Theory]
        [AutoMoqData]
        public void NotThrow_When_Creating_And_AllArgumentsAreNotNull(IServiceProvider serviceProvider)
        {
            _ = new EventHandlerProvider(serviceProvider);
        }

        [Theory]
        [AutoMoqData]
        internal void Throw_When_GettingHandlersForType_And_ProvidedTypeIsNull(EventHandlerProvider handlerProvider)
        {
            Assert.Throws<ArgumentNullException>(() => handlerProvider.GetHandlersForType(null));
        }

        [Fact]
        public void ReturnEmptyCollection_When_GettingHandlersForType_And_NoHandlersForGivenTypeAreRegistered()
        {
            var serviceProvider = new ServiceCollection().BuildServiceProvider();
            var handlerProvider = new EventHandlerProvider(serviceProvider);

            var result = handlerProvider.GetHandlersForType(typeof(object));

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnSingleHandler_When_GettingHandlersForType_And_SingleHandlerIsRegisteredForGivenType(IEventHandler<object> eventHandler)
        {
            var serviceProvider = new ServiceCollection()
                .AddTransient(_ => eventHandler)
                .BuildServiceProvider();

            var handlerProvider = new EventHandlerProvider(serviceProvider);

            var result = handlerProvider.GetHandlersForType(typeof(object));

            Assert.NotNull(result);
            var singleHandler = Assert.Single(result);
            Assert.Equal(eventHandler, singleHandler);
        }

        [Theory]
        [AutoMoqData]
        public void ReturnAllRegisteredHandlersForType_When_GettingHandlersForType(List<IEventHandler<object>> eventHandlers)
        {
            var serviceCollection = new ServiceCollection();
            foreach (var eventHandler in eventHandlers)
            {
                serviceCollection.AddTransient(_ => eventHandler);
            }

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var handlerProvider = new EventHandlerProvider(serviceProvider);

            var result = handlerProvider.GetHandlersForType(typeof(object));

            Assert.NotNull(result);
            Assert.All(eventHandlers, handler => Assert.Contains(handler, result));
        }
    }
}

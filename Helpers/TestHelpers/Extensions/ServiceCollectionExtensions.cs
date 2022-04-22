using System;
using EventSourcing.Bus.Abstractions;
using EventSourcing.Persistence.Abstractions;
using EventSourcing.Serialization.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace TestHelpers.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMock<T>(this IServiceCollection serviceCollection) where T : class =>
            serviceCollection.AddTransient(_ => new Mock<T>().Object);

        public static IServiceCollection AddEventSourcingSerializationMocks(this IServiceCollection serviceCollection) =>
            serviceCollection.AddMock<ISerializerProvider>();

        public static IServiceCollection AddEventSourcingPersistenceMocks(this IServiceCollection serviceCollection) =>
            serviceCollection
                .AddMock<IEventStreamStagingReader>()
                .AddMock<IEventStreamStagingWriter>()
                .AddMock<IEventStreamReader>()
                .AddMock<IEventStreamWriter>();

        public static IServiceCollection AddEventSourcingBusMocks(this IServiceCollection serviceCollection) =>
            serviceCollection
                .AddMock<IEventSourcingBusPublisher>()
                .AddMock<IEventSourcingBusHandlingExceptionPublisher>()
                .AddMock<IEventSourcingBusHandlingExceptionPublisherConfiguration>();

        public static IServiceCollection AddMicrosoftLoggerMock(this IServiceCollection serviceCollection) =>
            serviceCollection
                .AddTransient(typeof(ILogger<>), typeof(LoggerMock<>))
                .AddMock<ILoggerFactory>();
        
        private class LoggerMock<T> : ILogger<T>
        {
            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                throw new NotImplementedException();
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                throw new NotImplementedException();
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                throw new NotImplementedException();
            }
        }
    }
}
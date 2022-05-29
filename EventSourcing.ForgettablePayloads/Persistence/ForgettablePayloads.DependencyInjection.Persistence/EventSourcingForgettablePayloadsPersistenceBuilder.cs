using System;
using Microsoft.Extensions.DependencyInjection;

namespace EventSourcing.ForgettablePayloads.Extensions.DependencyInjection.Persistence
{
    internal class EventSourcingForgettablePayloadsPersistenceBuilder : IEventSourcingForgettablePayloadsPersistenceBuilder
    {
        private readonly IEventSourcingForgettablePayloadsBuilder _builder;
        public IServiceCollection Services => _builder.Services;

        public EventSourcingForgettablePayloadsPersistenceBuilder(IEventSourcingForgettablePayloadsBuilder builder)
        {
            _builder = builder ?? throw new ArgumentNullException(nameof(builder));
        }
    }
}
using System;
using EventSourcing.ForgettablePayloads.Abstractions.DependencyInjection;
using EventSourcing.ForgettablePayloads.Persistence.Abstractions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace EventSourcing.ForgettablePayloads.Persistence.DependencyInjection
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
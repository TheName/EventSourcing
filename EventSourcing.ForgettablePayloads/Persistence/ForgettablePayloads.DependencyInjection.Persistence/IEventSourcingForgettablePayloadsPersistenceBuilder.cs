using EventSourcing.ForgettablePayloads.Abstractions.DependencyInjection;

namespace EventSourcing.ForgettablePayloads.Extensions.DependencyInjection.Persistence
{
    /// <summary>
    /// Builder for EventSourcing.ForgettablePayloads.Persistence
    /// </summary>
    public interface IEventSourcingForgettablePayloadsPersistenceBuilder : IEventSourcingForgettablePayloadsBuilder
    {
    }
}
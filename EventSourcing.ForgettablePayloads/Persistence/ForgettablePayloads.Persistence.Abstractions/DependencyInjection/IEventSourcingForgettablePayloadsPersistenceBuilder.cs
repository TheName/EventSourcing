using EventSourcing.ForgettablePayloads.Abstractions.DependencyInjection;

namespace EventSourcing.ForgettablePayloads.Persistence.Abstractions.DependencyInjection
{
    /// <summary>
    /// Builder for EventSourcing.ForgettablePayloads.Persistence
    /// </summary>
    public interface IEventSourcingForgettablePayloadsPersistenceBuilder : IEventSourcingForgettablePayloadsBuilder
    {
    }
}
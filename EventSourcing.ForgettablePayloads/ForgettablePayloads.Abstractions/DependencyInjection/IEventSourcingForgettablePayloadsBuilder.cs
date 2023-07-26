using EventSourcing.Abstractions.DependencyInjection;

namespace EventSourcing.ForgettablePayloads.Abstractions.DependencyInjection
{
    /// <summary>
    /// Builder for EventSourcing.ForgettablePayloads
    /// </summary>
    public interface IEventSourcingForgettablePayloadsBuilder : IEventSourcingBuilder
    {
    }
}
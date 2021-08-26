using System;

namespace EventSourcing.Bus.RabbitMQ.Abstractions.Helpers
{
    internal interface IDisposingThreadLocal<out T> : IDisposable where T : IDisposable
    {
        T Value { get; }
    }
}
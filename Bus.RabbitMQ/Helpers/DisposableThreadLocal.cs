using System;
using System.Threading;

namespace EventSourcing.Bus.RabbitMQ.Helpers
{
    internal class DisposableThreadLocal<T> : ThreadLocal<T>
        where T : IDisposable
    {
        public DisposableThreadLocal(Func<T> valueFactory, bool trackAllValues) : base(valueFactory, trackAllValues)
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (IsValueCreated)
            {
                Value.Dispose();
            }
            
            base.Dispose(disposing);
        }
    }
}
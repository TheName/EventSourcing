using System;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace TestHelpers.Logging
{
    public class XUnitLogger : ILogger
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly string _name;

        public XUnitLogger(ITestOutputHelper testOutputHelper, string name)
        {
            _testOutputHelper = testOutputHelper;
            _name = string.IsNullOrWhiteSpace(name) ? nameof(XUnitLogger) : name.Trim();
        }
        
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _testOutputHelper.WriteLine($"{DateTime.Now:O} - {logLevel:G} - {eventId} - {_name} - {formatter(state, null)}");
            if (exception != null)
            {
                _testOutputHelper.WriteLine($"{exception}");
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }
}
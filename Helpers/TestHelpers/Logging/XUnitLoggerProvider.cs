using System;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace TestHelpers.Logging
{
    public class XUnitLoggerProvider : ILoggerProvider
    {
        private readonly Func<ITestOutputHelper> _testOutputHelperFunc;

        public XUnitLoggerProvider(Func<ITestOutputHelper> testOutputHelperFunc)
        {
            _testOutputHelperFunc = testOutputHelperFunc;
        }
        
        public ILogger CreateLogger(string categoryName)
        {
            return new XUnitLogger(_testOutputHelperFunc(), categoryName);
        }
        
        public void Dispose()
        {
        }
    }
}
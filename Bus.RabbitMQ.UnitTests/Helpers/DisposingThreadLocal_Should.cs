using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using EventSourcing.Bus.RabbitMQ.Helpers;
using Moq;
using TestHelpers.Attributes;
using Xunit;

namespace Bus.RabbitMQ.UnitTests.Helpers
{
    public class DisposingThreadLocal_Should
    {
        [Theory]
        [AutoMoqData]
        public void NotCreateValueUntilValueIsRequested(IDisposable disposable)
        {
            var objectCreated = false;
            var sut = new DisposingThreadLocal<IDisposable>(() =>
            {
                objectCreated = true;
                return disposable;
            });
            
            sut.Dispose();

            Assert.False(objectCreated);
        }
        
        [Theory]
        [AutoMoqData]
        public void CreateValue_When_ValueIsRequested(IDisposable disposable)
        {
            var objectCreated = false;
            var sut = new DisposingThreadLocal<IDisposable>(() =>
            {
                objectCreated = true;
                return disposable;
            });

            var result = sut.Value;
            
            Assert.Equal(disposable, result);
            Assert.True(objectCreated);
        }

        [Theory]
        [AutoMoqWithInlineData(3)]
        [AutoMoqWithInlineData(10)]
        [AutoMoqWithInlineData(100)]
        public void CreateValueOncePerThread_When_ValueIsRequested(
            int numberOfThreads,
            IDisposable disposable)
        {
            uint createdObjectsCounter = 0;
            var cancellationTokenSource = new CancellationTokenSource();
            var sut = new DisposingThreadLocal<IDisposable>(() =>
            {
                Interlocked.Increment(ref createdObjectsCounter);
                return disposable;
            });

            foreach (var i in Enumerable.Range(0, numberOfThreads))
            {
                var thread = new Thread(() =>
                {
                    _ = sut.Value;
                    Task.Delay(-1, cancellationTokenSource.Token);
                });
                
                thread.Start();
                thread.Join();
            }

            Assert.Equal(numberOfThreads, Convert.ToInt32(createdObjectsCounter));
            cancellationTokenSource.Cancel(); 
        }

        [Theory]
        [AutoMoqWithInlineData(3)]
        [AutoMoqWithInlineData(10)]
        [AutoMoqWithInlineData(100)]
        public void DisposeAllCreatedValueObjects_When_ThreadIsFinished(int numberOfThreads)
        {
            var createdObjectsCounter = -1;
            var cancellationTokenSource = new CancellationTokenSource();
            var createdMocks = Enumerable.Range(0, numberOfThreads)
                .Select(i => new Mock<IDisposable>())
                .ToList();
            
            var sut = new DisposingThreadLocal<IDisposable>(() =>
            {
                var index = Interlocked.Increment(ref createdObjectsCounter);
                var result = createdMocks[index];
                return result.Object;
            });

            foreach (var i in Enumerable.Range(0, numberOfThreads))
            {
                var thread = new Thread(() =>
                {
                    _ = sut.Value;
                    Task.Delay(-1, cancellationTokenSource.Token);
                });
                
                thread.Start();
                thread.Join();
            }

            cancellationTokenSource.Cancel(); 
            GC.GetTotalMemory(forceFullCollection: true);
            Thread.Sleep(5);
            Assert.All(createdMocks, mock => mock.Verify(disposable => disposable.Dispose(), Times.Once));
        }

        [Theory]
        [AutoMoqWithInlineData(3)]
        [AutoMoqWithInlineData(10)]
        [AutoMoqWithInlineData(100)]
        public void DisposeAllCreatedValueObjects_When_DisposingThreadLocalIsDisposed(int numberOfThreads)
        {
            var createdObjectsCounter = -1;
            var cancellationTokenSource = new CancellationTokenSource();
            var createdMocks = Enumerable.Range(0, numberOfThreads)
                .Select(i => new Mock<IDisposable>())
                .ToList();
            
            var sut = new DisposingThreadLocal<IDisposable>(() =>
            {
                var index = Interlocked.Increment(ref createdObjectsCounter);
                var result = createdMocks[index];
                return result.Object;
            });

            foreach (var i in Enumerable.Range(0, numberOfThreads))
            {
                var thread = new Thread(() =>
                {
                    _ = sut.Value;
                    Task.Delay(-1, cancellationTokenSource.Token);
                });
                
                thread.Start();
                thread.Join();
            }
            
            sut.Dispose();

            Assert.All(createdMocks, mock => mock.Verify(disposable => disposable.Dispose(), Times.Once));
            cancellationTokenSource.Cancel(); 
        }
    }
}
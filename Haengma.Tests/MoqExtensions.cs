using Moq;
using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Haengma.Tests
{
    public static class MoqExtensions
    {
        public static async Task VerifyWithTimeoutAsync<T>(this Mock<T> mock, Expression<Action<T>> expr, Times times, int timeOutInMs = 1000) where T : class
        {
            bool hasBeenExecuted = false;
            bool hasTimedOut = false;

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            while (!hasBeenExecuted && !hasTimedOut)
            {
                if (stopwatch.ElapsedMilliseconds > timeOutInMs)
                {
                    hasTimedOut = true;
                }

                try
                {
                    mock.Verify(expr, times);
                    hasBeenExecuted = true;
                }
                catch (Exception ex)
                {
                }

                // Feel free to make this configurable
                await Task.Delay(20);
            }

            if (!hasBeenExecuted)
            {
                Assert.False(true, "Couldn't verify.");
            }
        }

        public static Task VerifyAsync<T>(this Mock<Action<T>> handler, T expected, Times times, int timeOutInMs = 1000) => VerifyWithTimeoutAsync(handler, x => x(expected), times, timeOutInMs);
        public static Task VerifyAsync<T>(this Mock<Action<T>> handler, Times times, int timeOutInMs = 1000) => VerifyWithTimeoutAsync(handler, x => x(It.IsAny<T>()), times, timeOutInMs);

        public static async Task<T> VerifyAndGetValueAsync<T>(this Mock<Action<T>> handler, Times times, int timeOutInMs = 1000)
        {
            await handler.VerifyAsync(times, timeOutInMs);
            return handler.GetValue();
        }

        public static T GetValue<T>(this Mock<Action<T>> handler) => handler.Invocations.SelectMany(x => x.Arguments).OfType<T>().Single();
    }
}

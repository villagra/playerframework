using System;
using System.Threading.Tasks;
using System.Threading;

namespace Microsoft.PlayerFramework.Advertising
{
    internal static class TaskHelpers
    {
        public static CancellationToken CreateTimeout(TimeSpan? timeout)
        {
            var cts = new CancellationTokenSource();
            if (timeout.HasValue)
            {
                cts.CancelAfter(timeout.Value);
            }
            return cts.Token;
        }

        internal static T Create<T>(Func<T> source)
        {
            return source();
        }

        internal static bool IsRunning(this Task source)
        {
            return source != null && !(source.IsCanceled || source.IsFaulted || source.IsCompleted);
        }


        internal static Task<T> FromEvent<T>(Action<EventHandler<T>> addHandlerAction, Action<EventHandler<T>> removeHandlerAction)
        {
            var tcs = new TaskCompletionSource<T>();
            EventHandler<T> completedEvent = null;
            completedEvent = (s, e) =>
            {
                removeHandlerAction(completedEvent);
                tcs.SetResult(e);
            };
            addHandlerAction(completedEvent);
            return tcs.Task;
        }

        internal static Task<T> FromEvent<T>(Action<EventHandler<T>> addHandlerAction, Action<EventHandler<T>> removeHandlerAction, CancellationToken token)
        {
            var tcs = new TaskCompletionSource<T>();
            EventHandler<T> completedEvent = null;
            completedEvent = (s, e) =>
            {
                removeHandlerAction(completedEvent);
                tcs.SetResult(e);
            };
            addHandlerAction(completedEvent);
            if (token != null) token.Register(() =>
            {
                removeHandlerAction(completedEvent);
                tcs.TrySetCanceled();
            });
            return tcs.Task;
        }

        internal static Task AsTask(this CancellationToken cancellationToken)
        { 
            var tcs = new TaskCompletionSource<object>();
            cancellationToken.Register(() => tcs.TrySetResult(null));
            return tcs.Task;
        }
    }
}

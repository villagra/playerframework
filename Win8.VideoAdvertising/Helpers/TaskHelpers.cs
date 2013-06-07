using System;
using System.Threading.Tasks;
using System.Threading;

namespace Microsoft.VideoAdvertising
{
    internal static class TaskHelpers
    {
        internal static T Create<T>(Func<T> source)
        {
            return source();
        }

        internal static bool IsRunning(this Task source)
        {
            return source != null && !(source.IsCanceled || source.IsFaulted || source.IsCompleted);
        }

#if SILVERLIGHT
        internal static Task<EventArgs> FromEvent(Action<EventHandler> addHandlerAction, Action<EventHandler> removeHandlerAction)
        {
            var tcs = new TaskCompletionSource<EventArgs>();
            EventHandler completedEvent = null;
            completedEvent = (s, e) =>
            {
                removeHandlerAction(completedEvent);
                tcs.SetResult(e);
            };
            addHandlerAction(completedEvent);
            return tcs.Task;
        }

        internal static Task<T> FromEvent<T>(Action<EventHandler<T>> addHandlerAction, Action<EventHandler<T>> removeHandlerAction) where T : EventArgs
#else
        internal static Task<T> FromEvent<T>(Action<EventHandler<T>> addHandlerAction, Action<EventHandler<T>> removeHandlerAction)
#endif
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

#if SILVERLIGHT
        internal static Task<EventArgs> FromEvent(Action<EventHandler> addHandlerAction, Action<EventHandler> removeHandlerAction, CancellationToken token)
        {
            var tcs = new TaskCompletionSource<EventArgs>();
            EventHandler completedEvent = null;
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

        internal static Task<T> FromEvent<T>(Action<EventHandler<T>> addHandlerAction, Action<EventHandler<T>> removeHandlerAction, CancellationToken token) where T : EventArgs
#else
        internal static Task<T> FromEvent<T>(Action<EventHandler<T>> addHandlerAction, Action<EventHandler<T>> removeHandlerAction, CancellationToken token)
#endif
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

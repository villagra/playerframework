using System;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Microsoft.PlayerFramework
{
    internal static class DispatcherExtensions
    {
        public static async Task InvokeAsync(this Dispatcher source, Action action)
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
            source.BeginInvoke(() =>
            {
                action();
                tcs.SetResult(null);
            });
            await tcs.Task;
        }
    }
}

using System;
using System.Threading.Tasks;
using System.Threading;
#if NETFX_CORE
using Windows.Foundation;
using System.Runtime.InteropServices.WindowsRuntime;
#endif

namespace Microsoft.VideoAdvertising
{
    public static class VpaidExtensions
    {
        internal static Task<Exception> GetErrorTask(this IVpaid vpaid, CancellationToken cancellationToken)
        {
            return TaskHelpers.FromEvent<VpaidMessageEventArgs>(eh => vpaid.AdError += eh, eh => vpaid.AdError -= eh, cancellationToken).ContinueWith(t => new Exception(t.Result.Message), TaskContinuationOptions.OnlyOnRanToCompletion);
        }

#if SILVERLIGHT
        internal static Task<EventArgs> GetStartedTask(this IVpaid vpaid, CancellationToken cancellationToken)
        {
            return TaskHelpers.FromEvent(eh => vpaid.AdStarted += eh, eh => vpaid.AdStarted -= eh, cancellationToken);
        }

        internal static Task<EventArgs> GetLoadedTask(this IVpaid vpaid, CancellationToken cancellationToken)
        {
            return TaskHelpers.FromEvent(eh => vpaid.AdLoaded += eh, eh => vpaid.AdLoaded -= eh, cancellationToken);
        }

        internal static Task<EventArgs> GetStoppedTask(this IVpaid vpaid, CancellationToken cancellationToken)
        {
            return TaskHelpers.FromEvent(eh => vpaid.AdStopped += eh, eh => vpaid.AdStopped -= eh, cancellationToken);
        }

        internal static Task<EventArgs> GetApproachingEndTask(this IVpaid vpaid, CancellationToken cancellationToken)
        {
            return TaskHelpers.FromEvent(eh => vpaid.AdVideoThirdQuartile += eh, eh => vpaid.AdVideoThirdQuartile -= eh, cancellationToken);
        }
#else
        internal static Task<object> GetStartedTask(this IVpaid vpaid, CancellationToken cancellationToken)
        {
            return TaskHelpers.FromEvent<object>(eh => vpaid.AdStarted += eh, eh => vpaid.AdStarted -= eh, cancellationToken);
        }

        internal static Task<object> GetLoadedTask(this IVpaid vpaid, CancellationToken cancellationToken)
        {
            return TaskHelpers.FromEvent<object>(eh => vpaid.AdLoaded += eh, eh => vpaid.AdLoaded -= eh, cancellationToken);
        }

        internal static Task<object> GetStoppedTask(this IVpaid vpaid, CancellationToken cancellationToken)
        {
            return TaskHelpers.FromEvent<object>(eh => vpaid.AdStopped += eh, eh => vpaid.AdStopped -= eh, cancellationToken);
        }

        internal static Task<object> GetApproachingEndTask(this IVpaid vpaid, CancellationToken cancellationToken)
        {
            return TaskHelpers.FromEvent<object>(eh => vpaid.AdVideoThirdQuartile += eh, eh => vpaid.AdVideoThirdQuartile -= eh, cancellationToken);
        }
#endif


#if NETFX_CORE
        public static IAsyncAction InitAdAsync(this IVpaid vpaid, double width, double height, string viewMode, int desiredBitrate, string creativeData, string environmentVariables)
        {
            return AsyncInfo.Run(c => vpaid.InitAdAsync(width, height, viewMode, desiredBitrate, creativeData, environmentVariables, c));
        }
#endif

#if SILVERLIGHT
        public static async Task InitAdAsync(this IVpaid vpaid, double width, double height, string viewMode, int desiredBitrate, string creativeData, string environmentVariables, CancellationToken cancellationToken)
#else
        internal static async Task InitAdAsync(this IVpaid vpaid, double width, double height, string viewMode, int desiredBitrate, string creativeData, string environmentVariables, CancellationToken cancellationToken)
#endif
        {
            var errorTask = vpaid.GetErrorTask(cancellationToken);
            var loadedTask = vpaid.GetLoadedTask(cancellationToken);
            var cancellationTask = cancellationToken.AsTask();

            vpaid.InitAd(width, height, viewMode, desiredBitrate, creativeData, environmentVariables);

#if SILVERLIGHT && !WINDOWS_PHONE || WINDOWS_PHONE7
            var completedTask = await TaskEx.WhenAny(loadedTask, errorTask, cancellationTask);
#else
            var completedTask = await Task.WhenAny(loadedTask, errorTask, cancellationTask);
#endif
            if (completedTask == errorTask)
            {
                throw errorTask.Result;
            }
            else if (completedTask == cancellationTask)
            {
                vpaid.StopAd();
            }
            cancellationToken.ThrowIfCancellationRequested();
        }

#if NETFX_CORE
        public static IAsyncAction StartAdAsync(this IVpaid vpaid)
        {
            return AsyncInfo.Run(c => vpaid.StartAdAsync(c));
        }
#endif

#if SILVERLIGHT
        public static async Task StartAdAsync(this IVpaid vpaid, CancellationToken cancellationToken)
#else
        internal static async Task StartAdAsync(this IVpaid vpaid, CancellationToken cancellationToken)
#endif
        {
            var errorTask = vpaid.GetErrorTask(cancellationToken);
            var startedTask = vpaid.GetStartedTask(cancellationToken);
            var cancellationTask = cancellationToken.AsTask();

            vpaid.StartAd();

#if SILVERLIGHT && !WINDOWS_PHONE || WINDOWS_PHONE7
            var completedTask = await TaskEx.WhenAny(startedTask, errorTask, cancellationTask);
#else
            var completedTask = await Task.WhenAny(startedTask, errorTask, cancellationTask);
#endif
            if (completedTask == errorTask)
            {
                throw errorTask.Result;
            }
            else if (completedTask == cancellationTask)
            {
                vpaid.StopAd();
            }
            cancellationToken.ThrowIfCancellationRequested();
        }

#if NETFX_CORE
        public static IAsyncOperation<bool> PlayAdAsync(this IVpaid vpaid)
        {
            return AsyncInfo.Run(c => vpaid.PlayAdAsync(c));
        }
#endif

#if SILVERLIGHT
        public static async Task<bool> PlayAdAsync(this IVpaid vpaid, CancellationToken cancellationToken)
#else
        internal static async Task<bool> PlayAdAsync(this IVpaid vpaid, CancellationToken cancellationToken)
#endif
        {
            var errorTask = vpaid.GetErrorTask(cancellationToken);
            var stoppedTask = vpaid.GetStoppedTask(cancellationToken);
            var approachingEndTask = vpaid.GetApproachingEndTask(cancellationToken);
            var cancellationTask = cancellationToken.AsTask();

#if SILVERLIGHT && !WINDOWS_PHONE || WINDOWS_PHONE7
            var completedTask = await TaskEx.WhenAny(stoppedTask, approachingEndTask, errorTask, cancellationTask);
#else
            var completedTask = await Task.WhenAny(stoppedTask, approachingEndTask, errorTask, cancellationTask);
#endif
            bool result = true;
            if (completedTask == errorTask)
            {
                throw errorTask.Result;
            }
            else if (completedTask == cancellationTask)
            {
                vpaid.StopAd();
            }
            else if (completedTask == approachingEndTask)
            {
                result = false;
            }
            cancellationToken.ThrowIfCancellationRequested();
            return result;
        }

#if NETFX_CORE
        public static IAsyncAction FinishAdAsync(this IVpaid vpaid)
        {
            return AsyncInfo.Run(c => vpaid.FinishAdAsync(c));
        }
#endif

#if SILVERLIGHT
        public static async Task FinishAdAsync(this IVpaid vpaid, CancellationToken cancellationToken)
#else
        internal static async Task FinishAdAsync(this IVpaid vpaid, CancellationToken cancellationToken)
#endif
        {
            var errorTask = vpaid.GetErrorTask(cancellationToken);
            var stoppedTask = vpaid.GetStoppedTask(cancellationToken);
            var cancellationTask = cancellationToken.AsTask();

#if SILVERLIGHT && !WINDOWS_PHONE || WINDOWS_PHONE7
            var completedTask = await TaskEx.WhenAny(stoppedTask, errorTask, cancellationTask);
#else
            var completedTask = await Task.WhenAny(stoppedTask, errorTask, cancellationTask);
#endif
            if (completedTask == errorTask)
            {
                throw errorTask.Result;
            }
            else if (completedTask == cancellationTask)
            {
                vpaid.StopAd();
            }
            cancellationToken.ThrowIfCancellationRequested();
        }

#if NETFX_CORE
        public static IAsyncAction StopAdAsync(this IVpaid vpaid)
        {
            return AsyncInfo.Run(c => vpaid.StopAdAsync(c));
        }
#endif

#if SILVERLIGHT
        public static async Task StopAdAsync(this IVpaid vpaid, CancellationToken cancellationToken)
#else
        internal static async Task StopAdAsync(this IVpaid vpaid, CancellationToken cancellationToken)
#endif
        {
            var errorTask = vpaid.GetErrorTask(cancellationToken);
            var stoppedTask = vpaid.GetStoppedTask(cancellationToken);
            var cancellationTask = cancellationToken.AsTask();

            vpaid.StopAd();
#if SILVERLIGHT && !WINDOWS_PHONE || WINDOWS_PHONE7
            var completedTask = await TaskEx.WhenAny(stoppedTask, errorTask, cancellationTask);
#else
            var completedTask = await Task.WhenAny(stoppedTask, errorTask, cancellationTask);
#endif
            if (completedTask == errorTask)
            {
                throw errorTask.Result;
            }
            cancellationToken.ThrowIfCancellationRequested();
        }

    }
}

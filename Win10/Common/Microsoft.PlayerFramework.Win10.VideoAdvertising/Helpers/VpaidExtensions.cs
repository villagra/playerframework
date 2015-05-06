using System;
using System.Threading.Tasks;
using System.Threading;
using Windows.Foundation;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Microsoft.VideoAdvertising
{
    public static class VpaidExtensions
    {
        internal static Task<Exception> GetErrorTask(this IVpaid vpaid, CancellationToken cancellationToken)
        {
            return TaskHelpers.FromEvent<VpaidMessageEventArgs>(eh => vpaid.AdError += eh, eh => vpaid.AdError -= eh, cancellationToken).ContinueWith(t => new Exception(t.Result.Message), TaskContinuationOptions.OnlyOnRanToCompletion);
        }

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

        public static IAsyncAction InitAdAsync(this IVpaid vpaid, double width, double height, string viewMode, int desiredBitrate, string creativeData, string environmentVariables)
        {
            return AsyncInfo.Run(c => vpaid.InitAdAsync(width, height, viewMode, desiredBitrate, creativeData, environmentVariables, c));
        }

        internal static async Task InitAdAsync(this IVpaid vpaid, double width, double height, string viewMode, int desiredBitrate, string creativeData, string environmentVariables, CancellationToken cancellationToken)
        {
            var errorTask = vpaid.GetErrorTask(cancellationToken);
            var loadedTask = vpaid.GetLoadedTask(cancellationToken);
            var cancellationTask = cancellationToken.AsTask();

            vpaid.InitAd(width, height, viewMode, desiredBitrate, creativeData, environmentVariables);

            var completedTask = await Task.WhenAny(loadedTask, errorTask, cancellationTask);
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

        public static IAsyncAction StartAdAsync(this IVpaid vpaid)
        {
            return AsyncInfo.Run(c => vpaid.StartAdAsync(c));
        }

        internal static async Task StartAdAsync(this IVpaid vpaid, CancellationToken cancellationToken)
        {
            var errorTask = vpaid.GetErrorTask(cancellationToken);
            var startedTask = vpaid.GetStartedTask(cancellationToken);
            var cancellationTask = cancellationToken.AsTask();

            vpaid.StartAd();

            var completedTask = await Task.WhenAny(startedTask, errorTask, cancellationTask);
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

        public static IAsyncOperation<bool> PlayAdAsync(this IVpaid vpaid)
        {
            return AsyncInfo.Run(c => vpaid.PlayAdAsync(c));
        }

        internal static async Task<bool> PlayAdAsync(this IVpaid vpaid, CancellationToken cancellationToken)
        {
            var errorTask = vpaid.GetErrorTask(cancellationToken);
            var stoppedTask = vpaid.GetStoppedTask(cancellationToken);
            var approachingEndTask = vpaid.GetApproachingEndTask(cancellationToken);
            var cancellationTask = cancellationToken.AsTask();
            var completedTask = await Task.WhenAny(stoppedTask, approachingEndTask, errorTask, cancellationTask);
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

        public static IAsyncAction FinishAdAsync(this IVpaid vpaid)
        {
            return AsyncInfo.Run(c => vpaid.FinishAdAsync(c));
        }

        internal static async Task FinishAdAsync(this IVpaid vpaid, CancellationToken cancellationToken)
        {
            var errorTask = vpaid.GetErrorTask(cancellationToken);
            var stoppedTask = vpaid.GetStoppedTask(cancellationToken);
            var cancellationTask = cancellationToken.AsTask();
            var completedTask = await Task.WhenAny(stoppedTask, errorTask, cancellationTask);

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

        public static IAsyncAction StopAdAsync(this IVpaid vpaid)
        {
            return AsyncInfo.Run(c => vpaid.StopAdAsync(c));
        }

        internal static async Task StopAdAsync(this IVpaid vpaid, CancellationToken cancellationToken)
        {
            var errorTask = vpaid.GetErrorTask(cancellationToken);
            var stoppedTask = vpaid.GetStoppedTask(cancellationToken);
            var cancellationTask = cancellationToken.AsTask();

            vpaid.StopAd();

            var completedTask = await Task.WhenAny(stoppedTask, errorTask, cancellationTask);
            if (completedTask == errorTask)
            {
                throw errorTask.Result;
            }
            cancellationToken.ThrowIfCancellationRequested();
        }

    }
}

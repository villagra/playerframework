using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
#if SILVERLIGHT
using System.Windows.Media;
#else
using Windows.Storage.Streams;
#endif

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// Provides data for a deferrable MediaLoading event.
    /// </summary>
    public class MediaLoadingEventArgs : MediaPlayerDeferrableEventArgs
    {
        internal MediaLoadingEventArgs(MediaPlayerDeferrableOperation deferrableOperation, Uri source)
            : base(deferrableOperation)
        {
            Source = source;
        }

        /// <summary>
        /// Gets or sets the source Uri of the loading operation. This may be null if the source is a stream.
        /// </summary>
        public Uri Source { get; set; }

#if !SILVERLIGHT
        internal MediaLoadingEventArgs(MediaPlayerDeferrableOperation deferrableOperation, IRandomAccessStream sourceStream, string mimeType)
            : base(deferrableOperation)
        {
            SourceStream = sourceStream;
            MimeType = mimeType;
        }
        
#if !WINDOWS80
        internal MediaLoadingEventArgs(MediaPlayerDeferrableOperation deferrableOperation, Windows.Media.Core.IMediaSource source)
            : base(deferrableOperation)
        {
            MediaStreamSource = source;
        }

        /// <summary>
        /// Gets or sets the media stream source of the loading operation. This may be null if the source is a Uri.
        /// </summary>
        public Windows.Media.Core.IMediaSource MediaStreamSource { get; set; }
#endif

        /// <summary>
        /// Gets or sets the source stream of the loading operation. This may be null if the source is a Uri.
        /// </summary>
        public IRandomAccessStream SourceStream { get; set; }

        /// <summary>
        /// Gets or sets the mime type of the loading operation. This is unused if the source is a Uri.
        /// </summary>
        public string MimeType { get; set; }
#else

        internal MediaLoadingEventArgs(MediaPlayerDeferrableOperation deferrableOperation, Stream sourceStream)
            : base(deferrableOperation)
        {
            SourceStream = sourceStream;
        }

        /// <summary>
        /// Gets or sets the source MediaStreamSource of the loading operation. This may be null if the source is a Uri or MediaStreamSource.
        /// </summary>
        public Stream SourceStream { get; set; }

        internal MediaLoadingEventArgs(MediaPlayerDeferrableOperation deferrableOperation, MediaStreamSource mediaStreamSource)
            : base(deferrableOperation)
        {
            MediaStreamSource = mediaStreamSource;
        }

        /// <summary>
        /// Gets or sets the source MediaStreamSource of the loading operation. This may be null if the source is a stream or Uri.
        /// </summary>
        public MediaStreamSource MediaStreamSource { get; set; }
#endif
    }

    /// <summary>
    /// Provides data for a deferrable event.
    /// </summary>
    public class MediaPlayerDeferrableEventArgs : EventArgs
    {
        internal MediaPlayerDeferrableEventArgs(MediaPlayerDeferrableOperation deferrableOperation)
        {
            DeferrableOperation = deferrableOperation;
        }

        /// <summary>
        /// Gets the deferrable operation.
        /// </summary>
        public MediaPlayerDeferrableOperation DeferrableOperation { get; private set; }
    }

    /// <summary>
    /// Provides info about a deferrable operation.
    /// </summary>
    public class MediaPlayerDeferrableOperation
    {
        readonly CancellationTokenSource cts;
        readonly List<MediaPlayerDeferral> deferrals;

        internal MediaPlayerDeferrableOperation(CancellationTokenSource cancellationTokenSource)
        {
            cts = cancellationTokenSource;
            deferrals = new List<MediaPlayerDeferral>();
        }

        internal bool DeferralsExist
        {
            get
            {
                return deferrals.Any();
            }
        }

        internal Task<bool[]> Task
        {
            get
            {
#if SILVERLIGHT && !WINDOWS_PHONE || WINDOWS_PHONE7
                return System.Threading.Tasks.TaskEx.WhenAll(deferrals.Select(d => d.Task));
#else
                return System.Threading.Tasks.Task.WhenAll(deferrals.Select(d => d.Task));
#endif
            }
        }

        internal void Cancel()
        {
            cts.Cancel();
        }

        /// <summary>
        /// Requests that the deferrable operation be delayed.
        /// </summary>
        /// <returns>The deferral.</returns>
        public MediaPlayerDeferral GetDeferral()
        {
            var result = new MediaPlayerDeferral(cts.Token);
            deferrals.Add(result);
            return result;
        }
    }

    /// <summary>
    /// Manages a delayed app deferrable operation.
    /// </summary>
    public class MediaPlayerDeferral
    {
        readonly TaskCompletionSource<bool> tcs;

        internal MediaPlayerDeferral(CancellationToken cancellationToken)
        {
            tcs = new TaskCompletionSource<bool>();
            CancellationToken = cancellationToken;
        }

        internal Task<bool> Task
        {
            get
            {
                return tcs.Task;
            }
        }

        /// <summary>
        /// Gets the CancellationToken associated with this class.
        /// </summary>
        public CancellationToken CancellationToken { get; private set; }

        /// <summary>
        /// Notifies the MediaPlayer that the operation is complete.
        /// </summary>
        public void Complete()
        {
            if (CancellationToken.IsCancellationRequested) tcs.SetCanceled();
            else tcs.TrySetResult(true);
        }

        /// <summary>
        /// Notifies the MediaPlayer that the operation should be cancelled.
        /// </summary>
        public void Cancel()
        {
            if (CancellationToken.IsCancellationRequested) tcs.SetCanceled();
            else tcs.TrySetResult(false);
        }
    }
}

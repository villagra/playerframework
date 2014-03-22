using ADMS.Measurement;
using Microsoft.Media.Analytics;
using System;

namespace Microsoft.Media.Analytics.SiteCatalyst
{
    /// <summary>
    /// http://microsite.omniture.com/t2/help/en_US/sc/appmeasurement/winrt/index.html#Video_Measurement_Quick_Start
    /// </summary>
    public sealed class SiteCatalyistLoggingTarget : ILoggingTarget
    {
        double playbackRate;

        public SiteCatalyistLoggingTarget()
        {
            PlayerName = "PlayerFramework";
            playbackRate = DefaultPlaybackRate = 1.0;
        }

        /// <summary>
        /// Gets or sets the PlayerName parameter used on MediaMeasurement.open(...)
        /// Default is "PlayerFramework"
        /// </summary>
        public string PlayerName { get; set; }

        /// <summary>
        /// Gets or sets the optional PlayerID parameter used on MediaMeasurement.open(...)
        /// </summary>
        public string PlayerId { get; set; }

        /// <summary>
        /// Gets or sets the default playback rate. This is 1 by default but can be changed to indicate what "normal" speed is.
        /// </summary>
        public double DefaultPlaybackRate { get; set; }

        /// <summary>
        /// Gets or sets whether the module should be ran in debug mode. If true, logs are sent to Debug window instead of server.
        /// </summary>
        public bool Debug { get; set; }

        public static string VideoIdKey { get { return "VideoId"; } }

        public void LogEntry(ILog log)
        {
            var mediaMeasure = ADMS_MediaMeasurement.Instance;
            var videoId = log.ExtraData[VideoIdKey] as string;

            if (log is StreamEventLog)
            {
                var streamEventLog = (StreamEventLog)log;

                switch (streamEventLog.StreamEventType)
                {
                    case StreamEventType.Loaded:
                        playbackRate = DefaultPlaybackRate; // reset this
                        if (string.IsNullOrEmpty(PlayerId))
                        {
                            if (Debug) System.Diagnostics.Debug.WriteLine("open({0}, {1}, {2})", videoId, streamEventLog.Duration.TotalSeconds, PlayerName);
                            mediaMeasure.open(videoId, streamEventLog.Duration.TotalSeconds, PlayerName);
                        }
                        else
                        {

                            if (Debug) System.Diagnostics.Debug.WriteLine("open({0}, {1}, {2}, {3})", videoId, streamEventLog.Duration.TotalSeconds, PlayerName, PlayerId);
                            mediaMeasure.open(videoId, streamEventLog.Duration.TotalSeconds, PlayerName, PlayerId);
                        }
                        break;
                    case StreamEventType.Ended:
                        if (Debug) System.Diagnostics.Debug.WriteLine("complete({0}, {1})", videoId, streamEventLog.Position.TotalSeconds);
                        mediaMeasure.complete(videoId, streamEventLog.Position.TotalSeconds);
                        break;
                    case StreamEventType.Failed:
                    case StreamEventType.Unloaded:
                        if (Debug) System.Diagnostics.Debug.WriteLine("close({0})", videoId);
                        mediaMeasure.close(videoId);
                        break;
                }
            }
            else if (log is DvrOperationLog)
            {
                var dvrOperationLog = (DvrOperationLog)log;
                switch (dvrOperationLog.OperationType)
                {
                    case DvrOperationType.PlayrateChanged:
                        if (!dvrOperationLog.IsPaused)
                        {
                            if (dvrOperationLog.PlaybackRate == DefaultPlaybackRate)
                            {
                                if (Debug) System.Diagnostics.Debug.WriteLine("play({0}, {1})", videoId, dvrOperationLog.Position.TotalSeconds);
                                mediaMeasure.play(videoId, dvrOperationLog.Position.TotalSeconds);
                            }
                            else if (playbackRate == DefaultPlaybackRate) // we were playing at normal speeed
                            {
                                if (Debug) System.Diagnostics.Debug.WriteLine("stop({0}, {1})", videoId, dvrOperationLog.Position.TotalSeconds);
                                mediaMeasure.stop(videoId, dvrOperationLog.Position.TotalSeconds);
                            }
                            playbackRate = dvrOperationLog.PlaybackRate;
                        }
                        break;
                    case DvrOperationType.ScrubCompleted:
                        if (!dvrOperationLog.IsPaused)
                        {
                            if (Debug) System.Diagnostics.Debug.WriteLine("play({0}, {1})", videoId, dvrOperationLog.Position.TotalSeconds);
                            mediaMeasure.play(videoId, dvrOperationLog.Position.TotalSeconds);
                        }
                        break;
                    case DvrOperationType.ScrubStarted:
                        if (!dvrOperationLog.IsPaused)
                        {
                            if (Debug) System.Diagnostics.Debug.WriteLine("stop({0}, {1})", videoId, dvrOperationLog.Position.TotalSeconds);
                            mediaMeasure.stop(videoId, dvrOperationLog.Position.TotalSeconds);
                        }
                        break;
                    case DvrOperationType.Seeked:
                        if (!dvrOperationLog.IsPaused)
                        {
                            if (Debug) System.Diagnostics.Debug.WriteLine("stop({0}, {1})", videoId, dvrOperationLog.PreviousPosition.GetValueOrDefault(TimeSpan.Zero).TotalSeconds);
                            mediaMeasure.stop(videoId, dvrOperationLog.Position.TotalSeconds);
                            if (Debug) System.Diagnostics.Debug.WriteLine("play({0}, {1})", videoId, dvrOperationLog.Position.TotalSeconds);
                            mediaMeasure.play(videoId, dvrOperationLog.Position.TotalSeconds);
                        }
                        break;
                    case DvrOperationType.Play:
                        if (Debug) System.Diagnostics.Debug.WriteLine("play({0}, {1})", videoId, dvrOperationLog.Position.TotalSeconds);
                        mediaMeasure.play(videoId, dvrOperationLog.Position.TotalSeconds);
                        break;
                    case DvrOperationType.Pause:
                        if (Debug) System.Diagnostics.Debug.WriteLine("stop({0}, {1})", videoId, dvrOperationLog.Position.TotalSeconds);
                        mediaMeasure.stop(videoId, dvrOperationLog.Position.TotalSeconds);
                        break;
                }
            }
        }
    }
}

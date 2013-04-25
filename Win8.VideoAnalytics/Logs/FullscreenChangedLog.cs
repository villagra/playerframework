using System;
using System.Collections.Generic;

namespace Microsoft.VideoAnalytics
{
    /// <summary>
    /// A log generated when the user goes in or out of fullscreen mode.
    /// </summary>
    public sealed class FullscreenChangedLog : ILog
    {
        /// <summary>
        /// Creates a new instance of FullscreenChangedLog.
        /// </summary>
        /// <param name="isFullScreen">Whether or not the media is currently playing back in fullscreen mode</param>
        public FullscreenChangedLog(bool isFullScreen)
        {
            TimeStamp = DateTimeOffset.Now;
            Type = EventTypes.FullScreenChanged;
            IsFullScreen = isFullScreen;
            Id = Guid.NewGuid();
            ExtraData = new Dictionary<string, object>();
        }

        /// <inheritdoc /> 
        public IDictionary<string, object> ExtraData { get; private set; }

        /// <inheritdoc /> 
        public Guid Id { get; set; }

        /// <inheritdoc /> 
        public DateTimeOffset TimeStamp { get; set; }

        /// <inheritdoc /> 
        public string Type { get; private set; }

        /// <inheritdoc /> 
        public IDictionary<string, object> GetData()
        {
            var result = this.CreateBasicLogData();
            result.Add("IsFullScreen", IsFullScreen);
            return result;
        }

        /// <summary>
        /// Gets if the player was changing to fullscreen or not.
        /// </summary>
        public bool IsFullScreen { get; private set; }
    }
}

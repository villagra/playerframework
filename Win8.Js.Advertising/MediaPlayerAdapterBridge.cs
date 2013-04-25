using System;
using Microsoft.VideoAdvertising;
using Windows.Foundation;

namespace Microsoft.PlayerFramework.Js.Advertising
{
    /// <summary>
    /// Provides an adapter between the advertising component and the player.
    /// </summary>
    public sealed class MediaPlayerAdapterBridge : IPlayer
    {
        int currentBitrate;
        int IPlayer.CurrentBitrate
        {
            get
            {
                if (CurrentBitrateRequested != null)
                {
                    var args = new CurrentBitrateRequestedEventArgs();
                    CurrentBitrateRequested(this, args);
                    return args.Result;
                }
                else return currentBitrate;
            }
            set 
            {
                currentBitrate = value;
            }
        }

        TimeSpan IPlayer.CurrentPosition
        {
            get
            {
                if (CurrentPositionRequested != null)
                {
                    var args = new CurrentPositionRequestedEventArgs();
                    CurrentPositionRequested(this, args);
                    return args.Result;
                }
                else return TimeSpan.Zero;
            }
        }

        Size dimensions;
        /// <inheritdoc /> 
        public Size Dimensions
        {
            get { return dimensions; }
            set
            {
                dimensions = value;
                if (DimensionsChanged != null) DimensionsChanged(this, EventArgs.Empty);
            }
        }

        bool isFullScreen;
        /// <inheritdoc /> 
        public bool IsFullScreen
        {
            get { return isFullScreen; }
            set
            {
                isFullScreen = value;
                if (FullscreenChanged != null) FullscreenChanged(this, EventArgs.Empty);
            }
        }

        double volume;
        /// <inheritdoc /> 
        public double Volume
        {
            get { return volume; }
            set
            {
                volume = value;
                if (VolumeChanged != null) VolumeChanged(this, EventArgs.Empty);
            }
        }

        bool isMuted;
        /// <inheritdoc /> 
        public bool IsMuted
        {
            get { return isMuted; }
            set
            {
                isMuted = value;
                if (IsMutedChanged != null) IsMutedChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raised when the current bitrate is requested
        /// </summary>
        public event EventHandler<CurrentBitrateRequestedEventArgs> CurrentBitrateRequested;

        /// <summary>
        /// Raised when the current position is requested
        /// </summary>
        public event EventHandler<CurrentPositionRequestedEventArgs> CurrentPositionRequested;

        /// <inheritdoc /> 
        public event EventHandler<object> FullscreenChanged;

        /// <inheritdoc /> 
        public event EventHandler<object> DimensionsChanged;

        /// <inheritdoc /> 
        public event EventHandler<object> VolumeChanged;

        /// <inheritdoc /> 
        public event EventHandler<object> IsMutedChanged;
    }

    /// <summary>
    /// Provides a way to return the current bitrate when the CurrentBitrateRequested event is raised.
    /// </summary>
    public sealed class CurrentBitrateRequestedEventArgs
    {
        internal CurrentBitrateRequestedEventArgs()
        { }

        /// <summary>
        /// The current bitrate. This should be set by the handler of the event.
        /// </summary>
        public int Result { get; set; }
    }

    /// <summary>
    /// Provides a way to return the current position when the CurrentPositionRequested event is raised.
    /// </summary>
    public sealed class CurrentPositionRequestedEventArgs
    {
        internal CurrentPositionRequestedEventArgs()
        { }

        /// <summary>
        /// The current Position. This should be set by the handler of the event.
        /// </summary>
        public TimeSpan Result { get; set; }
    }
}
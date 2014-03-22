using System;
using Microsoft.Media.Advertising;

namespace Microsoft.PlayerFramework.Js.Advertising
{
    /// <summary>
    /// Provides an adapter between the player and the MAST mainsail ad scheduler.
    /// </summary>
    public sealed class MastAdapter : IMastAdapter
    {
        DateTime startPlayTimestamp;
        TimeSpan totalWatchedTime = TimeSpan.Zero;
        TimeSpan watchedTime = TimeSpan.Zero;
        int itemCount = 0;
        bool isPlaying = false;
        bool isPaused = false;

        /// <summary>
        /// Indicates that playback has started
        /// </summary>
        public void InvokePlay()
        {
            isPaused = false;
            isPlaying = true;
            startPlayTimestamp = DateTime.Now; 
            if (OnPlay != null) OnPlay(this, EventArgs.Empty);
        }

        /// <inheritdoc /> 
        public event EventHandler<object> OnPlay;

        /// <summary>
        /// Indicates that playback has stopped
        /// </summary>
        public void InvokeStop()
        {
            if (isPlaying)  // added just to be safe so we don't somehow run this more than once
            {
                isPlaying = false;
                watchedTime = TimeSpan.Zero;
                totalWatchedTime = totalWatchedTime.Add(DateTime.Now.Subtract(startPlayTimestamp));
            }
            if (OnStop != null) OnStop(this, EventArgs.Empty);
        }

        /// <inheritdoc /> 
        public event EventHandler<object> OnStop;

        /// <summary>
        /// Indicates that playback has been paused.
        /// </summary>
        public void InvokePause()
        {
            if (isPlaying)  // added just to be safe so we don't somehow run this more than once
            {
                isPaused = true;
                isPlaying = false;
                var now = DateTime.Now;
                watchedTime = watchedTime.Add(now.Subtract(startPlayTimestamp));
                totalWatchedTime = totalWatchedTime.Add(now.Subtract(startPlayTimestamp));
            }
            if (OnPause != null) OnPause(this, EventArgs.Empty);
        }

        /// <inheritdoc /> 
        public event EventHandler<object> OnPause;

        /// <summary>
        /// Indicates thats mute has been selected
        /// </summary>
        public void InvokeMute() { if (OnMute != null) OnMute(this, EventArgs.Empty); }

        /// <inheritdoc /> 
        public event EventHandler<object> OnMute;

        /// <summary>
        /// Indicates that the volume has changed.
        /// </summary>
        public void InvokeVolumeChange() { if (OnVolumeChange != null) OnVolumeChange(this, EventArgs.Empty); }

        /// <inheritdoc /> 
        public event EventHandler<object> OnVolumeChange;

        /// <summary>
        /// Indicates that playback has ended.
        /// </summary>
        public void InvokeEnd()
        {
            if (isPlaying)  // added just to be safe so we don't somehow run this more than once
            {
                isPlaying = false;
                watchedTime = TimeSpan.Zero;
                totalWatchedTime = totalWatchedTime.Add(DateTime.Now.Subtract(startPlayTimestamp));
            } 
            if (OnEnd != null) OnEnd(this, EventArgs.Empty);
        }

        /// <inheritdoc /> 
        public event EventHandler<object> OnEnd;

        /// <summary>
        /// Indicates that seeking has occured
        /// </summary>
        public void InvokeSeek() { if (OnSeek != null) OnSeek(this, EventArgs.Empty); }

        /// <inheritdoc /> 
        public event EventHandler<object> OnSeek;

        /// <summary>
        /// Indicates that playback of a new playlist item has started.
        /// </summary>
        public void InvokeItemStart()
        {
            itemCount++;
            isPaused = false;
            isPlaying = true;
            startPlayTimestamp = DateTime.Now; 
            if (OnItemStart != null) OnItemStart(this, EventArgs.Empty);
        }

        /// <inheritdoc /> 
        public event EventHandler<object> OnItemStart;

        /// <summary>
        /// Indicates that playback of a playlist item has finished.
        /// </summary>
        public void InvokeItemEnd()
        {
            if (isPlaying)  // added just to be safe so we don't somehow run this more than once
            {
                isPlaying = false;
                watchedTime = TimeSpan.Zero;
                totalWatchedTime = totalWatchedTime.Add(DateTime.Now.Subtract(startPlayTimestamp));
            } 
            if (OnItemEnd != null) OnItemEnd(this, EventArgs.Empty);
        }

        /// <inheritdoc /> 
        public event EventHandler<object> OnItemEnd;

        /// <summary>
        /// Indicates that full screen state has changed.
        /// </summary>
        public void InvokeFullScreenChange() { if (OnFullScreenChange != null) OnFullScreenChange(this, EventArgs.Empty); }

        /// <inheritdoc /> 
        public event EventHandler<object> OnFullScreenChange;

        /// <summary>
        /// Indicates that the size of the player has changed. 
        /// </summary>
        public void InvokePlayerSizeChanged() { if (OnPlayerSizeChanged != null) OnPlayerSizeChanged(this, EventArgs.Empty); }

        /// <inheritdoc /> 
        public event EventHandler<object> OnPlayerSizeChanged;

        /// <summary>
        /// Indicates that an error has occured.
        /// </summary>
        public void InvokeError() { if (OnError != null) OnError(this, EventArgs.Empty); }

        /// <inheritdoc /> 
        public event EventHandler<object> OnError;

        /// <summary>
        /// Indicates that the mouse has moved over the player
        /// </summary>
        public void InvokeMouseOver() { if (OnMouseOver != null) OnMouseOver(this, EventArgs.Empty); }

        /// <inheritdoc /> 
        public event EventHandler<object> OnMouseOver;

        /// <inheritdoc /> 
        public TimeSpan WatchedTime
        {
            get
            {
                if (IsPlaying)
                    return watchedTime.Add(DateTime.Now.Subtract(startPlayTimestamp));
                else
                    return watchedTime;
            }
        }

        /// <inheritdoc /> 
        public TimeSpan TotalWatchedTime
        {
            get
            {
                if (IsPlaying)
                    return totalWatchedTime.Add(DateTime.Now.Subtract(startPlayTimestamp));
                else
                    return totalWatchedTime;
            }
        }

        /// <summary>
        /// Updates the duration of the current playlist item.
        /// </summary>
        /// <param name="value">The new duration.</param>
        public void SetDuration(TimeSpan value) { Duration = value; }

        /// <inheritdoc /> 
        public TimeSpan Duration { get; private set; }

        /// <summary>
        /// Updates the position of playback.
        /// </summary>
        /// <param name="value"></param>
        public void SetPosition(TimeSpan value) { Position = value; }

        /// <inheritdoc /> 
        public TimeSpan Position { get; private set; }

        /// <inheritdoc /> 
        public bool IsPlaying { get { return isPlaying; } }

        /// <inheritdoc /> 
        public bool IsPaused { get { return !isPaused; } }

        /// <inheritdoc /> 
        public bool IsStopped { get { return !isPaused && !isPlaying; } }

        /// <inheritdoc /> 
        public int ItemsPlayed { get { return itemCount; } }

        /// <summary>
        /// Updates the fullscreen flag.
        /// </summary>
        /// <param name="value">The new Fullscreen flag value.</param>
        public void SetFullScreen(bool value) { FullScreen = value; }

        /// <inheritdoc /> 
        public bool FullScreen { get; private set; }

        /// <summary>
        /// Updates the flag indicating if captions are active.
        /// </summary>
        /// <param name="value">The new CaptionsActive flag value.</param>
        public void SetCaptionsActive(bool value) { CaptionsActive = value; }

        /// <inheritdoc /> 
        public bool CaptionsActive { get; private set; }

        /// <summary>
        /// Updates the flag indicating if the stream contains video (vs. just audio)
        /// </summary>
        /// <param name="value">The new HasVideo flag value.</param>
        public void SetHasVideo(bool value) { HasVideo = value; }

        /// <inheritdoc /> 
        public bool HasVideo { get; private set; }

        /// <summary>
        /// Updates the flag indicating if the stream contains audio (vs. just video)
        /// </summary>
        /// <param name="value">The new HasAudio flag value.</param>
        public void SetHasAudio(bool value) { HasAudio = value; }

        /// <inheritdoc /> 
        public bool HasAudio { get; private set; }

        /// <summary>
        /// Updates the flag indicating if the stream contains captions
        /// </summary>
        /// <param name="value">The new HasCaptions flag value.</param>
        public void SetHasCaptions(bool value) { HasCaptions = value; }

        /// <inheritdoc /> 
        public bool HasCaptions { get; private set; }

        /// <summary>
        /// Relays the current width of the player.
        /// </summary>
        /// <param name="value">The new PlayerWidth value.</param>
        public void SetPlayerWidth(int value) { PlayerWidth = value; }

        /// <inheritdoc /> 
        public int PlayerWidth { get; private set; }

        /// <summary>
        /// Relays the current height of the player.
        /// </summary>
        /// <param name="value">The new PlayerHeight value.</param>
        public void SetPlayerHeight(int value) { PlayerHeight = value; }

        /// <inheritdoc /> 
        public int PlayerHeight { get; private set; }

        /// <summary>
        /// Relays the current width of the media.
        /// </summary>
        /// <param name="value">The new ContentWidth value.</param>
        public void SetContentWidth(int value) { ContentWidth = value; }

        /// <inheritdoc /> 
        public int ContentWidth { get; private set; }

        /// <summary>
        /// Relays the current height of the player.
        /// </summary>
        /// <param name="value">The new ContentHeight value.</param>
        public void SetContentHeight(int value) { ContentHeight = value; }

        /// <inheritdoc /> 
        public int ContentHeight { get; private set; }

        /// <summary>
        /// Relays the current bitrate of the media.
        /// </summary>
        /// <param name="value">The new ContentBitrate value.</param>
        public void SetContentBitrate(long value) { ContentBitrate = value; }

        /// <inheritdoc /> 
        public long ContentBitrate { get; private set; }

        /// <summary>
        /// Relays the current title of the media.
        /// </summary>
        /// <param name="value">The new ContentTitle value.</param>
        public void SetContentTitle(string value) { ContentTitle = value; }

        /// <inheritdoc /> 
        public string ContentTitle { get; private set; }

        /// <summary>
        /// Relays the current url of the media.
        /// </summary>
        /// <param name="value">The new ContentUrl value.</param>
        public void SetContentUrl(string value) { ContentUrl = value; }

        /// <inheritdoc /> 
        public string ContentUrl { get; private set; }
    }
}
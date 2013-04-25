using System;
using System.Linq;
using Microsoft.VideoAdvertising;
#if SILVERLIGHT
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#endif

namespace Microsoft.PlayerFramework.Advertising
{
    /// <summary>
    /// Adapts an IPlayer to IMastAdapter.
    /// Used to provide the MAST conditions all the info they need to fire triggers.
    /// </summary>
    internal class MastAdapter : IMastAdapter, IDisposable
    {
        readonly MediaPlayer player;
        DateTime startPlayTimestamp;
        TimeSpan totalWatchedTime = TimeSpan.Zero;
        TimeSpan watchedTime = TimeSpan.Zero;
        int itemCount = 0;

        public MastAdapter(MediaPlayer player)
        {
            if (player == null) throw new NullReferenceException("Player cannot be null.");
            this.player = player;
            HookPlayerEvents();
        }

        #region MAST Events

#if SILVERLIGHT
        public event EventHandler OnPlay;
        public event EventHandler OnStop;
        public event EventHandler OnPause;
        public event EventHandler OnMute;
        public event EventHandler OnVolumeChange;
        public event EventHandler OnEnd;
        public event EventHandler OnItemStart;
        public event EventHandler OnItemEnd;
        public event EventHandler OnSeek;
        public event EventHandler OnFullScreenChange;
        public event EventHandler OnError;
        public event EventHandler OnMouseOver;
        public event EventHandler OnPlayerSizeChanged;
#else
        public event EventHandler<object> OnPlay;
        public event EventHandler<object> OnStop;
        public event EventHandler<object> OnPause;
        public event EventHandler<object> OnMute;
        public event EventHandler<object> OnVolumeChange;
        public event EventHandler<object> OnEnd;
        public event EventHandler<object> OnItemStart;
        public event EventHandler<object> OnItemEnd;
        public event EventHandler<object> OnSeek;
        public event EventHandler<object> OnFullScreenChange;
        public event EventHandler<object> OnError;
        public event EventHandler<object> OnMouseOver;
        public event EventHandler<object> OnPlayerSizeChanged;
#endif
        #endregion

        #region MAST Properties

        public TimeSpan Duration
        {
            get { return player.Duration; }
        }

        public TimeSpan Position
        {
            get { return player.Position; }
        }

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

#if !WINDOWS_PHONE
        public bool FullScreen
        {
            get { return player.IsFullScreen; }
        }
#endif

        public bool IsPlaying
        {
            get { return player.CurrentState == MediaElementState.Playing; }
        }

        public bool IsPaused
        {
            get { return player.CurrentState == MediaElementState.Paused || player.CurrentState == MediaElementState.Buffering; }
        }

        public bool IsStopped
        {
            get { return player.CurrentState == MediaElementState.Stopped || player.CurrentState == MediaElementState.Closed; }
        }

        public bool CaptionsActive
        {
            get { return player.IsCaptionsActive; }
        }

        public bool HasCaptions
        {
            get { return player.AvailableCaptions.Any(); }
        }

        public bool HasVideo
        {
            get { return player.NaturalVideoHeight != 0; }
        }

        public bool HasAudio
        {
            get { return player.AudioStreamCount >= 0; }
        }

        public int ItemsPlayed
        {
            get { return itemCount; }
        }

        public int PlayerWidth
        {
            get { return (int)player.ActualWidth; }
        }

        public int PlayerHeight
        {
            get { return (int)player.ActualHeight; }
        }

        public int ContentWidth
        {
            get { return player.NaturalVideoWidth; }
        }

        public int ContentHeight
        {
            get { return player.NaturalVideoHeight; }
        }

        public long ContentBitrate
        {
            get { return 0; }
        }

        public string ContentTitle
        {
            get { return ""; }
        }

        public string ContentUrl
        {
            get { return player.Source != null ? player.Source.OriginalString : ""; }
        }

        #endregion

        #region MAST Event Handling

        void HookPlayerEvents()
        {
            {
                var element = player as FrameworkElement;
                element.SizeChanged += player_SizeChanged;
#if SILVERLIGHT
                element.MouseEnter += element_MouseEnter;
#else
                element.PointerEntered += element_PointerEnterd;
#endif
            }
#if !WINDOWS_PHONE
            player.IsFullScreenChanged += player_FullScreenChanged;
#endif
#if NETFX_CORE
            player.SeekCompleted += player_SeekCompleted;
#endif
            player.VolumeChanged += player_VolumeChanged;
            player.IsMutedChanged += player_IsMutedChanged;
            player.CurrentStateChanged += player_StateChanged;
            player.MediaFailed += player_MediaFailed;
        }

        void UnhookPlayerEvents()
        {
            {
                var element = player as FrameworkElement;
                element.SizeChanged -= player_SizeChanged;
#if SILVERLIGHT
                element.MouseEnter -= element_MouseEnter;
#else
                element.PointerEntered -= element_PointerEnterd;
#endif
                }
#if !WINDOWS_PHONE
            player.IsFullScreenChanged -= player_FullScreenChanged;
#endif
#if NETFX_CORE
            player.SeekCompleted -= player_SeekCompleted;
#endif
            player.VolumeChanged -= player_VolumeChanged;
            player.IsMutedChanged -= player_IsMutedChanged;
            player.CurrentStateChanged -= player_StateChanged;
            player.MediaFailed -= player_MediaFailed;
        }

        void player_MediaFailed(object sender, object e)
        {
            //System.Diagnostics.Debug.WriteLine("OnError");
            if (OnError != null) OnError(this, EventArgs.Empty);
        }

#if SILVERLIGHT
        void element_MouseEnter(object sender, MouseEventArgs e)
#else
        void element_PointerEnterd(object sender, PointerRoutedEventArgs e)
#endif
        {
            //System.Diagnostics.Debug.WriteLine("OnMouseOver");
            if (OnMouseOver != null) OnMouseOver(this, EventArgs.Empty);
        }

        bool isPlaying = false;
        void player_StateChanged(object sender, object e)
        {
            switch (player.CurrentState)
            {
                case MediaElementState.Paused:
                    if (isPlaying)  // added just to be safe so we don't somehow run this more than once
                    {
                        isPlaying = false;
                        var now = DateTime.Now;
                        watchedTime = watchedTime.Add(now.Subtract(startPlayTimestamp));
                        totalWatchedTime = totalWatchedTime.Add(now.Subtract(startPlayTimestamp));
                    }
                    //System.Diagnostics.Debug.WriteLine("OnPause");
                    if (OnPause != null) OnPause(this, EventArgs.Empty);
                    break;
                case MediaElementState.Playing:
                    isPlaying = true;
                    startPlayTimestamp = DateTime.Now;
                    //System.Diagnostics.Debug.WriteLine("OnPlay");
                    if (OnPlay != null) OnPlay(this, EventArgs.Empty);
                    break;
                case MediaElementState.Stopped:
                    if (isPlaying)  // added just to be safe so we don't somehow run this more than once
                    {
                        isPlaying = false;
                        watchedTime = TimeSpan.Zero;
                        totalWatchedTime = totalWatchedTime.Add(DateTime.Now.Subtract(startPlayTimestamp));
                    }
                    //System.Diagnostics.Debug.WriteLine("OnStop");
                    if (OnStop != null) OnStop(this, EventArgs.Empty);
                    break;
                case MediaElementState.Closed:
                    if (isPlaying)  // added just to be safe so we don't somehow run this more than once
                    {
                        isPlaying = false;
                        watchedTime = TimeSpan.Zero;
                        totalWatchedTime = totalWatchedTime.Add(DateTime.Now.Subtract(startPlayTimestamp));
                    }
                    break;
            }
        }

#if SILVERLIGHT
        void player_SeekCompleted(object sender, EventArgs e)
#else
        void player_SeekCompleted(object sender, object e)
#endif
        {
            //System.Diagnostics.Debug.WriteLine("OnSeek");
            if (OnSeek != null) OnSeek(this, EventArgs.Empty);
        }

        internal void InvokeMediaEnded()
        {
            //System.Diagnostics.Debug.WriteLine("OnItemEnd");
            if (OnItemEnd != null) OnItemEnd(this, EventArgs.Empty);
        }

        internal void InvokeMediaStarting()
        { 
            itemCount++;
            if (OnItemStart != null) OnItemStart(this, EventArgs.Empty);
        }

#if SILVERLIGHT
        void player_VolumeChanged(object sender, EventArgs e)
#else
        void player_VolumeChanged(object sender, object e)
#endif
        {
            //System.Diagnostics.Debug.WriteLine("OnVolumeChange");
            if (OnVolumeChange != null) OnVolumeChange(this, EventArgs.Empty);
        }

        void player_IsMutedChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            //System.Diagnostics.Debug.WriteLine("OnMute");
            if (e.NewValue)
            {
                if (OnMute != null) OnMute(this, EventArgs.Empty);
            }
        }

#if SILVERLIGHT
        void player_FullScreenChanged(object sender, EventArgs e)
#else
        void player_FullScreenChanged(object sender, object e)
#endif
        {
            //System.Diagnostics.Debug.WriteLine("OnFullScreenChange");
            if (OnFullScreenChange != null) OnFullScreenChange(this, EventArgs.Empty);
        }

        void player_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("OnPlayerSizeChanged");
            if (OnPlayerSizeChanged != null) OnPlayerSizeChanged(this, EventArgs.Empty);
        }
        #endregion

        public void Dispose()
        {
            if (OnEnd != null) OnEnd(this, EventArgs.Empty);
            UnhookPlayerEvents();
        }
    }
}

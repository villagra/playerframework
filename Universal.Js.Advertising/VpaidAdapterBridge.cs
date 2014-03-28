using System;
using Microsoft.Media.Advertising;

namespace Microsoft.PlayerFramework.Js.Advertising
{
    /// <summary>
    /// Provides an adapter between a VPAID ad player and the advertising component.
    /// </summary>
    public sealed class VpaidAdapterBridge : IVpaid2
    {
        /// <summary>
        /// Indicates that the HandshakeVersion method was called.
        /// </summary>
        public event EventHandler<HandshakeVersionRequestedEventArgs> HandshakeVersionRequested;
        string IVpaid.HandshakeVersion(string version)
        {
            if (HandshakeVersionRequested != null)
            {
                var args = new HandshakeVersionRequestedEventArgs(version);
                HandshakeVersionRequested(this, args);
                return args.Result;
            }
            else return "1.0";
        }

        /// <summary>
        /// Indicates that the SkipAd method was called.
        /// </summary>
        public event EventHandler<object> SkipAdRequested;
        void IVpaid2.SkipAd()
        {
            if (AdSkippableState)
            {
                if (SkipAdRequested != null) SkipAdRequested(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Indicates that the InitAd method was called.
        /// </summary>
        public event EventHandler<InitAdRequestedEventArgs> InitAdRequested;
        void IVpaid.InitAd(double width, double height, string viewMode, int desiredBitrate, string creativeData, string environmentVariables)
        {
            if (InitAdRequested != null) InitAdRequested(this, new InitAdRequestedEventArgs(width, height, viewMode, desiredBitrate, creativeData, environmentVariables ?? ""));
        }

        /// <summary>
        /// Indicates that the StartAd method was called.
        /// </summary>
        public event EventHandler<object> StartAdRequested;
        void IVpaid.StartAd()
        {
            if (StartAdRequested != null) StartAdRequested(this, EventArgs.Empty);
        }

        /// <summary>
        /// Indicates that the StopAd method was called.
        /// </summary>
        public event EventHandler<object> StopAdRequested;
        void IVpaid.StopAd()
        {
            if (StopAdRequested != null) StopAdRequested(this, EventArgs.Empty);
        }

        /// <summary>
        /// Indicates that the ResizeAd method was called.
        /// </summary>
        public event EventHandler<ResizeAdRequestedEventArgs> ResizeAdRequested;
        void IVpaid.ResizeAd(double width, double height, string viewMode)
        {
            if (ResizeAdRequested != null) ResizeAdRequested(this, new ResizeAdRequestedEventArgs(width, height, viewMode));
        }

        /// <summary>
        /// Indicates that the PauseAd method was called.
        /// </summary>
        public event EventHandler<object> PauseAdRequested;
        void IVpaid.PauseAd()
        {
            if (PauseAdRequested != null) PauseAdRequested(this, EventArgs.Empty);
        }

        /// <summary>
        /// Indicates that the ResumeAd method was called.
        /// </summary>
        public event EventHandler<object> ResumeAdRequested;
        void IVpaid.ResumeAd()
        {
            if (ResumeAdRequested != null) ResumeAdRequested(this, EventArgs.Empty);
        }

        /// <summary>
        /// Indicates that the ExpandAd method was called.
        /// </summary>
        public event EventHandler<object> ExpandAdRequested;
        void IVpaid.ExpandAd()
        {
            if (ExpandAdRequested != null) ExpandAdRequested(this, EventArgs.Empty);
        }

        /// <summary>
        /// Indicates that the CollapseAd method was called.
        /// </summary>
        public event EventHandler<object> CollapseAdRequested;
        void IVpaid.CollapseAd()
        {
            if (CollapseAdRequested != null) CollapseAdRequested(this, EventArgs.Empty);
        }

        /// <summary>
        /// Indicates that the AdWidth property was requested.
        /// </summary>
        public event EventHandler<DoubleRequestedEventArgs> GetAdWidthRequested;
        /// <inheritdoc /> 
        public double AdWidth
        {
            get
            {
                var eventArgs = new DoubleRequestedEventArgs();
                if (GetAdWidthRequested != null) GetAdWidthRequested(this, eventArgs);
                return eventArgs.Result;
            }
        }

        /// <summary>
        /// Indicates that the AdHeight property was requested.
        /// </summary>
        public event EventHandler<DoubleRequestedEventArgs> GetAdHeightRequested;
        /// <inheritdoc /> 
        public double AdHeight
        {
            get
            {
                var eventArgs = new DoubleRequestedEventArgs();
                if (GetAdHeightRequested != null) GetAdHeightRequested(this, eventArgs);
                return eventArgs.Result;
            }
        }

        /// <summary>
        /// Indicates that the AdSkippableState property was requested.
        /// </summary>
        public event EventHandler<BoolRequestedEventArgs> GetAdSkippableStateRequested;
        /// <inheritdoc /> 
        public bool AdSkippableState
        {
            get
            {
                var eventArgs = new BoolRequestedEventArgs();
                if (GetAdSkippableStateRequested != null) GetAdSkippableStateRequested(this, eventArgs);
                return eventArgs.Result;
            }
        }

        /// <summary>
        /// Indicates that the AdCompanions property was requested.
        /// </summary>
        public event EventHandler<StringRequestedEventArgs> GetAdCompanionsRequested;
        /// <inheritdoc /> 
        public string AdCompanions
        {
            get
            {
                var eventArgs = new StringRequestedEventArgs();
                if (GetAdCompanionsRequested != null) GetAdCompanionsRequested(this, eventArgs);
                return eventArgs.Result;
            }
        }

        /// <summary>
        /// Indicates that the AdIcons property was requested.
        /// </summary>
        public event EventHandler<BoolRequestedEventArgs> GetAdIconsRequested;
        /// <inheritdoc /> 
        public bool AdIcons
        {
            get
            {
                var eventArgs = new BoolRequestedEventArgs();
                if (GetAdIconsRequested != null) GetAdIconsRequested(this, eventArgs);
                return eventArgs.Result;
            }
        }

        /// <summary>
        /// Indicates that the AdDuration property was requested.
        /// </summary>
        public event EventHandler<TimeSpanRequestedEventArgs> GetAdDurationRequested;
        /// <inheritdoc /> 
        public TimeSpan AdDuration
        {
            get
            {
                var eventArgs = new TimeSpanRequestedEventArgs();
                if (GetAdDurationRequested != null) GetAdDurationRequested(this, eventArgs);
                return eventArgs.Result;
            }
        }

        /// <summary>
        /// Indicates that the AdLinear property was requested.
        /// </summary>
        public event EventHandler<BoolRequestedEventArgs> GetAdLinearRequested;
        /// <inheritdoc /> 
        public bool AdLinear
        {
            get
            {
                var eventArgs = new BoolRequestedEventArgs();
                if (GetAdLinearRequested != null) GetAdLinearRequested(this, eventArgs);
                return eventArgs.Result;
            }
        }

        /// <summary>
        /// Indicates that the AdExpanded property was requested.
        /// </summary>
        public event EventHandler<BoolRequestedEventArgs> GetAdExpandedRequested;
        /// <inheritdoc /> 
        public bool AdExpanded
        {
            get
            {
                var eventArgs = new BoolRequestedEventArgs();
                if (GetAdExpandedRequested != null) GetAdExpandedRequested(this, eventArgs);
                return eventArgs.Result;
            }
        }

        /// <summary>
        /// Indicates that the AdRemainingTime property was requested.
        /// </summary>
        public event EventHandler<TimeSpanRequestedEventArgs> GetAdRemainingTimeRequested;
        /// <inheritdoc /> 
        public TimeSpan AdRemainingTime
        {
            get
            {
                var eventArgs = new TimeSpanRequestedEventArgs();
                if (GetAdRemainingTimeRequested != null) GetAdRemainingTimeRequested(this, eventArgs);
                return eventArgs.Result;
            }
        }

        /// <summary>
        /// Indicates that the AdVolume property was updated.
        /// </summary>
        public event EventHandler<SetVolumeRequestedEventArgs> SetAdVolumeRequested;

        /// <summary>
        /// Indicates that the AdVolume property was requested.
        /// </summary>
        public event EventHandler<DoubleRequestedEventArgs> GetAdVolumeRequested;
        /// <inheritdoc /> 
        public double AdVolume
        {
            get
            {
                var eventArgs = new DoubleRequestedEventArgs();
                if (GetAdVolumeRequested != null) GetAdVolumeRequested(this, eventArgs);
                return eventArgs.Result;
            }
            set
            {
                if (SetAdVolumeRequested != null) SetAdVolumeRequested(this, new SetVolumeRequestedEventArgs(value));
            }
        }

        /// <summary>
        /// Indicates that the AdLoaded event should be raised.
        /// </summary>
        public void OnAdLoaded() { if (AdLoaded != null) AdLoaded(this, EventArgs.Empty); }
        /// <inheritdoc /> 
        public event EventHandler<object> AdLoaded;

        /// <summary>
        /// Indicates that the AdStarted event should be raised.
        /// </summary>
        public void OnAdStarted() { if (AdStarted != null) AdStarted(this, EventArgs.Empty); }
        /// <inheritdoc /> 
        public event EventHandler<object> AdStarted;

        /// <summary>
        /// Indicates that the AdStopped event should be raised.
        /// </summary>
        public void OnAdStopped() { if (AdStopped != null) AdStopped(this, EventArgs.Empty); }
        /// <inheritdoc /> 
        public event EventHandler<object> AdStopped;

        /// <summary>
        /// Indicates that the AdPaused event should be raised.
        /// </summary>
        public void OnAdPaused() { if (AdPaused != null) AdPaused(this, EventArgs.Empty); }
        /// <inheritdoc /> 
        public event EventHandler<object> AdPaused;

        /// <summary>
        /// Indicates that the AdPlaying event should be raised.
        /// </summary>
        public void OnAdPlaying() { if (AdPlaying != null) AdPlaying(this, EventArgs.Empty); }
        /// <inheritdoc /> 
        public event EventHandler<object> AdPlaying;

        /// <summary>
        /// Indicates that the AdExpandedChanged event should be raised.
        /// </summary>
        public void OnAdExpandedChanged() { if (AdExpandedChanged != null) AdExpandedChanged(this, EventArgs.Empty); }
        /// <inheritdoc /> 
        public event EventHandler<object> AdExpandedChanged;

        /// <summary>
        /// Indicates that the AdLinearChanged event should be raised.
        /// </summary>
        public void OnAdLinearChanged() { if (AdLinearChanged != null) AdLinearChanged(this, EventArgs.Empty); }
        /// <inheritdoc /> 
        public event EventHandler<object> AdLinearChanged;

        /// <summary>
        /// Indicates that the AdVolumeChanged event should be raised.
        /// </summary>
        public void OnAdVolumeChanged() { if (AdVolumeChanged != null) AdVolumeChanged(this, EventArgs.Empty); }
        /// <inheritdoc /> 
        public event EventHandler<object> AdVolumeChanged;

        /// <summary>
        /// Indicates that the AdVideoStart event should be raised.
        /// </summary>
        public void OnAdVideoStart() { if (AdVideoStart != null) AdVideoStart(this, EventArgs.Empty); }
        /// <inheritdoc /> 
        public event EventHandler<object> AdVideoStart;

        /// <summary>
        /// Indicates that the AdVideoFirstQuartile event should be raised.
        /// </summary>
        public void OnAdVideoFirstQuartile() { if (AdVideoFirstQuartile != null) AdVideoFirstQuartile(this, EventArgs.Empty); }
        /// <inheritdoc /> 
        public event EventHandler<object> AdVideoFirstQuartile;

        /// <summary>
        /// Indicates that the AdVideoMidpoint event should be raised.
        /// </summary>
        public void OnAdVideoMidpoint() { if (AdVideoMidpoint != null) AdVideoMidpoint(this, EventArgs.Empty); }
        /// <inheritdoc /> 
        public event EventHandler<object> AdVideoMidpoint;

        /// <summary>
        /// Indicates that the AdVideoThirdQuartile event should be raised.
        /// </summary>
        public void OnAdVideoThirdQuartile() { if (AdVideoThirdQuartile != null) AdVideoThirdQuartile(this, EventArgs.Empty); }
        /// <inheritdoc /> 
        public event EventHandler<object> AdVideoThirdQuartile;

        /// <summary>
        /// Indicates that the AdVideoComplete event should be raised.
        /// </summary>
        public void OnAdVideoComplete() { if (AdVideoComplete != null) AdVideoComplete(this, EventArgs.Empty); }
        /// <inheritdoc /> 
        public event EventHandler<object> AdVideoComplete;

        /// <summary>
        /// Indicates that the AdUserAcceptInvitation event should be raised.
        /// </summary>
        public void OnAdUserAcceptInvitation() { if (AdUserAcceptInvitation != null) AdUserAcceptInvitation(this, EventArgs.Empty); }
        /// <inheritdoc /> 
        public event EventHandler<object> AdUserAcceptInvitation;

        /// <summary>
        /// Indicates that the AdUserClose event should be raised.
        /// </summary>
        public void OnAdUserClose() { if (AdUserClose != null) AdUserClose(this, EventArgs.Empty); }
        /// <inheritdoc /> 
        public event EventHandler<object> AdUserClose;

        /// <summary>
        /// Indicates that the AdUserMinimize event should be raised.
        /// </summary>
        public void OnAdUserMinimize() { if (AdUserMinimize != null) AdUserMinimize(this, EventArgs.Empty); }
        /// <inheritdoc /> 
        public event EventHandler<object> AdUserMinimize;

        /// <summary>
        /// Indicates that the AdRemainingTimeChange event should be raised.
        /// </summary>
        public void OnAdRemainingTimeChange() { if (AdRemainingTimeChange != null) AdRemainingTimeChange(this, EventArgs.Empty); }
        /// <inheritdoc /> 
        public event EventHandler<object> AdRemainingTimeChange;

        /// <summary>
        /// Indicates that the AdImpression event should be raised.
        /// </summary>
        public void OnAdImpression() { if (AdImpression != null) AdImpression(this, EventArgs.Empty); }
        /// <inheritdoc /> 
        public event EventHandler<object> AdImpression;

        /// <summary>
        /// Indicates that the AdClickThru event should be raised.
        /// </summary>
        public void OnAdClickThru(string url, string id, bool playerHandles) { if (AdClickThru != null) AdClickThru(this, new ClickThroughEventArgs() { Url = url, Id = id, PlayerHandles = playerHandles }); }
        /// <inheritdoc /> 
        public event EventHandler<ClickThroughEventArgs> AdClickThru;

        /// <summary>
        /// Indicates that the AdError event should be raised.
        /// </summary>
        public void OnAdError(string message) { if (AdError != null) AdError(this, new VpaidMessageEventArgs() { Message = message }); }
        /// <inheritdoc /> 
        public event EventHandler<VpaidMessageEventArgs> AdError;

        /// <summary>
        /// Indicates that the AdLog event should be raised.
        /// </summary>
        public void OnAdLog(string message) { if (AdLog != null) AdLog(this, new VpaidMessageEventArgs() { Message = message }); }
        /// <inheritdoc /> 
        public event EventHandler<VpaidMessageEventArgs> AdLog;

        // vpaid 2.0 events
        /// <summary>
        /// Indicates that the AdInteraction event should be raised.
        /// </summary>
        public void OnAdInteraction(string id) { if (AdInteraction != null) AdInteraction(this, new AdInteractionEventArgs() { Id = id }); }
        /// <inheritdoc /> 
        public event EventHandler<AdInteractionEventArgs> AdInteraction;

        /// <summary>
        /// Indicates that the AdSkipped event should be raised.
        /// </summary>
        public void OnAdSkipped() { if (AdSkipped != null) AdSkipped(this, EventArgs.Empty); }
        /// <inheritdoc /> 
        public event EventHandler<object> AdSkipped;

        /// <summary>
        /// Indicates that the AdSizeChanged event should be raised.
        /// </summary>
        public void OnAdSizeChanged() { if (AdSizeChanged != null) AdSizeChanged(this, EventArgs.Empty); }
        /// <inheritdoc /> 
        public event EventHandler<object> AdSizeChanged;

        /// <summary>
        /// Indicates that the AdSkippableStateChange event should be raised.
        /// </summary>
        public void OnAdSkippableStateChange() { if (AdSkippableStateChange != null) AdSkippableStateChange(this, EventArgs.Empty); }
        /// <inheritdoc /> 
        public event EventHandler<object> AdSkippableStateChange;

        /// <summary>
        /// Indicates that the AdDurationChange event should be raised.
        /// </summary>
        public void OnAdDurationChange() { if (AdDurationChange != null) AdDurationChange(this, EventArgs.Empty); }
        /// <inheritdoc /> 
        public event EventHandler<object> AdDurationChange;
    }

    /// <summary>
    /// Provides information about calls to InitAd
    /// </summary>
    public sealed class InitAdRequestedEventArgs
    {
        internal InitAdRequestedEventArgs(double width, double height, string viewMode, int desiredBitrate, string creativeData, string environmentVariables)
        {
            Width = width;
            Height = height;
            ViewMode = viewMode;
            DesiredBitrate = desiredBitrate;
            CreativeData = creativeData;
            EnvironmentVariables = environmentVariables;
        }

        /// <summary>
        /// The width of the player
        /// </summary>
        public double Width { get; private set; }
        /// <summary>
        /// The height of the player
        /// </summary>
        public double Height { get; private set; }
        /// <summary>
        /// The view mode of the player ("fullscreen" or "normal")
        /// </summary>
        public string ViewMode { get; private set; }
        /// <summary>
        /// The desired bitrate for the ad
        /// </summary>
        public int DesiredBitrate { get; private set; }
        /// <summary>
        /// The data for the ad (usually the Url of the media)
        /// </summary>
        public string CreativeData { get; private set; }
        /// <summary>
        /// Additional information for the ad.
        /// </summary>
        public string EnvironmentVariables { get; private set; }
    }

    /// <summary>
    /// Provides information about calls to ResizeAd
    /// </summary>
    public sealed class ResizeAdRequestedEventArgs
    {
        internal ResizeAdRequestedEventArgs(double width, double height, string viewMode)
        {
            Width = width;
            Height = height;
            ViewMode = viewMode;
        }

        /// <summary>
        /// The width of the player
        /// </summary>
        public double Width { get; private set; }
        /// <summary>
        /// The height of the player
        /// </summary>
        public double Height { get; private set; }
        /// <summary>
        /// The view mode of the player ("fullscreen" or "normal")
        /// </summary>
        public string ViewMode { get; private set; }
    }

    /// <summary>
    /// Provides information about calls to HandshakeVersion
    /// </summary>
    public sealed class HandshakeVersionRequestedEventArgs
    {
        internal HandshakeVersionRequestedEventArgs(string version)
        {
            Version = version;
        }

        /// <summary>
        /// The Version of VPAID that the player supports.
        /// </summary>
        public string Version { get; private set; }
        /// <summary>
        /// The version of VPAID that the ad supports.
        /// </summary>
        public string Result { get; set; }
    }

    /// <summary>
    /// Provides information about calls to SetVolume
    /// </summary>
    public sealed class SetVolumeRequestedEventArgs
    {
        internal SetVolumeRequestedEventArgs(double value)
        {
            Value = value;
        }

        /// <summary>
        /// The new volume for the ad.
        /// </summary>
        public double Value { get; private set; }
    }

    /// <summary>
    /// Provides information about property requests that are of type Double.
    /// </summary>
    public sealed class DoubleRequestedEventArgs
    {
        internal DoubleRequestedEventArgs()
        { }

        /// <summary>
        /// The value or result of the call.
        /// </summary>
        public double Result { get; set; }
    }

    /// <summary>
    /// Provides information about property requests that are of type TimeSpan.
    /// </summary>
    public sealed class TimeSpanRequestedEventArgs
    {
        internal TimeSpanRequestedEventArgs()
        { }

        /// <summary>
        /// The value or result of the call.
        /// </summary>
        public TimeSpan Result { get; set; }
    }

    /// <summary>
    /// Provides information about property requests that are of type Boolean.
    /// </summary>
    public sealed class BoolRequestedEventArgs
    {
        internal BoolRequestedEventArgs()
        { }

        /// <summary>
        /// The value or result of the call.
        /// </summary>
        public bool Result { get; set; }
    }

    /// <summary>
    /// Provides information about property requests that are of type String.
    /// </summary>
    public sealed class StringRequestedEventArgs
    {
        internal StringRequestedEventArgs()
        { }

        /// <summary>
        /// The value or result of the call.
        /// </summary>
        public string Result { get; set; }
    }
}
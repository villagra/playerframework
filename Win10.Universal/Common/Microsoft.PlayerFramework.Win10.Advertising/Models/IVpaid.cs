using System;

namespace Microsoft.Media.Advertising
{
    /// <summary>
    /// VPAID 2.0 interface. For more info: http://www.iab.net/media/file/VPAID_2.0_Final_04-10-2012.pdf
    /// Players capable of playing ads must implement this interface.
    /// </summary>
    public interface IVpaid2 : IVpaid
    { 
        void SkipAd();
        double AdWidth { get; }
        double AdHeight { get; }
        bool AdSkippableState { get; }
        TimeSpan AdDuration { get; }
        string AdCompanions { get; }
        bool AdIcons { get; }
        event EventHandler<AdInteractionEventArgs> AdInteraction;
        event EventHandler<object> AdSkipped;
        event EventHandler<object> AdSizeChanged;
        event EventHandler<object> AdSkippableStateChange;
        event EventHandler<object> AdDurationChange;
    }


    public sealed class AdInteractionEventArgs
    {
        public string Id { get; set; }
    }

    /// <summary>
    /// VPAID 1.1 interface. For more info: http://www.iab.net/media/file/VPAIDFINAL51109.pdf
    /// Players capable of playing ads must implement this interface.
    /// </summary>
    public interface IVpaid
    {
        #region VPAID Methods
        string HandshakeVersion(string version);
        void InitAd(double width, double height, string viewMode, int desiredBitrate, string creativeData, string environmentVariables);
        void StartAd();
        void StopAd();
        void ResizeAd(double width, double height, string viewMode);
        void PauseAd();
        void ResumeAd();
        void ExpandAd();
        void CollapseAd();
        #endregion
        #region VPAID Properties
        bool AdLinear { get; }
        bool AdExpanded { get; }
        TimeSpan AdRemainingTime { get; }
        double AdVolume { get; set; }
        #endregion
        #region VPAID Events

        event EventHandler<object> AdLoaded;
        event EventHandler<object> AdStarted;
        event EventHandler<object> AdStopped;
        event EventHandler<object> AdPaused;
        event EventHandler<object> AdPlaying;
        event EventHandler<object> AdExpandedChanged;
        event EventHandler<object> AdLinearChanged;
        event EventHandler<object> AdVolumeChanged;
        event EventHandler<object> AdVideoStart;
        event EventHandler<object> AdVideoFirstQuartile;
        event EventHandler<object> AdVideoMidpoint;
        event EventHandler<object> AdVideoThirdQuartile;
        event EventHandler<object> AdVideoComplete;
        event EventHandler<object> AdUserAcceptInvitation;
        event EventHandler<object> AdUserClose;
        event EventHandler<object> AdUserMinimize;
        event EventHandler<object> AdRemainingTimeChange;
        event EventHandler<object> AdImpression;
        event EventHandler<ClickThroughEventArgs> AdClickThru;
        event EventHandler<VpaidMessageEventArgs> AdError;
        event EventHandler<VpaidMessageEventArgs> AdLog;

        #endregion
    }
    #region Event Argument classes
    
    public sealed class VpaidMessageEventArgs
    {
        public string Message { get; set; }
    }

    public sealed class ClickThroughEventArgs
    {
        public string Url { get; set; }
        public string Id { get; set; }
        public bool PlayerHandles { get; set; }
    }

    #endregion
}

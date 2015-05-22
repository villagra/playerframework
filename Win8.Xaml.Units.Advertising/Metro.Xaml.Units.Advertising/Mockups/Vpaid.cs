using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Media.Advertising;

namespace Microsoft.PlayerFramework.Units.Advertising.Mockups
{
    public enum ErrorPlacement
    {
        None,
        Handshake,
        Init,
        InitAsync,
        Start,
        StartAsync,
        Play,
        PlayAsync,
        Stop,
        StopAsync,
    }

    public class Vpaid : IVpaid
    {
        public ErrorPlacement ErrorPlacement { get; set; }
        public TimeSpan? Delay { get; set; }

        public Vpaid()
        {
            ErrorPlacement = ErrorPlacement.None;
            Delay = TimeSpan.FromSeconds(2);
            AdLinear = true;
        }

        public string HandshakeVersion(string version)
        {
            if (ErrorPlacement == ErrorPlacement.Handshake) throw new NotImplementedException();
            return "1.1";
        }

        void OnError()
        {
            if (AdError != null) AdError(this, new VpaidMessageEventArgs() { Message = "mock error" });
        }

        public async void InitAd(double width, double height, string viewMode, int desiredBitrate, string creativeData, string environmentVariables)
        {
            if (ErrorPlacement == ErrorPlacement.Init) throw new NotImplementedException();
            if (Delay.HasValue) await Task.Delay(Delay.Value);
            if (ErrorPlacement == ErrorPlacement.InitAsync) { OnError(); return; }
            if (!isStopped)
            {
                if (AdLoaded != null) AdLoaded(this, EventArgs.Empty);
            }
        }

        public async void StartAd()
        {
            if (ErrorPlacement == ErrorPlacement.Start) throw new NotImplementedException();
            if (Delay.HasValue) await Task.Delay(Delay.Value);
            if (ErrorPlacement == ErrorPlacement.StartAsync) { OnError(); return; }
            if (!isStopped)
            {
                if (AdStarted != null) AdStarted(this, EventArgs.Empty);
                if (ErrorPlacement == ErrorPlacement.Play) throw new NotImplementedException();
                if (Delay.HasValue)
                {
                    await Task.Delay(Delay.Value);
                }
                else
                {
                    await Task.Delay(1);
                }
                if (ErrorPlacement == ErrorPlacement.PlayAsync) { OnError(); return; }
                StopAd();
            }
        }

        bool isStopped;
        public async void StopAd()
        {
            if (!isStopped)
            {
                isStopped = true;
                if (ErrorPlacement == ErrorPlacement.Stop) throw new NotImplementedException();
                if (Delay.HasValue) await Task.Delay(Delay.Value);
                if (ErrorPlacement == ErrorPlacement.StopAsync) { OnError(); return; }
                if (AdStopped != null) AdStopped(this, EventArgs.Empty);
            }
        }

        public void ResizeAd(double width, double height, string viewMode)
        {

        }

        public async void PauseAd()
        {
            if (Delay.HasValue) await Task.Delay(Delay.Value);
            if (AdPaused != null) AdPaused(this, EventArgs.Empty);
        }

        public async void ResumeAd()
        {
            if (Delay.HasValue) await Task.Delay(Delay.Value);
            if (AdPlaying != null) AdPlaying(this, EventArgs.Empty);
        }

        public void ExpandAd()
        {

        }

        public void CollapseAd()
        {

        }

        public bool AdLinear { get; set; }

        public bool AdExpanded { get; set; }

        public TimeSpan AdRemainingTime { get; set; }

        public double AdVolume { get; set; }

        public event EventHandler<object> AdLoaded;

        public event EventHandler<object> AdStarted;

        public event EventHandler<object> AdStopped;

        public event EventHandler<object> AdPaused;

        public event EventHandler<object> AdPlaying;

#pragma warning disable 0067
        public event EventHandler<object> AdExpandedChanged;

        public event EventHandler<object> AdLinearChanged;

        public event EventHandler<object> AdVolumeChanged;

        public event EventHandler<object> AdVideoStart;

        public event EventHandler<object> AdVideoFirstQuartile;

        public event EventHandler<object> AdVideoMidpoint;

        public event EventHandler<object> AdVideoThirdQuartile;

        public event EventHandler<object> AdVideoComplete;

        public event EventHandler<object> AdUserAcceptInvitation;

        public event EventHandler<object> AdUserClose;

        public event EventHandler<object> AdUserMinimize;

        public event EventHandler<object> AdRemainingTimeChange;

        public event EventHandler<object> AdImpression;

        public event EventHandler<ClickThroughEventArgs> AdClickThru;

        public event EventHandler<VpaidMessageEventArgs> AdLog;
#pragma warning restore 0067

        public event EventHandler<VpaidMessageEventArgs> AdError;
    }
}

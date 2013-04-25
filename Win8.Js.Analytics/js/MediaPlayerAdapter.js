(function (PlayerFramework, undefined) {
    "use strict";

    // MediaPlayerAdapter Errors
    var invalidConstruction = "Invalid construction: MediaPlayerAdapter constructor must be called using the \"new\" operator.",
        invalidMediaPlayer = "Invalid argument: MediaPlayerAdapter expects a MediaPlayer as the first argument.";

    var analyticsTrackingEventArea = "analytics";

    // MediaPlayerAdapter Class
    var MediaPlayerAdapter = WinJS.Class.define(function (mediaPlayer) {
        if (!(this instanceof PlayerFramework.Analytics.MediaPlayerAdapter)) {
            throw invalidConstruction;
        }

        if (!(mediaPlayer instanceof PlayerFramework.MediaPlayer)) {
            throw invalidMediaPlayer;
        }

        this._mediaPlayer = mediaPlayer;
        this._playbackRate = this._mediaPlayer.defaultPlaybackRate;
        this._isScrubbing = false;
        
        this._nativeInstance = new Microsoft.PlayerFramework.Js.Analytics.MediaPlayerAdapterBridge();
        this._nativeInstance.setPlaybackRate(this._mediaPlayer.playbackRate);
        this._nativeInstance.setIsFullScreen(this._mediaPlayer.isFullScreen);
        this._nativeInstance.setIsLive(this._mediaPlayer.isLive);
        this._nativeInstance.setIsBuffering(false);
        if (this._mediaPlayer.currentAudioTrack) {
            this._nativeInstance.setAudioTrackId(this._mediaPlayer.currentAudioTrack.language);
        }
        if (this._mediaPlayer.currentCaptionTrack) {
            this._nativeInstance.setCaptionTrackId(this._mediaPlayer.currentCaptionTrack.label);
        }

        this._bindEvent("positionrequested", this._nativeInstance, this._onPositionRequested);
        this._bindEvent("durationrequested", this._nativeInstance, this._onDurationRequested);

        this._bindEvent("fullscreenchange", this._mediaPlayer, this._onMediaPlayerFullScreenChange);
        this._bindEvent("waiting", this._mediaPlayer, this._onMediaPlayerWaiting);
        this._bindEvent("playing", this._mediaPlayer, this._onMediaPlayerPlaying);
        this._bindEvent("islivechange", this._mediaPlayer, this._onMediaPlayerIsLiveChange);
        this._bindEvent("ratechange", this._mediaPlayer, this._onMediaPlayerRateChange);
        this._bindEvent("currentaudiotrackchange", this._mediaPlayer, this._onMediaPlayerAudioTrackChange);
        this._bindEvent("currentcaptiontrackchange", this._mediaPlayer, this._onMediaPlayerCaptionTrackChange);
        this._bindEvent("advertisingstatechange", this._mediaPlayer, this._onMediaPlayerAdvertisingStateChange);
        this._bindEvent("error", this._mediaPlayer, this._onMediaPlayerError);
        this._bindEvent("canplaythrough", this._mediaPlayer, this._onMediaPlayerCanPlayThrough);
        this._bindEvent("started", this._mediaPlayer, this._onMediaPlayerStarted);
        this._bindEvent("emptied", this._mediaPlayer, this._onMediaPlayerEmptied);
        this._bindEvent("ending", this._mediaPlayer, this._onMediaPlayerEnding);
        this._bindEvent("play", this._mediaPlayer, this._onMediaPlayerPlay);
        this._bindEvent("pause", this._mediaPlayer, this._onMediaPlayerPause);
        this._bindEvent("seek", this._mediaPlayer, this._onMediaPlayerSeek);
        this._bindEvent("scrub", this._mediaPlayer, this._onMediaPlayerScrub);
        this._bindEvent("scrubbed", this._mediaPlayer, this._onMediaPlayerScrubbed);
        this._bindEvent("eventtracked", this._mediaPlayer.playTimeTrackingPlugin, this._onMediaPlayerPlayTimeEventTracked);
        this._bindEvent("eventtracked", this._mediaPlayer.positionTrackingPlugin, this._onMediaPlayerPositionEventTracked);
    }, {
        // Public Properties
        nativeInstance: {
            get: function () {
                return this._nativeInstance;
            }
        },

        // Public Methods
        dispose: function () {
            this._unbindEvents();
            this._mediaPlayer = null;
            this._nativeInstance = null;
        },

        // Private Methods
        _onPositionRequested: function (e) {
            e.result = this._mediaPlayer.currentTime * 1000;
        },
        
        _onDurationRequested: function (e) {
            e.result = this._mediaPlayer.duration * 1000;
        },

        _onMediaPlayerFullScreenChange: function (e) {
            this._nativeInstance.setIsFullScreen(this._mediaPlayer.isFullScreen);
        },

        _onMediaPlayerWaiting: function (e) {
            this._nativeInstance.setIsBuffering(true);
        },

        _onMediaPlayerPlaying: function (e) {
            this._nativeInstance.setIsBuffering(false);
        },

        _onMediaPlayerIsLiveChange: function (e) {
            this._nativeInstance.setIsLive(this._mediaPlayer.isLive);
        },

        _onMediaPlayerRateChange: function (e) {
            if (!this._isScrubbing) {
                this._nativeInstance.setPlaybackRate(this._mediaPlayer.playbackRate);
            }
        },

        _onMediaPlayerAudioTrackChange: function (e) {
            if (this._mediaPlayer.currentAudioTrack) {
                this._nativeInstance.setAudioTrackId(this._mediaPlayer.currentAudioTrack.language);
            }
            else {
                this._nativeInstance.setAudioTrackId(null);
            }
        },

        _onMediaPlayerCaptionTrackChange: function (e) {
            if (this._mediaPlayer.currentCaptionTrack) {
                this._nativeInstance.setCaptionTrackId(this._mediaPlayer.currentCaptionTrack.label);
            }
            else {
                this._nativeInstance.setCaptionTrackId(null);
            }
        },

        _onMediaPlayerAdvertisingStateChange: function (e) {
            switch (this._mediaPlayer.advertisingState) {
                case PlayerFramework.AdvertisingState.linear:
                    this._nativeInstance.onClipStarted("");
                    break;
                default:
                    this._nativeInstance.onClipEnded("");
                    break;
            }
        },

        _onMediaPlayerError: function (e) {
            var error = e.detail.error;
            var message = PlayerFramework.Utilities.getMediaErrorMessage(error);
            this._nativeInstance.onStreamFailed(message);
        },

        _onMediaPlayerCanPlayThrough: function (e) {
            this._nativeInstance.setSource(new Windows.Foundation.Uri(this._mediaPlayer.src));
            this._nativeInstance.onStreamLoaded();
        },

        _onMediaPlayerStarted: function (e) {
            this._nativeInstance.onStreamStarted();
            this._nativeInstance.onPlaying();
        },

        _onMediaPlayerEmptied: function (e) {
            this._nativeInstance.onStreamClosed();
        },

        _onMediaPlayerEnding: function (e) {
            this._nativeInstance.onStreamEnded();
        },
        
        _onMediaPlayerPlay: function (e) {
            this._nativeInstance.onPlaying();
        },

        _onMediaPlayerPause: function (e) {
            this._nativeInstance.onPaused();
        },

        _onMediaPlayerSeek: function (e) {
            this._nativeInstance.onSeeked(e.detail.previousTime * 1000, e.detail.time * 1000);
        },

        _onMediaPlayerScrub: function (e) {
            this._isScrubbing = true;
            this._nativeInstance.onScrubStarted();
        },

        _onMediaPlayerScrubbed: function (e) {
            this._nativeInstance.onScrubCompleted(e.detail.time * 1000);
            this._isScrubbing = false;
        },

        _onMediaPlayerPlayTimeEventTracked: function (e) {
            var trackingEvent = e.detail.trackingEvent;
            if (trackingEvent.area === PlayerFramework.Plugins.AnalyticsPlugin.trackingEventArea) {
                if (!isNaN(trackingEvent.playTimePercentage))
                    this._nativeInstance.onPlayTimePercentageReached(trackingEvent.playTimePercentage);
                else
                    this._nativeInstance.onPlayTimeReached(trackingEvent.playTime * 1000);
            }
        },

        _onMediaPlayerPositionEventTracked: function (e) {
            var trackingEvent = e.detail.trackingEvent;
            if (trackingEvent.area === PlayerFramework.Plugins.AnalyticsPlugin.trackingEventArea) {
                if (!isNaN(trackingEvent.positionPercentage))
                    this._nativeInstance.onPositionPercentageReached(trackingEvent.positionPercentage);
                else
                    this._nativeInstance.onPositionReached(trackingEvent.position * 1000);
            }
        }
    });

    // MediaPlayerAdapter Mixins
    WinJS.Class.mix(MediaPlayerAdapter, PlayerFramework.Utilities.eventBindingMixin);

    // MediaPlayerAdapter Exports
    WinJS.Namespace.define("PlayerFramework.Analytics", {
        MediaPlayerAdapter: MediaPlayerAdapter
    });

})(PlayerFramework);


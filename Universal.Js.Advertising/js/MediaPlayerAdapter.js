(function (PlayerFramework, undefined) {
    "use strict";

    // MediaPlayerAdapter Errors
    var invalidConstruction = "Invalid construction: MediaPlayerAdapter constructor must be called using the \"new\" operator.",
        invalidMediaPlayer = "Invalid argument: MediaPlayerAdapter expects a MediaPlayer as the first argument.";

    // MediaPlayerAdapter Class
    var MediaPlayerAdapter = WinJS.Class.define(function (mediaPlayer) {
        if (!(this instanceof PlayerFramework.Advertising.MediaPlayerAdapter)) {
            throw invalidConstruction;
        }

        if (!(mediaPlayer instanceof PlayerFramework.MediaPlayer)) {
            throw invalidMediaPlayer;
        }

        this._mediaPlayer = mediaPlayer;
        
        this._nativeInstance = new Microsoft.PlayerFramework.Js.Advertising.MediaPlayerAdapterBridge();
        this._nativeInstance.volume = this._mediaPlayer.volume;
        this._nativeInstance.isMuted = this._mediaPlayer.muted;
        this._nativeInstance.isFullScreen = this._mediaPlayer.isFullScreen;
        this._nativeInstance.dimensions = PlayerFramework.Utilities.measureElement(this._mediaPlayer.element);

        this._currentBitrate = 0;

        this._bindEvent("volumechange", this._mediaPlayer, this._onMediaPlayerVolumeChange);
        this._bindEvent("mutedchange", this._mediaPlayer, this._onMediaPlayerMutedChange);
        this._bindEvent("fullscreenchange", this._mediaPlayer, this._onMediaPlayerFullScreenChange);
        this._bindEvent("currentbitraterequested", this._nativeInstance, this._onCurrentBitrateRequested);
        this._bindEvent("currentpositionrequested", this._nativeInstance, this._onCurrentPositionRequested);
        if (this._mediaPlayer.element.onresize !== undefined) {
            this._bindEvent("resize", this._mediaPlayer.element, this._onMediaPlayerResize);
        }
        else { // IE11 no longer supports resize event for arbitrary elements. The best we can do is listen to the window resize event.
            this._bindEvent("resize", window, this._onMediaPlayerResize);
        }
    }, {
        // Public Properties
        nativeInstance: {
            get: function () {
                return this._nativeInstance;
            }
        },

        currentBitrate: {
            get: function () {
                return this._currentBitrate;
            },
            set: function (value) {
                this._currentBitrate = value;
            }
        },

        // Public Methods
        dispose: function () {
            this._unbindEvents();
            this._mediaPlayer = null;
            this._nativeInstance = null;
        },

        // Private Methods
        _onMediaPlayerVolumeChange: function (e) {
            this._nativeInstance.volume = this._mediaPlayer.volume;
        },

        _onMediaPlayerMutedChange: function (e) {
            this._nativeInstance.isMuted = this._mediaPlayer.muted;
        },

        _onMediaPlayerFullScreenChange: function (e) {
            this._nativeInstance.isFullScreen = this._mediaPlayer.isFullScreen;
        },

        _onMediaPlayerResize: function (e) {
            this._nativeInstance.dimensions = PlayerFramework.Utilities.measureElement(this._mediaPlayer.element);
        },

        _onCurrentBitrateRequested: function (e) {
            e.result = this._currentBitrate;
        },

        _onCurrentPositionRequested: function (e) {
            e.result = this._mediaPlayer.currentTime;
        }
    });

    // MediaPlayerAdapter Mixins
    WinJS.Class.mix(MediaPlayerAdapter, PlayerFramework.Utilities.eventBindingMixin);

    // MediaPlayerAdapter Exports
    WinJS.Namespace.define("PlayerFramework.Advertising", {
        MediaPlayerAdapter: MediaPlayerAdapter
    });

})(PlayerFramework);


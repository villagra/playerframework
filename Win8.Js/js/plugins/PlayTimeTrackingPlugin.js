(function (PlayerFramework, undefined) {
    "use strict";

    // PlayTimeTrackingPlugin Errors
    var invalidConstruction = "Invalid construction: PlayTimeTrackingPlugin constructor must be called using the \"new\" operator.";

    // PlayTimeTrackingPlugin Class
    var PlayTimeTrackingPlugin = WinJS.Class.derive(PlayerFramework.TrackingPluginBase, function (options) {
        if (!(this instanceof PlayerFramework.Plugins.PlayTimeTrackingPlugin)) {
            throw invalidConstruction;
        }

        this._playTime = 0;
        this._playTimePercentage = 0;
        this._dispatchedTrackingEvents = [];
        this._startTime = null;

        PlayerFramework.TrackingPluginBase.call(this, options);
    }, {
        // Public Properties
        playTime: {
            get: function () {
                return this._playTime;
            },
            set: function (value) {
                var oldValue = this._playTime;
                if (oldValue !== value) {
                    this._playTime = value;
                    this._observablePlugin.notify("playTime", value, oldValue);
                }
            }
        },

        playTimePercentage: {
            get: function () {
                return this._playTimePercentage;
            },
            set: function (value) {
                var oldValue = this._playTimePercentage;
                if (oldValue !== value) {
                    this._playTimePercentage = value;
                    this._observablePlugin.notify("playTimePercentage", value, oldValue);
                }
            }
        },

        // Private Methods
        _evaluateTrackingEvents: function () {
            if (this.trackingEvents) {
                var dispatchedTrackingEvents = this._dispatchedTrackingEvents;
                var undispatchedTrackingEvents = this.trackingEvents.filter(function (trackingEvent) { return dispatchedTrackingEvents.indexOf(trackingEvent) === -1; });
                for (var i = 0; i < undispatchedTrackingEvents.length; i++) {
                    var trackingEvent = undispatchedTrackingEvents[i];
                    if ((!isNaN(trackingEvent.playTimePercentage) && trackingEvent.playTimePercentage <= this.playTimePercentage) || (!isNaN(trackingEvent.playTime) && trackingEvent.playTime <= this.playTime)) {
                        dispatchedTrackingEvents.push(trackingEvent);
                        this.dispatchEvent("eventtracked", { trackingEvent: trackingEvent, timestamp: Date.now() });
                    }
                }
            }
        },

        _resetStartTime: function () {
            this._startTime = Date.now() - (this.playTime * 1000);
        },

        _onActivate: function () {
            this._bindEvent("playerstatechange", this.mediaPlayer, this._onMediaPlayerPlayerStateChange);
            this._bindEvent("pause", this.mediaPlayer, this._onMediaPlayerPause);
            this._bindEvent("playing", this.mediaPlayer, this._onMediaPlayerPlaying);
            this._bindEvent("timeupdate", this.mediaPlayer, this._onMediaPlayerTimeUpdate);

            return PlayerFramework.TrackingPluginBase.prototype._onActivate.call(this);
        },

        _onDeactivate: function () {
            this._unbindEvent("playerstatechange", this.mediaPlayer, this._onMediaPlayerPlayerStateChange);
            this._unbindEvent("pause", this.mediaPlayer, this._onMediaPlayerPause);
            this._unbindEvent("playing", this.mediaPlayer, this._onMediaPlayerPlaying);
            this._unbindEvent("timeupdate", this.mediaPlayer, this._onMediaPlayerTimeUpdate);

            PlayerFramework.TrackingPluginBase.prototype._onDeactivate.call(this);
        },

        _onUpdate: function () {
            this.playTime = 0;
            this.playTimePercentage = 0;
            this._dispatchedTrackingEvents = [];
            this._startTime = null;
        },

        _onMediaPlayerPlayerStateChange: function (e) {
            this._startTime = null;
        },

        _onMediaPlayerPause: function (e) {
            this._startTime = null;
        },

        _onMediaPlayerPlaying: function (e) {
            this._resetStartTime();
        },

        _onMediaPlayerTimeUpdate: function (e) {
            if (this._startTime === null && !this.mediaPlayer.paused && !this.mediaPlayer.ended) {
                this._resetStartTime();
            }

            if (this._startTime !== null) {
                this.playTime = (Date.now() - this._startTime) / 1000;
                this.playTimePercentage = this.playTime / this.mediaPlayer.duration;
                this._evaluateTrackingEvents();
            }
        }
    });

    // PlayTimeTrackingPlugin Mixins
    WinJS.Class.mix(PlayerFramework.MediaPlayer, {
        playTimeTrackingPlugin: {
            value: null,
            writable: true,
            configurable: true
        }
    });

    // PlayTimeTrackingPlugin Exports
    WinJS.Namespace.define("PlayerFramework.Plugins", {
        PlayTimeTrackingPlugin: PlayTimeTrackingPlugin
    });

})(PlayerFramework);


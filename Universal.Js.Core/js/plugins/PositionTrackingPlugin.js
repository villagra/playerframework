(function (PlayerFramework, undefined) {
    "use strict";

    // PositionTrackingPlugin Errors
    var invalidConstruction = "Invalid construction: PositionTrackingPlugin constructor must be called using the \"new\" operator.";

    // PositionTrackingPlugin Class
    var PositionTrackingPlugin = WinJS.Class.derive(PlayerFramework.TrackingPluginBase, function (options) {
        if (!(this instanceof PlayerFramework.Plugins.PositionTrackingPlugin)) {
            throw invalidConstruction;
        }

        this._position = 0;
        this._positionPercentage = 0;
        this._dispatchedTrackingEvents = [];
        this._evaluateOnForwardOnly = true;

        PlayerFramework.TrackingPluginBase.call(this, options);
    }, {
        // Public Properties
        evaluateOnForwardOnly: {
            get: function () {
                return this._evaluateOnForwardOnly;
            },
            set: function (value) {
                var oldValue = this._evaluateOnForwardOnly;
                if (oldValue !== value) {
                    this._evaluateOnForwardOnly = value;
                    this._observablePlugin.notify("evaluateOnForwardOnly", value, oldValue);
                }
            }
        },

        position: {
            get: function () {
                return this._position;
            },
            set: function (value) {
                var oldValue = this._position;
                if (oldValue !== value) {
                    this._position = value;
                    this._observablePlugin.notify("position", value, oldValue);
                }
            }
        },

        positionPercentage: {
            get: function () {
                return this._positionPercentage;
            },
            set: function (value) {
                var oldValue = this._positionPercentage;
                if (oldValue !== value) {
                    this._positionPercentage = value;
                    this._observablePlugin.notify("positionPercentage", value, oldValue);
                }
            }
        },

        // Private Methods
        _evaluateTrackingEvents: function (previousTime, currentTime, skippedPast) {
            if (this.trackingEvents) {
                for (var i = 0; i < this.trackingEvents.length; i++) {
                    var trackingEvent = this.trackingEvents[i];
                    var time = this._getTrackingEventTime(trackingEvent);
                    var index = this._dispatchedTrackingEvents.indexOf(trackingEvent);
                    
                    if (index !== -1 && time > currentTime) {
                        this._dispatchedTrackingEvents.splice(index, 1);
                        index = -1;
                    }

                    if (index === -1 && (!this.evaluateOnForwardOnly || currentTime > previousTime) && time <= currentTime && time > previousTime) {
                        this._dispatchedTrackingEvents.push(trackingEvent);
                        this.dispatchEvent("eventtracked", { trackingEvent: trackingEvent, timestamp: Date.now(), skippedPast: skippedPast });
                    }
                }
            }
        },

        _getTrackingEventTime: function (trackingEvent) {
            if (!isNaN(trackingEvent.positionPercentage)) {
                return trackingEvent.positionPercentage * this.mediaPlayer.duration;
            } else if (!isNaN(trackingEvent.position)) {
                return trackingEvent.position;
            } else {
                return NaN;
            }
        },

        _onActivate: function () {
            this._bindEvent("seek", this.mediaPlayer, this._onMediaPlayerSeek);
            this._bindEvent("scrubbed", this.mediaPlayer, this._onMediaPlayerScrubbed);
            this._bindEvent("timeupdate", this.mediaPlayer, this._onMediaPlayerTimeUpdate);

            return PlayerFramework.TrackingPluginBase.prototype._onActivate.call(this);
        },

        _onDeactivate: function () {
            this._unbindEvent("seek", this.mediaPlayer, this._onMediaPlayerSeek);
            this._unbindEvent("scrubbed", this.mediaPlayer, this._onMediaPlayerScrubbed);
            this._unbindEvent("timeupdate", this.mediaPlayer, this._onMediaPlayerTimeUpdate);

            PlayerFramework.TrackingPluginBase.prototype._onDeactivate.call(this);
        },

        _onUpdate: function () {
            this.position = 0;
            this.positionPercentage = 0;
            this._dispatchedTrackingEvents = [];
        },

        _onMediaPlayerSeek: function (e) {
            this._evaluateTrackingEvents(e.detail.previousTime, e.detail.time, true);
        },

        _onMediaPlayerScrubbed: function (e) {
            this._evaluateTrackingEvents(e.detail.startTime, e.detail.time, true);
        },

        _onMediaPlayerTimeUpdate: function (e) {
            var currentTime = this.mediaPlayer.virtualTime;

            this.position = currentTime;
            this.positionPercentage = currentTime / this.mediaPlayer.duration;

            if (!this.mediaPlayer.scrubbing) {
                this._evaluateTrackingEvents(-1, currentTime, false);
            }
        }
    });

    // PositionTrackingPlugin Mixins
    WinJS.Class.mix(PlayerFramework.MediaPlayer, {
        positionTrackingPlugin: {
            value: null,
            writable: true,
            configurable: true
        }
    });

    // PositionTrackingPlugin Exports
    WinJS.Namespace.define("PlayerFramework.Plugins", {
        PositionTrackingPlugin: PositionTrackingPlugin
    });

})(PlayerFramework);


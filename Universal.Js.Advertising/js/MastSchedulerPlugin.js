(function (PlayerFramework, undefined) {
    "use strict";

    // MastSchedulerPlugin Errors
    var invalidConstruction = "Invalid construction: MastSchedulerPlugin constructor must be called using the \"new\" operator.";

    // MastSchedulerPlugin Class
    var MastSchedulerPlugin = WinJS.Class.derive(PlayerFramework.PluginBase, function (options) {
        if (!(this instanceof PlayerFramework.Plugins.MastSchedulerPlugin)) {
            throw invalidConstruction;
        }

        this._activeTriggers = [];

        PlayerFramework.PluginBase.call(this, options);
    }, {
        // Public Properties
        source: {
            get: function () {
                return this._source;
            },
            set: function (value) {
                this._source = value;
            }
        },

        activeTriggers: {
            get: function () {
                return this._activeTriggers;
            }
        },

        // Public Methods
        loadAds: function (source) {
            this._mainsail.clear();

            var promise = this._mainsail.loadSource(new Windows.Foundation.Uri(source));

            promise.done(
                function () {
                    PlayerFramework.Utilities.remove(this._activePromises, promise);
                }.bind(this),
                function (e) {
                    PlayerFramework.Utilities.remove(this._activePromises, promise);
                }.bind(this)
            );
            
            this._activePromises.push(promise);
            return promise;
        },

        cancelActiveTriggers: function () {
            for (var i = 0; i < this._activeTriggers.length; i++) {
                var trigger = this._activeTriggers[i];
                trigger.promise.cancel();
            }

            this._activeTriggers = [];
        },

        // Private Methods
        _setOptions: function (options) {
            PlayerFramework.Utilities.setOptions(this, options, {
                isEnabled: true,
                source: null
            });
        },

        _onLoad: function () {
            this._isStarted = false;
            this._mastAdapter = new Microsoft.PlayerFramework.Js.Advertising.MastAdapter();
            this._mainsail = new Microsoft.Media.Advertising.Mainsail(this._mastAdapter);
        },

        _onUnload: function () {
            this._isStarted = false;
            this._mastAdapter = null;
            this._mainsail.MastInterface = null;
            this._mainsail = null;
        },

        _onActivate: function () {
            this._bindEvent("activatetrigger", this._mainsail, this._onMainsailActivateTrigger);
            this._bindEvent("deactivatetrigger", this._mainsail, this._onMainsailDeactivateTrigger);
            this._bindEvent("canplay", this.mediaPlayer, this._onMediaPlayerCanPlay);
            this._bindEvent("play", this.mediaPlayer, this._onMediaPlayerPlay);
            this._bindEvent("pause", this.mediaPlayer, this._onMediaPlayerPause);
            this._bindEvent("stopped", this.mediaPlayer, this._onMediaPlayerStopped);
            this._bindEvent("seeked", this.mediaPlayer, this._onMediaPlayerSeeked);
            this._bindEvent("loading", this.mediaPlayer, this._onMediaPlayerLoading);
            this._bindEvent("starting", this.mediaPlayer, this._onMediaPlayerStarting);
            this._bindEvent("ending", this.mediaPlayer, this._onMediaPlayerEnding);
            this._bindEvent("timeupdate", this.mediaPlayer, this._onMediaPlayerTimeUpdate);
            this._bindEvent("durationchange", this.mediaPlayer, this._onMediaPlayerDurationChange);
            this._bindEvent("volumechange", this.mediaPlayer, this._onMediaPlayerVolumeChange);
            this._bindEvent("mutedchange", this.mediaPlayer, this._onMediaPlayerMutedChange);
            this._bindEvent("fullscreenchange", this.mediaPlayer, this._onMediaPlayerFullScreenChange);
            this._bindEvent("error", this.mediaPlayer, this._onMediaPlayerError);
            if (PlayerFramework.Utilities.isWinJS1) {
                this._bindEvent("resize", this.mediaPlayer.element, this._onMediaPlayerResize);
            }
            else { // IE11 no longer supports resize event for arbitrary elements. The best we can do is listen to the window resize event.
                this._bindEvent("resize", window, this._onMediaPlayerResize);
            }
            if (window.PointerEvent) {
                this._bindEvent("pointerover", this.mediaPlayer.element, this._onMediaPlayerMSPointerOver);
            }
            else {
                this._bindEvent("MSPointerOver", this.mediaPlayer.element, this._onMediaPlayerMSPointerOver);
            }
            
            this._mastAdapter.setContentTitle("");
            this._mastAdapter.setContentUrl(this.mediaPlayer.src);
            this._mastAdapter.setContentBitrate(0);
            this._mastAdapter.setContentWidth(0);
            this._mastAdapter.setContentHeight(0);
            this._mastAdapter.setPosition(0);
            this._mastAdapter.setDuration(0);
            this._mastAdapter.setPlayerWidth(this.mediaPlayer.element.scrollWidth);
            this._mastAdapter.setPlayerHeight(this.mediaPlayer.element.scrollHeight);
            this._mastAdapter.setFullScreen(this.mediaPlayer.isFullScreen);
            this._mastAdapter.setHasAudio(true);
            this._mastAdapter.setHasVideo(true);
            this._mastAdapter.setHasCaptions(this.mediaPlayer.captionTracks && this.mediaPlayer.captionTracks.length > 0);
            this._mastAdapter.setCaptionsActive(!!this.mediaPlayer.currentCaptionTrack);

            return true;
        },

        _onDeactivate: function () {
            this._mastAdapter.invokeEnd();

            this.cancelActiveTriggers();
            this._mainsail.clear();
            
            this._unbindEvent("activatetrigger", this._mainsail, this._onMainsailActivateTrigger);
            this._unbindEvent("deactivatetrigger", this._mainsail, this._onMainsailDeactivateTrigger);
            this._unbindEvent("canplay", this.mediaPlayer, this._onMediaPlayerCanPlay);
            this._unbindEvent("play", this.mediaPlayer, this._onMediaPlayerPlay);
            this._unbindEvent("pause", this.mediaPlayer, this._onMediaPlayerPause);
            this._unbindEvent("stopped", this.mediaPlayer, this._onMediaPlayerStopped);
            this._unbindEvent("seeked", this.mediaPlayer, this._onMediaPlayerSeeked);
            this._unbindEvent("loading", this.mediaPlayer, this._onMediaPlayerLoading);
            this._unbindEvent("starting", this.mediaPlayer, this._onMediaPlayerStarting);
            this._unbindEvent("ending", this.mediaPlayer, this._onMediaPlayerEnding);
            this._unbindEvent("timeupdate", this.mediaPlayer, this._onMediaPlayerTimeUpdate);
            this._unbindEvent("durationchange", this.mediaPlayer, this._onMediaPlayerDurationChange);
            this._unbindEvent("volumechange", this.mediaPlayer, this._onMediaPlayerVolumeChange);
            this._unbindEvent("mutedchange", this.mediaPlayer, this._onMediaPlayerMutedChange);
            this._unbindEvent("fullscreenchange", this.mediaPlayer, this._onMediaPlayerFullScreenChange);
            this._unbindEvent("error", this.mediaPlayer, this._onMediaPlayerError);
            if (PlayerFramework.Utilities.isWinJS1) {
                this._unbindEvent("resize", this.mediaPlayer.element, this._onMediaPlayerResize);
            }
            else { // IE11 no longer supports resize event for arbitrary elements. The best we can do is listen to the window resize event.
                this._unbindEvent("resize", window, this._onMediaPlayerResize);
            }
            if (window.PointerEvent) {
                this._unbindEvent("pointerover", this.mediaPlayer.element, this._onMediaPlayerMSPointerOver);
            }
            else {
                this._unbindEvent("MSPointerOver", this.mediaPlayer.element, this._onMediaPlayerMSPointerOver);
            }
        },

        _onUpdate: function () {
            this.cancelActiveTriggers();
            this._mainsail.clear();

            this._mastAdapter.setContentTitle("");
            this._mastAdapter.setContentUrl(this.mediaPlayer.src);
            this._mastAdapter.setContentBitrate(0);
            this._mastAdapter.setContentWidth(0);
            this._mastAdapter.setContentHeight(0);
            this._mastAdapter.setPosition(0);
            this._mastAdapter.setDuration(0);
            this._mastAdapter.setPlayerWidth(this.mediaPlayer.element.scrollWidth);
            this._mastAdapter.setPlayerHeight(this.mediaPlayer.element.scrollHeight);
            this._mastAdapter.setFullScreen(this.mediaPlayer.isFullScreen);
            this._mastAdapter.setHasAudio(true);
            this._mastAdapter.setHasVideo(true);
            this._mastAdapter.setHasCaptions(this.mediaPlayer.captionTracks && this.mediaPlayer.captionTracks.length > 0);
            this._mastAdapter.setCaptionsActive(!!this.mediaPlayer.currentCaptionTrack);
        },

        _onMainsailActivateTrigger: function (e) {
            if (e.trigger.sources.length) {
                var source = e.trigger.sources[0];
                var sourceUri = new Windows.Foundation.Uri(source.uri);
                var remoteSource = new Microsoft.PlayerFramework.Js.Advertising.RemoteAdSource(sourceUri, source.format);
                var promise = this.mediaPlayer.adHandlerPlugin.playAd(remoteSource).then(null, function () { /* swallow */ });
                var trigger = { trigger: e.trigger, promise: promise };

                promise.done(
                    function () {
                        PlayerFramework.Utilities.remove(this._activeTriggers, trigger);
                    }.bind(this)
                );

                this._activeTriggers.push(trigger);
            }
        },

        _onMainsailDeactivateTrigger: function (e) {
            for (var i = 0; i < this._activeTriggers.length; i++) {
                var trigger = this._activeTriggers[i];
                if (trigger === e.trigger) {
                    trigger.promise.cancel();
                    PlayerFramework.Utilities.remove(this._activeTriggers, trigger);
                    return;
                }
            }
        },

        _onMediaPlayerCanPlay: function (e) {
            this._mastAdapter.setContentWidth(this.mediaPlayer.videoWidth);
            this._mastAdapter.setContentHeight(this.mediaPlayer.videoHeight);
        },

        _onMediaPlayerPlay: function (e) {
            this._mastAdapter.invokePlay();
        },

        _onMediaPlayerPause: function (e) {
            this._mastAdapter.invokePause();
        },

        _onMediaPlayerStopped: function (e) {
            this._mastAdapter.invokeStop();
        },

        _onMediaPlayerSeeked: function (e) {
            this._mastAdapter.invokeSeek();
        },

        _onMediaPlayerLoading: function (e) {
            if (this.source) {
                var promise = this.loadAds(this.source);
                e.detail.setPromise(promise);
            }
        },

        _onMediaPlayerStarting: function (e) {
            if (!this._isStarted) {
                this._isStarted = true;
                this._mastAdapter.invokeItemStart();

                if (this.mediaPlayer.allowStartingDeferrals) {
                    var triggerPromises = [];

                    for (var i = 0; i < this._activeTriggers.length; i++) {
                        var trigger = this._activeTriggers[i];
                        triggerPromises.push(trigger.promise);
                    }

                    var promise = WinJS.Promise.join(triggerPromises);

                    promise.done(
                        function () {
                            PlayerFramework.Utilities.remove(this._activePromises, promise);
                        }.bind(this),
                        function (e) {
                            PlayerFramework.Utilities.remove(this._activePromises, promise);
                        }.bind(this)
                    );

                    this._activePromises.push(promise);
                    e.detail.setPromise(promise);
                }
            }
        },

        _onMediaPlayerEnding: function (e) {
            var activeTriggersBeforeEnd = [];
            var activeTriggerPromises = [];

            for (var i = 0; i < this._activeTriggers.length; i++) {
                var trigger = this._activeTriggers[i];
                activeTriggersBeforeEnd.push(trigger);
            }
            
            this._mastAdapter.invokeItemEnd();

            for (var i = 0; i < this._activeTriggers.length; i++) {
                var trigger = this._activeTriggers[i];
                if (activeTriggersBeforeEnd.indexOf(trigger) === -1) {
                    activeTriggerPromises.push(trigger.promise);
                }
            }
            
            if (activeTriggerPromises.length) {
                var promise = WinJS.Promise.join(activeTriggerPromises);
                
                promise.done(
                    function () {
                        PlayerFramework.Utilities.remove(this._activePromises, promise);
                    }.bind(this),
                    function (e) {
                        PlayerFramework.Utilities.remove(this._activePromises, promise);
                    }.bind(this)
                );

                this._activePromises.push(promise);
                e.detail.setPromise(promise);
            }
        },

        _onMediaPlayerTimeUpdate: function (e) {
            if (this._mastAdapter.isPlaying && !this.mediaPlayer.scrubbing) {
                this._mastAdapter.setPosition(this.mediaPlayer.currentTime * 1000);
                this._mainsail.evaluateTriggers();
            }
        },

        _onMediaPlayerDurationChange: function (e) {
            this._mastAdapter.setDuration(this.mediaPlayer.duration * 1000);
        },

        _onMediaPlayerVolumeChange: function (e) {
            this._mastAdapter.invokeVolumeChange();
        },

        _onMediaPlayerMutedChange: function (e) {
            if (this.mediaPlayer.muted) {
                this._mastAdapter.invokeMute();
            }
        },

        _onMediaPlayerFullScreenChange: function (e) {
            this._mastAdapter.invokeFullScreenChange();
        },

        _onMediaPlayerError: function (e) {
            this._mastAdapter.invokeError();
        },

        _onMediaPlayerResize: function (e) {
            this._mastAdapter.invokePlayerSizeChanged();
            this._mastAdapter.setPlayerWidth(this.mediaPlayer.element.scrollWidth);
            this._mastAdapter.setPlayerHeight(this.mediaPlayer.element.scrollHeight);
        },

        _onMediaPlayerMSPointerOver: function (e) {
            this._mastAdapter.invokeMouseOver();
        }
    });

    // MastSchedulerPlugin Mixins
    WinJS.Class.mix(PlayerFramework.MediaPlayer, {
        mastSchedulerPlugin: {
            value: null,
            writable: true,
            configurable: true
        }
    });

    // MastSchedulerPlugin Exports
    WinJS.Namespace.define("PlayerFramework.Plugins", {
        MastSchedulerPlugin: MastSchedulerPlugin
    });

})(PlayerFramework);


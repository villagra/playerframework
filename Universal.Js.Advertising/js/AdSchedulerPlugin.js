(function (PlayerFramework, undefined) {
    "use strict";

    // AdSchedulerPlugin Errors
    var invalidConstruction = "Invalid construction: AdSchedulerPlugin constructor must be called using the \"new\" operator.";

    // AdSchedulerPlugin Class
    var AdSchedulerPlugin = WinJS.Class.derive(PlayerFramework.PluginBase, function (options) {
        if (!(this instanceof PlayerFramework.Plugins.AdSchedulerPlugin)) {
            throw invalidConstruction;
        }

        this._handledAds = null;
        this._preloadedAds = null;
        this._preloadableAds = null;
        this._advertisements = null;

        PlayerFramework.PluginBase.call(this, options);
    }, {
        // Public Properties
        advertisements: {
            get: function () {
                return this._advertisements;
            },
            set: function (value) {
                this._advertisements = value;
            }
        },

        preloadTime: {
            get: function () {
                return this._preloadTime;
            },
            set: function (value) {
                this._preloadTime = value;
            }
        },

        evaluateOnForwardOnly: {
            get: function () {
                return this._evaluateOnForwardOnly;
            },
            set: function (value) {
                this._evaluateOnForwardOnly = value;
            }
        },

        interruptScrub: {
            get: function () {
                return this._interruptScrub;
            },
            set: function (value) {
                this._interruptScrub = value;
            }
        },

        // Public Methods
        evaluate: function (previousTime, currentTime, syncTime, preload) {
            if (this.advertisements) {
                for (var i = 0; i < this.advertisements.length; i++) {
                    var ad = this.advertisements[i];
                    if (ad instanceof PlayerFramework.Advertising.MidrollAdvertisement) {
                        var time = this._getAdTime(ad);
                        if ((!this.evaluateOnForwardOnly || currentTime > previousTime) && time <= currentTime && time > previousTime) {
                            if (this._handledAds.indexOf(ad) === -1) {
                                this._handledAds.push(ad);
                                this._playAd(ad);
                                if (syncTime) {
                                    this.mediaPlayer.currentTime = time;
                                }
                                return true;
                            }
                        } else if (preload && this.preloadTime && Math.max(0, time - this.preloadTime) <= currentTime) {
                            if (this._handledAds.indexOf(ad) === -1 && this._preloadedAds.indexOf(ad) === -1 && this._preloadableAds.indexOf(ad) !== -1) {
                                this._preloadedAds.push(ad);
                                this._preloadAd(ad);
                            }
                        }
                    } else if (ad instanceof PlayerFramework.Advertising.PostrollAdvertisement) {
                        if (preload && this.preloadTime && Math.max(0, this.mediaPlayer.duration - this.preloadTime) <= currentTime) {
                            if (this._handledAds.indexOf(ad) === -1 && this._preloadedAds.indexOf(ad) === -1 && this._preloadableAds.indexOf(ad) !== -1) {
                                this._preloadedAds.push(ad);
                                this._preloadAd(ad);
                            }
                        }
                    }
                }
            }

            return false;
        },

        // Private Methods
        _setOptions: function (options) {
            PlayerFramework.Utilities.setOptions(this, options, {
                isEnabled: true,
                advertisements: [],
                preloadTime: 5,
                evaluateOnForwardOnly: true,
                interruptScrub: true
            });
        },

        _initializeAds: function () {
            this._handledAds = [];
            this._preloadedAds = [];
            this._preloadableAds = [];

            if (this.advertisements) {
                var seenPrerollAd = false;
                for (var i = 0; i < this.advertisements.length; i++) {
                    var ad = this.advertisements[i];
                    if (ad instanceof PlayerFramework.Advertising.PrerollAdvertisement) {
                        // don't play preroll ads if there's an initial startup time
                        if (this.mediaPlayer.startupTime) {
                            this._handledAds.push(ad);
                        }
                    } else if (ad instanceof PlayerFramework.Advertising.MidrollAdvertisement) {
                        // don't play midroll ads scheduled before the initial startup time
                        if (this.mediaPlayer.startupTime && this._getAdTime(ad) <= this.mediaPlayer.startupTime) {
                            this._handledAds.push(ad);
                        } else {
                            this._preloadableAds.push(ad);
                        }
                    } else if (ad instanceof PlayerFramework.Advertising.PostrollAdvertisement) {
                        // only preload the first postroll ad
                        if (!seenPrerollAd) {
                            seenPrerollAd = true;
                            this._preloadableAds.push(ad);
                        }
                    }
                }
            }
        },

        _getAdTime: function (ad) {
            if (ad.timePercentage !== null) {
                return ad.timePercentage * this.mediaPlayer.duration;
            } else if (ad.time !== null) {
                return ad.time;
            } else {
                return -1;
            }
        },

        _playAd: function (ad) {
            return this.mediaPlayer.adHandlerPlugin.playAd(ad.source).then(null, function () { /* swallow */ });
        },

        _playAds: function (ads) {
            var promise = null;

            for (var i = 0; i < ads.length; i++) {
                var ad = ads[i];
                if (!promise) {
                    promise = this._playAd(ad);
                } else {
                    promise = promise.then(function () {
                        return this._playAd(ad);
                    }.bind(this));
                }
            }

            return promise;
        },

        _playAdsOfType: function (type) {
            var ads = [];

            if (this.advertisements) {
                for (var i = 0; i < this.advertisements.length; i++) {
                    var ad = this.advertisements[i];
                    if (ad instanceof type && this._handledAds.indexOf(ad) === -1) {
                        this._handledAds.push(ad);
                        ads.push(ad);
                    }
                }
            }

            return this._playAds(ads);
        },

        _preloadAd: function (ad) {
            this._cancelActivePreload();
            return this._activePreloadPromise = this.mediaPlayer.adHandlerPlugin.preloadAd(ad.source);
        },

        _cancelActivePreload: function () {
            if (this._activePreloadPromise) {
                this._activePreloadPromise.cancel();
                this._activePreloadPromise = null;
            }
        },

        _onLoad: function () {
            this._handledAds = [];
            this._preloadedAds = [];
            this._preloadableAds = [];
        },

        _onUnload: function () {
            this._handledAds = null;
            this._preloadedAds = null;
            this._preloadableAds = null;
            this._advertisements = null;
        },

        _onActivate: function () {
            this._bindEvent("canplay", this.mediaPlayer, this._onMediaPlayerCanPlay);
            this._bindEvent("starting", this.mediaPlayer, this._onMediaPlayerStarting);
            this._bindEvent("ending", this.mediaPlayer, this._onMediaPlayerEnding);
            this._bindEvent("seek", this.mediaPlayer, this._onMediaPlayerSeek);
            this._bindEvent("scrub", this.mediaPlayer, this._onMediaPlayerScrub);
            this._bindEvent("scrubbing", this.mediaPlayer, this._onMediaPlayerScrubbing);
            this._bindEvent("scrubbed", this.mediaPlayer, this._onMediaPlayerScrubbed);
            this._bindEvent("timeupdate", this.mediaPlayer, this._onMediaPlayerTimeUpdate);

            return true;
        },

        _onDeactivate: function () {
            this._cancelActivePreload();
            
            this._unbindEvent("canplay", this.mediaPlayer, this._onMediaPlayerCanPlay);
            this._unbindEvent("starting", this.mediaPlayer, this._onMediaPlayerStarting);
            this._unbindEvent("ending", this.mediaPlayer, this._onMediaPlayerEnding);
            this._unbindEvent("seek", this.mediaPlayer, this._onMediaPlayerSeek);
            this._unbindEvent("scrub", this.mediaPlayer, this._onMediaPlayerScrub);
            this._unbindEvent("scrubbing", this.mediaPlayer, this._onMediaPlayerScrubbing);
            this._unbindEvent("scrubbed", this.mediaPlayer, this._onMediaPlayerScrubbed);
            this._unbindEvent("timeupdate", this.mediaPlayer, this._onMediaPlayerTimeUpdate);
        },

        _onUpdate: function () {
            this._cancelActivePreload();
        },

        _onMediaPlayerCanPlay: function (e) {
            this._initializeAds();
        },

        _onMediaPlayerStarting: function (e) {
            if (this.mediaPlayer.allowStartingDeferrals) {
                var promise = this._playAdsOfType(PlayerFramework.Advertising.PrerollAdvertisement);
                if (promise) {
                    e.detail.setPromise(promise);
                }
            }
        },

        _onMediaPlayerEnding: function (e) {
            var promise = this._playAdsOfType(PlayerFramework.Advertising.PostrollAdvertisement);
            if (promise) {
                e.detail.setPromise(promise);
            }
        },

        _onMediaPlayerSeek: function (e) {
            if (!e.detail.canceled) {
                e.detail.canceled = this.evaluate(e.detail.previousTime, e.detail.time, true, false);
            }
        },

        _onMediaPlayerScrub: function (e) {
            this._cancelActivePreload();
        },

        _onMediaPlayerScrubbing: function (e) {
            if (this.interruptScrub && !e.detail.canceled) {
                e.detail.canceled = this.evaluate(e.detail.startTime, e.detail.time, true, false);
            }
        },

        _onMediaPlayerScrubbed: function (e) {
            if (!e.detail.canceled) {
                e.detail.canceled = this.evaluate(e.detail.startTime, e.detail.time, this.interruptScrub, false);
            }
        },

        _onMediaPlayerTimeUpdate: function (e) {
            if (!this.mediaPlayer.scrubbing) {
                this.evaluate(-1, this.mediaPlayer.currentTime, false, true);
            }
        }
    });

    // AdSchedulerPlugin Mixins
    WinJS.Class.mix(PlayerFramework.MediaPlayer, {
        adSchedulerPlugin: {
            value: null,
            writable: true,
            configurable: true
        }
    });

    // AdSchedulerPlugin Exports
    WinJS.Namespace.define("PlayerFramework.Plugins", {
        AdSchedulerPlugin: AdSchedulerPlugin
    });

})(PlayerFramework);


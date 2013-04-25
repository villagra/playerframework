(function (PlayerFramework, undefined) {
    "use strict";

    // FreeWheelPlugin Errors
    var invalidConstruction = "Invalid construction: FreeWheelPlugin constructor must be called using the \"new\" operator.",
        companionAdFailed = "Companion ad failed.";

    var freeWheelTrackingEventArea = "freewheel";

    // FreeWheelPlugin Class
    var FreeWheelPlugin = WinJS.Class.derive(PlayerFramework.Plugins.AdSchedulerPlugin, function (options) {
        if (!(this instanceof PlayerFramework.Plugins.FreeWheelPlugin)) {
            throw invalidConstruction;
        }

        this._lastTrackingTime = null;

        PlayerFramework.Plugins.AdSchedulerPlugin.call(this, options);
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

        // Public Methods
        loadAds: function (source) {
            this._adSlots = [];

            var promise = Microsoft.VideoAdvertising.FreeWheelFactory.loadSource(new Windows.Foundation.Uri(source)).then(
                function (result) {
                    var adResponse = result;

                    var videoTracking = PlayerFramework.Utilities.first(adResponse.siteSection.videoPlayer.videoAsset.eventCallbacks, function (eventCallback) {
                        return eventCallback.name === Microsoft.VideoAdvertising.FWEventCallback.videoView;
                    });

                    if (videoTracking) {
                        this._lastTrackingTime = null;

                        // position tracking
                        this.mediaPlayer.positionTrackingPlugin.trackingEvents.push({ positionPercentage: 0, data: videoTracking, area: freeWheelTrackingEventArea });
                        this.mediaPlayer.positionTrackingPlugin.trackingEvents.push({ positionPercentage: 1, data: videoTracking, area: freeWheelTrackingEventArea });

                        // play time tracking
                        for (var i = 15; i < 60; i += 15) {
                            this.mediaPlayer.playTimeTrackingPlugin.trackingEvents.push({ playTime: i, data: videoTracking, area: freeWheelTrackingEventArea });
                        }

                        for (var i = 60; i < 60 * 3; i += 30) {
                            this.mediaPlayer.playTimeTrackingPlugin.trackingEvents.push({ playTime: i, data: videoTracking, area: freeWheelTrackingEventArea });
                        }

                        for (var i = 60 * 3; i < 60 * 10; i += 60) {
                            this.mediaPlayer.playTimeTrackingPlugin.trackingEvents.push({ playTime: i, data: videoTracking, area: freeWheelTrackingEventArea });
                        }

                        for (var i = 60 * 10; i < 60 * 30; i += 120) {
                            this.mediaPlayer.playTimeTrackingPlugin.trackingEvents.push({ playTime: i, data: videoTracking, area: freeWheelTrackingEventArea });
                        }

                        for (var i = 60 * 30; i < 60 * 60; i += 300) {
                            this.mediaPlayer.playTimeTrackingPlugin.trackingEvents.push({ playTime: i, data: videoTracking, area: freeWheelTrackingEventArea });
                        }

                        for (var i = 60 * 60; i < 60 * 180; i += 600) {
                            this.mediaPlayer.playTimeTrackingPlugin.trackingEvents.push({ playTime: i, data: videoTracking, area: freeWheelTrackingEventArea });
                        }
                    }

                    var videoAsset = adResponse.siteSection.videoPlayer.videoAsset;

                    var promises = null;
                    if (videoAsset) {
                        promises = [];
                        for (var i = 0; i < videoAsset.adSlots.length; i++) {
                            var adSlot = videoAsset.adSlots[i];
                            (function(adSlot){
                                promises.push(Microsoft.VideoAdvertising.FreeWheelFactory.getAdDocumentPayload(adSlot, adResponse).then(function (payload) {
                                        var ad = null;
                                        switch (adSlot.timePositionClass) {
                                            case "preroll":
                                                ad = new PlayerFramework.Advertising.PrerollAdvertisement();
                                                break;

                                            case "postroll":
                                                ad = new PlayerFramework.Advertising.PostrollAdvertisement();
                                                break;

                                            default:
                                                ad = new PlayerFramework.Advertising.MidrollAdvertisement();
                                                ad.time = adSlot.timePosition / 1000;
                                                break;
                                        }

                                        ad.source = new Microsoft.PlayerFramework.Js.Advertising.AdSource();
                                        ad.source.type = Microsoft.VideoAdvertising.DocumentAdPayloadHandler.adType;
                                        ad.source.payload = payload;

                                        this.advertisements.push(ad);
                                        this._adSlots.push({ "ad": ad, "adSlot": adSlot });
                                }.bind(this), function () { /* ignore */ }));
                            }).bind(this)(adSlot);
                        }
                        this._adResponse = adResponse;
                    }

                    this._loadCompanions();
                    if (promises) {
                        return WinJS.Promise.join(promises);
                    }
                }.bind(this)
            );

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

        // Private Methods
        _setOptions: function (options) {
            PlayerFramework.Utilities.setOptions(this, options, {
                isEnabled: true,
                advertisements: [],
                preloadTime: 5,
                evaluateOnForwardOnly: true,
                interruptScrub: true,
                source: null
            });
        },

        _getAdSlot: function (ad) {
            var item = PlayerFramework.Utilities.first(this._adSlots, function (item) {
                return item.ad === ad;
            });

            if (item) {
                return item.adSlot;
            }

            return null;
        },

        _playAd: function (ad) {
            var adSlot = this._getAdSlot(ad);

            if (adSlot) {
                var impression = PlayerFramework.Utilities.first(adSlot.eventCallbacks, function (eventCallback) {
                    return eventCallback.type === Microsoft.VideoAdvertising.FWCallbackType.impression && eventCallback.name === Microsoft.VideoAdvertising.FWEventCallback.slotImpression;
                });

                if (impression) {
                    var urls = impression.getUrls();
                    for (var i = 0; i < urls.length; i++) {
                        var url = urls[i];
                        Microsoft.VideoAdvertising.AdTracking.current.fireTracking(url);
                    }
                }
            }
            
            return PlayerFramework.Plugins.AdSchedulerPlugin.prototype._playAd.call(this, ad);
        },

        _loadCompanions: function () {
            if (this._adResponse) {
                var enumerable = Microsoft.VideoAdvertising.FreeWheelFactory.getNonTemporalCompanions(this._adResponse);
                this._companions = PlayerFramework.Utilities.getArray(enumerable);
                this.mediaPlayer.adHandlerPlugin.loadCompanions(this._companions, Microsoft.VideoAdvertising.CompanionAdsRequired.all)
            }
        },

        _unloadCompanions: function () {
            if (this._companions) {
                this.mediaPlayer.adHandlerPlugin.unloadCompanions(this._companions);
                this._companions = null;
            }
        },

        _getTrackingUrl: function (url, start, end) {
            var now = Date.now();
            var init = start ? 1 : 0;
            var last = end ? 1 : 0;
            var ct = this._lastTrackingTime ? Math.round((now - this._lastTrackingTime) / 1000) : 0;
            
            // save for next time
            this._lastTrackingTime = now;

            if (url.indexOf("?") !== -1) {
                return PlayerFramework.Utilities.formatString("{0}&init={1}&last={2}&ct={3}", url, init, last, ct);
            } else {
                return PlayerFramework.Utilities.formatString("{0}?init={1}&last={2}&ct={3}", url, init, last, ct);
            }
        },

        _onLoad: function () {
            this._adSlots = [];
            this._adResponse = null;
            this._companions = null;

            PlayerFramework.Plugins.AdSchedulerPlugin.prototype._onLoad.call(this);
        },

        _onUnload: function () {
            this._adSlots = null;
            this._adResponse = null;
            this._companions = null;

            PlayerFramework.Plugins.AdSchedulerPlugin.prototype._onUnload.call(this);
        },

        _onActivate: function () {
            this._bindEvent("loading", this.mediaPlayer, this._onMediaPlayerLoading);
            this._bindEvent("eventtracked", this.mediaPlayer.playTimeTrackingPlugin, this._onMediaPlayerPlayTimeEventTracked);
            this._bindEvent("eventtracked", this.mediaPlayer.positionTrackingPlugin, this._onMediaPlayerPositionEventTracked);

            this._loadCompanions();

            return PlayerFramework.Plugins.AdSchedulerPlugin.prototype._onActivate.call(this);
        },

        _onDeactivate: function () {
            this._unloadCompanions();

            this._unbindEvent("loading", this.mediaPlayer, this._onMediaPlayerLoading);
            this._unbindEvent("eventtracked", this.mediaPlayer.playTimeTrackingPlugin, this._onMediaPlayerPlayTimeEventTracked);
            this._unbindEvent("eventtracked", this.mediaPlayer.positionTrackingPlugin, this._onMediaPlayerPositionEventTracked);

            PlayerFramework.Plugins.AdSchedulerPlugin.prototype._onDeactivate.call(this);
        },

        _onUpdate: function () {
            this._unloadCompanions();

            this._adSlots = [];
            this._adResponse = null;
            this._companions = null;

            PlayerFramework.Plugins.AdSchedulerPlugin.prototype._onUpdate.call(this);
        },

        _onMediaPlayerLoading: function (e) {
            if (this.source) {
                var promise = this.loadAds(this.source);
                e.detail.setPromise(promise);
            }
        },

        _onMediaPlayerPlayTimeEventTracked: function (e) {
            var trackingEvent = e.detail.trackingEvent;
            if (trackingEvent.area === freeWheelTrackingEventArea) {
                if (trackingEvent.data instanceof Microsoft.VideoAdvertising.FWEventCallback) {
                    var urls = PlayerFramework.Utilities.getArray(trackingEvent.data.getUrls());
                    for (var i = 0; i < urls.length; i++) {
                        var url = urls[i];
                        var trackingUrl = this._getTrackingUrl(url);
                        Microsoft.VideoAdvertising.AdTracking.current.fireTracking(trackingUrl);
                    }
                }
            }
        },

        _onMediaPlayerPositionEventTracked: function (e) {
            var trackingEvent = e.detail.trackingEvent;
            if (trackingEvent.area === freeWheelTrackingEventArea) {
                if (trackingEvent.data instanceof Microsoft.VideoAdvertising.FWEventCallback) {
                    var urls = PlayerFramework.Utilities.getArray(trackingEvent.data.getUrls());
                    for (var i = 0; i < urls.length; i++) {
                        var url = urls[i];
                        var start = !isNaN(trackingEvent.positionPercentage) && trackingEvent.positionPercentage === 0;
                        var end = !isNaN(trackingEvent.positionPercentage) && trackingEvent.positionPercentage === 1;
                        var trackingUrl = this._getTrackingUrl(url, start, end);
                        Microsoft.VideoAdvertising.AdTracking.current.fireTracking(trackingUrl);
                    }
                }
            }
        }
    });

    // FreeWheelPlugin Mixins
    WinJS.Class.mix(PlayerFramework.MediaPlayer, {
        freeWheelPlugin: {
            value: null,
            writable: true,
            configurable: true
        }
    });

    // FreeWheelPlugin Exports
    WinJS.Namespace.define("PlayerFramework.Plugins", {
        FreeWheelPlugin: FreeWheelPlugin
    });

})(PlayerFramework);


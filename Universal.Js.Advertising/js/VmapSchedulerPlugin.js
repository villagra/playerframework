(function (PlayerFramework, undefined) {
    "use strict";

    // VmapSchedulerPlugin Errors
    var invalidConstruction = "Invalid construction: VmapSchedulerPlugin constructor must be called using the \"new\" operator.";

    // VmapSchedulerPlugin Class
    var VmapSchedulerPlugin = WinJS.Class.derive(PlayerFramework.Plugins.AdSchedulerPlugin, function (options) {
        if (!(this instanceof PlayerFramework.Plugins.VmapSchedulerPlugin)) {
            throw invalidConstruction;
        }

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
            this._adBreaks = [];

            var promise = Microsoft.Media.Advertising.VmapFactory.loadSource(new Windows.Foundation.Uri(source)).then(
                function (result) {
                    for (var i = 0; i < result.adBreaks.length; i++) {
                        var adBreak = result.adBreaks[i];
                        var ad = null;

                        switch (adBreak.timeOffset) {
                            case "start":
                                ad = new PlayerFramework.Advertising.PrerollAdvertisement();
                                break;

                            case "end":
                                ad = new PlayerFramework.Advertising.PostrollAdvertisement();
                                break;

                            default:
                                var offset = Microsoft.Media.Advertising.FlexibleOffset.parse(adBreak.timeOffset);

                                if (offset) {
                                    ad = new PlayerFramework.Advertising.MidrollAdvertisement();
                                    if (offset.isAbsolute) {
                                        ad.time = offset.absoluteOffset / 1000;
                                    } else {
                                        ad.timePercentage = offset.relativeOffset;
                                    }
                                }

                                break;
                        }

                        if (ad) {
                            ad.source = this._getAdSource(adBreak.adSource);
                            if (ad.source) {
                                this.advertisements.push(ad);
                                this._adBreaks.push({ "ad": ad, "adBreak": adBreak });
                            }
                        }
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

        _getAdSource: function (source) {
            var adSource = null;

            if (source.vastData) {
                adSource = new Microsoft.PlayerFramework.Js.Advertising.AdSource();
                adSource.type = Microsoft.Media.Advertising.VastAdPayloadHandler.adType;
                adSource.payload = source.vastData;
            } else if (source.customAdData) {
                adSource = new Microsoft.PlayerFramework.Js.Advertising.AdSource();
                adSource.type = source.customAdDataTemplateType;
                adSource.payload = source.customAdData;
            } else if (source.adTag) {
                adSource = new Microsoft.PlayerFramework.Js.Advertising.RemoteAdSource();
                adSource.type = source.adTagTemplateType;
                adSource.uri = source.adTag;
            }

            if (adSource) {
                adSource.allowMultipleAds = source.allowMultipleAds;
                adSource.maxRedirectDepth = source.followsRedirect ? null : 0;
            }

            return adSource;
        },

        _getAdBreak: function (ad) {
            var item = PlayerFramework.Utilities.first(this._adBreaks, function (item) {
                return item.ad === ad;
            });

            if (item) {
                return item.adBreak;
            }

            return null;
        },

        _playAd: function (ad) {
            var adBreak = this._getAdBreak(ad);

            if (adBreak) {
                this._trackEvents(adBreak, Microsoft.Media.Advertising.VmapTrackingEventType.breakStart);

                return this.mediaPlayer.adHandlerPlugin.playAd(ad.source).then(
                    function () {
                        this._trackEvents(adBreak, Microsoft.Media.Advertising.VmapTrackingEventType.breakEnd);
                    }.bind(this),
                    function () {
                        this._trackEvents(adBreak, Microsoft.Media.Advertising.VmapTrackingEventType.error);
                    }.bind(this)
                );
            }
            
            return PlayerFramework.Plugins.AdSchedulerPlugin.prototype._playAd.call(this, ad);
        },

        _trackEvents: function (adBreak, eventType) {
            for (var i = 0; i < adBreak.trackingEvents.length; i++) {
                var event = adBreak.trackingEvents[i];
                if (event.eventType === eventType) {
                    Microsoft.Media.Advertising.AdTracking.current.fireTrackingUri(event.trackingUri);
                }
            }
        },

        _onLoad: function () {
            this._adBreaks = [];

            PlayerFramework.Plugins.AdSchedulerPlugin.prototype._onLoad.call(this);
        },

        _onUnload: function () {
            this._adBreaks = null;

            PlayerFramework.Plugins.AdSchedulerPlugin.prototype._onUnload.call(this);
        },

        _onActivate: function () {
            this._bindEvent("loading", this.mediaPlayer, this._onMediaPlayerLoading);

            return PlayerFramework.Plugins.AdSchedulerPlugin.prototype._onActivate.call(this);
        },

        _onDeactivate: function () {
            this._unbindEvent("loading", this.mediaPlayer, this._onMediaPlayerLoading);

            PlayerFramework.Plugins.AdSchedulerPlugin.prototype._onDeactivate.call(this);
        },

        _onUpdate: function () {
            this._adBreaks = [];

            PlayerFramework.Plugins.AdSchedulerPlugin.prototype._onUpdate.call(this);
        },

        _onMediaPlayerLoading: function (e) {
            if (this.source) {
                var promise = this.loadAds(this.source);
                e.detail.setPromise(promise);
            }
        }
    });

    // VmapSchedulerPlugin Mixins
    WinJS.Class.mix(PlayerFramework.MediaPlayer, {
        vmapSchedulerPlugin: {
            value: null,
            writable: true,
            configurable: true
        }
    });

    // VmapSchedulerPlugin Exports
    WinJS.Namespace.define("PlayerFramework.Plugins", {
        VmapSchedulerPlugin: VmapSchedulerPlugin
    });

})(PlayerFramework);


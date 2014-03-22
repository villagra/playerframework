(function (PlayerFramework, undefined) {
    "use strict";

    // AdHandlerPlugin Errors
    var invalidConstruction = "Invalid construction: AdHandlerPlugin constructor must be called using the \"new\" operator.",
        allCompanionAdsFailed = "All companion ads failed.",
        companionAdFailed = "Companion ad failed.";

    // AdHandlerPlugin Events
    var events = [
        "activeadplayerchanged",
        "adstatechanged",
        "adfailure",
        "activateadunit",
        "deactivateadunit"
    ];

    // AdHandlerPlugin Class
    var AdHandlerPlugin = WinJS.Class.derive(PlayerFramework.PluginBase, function (options) {
        if (!(this instanceof PlayerFramework.Plugins.AdHandlerPlugin)) {
            throw invalidConstruction;
        }

        PlayerFramework.PluginBase.call(this, options);
    }, {
        // Public Properties
        startTimeout: {
            get: function () {
                return this._startTimeout;
            },
            set: function (value) {
                this._startTimeout = value;
                if (this._controller) {
                    this._controller.startTimeout = value * 1000;
                }
            }
        },

        preferredBitrate: {
            get: function () {
                return this._preferredBitrate;
            },
            set: function (value) {
                this._preferredBitrate = value;
                if (this._mediaPlayerAdapter) {
                    this._mediaPlayerAdapter.currentBitrate = value;
                }
            }
        },

        adHandlerController: {
            get: function () {
                return this._controller;
            }
        },
        
        adPayloadHandlers: {
            get: function () {
                if (this._controller) {
                    return this._controller.adPayloadHandlers;
                } else {
                    return null;
                }
            }
        },

        activeAdPlayer: {
            get: function () {
                if (this._controller && this._controller.activeAdPlayer) {
                    return this._getVpaidAdapter(this._controller.activeAdPlayer).adPlayer;
                } else {
                    return null;
                }
            }
        },

        // Public Methods
        show: function () {
            this.mediaPlayer.addClass("pf-show-ad-container");
        },

        hide: function () {
            this.mediaPlayer.removeClass("pf-show-ad-container");
        },

        preloadAd: function (source) {
            return this._controller.preloadAdAsync(source);
        },

        playAd: function (source) {
            return this._controller.playAdAsync(source);
        },

        cancelActiveAds: function () {
            return this._controller.cancelActiveAds();
        },

        loadCompanions: function (companions, suggestedCompanionRules) {
            this._loadCompanions(null, companions, suggestedCompanionRules, null, null, null);
        },

        unloadCompanions: function (companions) {
            this._unloadCompanions(null, companions);
        },

        // Private Methods
        _setElement: function () {
            this._adContainerElement = PlayerFramework.Utilities.createElement(this.mediaPlayer.element, ["div", { "class": "pf-ad-container" }]);
        },

        _setOptions: function (options) {
            PlayerFramework.Utilities.setOptions(this, options, {
                isEnabled: true,
                startTimeout: 8,
                preferredBitrate: 0
            });
        },

        _getVpaidAdapter: function (adPlayer) {
            return PlayerFramework.Utilities.first(this._vpaidAdapters, function (item) {
                return item.nativeInstance === adPlayer;
            });
        },

        _getActiveIcons: function (adPlayer) {
            return PlayerFramework.Utilities.first(this._activeIcons, function (item) {
                return item.adPlayer === adPlayer;
            });
        },

        _getActiveCompanion: function (companion) {
            return PlayerFramework.Utilities.first(this._activeCompanions, function (item) {
                return item.companion === companion;
            });
        },

        _getAdPlayer: function (creativeSource) {
            // search plugins for ad player factories
            for (var i = 0; i < this.mediaPlayer.plugins.length; i++) {
                var plugin = this.mediaPlayer.plugins[i];
                if (plugin instanceof PlayerFramework.Advertising.AdPlayerFactoryPluginBase && plugin.isEnabled) {
                    var adPlayer = plugin.getPlayer(creativeSource);
                    if (adPlayer) {
                        var vpaidAdapter = new PlayerFramework.Advertising.VpaidAdapter(adPlayer);
                        this._vpaidAdapters.push(vpaidAdapter);
                        return vpaidAdapter.nativeInstance;
                    }
                }
            }
        },

        _setAdState: function (adState) {
            var newState = adState;
            var oldState = this.mediaPlayer.advertisingState;

            if (newState !== oldState) {
                // pause the media player if we're loading an ad or playing a linear ad
                if (this.mediaPlayer.playerState === PlayerFramework.PlayerState.started) {
                    if ((newState === Microsoft.Media.Advertising.AdState.loading || newState === Microsoft.Media.Advertising.AdState.linear) && (oldState === Microsoft.Media.Advertising.AdState.none || oldState === Microsoft.Media.Advertising.AdState.nonLinear)) {
                        this.mediaPlayer.pause();
                    } else if ((oldState === Microsoft.Media.Advertising.AdState.loading || oldState === Microsoft.Media.Advertising.AdState.linear) && (newState === Microsoft.Media.Advertising.AdState.none || newState === Microsoft.Media.Advertising.AdState.nonLinear)) {
                        this.mediaPlayer.play();
                    }
                }

                // update the media player's ad state
                this.mediaPlayer.advertisingState = newState;
            }

            // swap out the media player's view model depending on the ad state
            switch (newState) {
                case Microsoft.Media.Advertising.AdState.linear:
                    this.mediaPlayer.interactiveViewModel = new PlayerFramework.Advertising.VpaidLinearAdViewModel(this.activeAdPlayer, this.mediaPlayer);
                    break;
                case Microsoft.Media.Advertising.AdState.nonLinear:
                    this.mediaPlayer.interactiveViewModel = new PlayerFramework.Advertising.VpaidNonLinearAdViewModel(this.activeAdPlayer, this.mediaPlayer);
                    break;
                default:
                    this.mediaPlayer.interactiveViewModel = this.mediaPlayer.defaultInteractiveViewModel;
                    break;
            }
        },

        _loadPlayer: function (adPlayer) {
            var vpaidAdapter = this._getVpaidAdapter(adPlayer);
            this._adContainerElement.appendChild(vpaidAdapter.adPlayer.adElement);
        },

        _unloadPlayer: function (adPlayer) {
            var vpaidAdapter = this._getVpaidAdapter(adPlayer);
            if (this._adContainerElement.contains(vpaidAdapter.adPlayer.adElement)) {
                this._adContainerElement.removeChild(vpaidAdapter.adPlayer.adElement);
            }
            vpaidAdapter.dispose();

            var vpaidAdapterIndex = this._vpaidAdapters.indexOf(vpaidAdapter);
            this._vpaidAdapters.splice(vpaidAdapterIndex, 1);
        },

        _loadCompanions: function (adPlayer, companions, suggestedCompanionRules, creativeSource, creativeConcept, adSource) {
            if (this._previousCompanionCreativeConcept && this._previousCompanionCreativeConcept !== creativeConcept) {
                this._unloadCompanions(adPlayer);
            }

            try {
                if (companions) {
                    var totalCount = 0;
                    var failureCount = 0;

                    for (var i = 0; i < companions.length; i++) {
                        var companion = companions[i];
                        var container = null;

                        if (companion.type === Microsoft.Media.Advertising.CompanionType["static"]) {
                            if (companion.adSlotId) {
                                container = document.getElementById(companion.adSlotId);
                            }

                            if (!container && companion.width && companion.height) {
                                container = PlayerFramework.Utilities.first(document.querySelectorAll(".pf-companion-ad"), function (element) {
                                    return element.scrollWidth === companion.width && element.scrollHeight === companion.height;
                                });
                            }

                            if (container) {
                                var image = document.createElement("img");

                                this._bindEvent("load", image, this._onCompanionLoad, companion);
                                this._bindEvent("click", image, this._onCompanionClick, companion);

                                image.alt = companion.altText;
                                image.src = companion.content;

                                container.appendChild(image);
                            }
                        }

                        if (container) {
                            this._activeCompanions.push({
                                "companion": companion,
                                "container": container
                            });
                        } else {
                            failureCount++;
                        }

                        totalCount++;
                    }

                    if (suggestedCompanionRules === Microsoft.Media.Advertising.CompanionAdsRequired.any && failureCount === totalCount && totalCount > 0) {
                        throw allCompanionAdsFailed;
                    }

                    if (suggestedCompanionRules === Microsoft.Media.Advertising.CompanionAdsRequired.all && failureCount > 0) {
                        throw companionAdFailed;
                    }
                }

                this._previousCompanionCreativeConcept = creativeConcept;
            } catch (error) {
                this._unloadCompanions(adPlayer);
                throw error;
            }
        },

        _unloadCompanions: function (adPlayer, companions) {
            if (companions) {
                for (var i = 0; i < companions.length; i++) {
                    var companion = companions[i];
                    var activeCompanion = this._getActiveCompanion(companion);
                    activeCompanion.container.innerHTML = "";
                    PlayerFramework.Utilities.remove(this._activeCompanions, activeCompanion);
                }
            } else {
                for (var i = 0; i < this._activeCompanions.length; i++) {
                    var activeCompanion = this._activeCompanions[i];
                    activeCompanion.container.innerHTML = "";
                }

                this._activeCompanions = [];
                this._previousCompanionCreativeConcept = null;
            }
        },

        _showIcons: function (adPlayer, creativeSource) {
            if (!adPlayer.adIcons && creativeSource.icons && creativeSource.icons.length > 0) {
                var container = PlayerFramework.Utilities.createElement(this._adContainerElement, [
                    "div", {
                        "class": "pf-ad-icon-container"
                    }
                ]);

                var timeoutIds = [];

                // saved for later deactivation
                this._activeIcons.push({
                    "adPlayer": adPlayer,
                    "container": container,
                    "timeoutIds": timeoutIds
                });

                creativeSource.icons.forEach(function (icon) {
                    var staticResource = icon.item;
                    if (staticResource) {
                        // icon element
                        var image = document.createElement("img");

                        // icon events
                        this._bindEvent("load", image, this._onIconLoad, icon);
                        this._bindEvent("click", image, this._onIconClick, icon);

                        // icon size
                        if (icon.width) {
                            image.width = icon.width;
                        }

                        if (icon.height) {
                            image.height = icon.height;
                        }

                        // icon position
                        switch (icon.xposition) {
                            case "left":
                                image.style.left = "0px";
                                break;
                            case "right":
                                image.style.right = "0px";
                                break;
                            default:
                                var x = parseFloat(icon.xposition) || 0;
                                image.style.left = x + "px";
                                break;
                        }

                        switch (icon.yposition) {
                            case "top":
                                image.style.top = "0px";
                                break;
                            case "bottom":
                                image.style.bottom = "0px";
                                break;
                            default:
                                var y = parseFloat(icon.yposition) || 0;
                                image.style.top = y + "px";
                                break;
                        }

                        // icon offset
                        // NOTE: this does not account for pause time
                        if (icon.offset) {
                            var offsetTimeout = icon.offset;
                            var offsetTimeoutId = window.setTimeout(function () {
                                image.src = staticResource.value.rawUri;
                                container.appendChild(image);
                            }, offsetTimeout);

                            timeoutIds.push(offsetTimeoutId);
                        } else {
                            image.src = staticResource.value.rawUri;
                            container.appendChild(image);
                        }

                        // icon duration
                        // NOTE: this does not account for pause time
                        if (icon.duration) {
                            var durationTimeout = icon.duration + (icon.offset || 0);
                            var durationTimeoutId = window.setTimeout(function () {
                                container.removeChild(image);
                            }, durationTimeout);

                            timeoutIds.push(durationTimeoutId);
                        }
                    }
                }, this);
            }
        },

        _hideIcons: function (adPlayer) {
            // get active icons for this ad player
            var activeIcons = this._getActiveIcons(adPlayer);

            if (activeIcons) {
                // clear icon timeouts
                for (var i = 0; i < activeIcons.timeoutIds.length; i++) {
                    var timeoutId = activeIcons.timeoutIds[i];
                    window.clearTimeout(timeoutId);
                }

                // remove icon container and array item
                PlayerFramework.Utilities.removeElement(activeIcons.container);
                PlayerFramework.Utilities.remove(this._activeIcons, activeIcons);
            }
        },

        _onLoad: function () {
            this._vpaidAdapters = [];
            this._activeIcons = [];
            this._activeCompanions = [];
            this._previousCompanionCreativeConcept = null;

            // initialize media player adapter
            this._mediaPlayerAdapter = new PlayerFramework.Advertising.MediaPlayerAdapter(this.mediaPlayer);
            this._mediaPlayerAdapter.currentBitrate = this.preferredBitrate;

            // initialize controller
            this._controller = new Microsoft.Media.Advertising.AdHandlerController();
            this._controller.startTimeout = this.startTimeout * 1000;
            this._controller.player = this._mediaPlayerAdapter.nativeInstance;

            // search plugins for ad payload handlers
            for (var i = 0; i < this.mediaPlayer.plugins.length; i++) {
                var plugin = this.mediaPlayer.plugins[i];
                if (plugin instanceof PlayerFramework.Advertising.AdPayloadHandlerPluginBase && plugin.isEnabled) {
                    this.adPayloadHandlers.append(plugin.nativeInstance);
                }
            }
        },

        _onUnload: function () {
            // clear ad payload handlers
            this.adPayloadHandlers.clear();

            // dispose media player adapter
            this._mediaPlayerAdapter.dispose();
            this._mediaPlayerAdapter = null;

            // dispose vpaid adapters
            for (var i = 0; i < this._vpaidAdapters.length; i++) {
                this._vpaidAdapters[i].dispose();
            }

            this._vpaidAdapters = null;
            this._activeIcons = null;
            this._activeCompanions = null;
            this._previousCompanionCreativeConcept = null;
            this._controller.player = null;
            this._controller.startTimeout = null;
            this._controller = null;
        },

        _onActivate: function () {
            this._setElement();

            this._bindEvent("advertisingstatechange", this.mediaPlayer, this._onMediaPlayerAdvertisingStateChange);
            this._bindEvent("playerstatechange", this.mediaPlayer, this._onMediaPlayerPlayerStateChange);
            this._bindEvent("navigationrequest", this._controller, this._onControllerNavigationRequest);
            this._bindEvent("loadplayer", this._controller, this._onControllerLoadPlayer);
            this._bindEvent("unloadplayer", this._controller, this._onControllerUnloadPlayer);
            this._bindEvent("activeadplayerchanged", this._controller, this._onControllerActiveAdPlayerChanged);
            this._bindEvent("adstatechanged", this._controller, this._onControllerAdStateChanged);
            this._bindEvent("adfailure", this._controller, this._onControllerAdFailure);
            this._bindEvent("activateadunit", this._controller, this._onControllerActivateAdUnit);
            this._bindEvent("deactivateadunit", this._controller, this._onControllerDeactivateAdUnit);

            return true;
        },

        _onDeactivate: function () {
            this.cancelActiveAds();
            this.unloadCompanions();

            this._unbindEvent("advertisingstatechange", this.mediaPlayer, this._onMediaPlayerAdvertisingStateChange);
            this._unbindEvent("playerstatechange", this.mediaPlayer, this._onMediaPlayerPlayerStateChange);
            this._unbindEvent("navigationrequest", this._controller, this._onControllerNavigationRequest);
            this._unbindEvent("loadplayer", this._controller, this._onControllerLoadPlayer);
            this._unbindEvent("unloadplayer", this._controller, this._onControllerUnloadPlayer);
            this._unbindEvent("activeadplayerchanged", this._controller, this._onControllerActiveAdPlayerChanged);
            this._unbindEvent("adstatechanged", this._controller, this._onControllerAdStateChanged);
            this._unbindEvent("adfailure", this._controller, this._onControllerAdFailure);
            this._unbindEvent("activateadunit", this._controller, this._onControllerActivateAdUnit);
            this._unbindEvent("deactivateadunit", this._controller, this._onControllerDeactivateAdUnit);

            PlayerFramework.Utilities.removeElement(this._adContainerElement);

            this._adContainerElement = null;
        },

        _onUpdate: function () {
            this.cancelActiveAds();
            this.unloadCompanions();
        },

        _onMediaPlayerAdvertisingStateChange: function (e) {
            if (this.mediaPlayer.advertisingState !== PlayerFramework.AdvertisingState.none) {
                this.show();
            } else {
                this.hide();
            }
        },

        _onMediaPlayerPlayerStateChange: function (e) {
            // hide media element for preroll ads
            if (this.mediaPlayer.playerState === PlayerFramework.PlayerState.opened && this.mediaPlayer.autoplay && !this.mediaPlayer.startupTime) {
                this.mediaPlayer.mediaElement.style.visibility = "hidden";
            } else if (this.mediaPlayer.playerState !== PlayerFramework.PlayerState.starting) {
                this.mediaPlayer.mediaElement.style.visibility = "visible";
            }
        },

        _onControllerNavigationRequest: function (e) {
            PlayerFramework.Utilities.launch(e.url);
        },

        _onControllerLoadPlayer: function (e) {
            e.player = this._getAdPlayer(e.creativeSource);
            if (e.player) {
                this._loadPlayer(e.player);
            }
        },

        _onControllerUnloadPlayer: function (e) {
            this._unloadPlayer(e.player);
        },

        _onControllerActiveAdPlayerChanged: function (e) {
            this.dispatchEvent("activeadplayerchanged", e);
        },

        _onControllerAdStateChanged: function (e) {
            var adState = this._controller.adState;
            this._setAdState(adState);
            this.dispatchEvent("adstatechanged", { "adState": adState });
        },

        _onControllerAdFailure: function (e) {
            this.dispatchEvent("adfailure", e);
        },

        _onControllerActivateAdUnit: function (e) {
            this.dispatchEvent("activateadunit", e);
            this._showIcons(e.player, e.creativeSource);
            this._loadCompanions(e.player, e.companions, e.suggestedCompanionRules, e.creativeSource, e.creativeConcept, e.adSource);
        },

        _onControllerDeactivateAdUnit: function (e) {
            this.dispatchEvent("deactivateadunit", e);
            this._hideIcons(e.player);

            if (e.error) {
                this._unloadCompanions(e.player);
            }
        },

        _onIconLoad: function (icon, e) {
            if (icon.viewTracking) {
                for (var i = 0; i < icon.viewTracking.length; i++) {
                    var url = icon.viewTracking[i];
                    Microsoft.Media.Advertising.VastHelpers.fireTracking(url);
                }
            }
        },

        _onIconClick: function (icon, e) {
            if (icon.clickThrough) {
                PlayerFramework.Utilities.launch(icon.clickThrough.rawUri);
            }

            if (icon.clickTracking) {
                for (var i = 0; i < icon.clickTracking.length; i++) {
                    var url = icon.clickTracking[i];
                    Microsoft.Media.Advertising.VastHelpers.fireTracking(url);
                }
            }
        },

        _onCompanionLoad: function (companion, e) {
            if (companion.viewTracking) {
                for (var i = 0; i < companion.viewTracking.length; i++) {
                    var url = companion.viewTracking[i];
                    Microsoft.Media.Advertising.VastHelpers.fireTracking(url);
                }
            }
        },

        _onCompanionClick: function (companion, e) {
            if (companion.clickThrough) {
                PlayerFramework.Utilities.launch(companion.clickThrough.rawUri);
            }

            if (companion.clickTracking) {
                for (var i = 0; i < companion.clickTracking.length; i++) {
                    var url = companion.clickTracking[i];
                    Microsoft.Media.Advertising.VastHelpers.fireTracking(url);
                }
            }
        }
    });

    // AdHandlerPlugin Mixins
    WinJS.Class.mix(AdHandlerPlugin, PlayerFramework.Utilities.createEventProperties(events));

    WinJS.Class.mix(PlayerFramework.MediaPlayer, {
        adHandlerPlugin: {
            value: null,
            writable: true,
            configurable: true
        }
    });

    // AdHandlerPlugin Exports
    WinJS.Namespace.define("PlayerFramework.Plugins", {
        AdHandlerPlugin: AdHandlerPlugin
    });
    
})(PlayerFramework);


(function (PlayerFramework, undefined) {
    "use strict";

    // VpaidNonLinearAdPlayer Errors
    var invalidConstruction = "Invalid construction: VpaidNonLinearAdPlayer constructor must be called using the \"new\" operator.",
        notImplemented = "Not implemented.";

    // VpaidNonLinearAdPlayer Class
    var VpaidNonLinearAdPlayer = WinJS.Class.derive(PlayerFramework.Advertising.VpaidAdPlayerBase, function (skippableOffset, suggestedDuration, clickThru, clickThruText, dimensions) {
        if (!(this instanceof PlayerFramework.Advertising.VpaidNonLinearAdPlayer)) {
            throw invalidConstruction;
        }

        this._dimensions = dimensions;
        this._skippableOffset = skippableOffset;
        this._suggestedDuration = suggestedDuration;
        this._clickThru = clickThru;
        this._clickThruText = clickThruText;

        this._itemElement = null;
        this._adElement = null;
        this._linkElement = null;

        this._adState = PlayerFramework.Advertising.VpaidAdPlayerBase.AD_STATE_NONE;
        this._adSkippableState = false;
        this._adVolume = null;
        this._timer = null;
        this._timerStartTime = null;
        this._timerPauseTime = null;
        this._skippableTime = null;
        this._skippableReached = false;
        this._firstQuartileTime = null;
        this._firstQuartileReached = false;
        this._midpointTime = null;
        this._midpointReached = false;
        this._thirdQuartileTime = null;
        this._thirdQuartileReached = false;
        this._endpointTime = null;
        this._endpointReached = false;

        this._setElement();
    }, {
        // Public Properties
        adElement: {
            get: function () {
                return this._adElement;
            }
        },

        adState: {
            get: function () {
                return this._adState;
            }
        },

        adSkippableState: {
            get: function () {
                return this._adSkippableState;
            }
        },

        skippableOffset: {
            get: function () {
                return this._skippableOffset;
            }
        },

        suggestedDuration: {
            get: function () {
                return this._suggestedDuration;
            }
        },

        clickThru: {
            get: function () {
                return this._clickThru;
            }
        },

        adLinear: {
            get: function () {
                return false;
            }
        },

        adVolume: {
            get: function () {
                return this._adVolume;
            },
            set: function (value) {
                this._adVolume = value;
                this.dispatchEvent("advolumechanged");
            }
        },

        adWidth: {
            get: function () {
                return this._itemElement.scrollWidth;
            }
        },

        adHeight: {
            get: function () {
                return this._itemElement.scrollHeight;
            }
        },

        adDuration: {
            get: function () {
                return this._suggestedDuration !== null ? this._suggestedDuration : 0;
            }
        },

        adRemainingTime: {
            get: function () {
                return this.adDuration > 0 ? this.adDuration - this._getCurrentTime() : 0;
            }
        },

        // Public Methods
        initAd: function (width, height, viewMode, desiredBitrate, creativeData, environmentVariables) {
            this._adState = PlayerFramework.Advertising.VpaidAdPlayerBase.AD_STATE_LOADING;
            this._adSkippableState = false;
        },

        startAd: function () {
            this._adState = PlayerFramework.Advertising.VpaidAdPlayerBase.AD_STATE_STARTING;
            this._onAdStarted();
        },

        stopAd: function () {
            this._onAdStopped();
        },

        pauseAd: function () {
            // TODO: test once this is properly wired up to view model
            this._adState = PlayerFramework.Advertising.VpaidAdPlayerBase.AD_STATE_PAUSED;
            this._pauseTimer();
            this.dispatchEvent("adpaused");
        },

        resumeAd: function () {
            // TODO: test once this is properly wired up to view model
            this._adState = PlayerFramework.Advertising.VpaidAdPlayerBase.AD_STATE_PLAYING;
            this._resumeTimer();
            this.dispatchEvent("adplaying");
        },

        resizeAd: function (width, height, viewMode) {
            // normally we don't have to do anything since we rely on CSS to resize us automatically
        },

        expandAd: function () {
            throw notImplemented;
        },

        collapseAd: function () {
            throw notImplemented;
        },

        skipAd: function () {
            if (this._adSkippableState) {
                this.dispatchEvent("adskipped");
                this._onAdStopped();
            }
        },
        
        // Private Methods
        _getElement: function () {
            return PlayerFramework.Utilities.createElement(null, [
                "div", {
                    "class": "pf-nonlinear-ad"
                }, [
                    "div", {
                        "style": "display: none;"
                    }, [
                        "a", {
                            "class": "pf-link",
                            "href": "javascript: void 0;"
                        }
                    ]
                ]
            ]);
        },

        _setElement: function () {
            this._adElement = this._getElement();

            this._itemElement = this._adElement.querySelector("#item");
            this._linkElement = this._adElement.querySelector("a");

            if (this._itemElement && this._dimensions)
            {
                this._itemElement.width = this._dimensions.width;
                this._itemElement.height = this._dimensions.height;
            }

            if (this._clickThru) {
                if (this._clickThruText) {
                    this._linkElement.textContent = this._clickThruText;
                    this._linkElement.parentElement.style.display = "";

                    WinJS.Utilities.addClass(this._linkElement, "pf-functional");
                    this._bindEvent("click", this._linkElement, this._onLinkClick);
                } else {
                    WinJS.Utilities.addClass(this._itemElement, "pf-functional");
                    this._bindEvent("click", this._itemElement, this._onItemClick);
                }
            }
        },

        _setAdSkippableState: function (value) {
            if (this._adSkippableState !== value) {
                this._adSkippableState = value;
                this.dispatchEvent("adskippablestatechange");
            }
        },

        _getCurrentTime: function () {
            if (this._timerPauseTime !== null) {
                return this._timerPauseTime - this._timerStartTime;
            } else if (this._timerStartTime !== null) {
                return Date.now() - this._timerStartTime;
            } else {
                return null;
            }
        },

        _startTimer: function () {
            this._timer = window.setInterval(this._onTimerTick.bind(this), 1000);
            this._timerStartTime = Date.now();
            this._timerPauseTime = null;
        },

        _stopTimer: function () {
            window.clearInterval(this._timer);
            this._timer = null;
            this._timerStartTime = null;
            this._timerPauseTime = null;
        },

        _pauseTimer: function () {
            window.clearInterval(this._timer);
            this._timer = null;
            this._timerPauseTime = Date.now();
        },

        _resumeTimer: function () {
            this._timer = window.setInterval(this._onTimerTick.bind(this), 1000);
            this._timerStartTime = Date.now() - this._getCurrentTime();
            this._timerPauseTime = null;
        },

        _onTimerTick: function () {
            this.dispatchEvent("adremainingtimechange");

            var currentTime = this._getCurrentTime();

            if (!this._skippableReached && this._skippableTime !== null && currentTime >= this._skippableTime) {
                this._skippableReached = true;
                this._setAdSkippableState(true);
            }

            if (!this._firstQuartileReached && this._firstQuartileTime !== null && currentTime >= this._firstQuartileTime) {
                this._firstQuartileReached = true;
                this.dispatchEvent("advideofirstquartile");
            }

            if (!this._midpointReached && this._midpointTime !== null && currentTime >= this._midpointTime) {
                this._midpointReached = true;
                this.dispatchEvent("advideomidpoint");
            }

            if (!this._thirdQuartileReached && this._thirdQuartileTime !== null && currentTime >= this._thirdQuartileTime) {
                this._thirdQuartileReached = true;
                this.dispatchEvent("advideothirdquartile");
            }

            if (!this._endpointReached && this._endpointTime !== null && currentTime >= this._endpointTime) {
                this._endpointReached = true;
                this.dispatchEvent("advideocomplete");
                this.stopAd();
            }
        },

        _onItemClick: function (e) {
            this._onAdClicked();
        },
        
        _onLinkClick: function (e) {
            this._onAdClicked();
        },

        _onAdClicked: function () {
            if (this._clickThru) {
                // TODO: provide id event argument
                this.dispatchEvent("adclickthru", { url: this._clickThru.rawUri, playerHandles: true });
            }
        },

        _onItemResize: function (e) {
            this.dispatchEvent("adsizechanged");
        },

        _onAdStarted: function () {
            this._adState = PlayerFramework.Advertising.VpaidAdPlayerBase.AD_STATE_PLAYING;
            this._adElement.style.visibility = "visible";

            if (PlayerFramework.Utilities.isWinJS1) {
                this._bindEvent("resize", this._itemElement, this._onItemResize);
            }
            else { // IE11 no longer supports resize event for arbitrary elements. The best we can do is listen to the window resize event.
                this._bindEvent("resize", window, this._onItemResize);
            }

            this.dispatchEvent("adstarted");
            this.dispatchEvent("adimpression");

            if (this._suggestedDuration !== null) {
                this._startTimer();
                this.dispatchEvent("advideostart");
            }
        },

        _onAdLoaded: function() {
            this._adState = PlayerFramework.Advertising.VpaidAdPlayerBase.AD_STATE_LOADED;
            this.dispatchEvent("adloaded");

            if (this._skippableOffset) {
                if (!this._skippableOffset.isAbsolute) {
                    this._skippableTime = this.adDuration * this._skippableOffset.relativeOffset;
                } else {
                    this._skippableTime = this._skippableOffset.absoluteOffset;
                }
            }

            if (this._suggestedDuration !== null) {
                this._firstQuartileTime = this.adDuration * 0.25;
                this._midpointTime = this.adDuration * 0.5;
                this._thirdQuartileTime = this.adDuration * 0.75;
                this._endpointTime = this.adDuration;
            }
        },

        _onAdStopped: function () {
            if (this._adState !== PlayerFramework.Advertising.VpaidAdPlayerBase.AD_STATE_COMPLETE && this._adState !== PlayerFramework.Advertising.VpaidAdPlayerBase.AD_STATE_FAILED) {
                this._adState = PlayerFramework.Advertising.VpaidAdPlayerBase.AD_STATE_COMPLETE;
                this._teardownAd();
                this.dispatchEvent("adstopped");
            }
        },

        _onAdFailed: function (message) {
            if (this._adState !== PlayerFramework.Advertising.VpaidAdPlayerBase.AD_STATE_COMPLETE && this._adState !== PlayerFramework.Advertising.VpaidAdPlayerBase.AD_STATE_FAILED) {
                this._adState = PlayerFramework.Advertising.VpaidAdPlayerBase.AD_STATE_FAILED;
                this._teardownAd();
                this.dispatchEvent("aderror", { message: message });
            }
        },

        _teardownAd: function () {
            this._stopTimer();
            this._unbindEvents();
            this._adElement.style.visibility = "hidden";
        }
    });

    // VpaidNonLinearAdPlayer Exports
    WinJS.Namespace.define("PlayerFramework.Advertising", {
        VpaidNonLinearAdPlayer: VpaidNonLinearAdPlayer
    });

    // VpaidIFrameAdPlayer Errors
    var invalidIFrameConstruction = "Invalid construction: VpaidIFrameAdPlayer constructor must be called using the \"new\" operator.",
        notImplemented = "Not implemented.";

    // VpaidIFrameAdPlayer Class
    var VpaidIFrameAdPlayer = WinJS.Class.derive(PlayerFramework.Advertising.VpaidNonLinearAdPlayer, function (skippableOffset, suggestedDuration, clickThru, clickThruText, dimensions) {
        if (!(this instanceof PlayerFramework.Advertising.VpaidIFrameAdPlayer)) {
            throw invalidIFrameConstruction;
        }

        PlayerFramework.Advertising.VpaidNonLinearAdPlayer.call(this, skippableOffset, suggestedDuration, clickThru, clickThruText, dimensions);
    }, {
        // overrides
        _getElement: function () {
            return PlayerFramework.Utilities.createElement(null, [
                "div", {
                    "class": "pf-nonlinear-ad"
                }, [
                    "div", [
                        "iframe", {
                            "id": "item",
                            "frameBorder": "no",
                            "scrolling": "no"
                        }
                    ]
                ], [
                    "div", {
                        "style": "display: none;"
                    }, [
                        "a", {
                            "class": "pf-link",
                            "href": "javascript: void 0;"
                        }
                    ]
                ]
            ])
        },

        _onItemReadyStateChange: function (x) {
            if (this._itemElement.readyState == "complete") {
                this._onAdLoaded();
            }
        },

        _onItemLoad: function (e) {
            // do nothing
        },

        _onItemAbort: function (e) {
            this._onAdFailed("IFRAME aborted");
        },

        _onItemError: function (e) {
            this._onAdFailed("IFRAME error");
        },

        initAd: function (width, height, viewMode, desiredBitrate, creativeData, environmentVariables) {
            PlayerFramework.Advertising.VpaidNonLinearAdPlayer.prototype.initAd.call(this, width, height, viewMode, desiredBitrate, creativeData, environmentVariables);

            this._bindEvent("load", this._itemElement, this._onItemLoad);
            this._bindEvent("abort", this._itemElement, this._onItemAbort);
            this._bindEvent("error", this._itemElement, this._onItemError);
            this._bindEvent("readystatechange", this._itemElement, this._onItemReadyStateChange);

            this._itemElement.src = creativeData;
        },

        stopAd: function () {
            this._itemElement.removeAttribute("src");
            PlayerFramework.Advertising.VpaidNonLinearAdPlayer.prototype.stopAd.call(this);
        }
    });

    // VpaidNonLinearAdPlayer Errors
    var invalidHtmlConstruction = "Invalid construction: VpaidHtmlAdPlayer constructor must be called using the \"new\" operator.",
        notImplemented = "Not implemented.";

    // VpaidNonLinearAdPlayer Class
    var VpaidHtmlAdPlayer = WinJS.Class.derive(PlayerFramework.Advertising.VpaidNonLinearAdPlayer, function (skippableOffset, suggestedDuration, clickThru, clickThruText, dimensions) {
        if (!(this instanceof PlayerFramework.Advertising.VpaidHtmlAdPlayer)) {
            throw invalidHtmlConstruction;
        }

        PlayerFramework.Advertising.VpaidNonLinearAdPlayer.call(this, skippableOffset, suggestedDuration, clickThru, clickThruText, dimensions);
    }, {
        // overrides
        _getElement: function () {
            return PlayerFramework.Utilities.createElement(null, [
                "div", {
                    "class": "pf-nonlinear-ad"
                }, [
                    "div", [
                        "div", {
                            "id": "item"
                        }
                    ]
                ], [
                    "div", {
                        "style": "display: none;"
                    }, [
                        "a", {
                            "class": "pf-link",
                            "href": "javascript: void 0;"
                        }
                    ]
                ]
            ])
        },

        initAd: function (width, height, viewMode, desiredBitrate, creativeData, environmentVariables) {
            PlayerFramework.Advertising.VpaidNonLinearAdPlayer.prototype.initAd.call(this, width, height, viewMode, desiredBitrate, creativeData, environmentVariables);

            this._itemElement.innerHTML = creativeData;
            this._onAdLoaded();
        },

        stopAd: function () {
            this._itemElement.innerHTML = "";

            PlayerFramework.Advertising.VpaidNonLinearAdPlayer.prototype.stopAd.call(this);
        }
    });


    // VpaidIFrameAdPlayer Errors
    var invalidImageConstruction = "Invalid construction: VpaidImageAdPlayer constructor must be called using the \"new\" operator.",
        notImplemented = "Not implemented.";

    // VpaidIFrameAdPlayer Class
    var VpaidImageAdPlayer = WinJS.Class.derive(PlayerFramework.Advertising.VpaidNonLinearAdPlayer, function (skippableOffset, suggestedDuration, clickThru, clickThruText, dimensions) {
        if (!(this instanceof PlayerFramework.Advertising.VpaidImageAdPlayer)) {
            throw invalidIFrameConstruction;
        }

        PlayerFramework.Advertising.VpaidNonLinearAdPlayer.call(this, skippableOffset, suggestedDuration, clickThru, clickThruText, dimensions);
    }, {
        // overrides
        _getElement: function () {
            return PlayerFramework.Utilities.createElement(null, [
                "div", {
                    "class": "pf-nonlinear-ad"
                }, [
                    "div", [
                        "img", {
                            "id": "item"
                        }
                    ]
                ], [
                    "div", {
                        "style": "display: none;"
                    }, [
                        "a", {
                            "class": "pf-link",
                            "href": "javascript: void 0;"
                        }
                    ]
                ]
            ])
        },

        _onItemLoad: function (e) {
            this._onAdLoaded();
        },

        _onItemAbort: function (e) {
            var message = PlayerFramework.Utilities.getImageErrorMessageForCode(PlayerFramework.ImageErrorCode.aborted);
            this._onAdFailed(message);
        },

        _onItemError: function (e) {
            var message = PlayerFramework.Utilities.getImageErrorMessageForCode(PlayerFramework.ImageErrorCode.unknown);
            this._onAdFailed(message);
        },

        initAd: function (width, height, viewMode, desiredBitrate, creativeData, environmentVariables) {
            PlayerFramework.Advertising.VpaidNonLinearAdPlayer.prototype.initAd.call(this, width, height, viewMode, desiredBitrate, creativeData, environmentVariables);

            this._bindEvent("load", this._itemElement, this._onItemLoad);
            this._bindEvent("abort", this._itemElement, this._onItemAbort);
            this._bindEvent("error", this._itemElement, this._onItemError);

            this._itemElement.src = creativeData;
        },

        stopAd: function () {
            this._itemElement.removeAttribute("src");
            PlayerFramework.Advertising.VpaidNonLinearAdPlayer.prototype.stopAd.call(this);
        }
    });

    // VpaidNonLinearAdPlayer Exports
    WinJS.Namespace.define("PlayerFramework.Advertising", {
        VpaidIFrameAdPlayer: VpaidIFrameAdPlayer,
        VpaidHtmlAdPlayer: VpaidHtmlAdPlayer,
        VpaidImageAdPlayer: VpaidImageAdPlayer
    });

})(PlayerFramework);


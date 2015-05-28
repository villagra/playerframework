(function (PlayerFramework, undefined) {
    "use strict";

    // VpaidVideoAdPlayer Errors
    var invalidConstruction = "Invalid construction: VpaidVideoAdPlayer constructor must be called using the \"new\" operator.",
        notImplemented = "Not implemented.";

    // VpaidVideoAdPlayer Class
    var VpaidVideoAdPlayer = WinJS.Class.derive(PlayerFramework.Advertising.VpaidAdPlayerBase, function (skippableOffset, maxDuration, clickThru, clickThruText) {
        if (!(this instanceof PlayerFramework.Advertising.VpaidVideoAdPlayer)) {
            throw invalidConstruction;
        }

        this._skippableOffset = skippableOffset;
        this._maxDuration = maxDuration;
        this._clickThru = clickThru;
        this._clickThruText = clickThruText;

        this._adElement = null;
        this._videoElement = null;
        this._linkElement = null;

        this._adState = PlayerFramework.Advertising.VpaidAdPlayerBase.AD_STATE_NONE;
        this._adSkippableState = false;
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

        msAudioCategory: {
            get: function () {
                return this._videoElement.msAudioCategory;
            },
            set: function (value) {
                this._videoElement.msAudioCategory = value;
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

        maxDuration: {
            get: function () {
                return this._maxDuration;
            }
        },

        clickThru: {
            get: function () {
                return this._clickThru;
            }
        },

        adLinear: {
            get: function () {
                return true;
            }
        },

        adVolume: {
            get: function () {
                return this._videoElement.volume;
            },
            set: function (value) {
                if (this._videoElement.volume !== value) {
                    this._videoElement.volume = value;
                    this.dispatchEvent("advolumechanged");
                }
            }
        },

        adWidth: {
            get: function () {
                return this._videoElement.videoWidth;
            }
        },

        adHeight: {
            get: function () {
                return this._videoElement.videoHeight;
            }
        },

        adDuration: {
            get: function () {
                return this._maxDuration !== null ? this._maxDuration : (this._videoElement.duration * 1000) || 0;
            }
        },

        adRemainingTime: {
            get: function () {
                return this.adDuration - this._videoElement.currentTime * 1000;
            }
        },

        // Public Methods
        initAd: function (width, height, viewMode, desiredBitrate, creativeData, environmentVariables) {
            this._adState = PlayerFramework.Advertising.VpaidAdPlayerBase.AD_STATE_LOADING;
            this._adSkippableState = false;

            this._bindEvent("canplaythrough", this._videoElement, this._onVideoCanPlayThrough);
            this._bindEvent("play", this._videoElement, this._onVideoPlay);
            this._bindEvent("error", this._videoElement, this._onVideoError);

            this._videoElement.autoplay = false;
            this._videoElement.preload = "auto";
            this._videoElement.src = creativeData;
        },

        startAd: function () {
            this._adState = PlayerFramework.Advertising.VpaidAdPlayerBase.AD_STATE_STARTING;
            this._videoElement.play();
        },

        stopAd: function () {
            this._videoElement.removeAttribute("src");
        },

        pauseAd: function () {
            this._videoElement.pause();
        },

        resumeAd: function () {
            this._videoElement.play();
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
        _setElement: function () {
            this._adElement = PlayerFramework.Utilities.createElement(null, [
                "div", {
                    "class": "pf-linear-ad"
                }, [
                    "div", [
                        "video"
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
            ]);

            this._videoElement = this._adElement.querySelector("video");
            this._linkElement = this._adElement.querySelector("a");

            if (this._clickThru) {
                if (this._clickThruText) {
                    this._linkElement.textContent = this._clickThruText;
                    this._linkElement.parentElement.style.display = "";

                    WinJS.Utilities.addClass(this._linkElement, "pf-functional");
                    this._bindEvent("click", this._linkElement, this._onLinkClick);
                } else {
                    WinJS.Utilities.addClass(this._videoElement, "pf-functional");
                    this._bindEvent("click", this._videoElement, this._onVideoClick);
                }
            }
        },

        _setAdSkippableState: function (value) {
            if (this._adSkippableState !== value) {
                this._adSkippableState = value;
                this.dispatchEvent("adskippablestatechange");
            }
        },

        _onVideoCanPlayThrough: function (e) {
            this._adState = PlayerFramework.Advertising.VpaidAdPlayerBase.AD_STATE_LOADED;
            this.dispatchEvent("adloaded");

            if (this._skippableOffset) {
                if (!this._skippableOffset.isAbsolute) {
                    this._skippableTime = this.adDuration * this._skippableOffset.relativeOffset;
                } else {
                    this._skippableTime = this._skippableOffset.absoluteOffset;
                }
            }

            this._firstQuartileTime = this.adDuration * 0.25;
            this._midpointTime = this.adDuration * 0.5;
            this._thirdQuartileTime = this.adDuration * 0.75;
            this._endpointTime = this.adDuration;
        },

        _onVideoPlay: function (e) {
            if (this._adState === PlayerFramework.Advertising.VpaidAdPlayerBase.AD_STATE_STARTING) {
                this._onAdStarted();
            }
        },

        _onVideoTimeUpdate: function (e) {
            this.dispatchEvent("adremainingtimechange");

            var currentTime = this._videoElement.currentTime * 1000;

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

        _onVideoPlaying: function (e) {
            if (this._adState === PlayerFramework.Advertising.VpaidAdPlayerBase.AD_STATE_PAUSED) {
                this._adState = PlayerFramework.Advertising.VpaidAdPlayerBase.AD_STATE_PLAYING;
                this.dispatchEvent("adplaying");
            }
        },

        _onVideoPause: function (e) {
            if (this._adState === PlayerFramework.Advertising.VpaidAdPlayerBase.AD_STATE_PLAYING) {
                this._adState = PlayerFramework.Advertising.VpaidAdPlayerBase.AD_STATE_PAUSED;
                this.dispatchEvent("adpaused");
            }
        },

        _onVideoEnded: function (e) {
            this.dispatchEvent("advideocomplete");
            this._onAdStopped();
        },

        _onVideoEmptied: function (e) {
            this._onAdStopped();
        },

        _onVideoError: function (e) {
            var message = PlayerFramework.Utilities.getMediaErrorMessage(this._videoElement.error);
            this._onAdFailed(message);
        },

        _onVideoResize: function (e) {
            this.dispatchEvent("adsizechanged");
        },

        _onVideoClick: function (e) {
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

        _onAdStarted: function () {
            this._adState = PlayerFramework.Advertising.VpaidAdPlayerBase.AD_STATE_PLAYING;
            this._adElement.style.visibility = "visible";

            this._bindEvent("timeupdate", this._videoElement, this._onVideoTimeUpdate);
            this._bindEvent("playing", this._videoElement, this._onVideoPlaying);
            this._bindEvent("pause", this._videoElement, this._onVideoPause);
            this._bindEvent("ended", this._videoElement, this._onVideoEnded);
            this._bindEvent("emptied", this._videoElement, this._onVideoEmptied);
            if (this._videoElement.onresize !== undefined) {
                this._bindEvent("resize", this._videoElement, this._onVideoResize);
            }
            else { // IE11 no longer supports resize event for arbitrary elements. The best we can do is listen to the window resize event.
                this._bindEvent("resize", window, this._onVideoResize);
            }

            this.dispatchEvent("adstarted");
            this.dispatchEvent("adimpression");
            this.dispatchEvent("advideostart");
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
            this._unbindEvents();
            this._adElement.style.visibility = "hidden";
        }
    });

    // VpaidVideoAdPlayer Exports
    WinJS.Namespace.define("PlayerFramework.Advertising", {
        VpaidVideoAdPlayer: VpaidVideoAdPlayer
    });

})(PlayerFramework);


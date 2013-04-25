(function (PlayerFramework, undefined) {
    "use strict";

    // VpaidAdapter Errors
    var invalidConstruction = "Invalid construction: VpaidAdapter constructor must be called using the \"new\" operator.",
        invalidAdPlayer = "Invalid argument: VpaidAdapter expects a VpaidAdPlayerBase as the first argument.";

    // VpaidAdapter Class
    var VpaidAdapter = WinJS.Class.define(function (adPlayer) {
        if (!(this instanceof PlayerFramework.Advertising.VpaidAdapter)) {
            throw invalidConstruction;
        }

        if (!(adPlayer instanceof PlayerFramework.Advertising.VpaidAdPlayerBase)) {
            throw invalidAdPlayer;
        }

        this._adPlayer = adPlayer;
        this._nativeInstance = new Microsoft.PlayerFramework.Js.Advertising.VpaidAdapterBridge();

        this._bindEvent("adloaded", this._adPlayer, this._onAdLoaded);
        this._bindEvent("adstarted", this._adPlayer, this._onAdStarted);
        this._bindEvent("adstopped", this._adPlayer, this._onAdStopped);
        this._bindEvent("adplaying", this._adPlayer, this._onAdPlaying);
        this._bindEvent("adpaused", this._adPlayer, this._onAdPaused);
        this._bindEvent("adexpandedchanged", this._adPlayer, this._onAdExpandedChanged);
        this._bindEvent("adlinearchanged", this._adPlayer, this._onAdLinearChanged);
        this._bindEvent("advolumechanged", this._adPlayer, this._onAdVolumeChanged);
        this._bindEvent("advideostart", this._adPlayer, this._onAdVideoStart);
        this._bindEvent("advideofirstquartile", this._adPlayer, this._onAdVideoFirstQuartile);
        this._bindEvent("advideomidpoint", this._adPlayer, this._onAdVideoMidpoint);
        this._bindEvent("advideothirdquartile", this._adPlayer, this._onAdVideoThirdQuartile);
        this._bindEvent("advideocomplete", this._adPlayer, this._onAdVideoComplete);
        this._bindEvent("aduseracceptinvitation", this._adPlayer, this._onAdUserAcceptInvitation);
        this._bindEvent("aduserclose", this._adPlayer, this._onAdUserClose);
        this._bindEvent("aduserminimize", this._adPlayer, this._onAdUserMinimize);
        this._bindEvent("adremainingtimechange", this._adPlayer, this._onAdRemainingTimeChange);
        this._bindEvent("adimpression", this._adPlayer, this._onAdImpression);
        this._bindEvent("adclickthru", this._adPlayer, this._onAdClickThru);
        this._bindEvent("aderror", this._adPlayer, this._onAdError);
        this._bindEvent("adlog", this._adPlayer, this._onAdLog);
        this._bindEvent("adskipped", this._adPlayer, this._onAdSkipped);
        this._bindEvent("adsizechanged", this._adPlayer, this._onAdSizeChanged);
        this._bindEvent("adskippablestatechange", this._adPlayer, this._onAdSkippableStateChange);
        this._bindEvent("addurationchange", this._adPlayer, this._onAdDurationChange);
        this._bindEvent("adinteraction", this._adPlayer, this._onAdInteraction);

        this._bindEvent("handshakeversionrequested", this._nativeInstance, this._onHandshakeVersionRequested);
        this._bindEvent("initadrequested", this._nativeInstance, this._onInitAdRequested);
        this._bindEvent("startadrequested", this._nativeInstance, this._onStartAdRequested);
        this._bindEvent("stopadrequested", this._nativeInstance, this._onStopAdRequested);
        this._bindEvent("pauseadrequested", this._nativeInstance, this._onPauseAdRequested);
        this._bindEvent("resumeadrequested", this._nativeInstance, this._onResumeAdRequested);
        this._bindEvent("resizeadrequested", this._nativeInstance, this._onResizeAdRequested);
        this._bindEvent("expandadrequested", this._nativeInstance, this._onExpandAdRequested);
        this._bindEvent("collapseadrequested", this._nativeInstance, this._onCollapseAdRequested);
        this._bindEvent("skipadrequested", this._nativeInstance, this._onSkipAdRequested);
        this._bindEvent("getadwidthrequested", this._nativeInstance, this._onGetAdWidthRequested);
        this._bindEvent("getadheightrequested", this._nativeInstance, this._onGetAdHeightRequested);
        this._bindEvent("getadskippablestaterequested", this._nativeInstance, this._onGetAdSkippablestateRequested);
        this._bindEvent("getadcompanionsrequested", this._nativeInstance, this._onGetAdCompanionsRequested);
        this._bindEvent("getadiconsrequested", this._nativeInstance, this._onGetAdIconsRequested);
        this._bindEvent("getaddurationrequested", this._nativeInstance, this._onGetAdDurationRequested);
        this._bindEvent("getadlinearrequested", this._nativeInstance, this._onGetAdLinearRequested);
        this._bindEvent("getadexpandedrequested", this._nativeInstance, this._onGetAdExpandedRequested);
        this._bindEvent("getadremainingtimerequested", this._nativeInstance, this._onGetAdRemainingTimeRequested);
        this._bindEvent("getadvolumerequested", this._nativeInstance, this._onGetAdVolumeRequested);
        this._bindEvent("setadvolumerequested", this._nativeInstance, this._onSetAdVolumeRequested);
    }, {
        // Public Properties
        nativeInstance: {
            get: function () {
                return this._nativeInstance;
            }
        },

        adPlayer: {
            get: function () {
                return this._adPlayer;
            }
        },

        // Public Methods
        dispose: function () {
            this._unbindEvents();

            this._adPlayer = null;
            this._nativeInstance = null;
        },

        // Private Methods
        _onAdLoaded: function (e) {
            this._nativeInstance.onAdLoaded();
        },

        _onAdStarted: function (e) {
            this._nativeInstance.onAdStarted();
        },

        _onAdStopped: function (e) {
            this._nativeInstance.onAdStopped();
        },

        _onAdPlaying: function (e) {
            this._nativeInstance.onAdPlaying();
        },

        _onAdPaused: function (e) {
            this._nativeInstance.onAdPaused();
        },

        _onAdExpandedChanged: function (e) {
            this._nativeInstance.onAdExpandedChanged();
        },

        _onAdLinearChanged: function (e) {
            this._nativeInstance.onAdLinearChanged();
        },

        _onAdVolumeChanged: function (e) {
            this._nativeInstance.onAdVolumeChanged();
        },

        _onAdVideoStart: function (e) {
            this._nativeInstance.onAdVideoStart();
        },

        _onAdVideoFirstQuartile: function (e) {
            this._nativeInstance.onAdVideoFirstQuartile();
        },

        _onAdVideoMidpoint: function (e) {
            this._nativeInstance.onAdVideoMidpoint();
        },

        _onAdVideoThirdQuartile: function (e) {
            this._nativeInstance.onAdVideoThirdQuartile();
        },

        _onAdVideoComplete: function (e) {
            this._nativeInstance.onAdVideoComplete();
        },

        _onAdUserAcceptInvitation: function (e) {
            this._nativeInstance.onAdUserAcceptInvitation();
        },

        _onAdUserClose: function (e) {
            this._nativeInstance.onAdUserClose();
        },

        _onAdUserMinimize: function (e) {
            this._nativeInstance.onAdUserMinimize();
        },

        _onAdRemainingTimeChange: function (e) {
            this._nativeInstance.onAdRemainingTimeChange();
        },

        _onAdImpression: function (e) {
            this._nativeInstance.onAdImpression();
        },

        _onAdClickThru: function (e) {
            this._nativeInstance.onAdClickThru(e.detail.url, e.detail.id, e.detail.playerHandles);
        },

        _onAdError: function (e) {
            this._nativeInstance.onAdError(e.detail.message);
        },

        _onAdLog: function (e) {
            this._nativeInstance.onAdLog(e.detail.message);
        },

        _onAdSkipped: function (e) {
            this._nativeInstance.onAdSkipped();
        },

        _onAdSizeChanged: function (e) {
            this._nativeInstance.onAdSizeChanged();
        },

        _onAdSkippableStateChange: function (e) {
            this._nativeInstance.onAdSkippableStateChange();
        },

        _onAdDurationChange: function (e) {
            this._nativeInstance.onAdDurationChange();
        },

        _onAdInteraction: function (e) {
            this._nativeInstance.onAdInteraction(e.detail.id);
        },

        _onHandshakeVersionRequested: function (e) {
            e.result = this._adPlayer.handshakeVersion(e.version);
        },

        _onInitAdRequested: function (e) {
            this._adPlayer.initAd(e.width, e.height, e.viewMode, e.desiredBitrate, e.creativeData, e.environmentVariables);
        },

        _onStartAdRequested: function (e) {
            this._adPlayer.startAd();
        },

        _onStopAdRequested: function (e) {
            this._adPlayer.stopAd();
        },

        _onPauseAdRequested: function (e) {
            this._adPlayer.pauseAd();
        },

        _onResumeAdRequested: function (e) {
            this._adPlayer.resumeAd();
        },

        _onResizeAdRequested: function (e) {
            this._adPlayer.resizeAd(e.width, e.height, e.viewMode);
        },

        _onExpandAdRequested: function (e) {
            this._adPlayer.expandAd();
        },

        _onCollapseAdRequested: function (e) {
            this._adPlayer.collapseAd();
        },

        _onSkipAdRequested: function (e) {
            this._adPlayer.skipAd();
        },

        _onGetAdWidthRequested: function (e) {
            e.result = this._adPlayer.adWidth;
        },

        _onGetAdHeightRequested: function (e) {
            e.result = this._adPlayer.adHeight;
        },

        _onGetAdSkippablestateRequested: function (e) {
            e.result = this._adPlayer.adSkippablestate;
        },

        _onGetAdCompanionsRequested: function (e) {
            e.result = this._adPlayer.adCompanions;
        },

        _onGetAdIconsRequested: function (e) {
            e.result = this._adPlayer.adIcons;
        },

        _onGetAdDurationRequested: function (e) {
            e.result = this._adPlayer.adDuration;
        },

        _onGetAdLinearRequested: function (e) {
            e.result = this._adPlayer.adLinear;
        },

        _onGetAdExpandedRequested: function (e) {
            e.result = this._adPlayer.adExpanded;
        },

        _onGetAdRemainingTimeRequested: function (e) {
            e.result = this._adPlayer.adRemainingTime;
        },

        _onGetAdVolumeRequested: function (e) {
            e.result = this._adPlayer.adVolume;
        },

        _onSetAdVolumeRequested: function (e) {
            this._adPlayer.adVolume = e.value;
        }
    });

    // VpaidAdapter Mixins
    WinJS.Class.mix(VpaidAdapter, PlayerFramework.Utilities.eventBindingMixin);

    // VpaidAdapter Exports
    WinJS.Namespace.define("PlayerFramework.Advertising", {
        VpaidAdapter: VpaidAdapter
    });

})(PlayerFramework);


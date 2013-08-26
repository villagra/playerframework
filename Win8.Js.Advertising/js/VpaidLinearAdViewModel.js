(function (PlayerFramework, undefined) {
    "use strict";

    // VpaidLinearAdViewModel Errors
    var invalidConstruction = "Invalid construction: VpaidLinearAdViewModel constructor must be called using the \"new\" operator.",
        invalidAdPlayer = "Invalid argument: VpaidLinearAdViewModel expects a VpaidAdPlayerBase as the first argument.";

    // VpaidLinearAdViewModel Class
    var VpaidLinearAdViewModel = WinJS.Class.derive(PlayerFramework.InteractiveViewModel, function (adPlayer, mediaPlayer) {
        if (!(this instanceof PlayerFramework.Advertising.VpaidLinearAdViewModel)) {
            throw invalidConstruction;
        }

        if (!(adPlayer instanceof PlayerFramework.Advertising.VpaidAdPlayerBase)) {
            throw invalidAdPlayer;
        }

        this._adPlayer = adPlayer;

        PlayerFramework.InteractiveViewModel.call(this, mediaPlayer);
    }, {
        // Public Properties
        startTime: {
            get: function () {
                return 0;
            }
        },

        endTime: {
            get: function () {
                return PlayerFramework.Utilities.convertMillisecondsToTicks(this._adPlayer.adDuration);
            }
        },

        currentTime: {
            get: function () {
                return PlayerFramework.Utilities.convertMillisecondsToTicks(this._adPlayer.adDuration - this._adPlayer.adRemainingTime);
            }
        },

        bufferedPercentage: {
            get: function () {
                return 1;
            }
        },

        playPauseIcon: {
            get: function () {
                return this._adPlayer.adState === PlayerFramework.Advertising.VpaidAdPlayerBase.AD_STATE_PAUSED ? PlayerFramework.Utilities.getResourceString("PlayIcon") : PlayerFramework.Utilities.getResourceString("PauseIcon");
            }
        },

        playPauseLabel: {
            get: function () {
                return this._adPlayer.adState === PlayerFramework.Advertising.VpaidAdPlayerBase.AD_STATE_PAUSED ? PlayerFramework.Utilities.getResourceString("PlayLabel") : PlayerFramework.Utilities.getResourceString("PauseLabel");
            }
        },

        playPauseTooltip: {
            get: function () {
                return this._adPlayer.adState === PlayerFramework.Advertising.VpaidAdPlayerBase.AD_STATE_PAUSED ? PlayerFramework.Utilities.getResourceString("PlayTooltip") : PlayerFramework.Utilities.getResourceString("PauseTooltip");
            }
        },

        isPlayPauseDisabled: {
            get: function () {
                return this._adPlayer.adState !== PlayerFramework.Advertising.VpaidAdPlayerBase.AD_STATE_PAUSED && this._adPlayer.adState !== PlayerFramework.Advertising.VpaidAdPlayerBase.AD_STATE_PLAYING;
            }
        },

        isPlayResumeDisabled: {
            get: function () {
                return this._adPlayer.adState !== PlayerFramework.Advertising.VpaidAdPlayerBase.AD_STATE_PAUSED;
            }
        },

        isPauseDisabled: {
            get: function () {
                return this._adPlayer.adState === PlayerFramework.Advertising.VpaidAdPlayerBase.AD_STATE_PAUSED;
            }
        },

        isSkipNextDisabled: {
            get: function () {
                return !this._adPlayer.adSkippableState;
            }
        },

        visualMarkers: {
            get: function () {
                return [];
            }
        },

        thumbnailImageSrc: {
            get: function () {
                return null;
            }
        },

        isThumbnailVisible: {
            get: function () {
                return false;
            }
        },

        // Public Methods
        initialize: function () {
            // ad player events
            this._bindEvent("adloaded", this._adPlayer, this._notifyProperties, ["playPauseIcon", "playPauseLabel", "playPauseTooltip", "isPlayPauseDisabled", "isPlayResumeDisabled", "isPauseDisabled"]);
            this._bindEvent("adstarted", this._adPlayer, this._notifyProperties, ["playPauseIcon", "playPauseLabel", "playPauseTooltip", "isPlayPauseDisabled", "isPlayResumeDisabled", "isPauseDisabled"]);
            this._bindEvent("adstopped", this._adPlayer, this._notifyProperties, ["playPauseIcon", "playPauseLabel", "playPauseTooltip", "isPlayPauseDisabled", "isPlayResumeDisabled", "isPauseDisabled"]);
            this._bindEvent("adplaying", this._adPlayer, this._notifyProperties, ["playPauseIcon", "playPauseLabel", "playPauseTooltip", "isPlayPauseDisabled", "isPlayResumeDisabled", "isPauseDisabled"]);
            this._bindEvent("adpaused", this._adPlayer, this._notifyProperties, ["playPauseIcon", "playPauseLabel", "playPauseTooltip", "isPlayPauseDisabled", "isPlayResumeDisabled", "isPauseDisabled"]);
            this._bindEvent("aderror", this._adPlayer, this._notifyProperties, ["playPauseIcon", "playPauseLabel", "playPauseTooltip", "isPlayPauseDisabled", "isPlayResumeDisabled", "isPauseDisabled"]);
            this._bindEvent("addurationchange", this._adPlayer, this._notifyProperties, ["endTime", "currentTime", "elapsedTime", "remainingTime"]);
            this._bindEvent("adremainingtimechange", this._adPlayer, this._notifyProperties, ["currentTime", "elapsedTime", "remainingTime"]);
            this._bindEvent("adskippablestatechange", this._adPlayer, this._notifyProperties, ["isSkipNextDisabled"]);

            // media player value properties
            this._bindProperty("volume", this._observableMediaPlayer, this._notifyProperties, ["volume"]);
            this._bindProperty("muted", this._observableMediaPlayer, this._notifyProperties, ["volumeMuteIcon", "volumeMuteLabel", "volumeMuteTooltip", "volumeIcon", "muteIcon", "muteLabel", "muteTooltip"]);
            this._bindProperty("isFullScreen", this._observableMediaPlayer, this._notifyProperties, ["fullScreenIcon", "fullScreenLabel", "fullScreenTooltip"]);

            // media player interaction properties
            this._bindProperty("isPlayPauseVisible", this._observableMediaPlayer, this._notifyProperties, ["isPlayPauseHidden"]);
            this._bindProperty("isPlayResumeVisible", this._observableMediaPlayer, this._notifyProperties, ["isPlayResumeHidden"]);
            this._bindProperty("isPauseVisible", this._observableMediaPlayer, this._notifyProperties, ["isPauseHidden"]);
            this._bindProperty("isReplayVisible", this._observableMediaPlayer, this._notifyProperties, ["isReplayHidden"]);
            this._bindProperty("isRewindVisible", this._observableMediaPlayer, this._notifyProperties, ["isRewindHidden"]);
            this._bindProperty("isFastForwardVisible", this._observableMediaPlayer, this._notifyProperties, ["isFastForwardHidden"]);
            this._bindProperty("isSlowMotionVisible", this._observableMediaPlayer, this._notifyProperties, ["isSlowMotionHidden"]);
            this._bindProperty("isSkipPreviousVisible", this._observableMediaPlayer, this._notifyProperties, ["isSkipPreviousHidden"]);
            this._bindProperty("isSkipNextVisible", this._observableMediaPlayer, this._notifyProperties, ["isSkipNextHidden"]);
            this._bindProperty("isSkipBackVisible", this._observableMediaPlayer, this._notifyProperties, ["isSkipBackHidden"]);
            this._bindProperty("isSkipAheadVisible", this._observableMediaPlayer, this._notifyProperties, ["isSkipAheadHidden"]);
            this._bindProperty("isElapsedTimeVisible", this._observableMediaPlayer, this._notifyProperties, ["isElapsedTimeHidden"]);
            this._bindProperty("isRemainingTimeVisible", this._observableMediaPlayer, this._notifyProperties, ["isRemainingTimeHidden"]);
            this._bindProperty("isTimelineVisible", this._observableMediaPlayer, this._notifyProperties, ["isTimelineHidden"]);
            this._bindProperty("isGoLiveVisible", this._observableMediaPlayer, this._notifyProperties, ["isGoLiveHidden"]);
            this._bindProperty("isCaptionsVisible", this._observableMediaPlayer, this._notifyProperties, ["isCaptionsHidden"]);
            this._bindProperty("isAudioVisible", this._observableMediaPlayer, this._notifyProperties, ["isAudioHidden"]);
            this._bindProperty("isVolumeMuteVisible", this._observableMediaPlayer, this._notifyProperties, ["isVolumeMuteHidden"]);
            this._bindProperty("isVolumeVisible", this._observableMediaPlayer, this._notifyProperties, ["isVolumeHidden"]);
            this._bindProperty("isMuteVisible", this._observableMediaPlayer, this._notifyProperties, ["isMuteHidden"]);
            this._bindProperty("isFullScreenVisible", this._observableMediaPlayer, this._notifyProperties, ["isFullScreenHidden"]);
            this._bindProperty("isSignalStrengthVisible", this._observableMediaPlayer, this._notifyProperties, ["isSignalStrengthHidden"]);
            this._bindProperty("isMediaQualityVisible", this._observableMediaPlayer, this._notifyProperties, ["isMediaQualityHidden"]);
        },

        playPause: function (e) {
            if (this._adPlayer.adState === PlayerFramework.Advertising.VpaidAdPlayerBase.AD_STATE_PAUSED) {
                this._adPlayer.resumeAd();
            } else {
                this._adPlayer.pauseAd();
            }
        },

        playResume: function () {
            this._adPlayer.resumeAd();
        },

        pause: function () {
            this._adPlayer.pauseAd();
        },

        skipNext: function () {
            if (!this.dispatchEvent("skipnext")) {
                this._adPlayer.skipAd();
            }
        },
    });

    // VpaidLinearAdViewModel Exports
    WinJS.Namespace.define("PlayerFramework.Advertising", {
        VpaidLinearAdViewModel: VpaidLinearAdViewModel
    });

})(PlayerFramework);


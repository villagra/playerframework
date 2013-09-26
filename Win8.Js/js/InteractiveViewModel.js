(function (PlayerFramework, undefined) {
    "use strict";

    // InteractiveViewModel Errors
    var invalidConstruction = "Invalid construction: InteractiveViewModel constructor must be called using the \"new\" operator.",
        invalidMediaPlayer = "Invalid argument: InteractiveViewModel expects a MediaPlayer as the first argument.";

    // InteractiveViewModel Events
    var events = [
        "skipprevious",
        "skipnext",
        "skipback",
        "skipahead"
    ];

    // InteractiveViewModel Class
    var InteractiveViewModel = WinJS.Class.define(function (mediaPlayer) {
        if (!(this instanceof PlayerFramework.InteractiveViewModel)) {
            throw invalidConstruction;
        }

        if (!(mediaPlayer instanceof PlayerFramework.MediaPlayer)) {
            throw invalidMediaPlayer;
        }

        this._mediaPlayer = mediaPlayer;
        this._observableMediaPlayer = WinJS.Binding.as(mediaPlayer);
        this._observableViewModel = WinJS.Binding.as(this);
    }, {
        // Public Properties
        startTime: {
            get: function () {
                return this._getViewModelTime(this._mediaPlayer.startTime);
            }
        },

        maxTime: {
            get: function () {
                return this._getViewModelTime(this._mediaPlayer.liveTime !== null ? this._mediaPlayer.liveTime : this._mediaPlayer.endTime);
            }
        },

        endTime: {
            get: function () {
                return this._getViewModelTime(this._mediaPlayer.endTime);
            }
        },

        currentTime: {
            get: function () {
                return this._getViewModelTime(this._mediaPlayer.virtualTime);
            }
        },

        bufferedPercentage: {
            get: function () {
                return PlayerFramework.Utilities.calculateBufferedPercentage(this._mediaPlayer.buffered, this._mediaPlayer.duration);
            }
        },

        playPauseIcon: {
            get: function () {
                return this._mediaPlayer.isPlayResumeAllowed ? PlayerFramework.Utilities.getResourceString("PlayIcon") : PlayerFramework.Utilities.getResourceString("PauseIcon");
            }
        },

        playPauseLabel: {
            get: function () {
                return this._mediaPlayer.isPlayResumeAllowed ? PlayerFramework.Utilities.getResourceString("PlayLabel") : PlayerFramework.Utilities.getResourceString("PauseLabel");
            }
        },

        playPauseTooltip: {
            get: function () {
                return this._mediaPlayer.isPlayResumeAllowed ? PlayerFramework.Utilities.getResourceString("PlayTooltip") : PlayerFramework.Utilities.getResourceString("PauseTooltip");
            }
        },

        isPlayPauseDisabled: {
            get: function () {
                return !this._mediaPlayer.isPlayPauseEnabled || !this._mediaPlayer.isPlayPauseAllowed;
            }
        },

        isPlayPauseHidden: {
            get: function () {
                return !this._mediaPlayer.isPlayPauseVisible;
            }
        },

        playResumeIcon: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("PlayIcon");
            }
        },

        playResumeLabel: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("PlayLabel");
            }
        },

        playResumeTooltip: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("PlayTooltip");
            }
        },

        isPlayResumeDisabled: {
            get: function () {
                return !this._mediaPlayer.isPlayResumeEnabled || !this._mediaPlayer.isPlayResumeAllowed;
            }
        },

        isPlayResumeHidden: {
            get: function () {
                return !this._mediaPlayer.isPlayResumeVisible;
            }
        },

        pauseIcon: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("PauseIcon");
            }
        },

        pauseLabel: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("PauseLabel");
            }
        },

        pauseTooltip: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("PauseTooltip");
            }
        },

        isPauseDisabled: {
            get: function () {
                return !this._mediaPlayer.isPauseEnabled || !this._mediaPlayer.isPauseAllowed;
            }
        },

        isPauseHidden: {
            get: function () {
                return !this._mediaPlayer.isPauseVisible;
            }
        },

        replayIcon: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("ReplayIcon");
            }
        },

        replayLabel: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("ReplayLabel");
            }
        },

        replayTooltip: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("ReplayTooltip");
            }
        },

        isReplayDisabled: {
            get: function () {
                return !this._mediaPlayer.isReplayEnabled || !this._mediaPlayer.isReplayAllowed;
            }
        },

        isReplayHidden: {
            get: function () {
                return !this._mediaPlayer.isReplayVisible;
            }
        },

        rewindIcon: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("RewindIcon");
            }
        },

        rewindLabel: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("RewindLabel");
            }
        },

        rewindTooltip: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("RewindTooltip");
            }
        },

        isRewindDisabled: {
            get: function () {
                return !this._mediaPlayer.isRewindEnabled || !this._mediaPlayer.isRewindAllowed;
            }
        },

        isRewindHidden: {
            get: function () {
                return !this._mediaPlayer.isRewindVisible;
            }
        },

        fastForwardIcon: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("FastForwardIcon");
            }
        },

        fastForwardLabel: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("FastForwardLabel");
            }
        },

        fastForwardTooltip: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("FastForwardTooltip");
            }
        },

        isFastForwardDisabled: {
            get: function () {
                return !this._mediaPlayer.isFastForwardEnabled || !this._mediaPlayer.isFastForwardAllowed;
            }
        },

        isFastForwardHidden: {
            get: function () {
                return !this._mediaPlayer.isFastForwardVisible;
            }
        },

        slowMotionIcon: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("SlowMotionIcon");
            }
        },

        slowMotionLabel: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("SlowMotionLabel");
            }
        },

        slowMotionTooltip: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("SlowMotionTooltip");
            }
        },

        isSlowMotionDisabled: {
            get: function () {
                return !this._mediaPlayer.isSlowMotionEnabled || !this._mediaPlayer.isSlowMotionAllowed;
            }
        },

        isSlowMotionHidden: {
            get: function () {
                return !this._mediaPlayer.isSlowMotionVisible;
            }
        },

        skipPreviousIcon: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("SkipPreviousIcon");
            }
        },

        skipPreviousLabel: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("SkipPreviousLabel");
            }
        },

        skipPreviousTooltip: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("SkipPreviousTooltip");
            }
        },

        isSkipPreviousDisabled: {
            get: function () {
                return !this._mediaPlayer.isSkipPreviousEnabled || !this._mediaPlayer.isSkipPreviousAllowed;
            }
        },

        isSkipPreviousHidden: {
            get: function () {
                return !this._mediaPlayer.isSkipPreviousVisible;
            }
        },

        skipNextIcon: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("SkipNextIcon");
            }
        },

        skipNextLabel: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("SkipNextLabel");
            }
        },

        skipNextTooltip: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("SkipNextTooltip");
            }
        },

        isSkipNextDisabled: {
            get: function () {
                return !this._mediaPlayer.isSkipNextEnabled || !this._mediaPlayer.isSkipNextAllowed;
            }
        },

        isSkipNextHidden: {
            get: function () {
                return !this._mediaPlayer.isSkipNextVisible;
            }
        },

        skipBackIcon: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("SkipBackIcon");
            }
        },

        skipBackLabel: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("SkipBackLabel");
            }
        },

        skipBackTooltip: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("SkipBackTooltip");
            }
        },

        isSkipBackDisabled: {
            get: function () {
                return !this._mediaPlayer.isSkipBackEnabled || !this._mediaPlayer.isSkipBackAllowed;
            }
        },

        isSkipBackHidden: {
            get: function () {
                return !this._mediaPlayer.isSkipBackVisible;
            }
        },

        skipAheadIcon: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("SkipAheadIcon");
            }
        },

        skipAheadLabel: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("SkipAheadLabel");
            }
        },

        skipAheadTooltip: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("SkipAheadTooltip");
            }
        },

        isSkipAheadDisabled: {
            get: function () {
                return !this._mediaPlayer.isSkipAheadEnabled || !this._mediaPlayer.isSkipAheadAllowed;
            }
        },

        isSkipAheadHidden: {
            get: function () {
                return !this._mediaPlayer.isSkipAheadVisible;
            }
        },

        elapsedTime: {
            get: function () {
                return PlayerFramework.Utilities.calculateElapsedTime(this.currentTime, this.startTime, this.endTime);
            }
        },

        elapsedTimeText: {
            get: function () {
                return PlayerFramework.Utilities.formatResourceString("ElapsedTimeText", this._mediaPlayer.skipBackInterval);
            }
        },

        elapsedTimeLabel: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("ElapsedTimeLabel");
            }
        },

        elapsedTimeTooltip: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("ElapsedTimeTooltip");
            }
        },

        isElapsedTimeDisabled: {
            get: function () {
                return !this._mediaPlayer.isElapsedTimeEnabled || !this._mediaPlayer.isElapsedTimeAllowed;
            }
        },

        isElapsedTimeHidden: {
            get: function () {
                return !this._mediaPlayer.isElapsedTimeVisible;
            }
        },

        remainingTime: {
            get: function () {
                return PlayerFramework.Utilities.calculateRemainingTime(this.currentTime, this.startTime, this.endTime);
            }
        },

        remainingTimeText: {
            get: function () {
                return PlayerFramework.Utilities.formatResourceString("RemainingTimeText", this._mediaPlayer.skipAheadInterval);
            }
        },

        remainingTimeLabel: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("RemainingTimeLabel");
            }
        },

        remainingTimeTooltip: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("RemainingTimeTooltip");
            }
        },

        isRemainingTimeDisabled: {
            get: function () {
                return !this._mediaPlayer.isRemainingTimeEnabled || !this._mediaPlayer.isRemainingTimeAllowed;
            }
        },

        isRemainingTimeHidden: {
            get: function () {
                return !this._mediaPlayer.isRemainingTimeVisible;
            }
        },

        totalTime: {
            get: function () {
                return PlayerFramework.Utilities.convertTicksToSeconds(this.endTime - this.startTime);
            }
        },

        totalTimeText: {
            get: function () {
                return PlayerFramework.Utilities.formatResourceString("TotalTimeText", this._mediaPlayer.skipAheadInterval);
            }
        },

        totalTimeLabel: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("TotalTimeLabel");
            }
        },

        totalTimeTooltip: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("TotalTimeTooltip");
            }
        },

        isTotalTimeDisabled: {
            get: function () {
                return !this._mediaPlayer.isTotalTimeEnabled || !this._mediaPlayer.isTotalTimeAllowed;
            }
        },

        isTotalTimeHidden: {
            get: function () {
                return !this._mediaPlayer.isTotalTimeVisible;
            }
        },

        timelineLabel: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("TimelineLabel");
            }
        },

        timelineTooltip: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("TimelineTooltip");
            }
        },

        isTimelineDisabled: {
            get: function () {
                return !this._mediaPlayer.isTimelineEnabled || !this._mediaPlayer.isTimelineAllowed;
            }
        },

        isTimelineHidden: {
            get: function () {
                return !this._mediaPlayer.isTimelineVisible;
            }
        },

        goLiveText: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("GoLiveText");
            }
        },

        goLiveLabel: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("GoLiveLabel");
            }
        },

        goLiveTooltip: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("GoLiveTooltip");
            }
        },

        isGoLiveDisabled: {
            get: function () {
                return !this._mediaPlayer.isGoLiveEnabled || !this._mediaPlayer.isGoLiveAllowed;
            }
        },

        isGoLiveHidden: {
            get: function () {
                return !this._mediaPlayer.isGoLiveVisible;
            }
        },

        captionsIcon: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("CaptionsIcon");
            }
        },

        captionsLabel: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("CaptionsLabel");
            }
        },

        captionsTooltip: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("CaptionsTooltip");
            }
        },

        isCaptionsDisabled: {
            get: function () {
                return !this._mediaPlayer.isCaptionsEnabled || !this._mediaPlayer.isCaptionsAllowed;
            }
        },

        isCaptionsHidden: {
            get: function () {
                return !this._mediaPlayer.isCaptionsVisible;
            }
        },

        audioIcon: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("AudioIcon");
            }
        },

        audioLabel: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("AudioLabel");
            }
        },

        audioTooltip: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("AudioTooltip");
            }
        },

        isAudioDisabled: {
            get: function () {
                return !this._mediaPlayer.isAudioEnabled || !this._mediaPlayer.isAudioAllowed;
            }
        },

        isAudioHidden: {
            get: function () {
                return !this._mediaPlayer.isAudioVisible;
            }
        },

        volume: {
            get: function () {
                return this._getViewModelVolume(this._mediaPlayer.volume);
            }
        },

        volumeMuteIcon: {
            get: function () {
                return this._mediaPlayer.muted ? PlayerFramework.Utilities.getResourceString("UnmuteIcon") : PlayerFramework.Utilities.getResourceString("VolumeMuteIcon");
            }
        },

        volumeMuteLabel: {
            get: function () {
                return this._mediaPlayer.muted ? PlayerFramework.Utilities.getResourceString("UnmuteLabel") : PlayerFramework.Utilities.getResourceString("VolumeMuteLabel");
            }
        },

        volumeMuteTooltip: {
            get: function () {
                return this._mediaPlayer.muted ? PlayerFramework.Utilities.getResourceString("UnmuteTooltip") : PlayerFramework.Utilities.getResourceString("VolumeMuteTooltip");
            }
        },

        isVolumeMuteDisabled: {
            get: function () {
                return !this._mediaPlayer.isVolumeMuteEnabled || !this._mediaPlayer.isVolumeMuteAllowed;
            }
        },

        isVolumeMuteHidden: {
            get: function () {
                return !this._mediaPlayer.isVolumeMuteVisible;
            }
        },

        volumeIcon: {
            get: function () {
                return this._mediaPlayer.muted ? PlayerFramework.Utilities.getResourceString("UnmuteIcon") : PlayerFramework.Utilities.getResourceString("VolumeIcon");
            }
        },

        volumeLabel: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("VolumeLabel");
            }
        },

        volumeTooltip: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("VolumeTooltip");
            }
        },

        isVolumeDisabled: {
            get: function () {
                return !this._mediaPlayer.isVolumeEnabled || !this._mediaPlayer.isVolumeAllowed;
            }
        },

        isVolumeHidden: {
            get: function () {
                return !this._mediaPlayer.isVolumeVisible;
            }
        },

        muteIcon: {
            get: function () {
                return this._mediaPlayer.muted ? PlayerFramework.Utilities.getResourceString("UnmuteIcon") : PlayerFramework.Utilities.getResourceString("MuteIcon");
            }
        },

        muteLabel: {
            get: function () {
                return this._mediaPlayer.muted ? PlayerFramework.Utilities.getResourceString("UnmuteLabel") : PlayerFramework.Utilities.getResourceString("MuteLabel");
            }
        },

        muteTooltip: {
            get: function () {
                return this._mediaPlayer.muted ? PlayerFramework.Utilities.getResourceString("UnmuteTooltip") : PlayerFramework.Utilities.getResourceString("MuteTooltip");
            }
        },

        isMuteDisabled: {
            get: function () {
                return !this._mediaPlayer.isMuteEnabled || !this._mediaPlayer.isMuteAllowed;
            }
        },

        isMuteHidden: {
            get: function () {
                return !this._mediaPlayer.isMuteVisible;
            }
        },

        fullScreenIcon: {
            get: function () {
                return this._mediaPlayer.isFullScreen ? PlayerFramework.Utilities.getResourceString("ExitFullScreenIcon") : PlayerFramework.Utilities.getResourceString("FullScreenIcon");
            }
        },

        fullScreenLabel: {
            get: function () {
                return this._mediaPlayer.isFullScreen ? PlayerFramework.Utilities.getResourceString("ExitFullScreenLabel") : PlayerFramework.Utilities.getResourceString("FullScreenLabel");
            }
        },

        fullScreenTooltip: {
            get: function () {
                return this._mediaPlayer.isFullScreen ? PlayerFramework.Utilities.getResourceString("ExitFullScreenTooltip") : PlayerFramework.Utilities.getResourceString("FullScreenTooltip");
            }
        },

        isFullScreenDisabled: {
            get: function () {
                return !this._mediaPlayer.isFullScreenEnabled || !this._mediaPlayer.isFullScreenAllowed;
            }
        },

        isFullScreenHidden: {
            get: function () {
                return !this._mediaPlayer.isFullScreenVisible;
            }
        },

        stopIcon: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("StopIcon");
            }
        },

        stopLabel: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("StopLabel");
            }
        },

        stopTooltip: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("StopTooltip");
            }
        },

        isStopDisabled: {
            get: function () {
                return !this._mediaPlayer.isStopEnabled || !this._mediaPlayer.isStopAllowed;
            }
        },

        isStopHidden: {
            get: function () {
                return !this._mediaPlayer.isStopVisible;
            }
        },

        infoIcon: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("InfoIcon");
            }
        },

        infoLabel: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("InfoLabel");
            }
        },

        infoTooltip: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("InfoTooltip");
            }
        },

        isInfoDisabled: {
            get: function () {
                return !this._mediaPlayer.isInfoEnabled || !this._mediaPlayer.isInfoAllowed;
            }
        },

        isInfoHidden: {
            get: function () {
                return !this._mediaPlayer.isInfoVisible;
            }
        },

        moreIcon: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("MoreIcon");
            }
        },

        moreLabel: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("MoreLabel");
            }
        },

        moreTooltip: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("MoreTooltip");
            }
        },

        isMoreDisabled: {
            get: function () {
                return !this._mediaPlayer.isMoreEnabled || !this._mediaPlayer.isMoreAllowed;
            }
        },

        isMoreHidden: {
            get: function () {
                return !this._mediaPlayer.isMoreVisible;
            }
        },

        displayModeIcon: {
            get: function () {
                return this._mediaPlayer.msZoom ? PlayerFramework.Utilities.getResourceString("DisplayModeLetterboxIcon") : PlayerFramework.Utilities.getResourceString("DisplayModeFillIcon");
            }
        },

        displayModeLabel: {
            get: function () {
                return this._mediaPlayer.msZoom ? PlayerFramework.Utilities.getResourceString("DisplayModeLetterboxLabel") : PlayerFramework.Utilities.getResourceString("DisplayModeFillLabel");
            }
        },

        displayModeTooltip: {
            get: function () {
                return this._mediaPlayer.msZoom ? PlayerFramework.Utilities.getResourceString("DisplayModeLetterboxTooltip") : PlayerFramework.Utilities.getResourceString("DisplayModeFillTooltip");
            }
        },

        isDisplayModeDisabled: {
            get: function () {
                return !this._mediaPlayer.isDisplayModeEnabled || !this._mediaPlayer.isDisplayModeAllowed;
            }
        },

        isDisplayModeHidden: {
            get: function () {
                return !this._mediaPlayer.isDisplayModeVisible;
            }
        },

        signalStrength: {
            get: function () {
                return this._mediaPlayer.signalStrength;
            }
        },

        signalStrengthLabel: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("SignalStrengthLabel");
            }
        },

        signalStrengthTooltip: {
            get: function () {
                return this._mediaPlayer.signalStrength < 0.25 ? PlayerFramework.Utilities.getResourceString("SignalStrengthTooltip1") : this._mediaPlayer.signalStrength < 0.5 ? PlayerFramework.Utilities.getResourceString("SignalStrengthTooltip2") : this._mediaPlayer.signalStrength < 0.75 ? PlayerFramework.Utilities.getResourceString("SignalStrengthTooltip3") : PlayerFramework.Utilities.getResourceString("SignalStrengthTooltip4");
            }
        },

        isSignalStrengthDisabled: {
            get: function () {
                return !this._mediaPlayer.isSignalStrengthEnabled || !this._mediaPlayer.isSignalStrengthAllowed;
            }
        },

        isSignalStrengthHidden: {
            get: function () {
                return !this._mediaPlayer.isSignalStrengthVisible;
            }
        },

        mediaQuality: {
            get: function () {
                return this._mediaPlayer.mediaQuality === PlayerFramework.MediaQuality.highDefinition ? PlayerFramework.Utilities.getResourceString("MediaQuality_HD") : PlayerFramework.Utilities.getResourceString("MediaQuality_SD");
            }
        },

        mediaQualityLabel: {
            get: function () {
                return PlayerFramework.Utilities.getResourceString("MediaQualityLabel");
            }
        },

        mediaQualityTooltip: {
            get: function () {
                return this._mediaPlayer.mediaQuality === PlayerFramework.MediaQuality.highDefinition ? PlayerFramework.Utilities.getResourceString("MediaQualityTooltip_HD") : PlayerFramework.Utilities.getResourceString("MediaQualityTooltip_SD");
            }
        },

        isMediaQualityDisabled: {
            get: function () {
                return !this._mediaPlayer.isMediaQualityEnabled || !this._mediaPlayer.isMediaQualityAllowed;
            }
        },

        isMediaQualityHidden: {
            get: function () {
                return !this._mediaPlayer.isMediaQualityVisible;
            }
        },

        visualMarkers: {
            get: function () {
                return this._mediaPlayer.visualMarkers;
            }
        },

        thumbnailImageSrc: {
            get: function () {
                return this._mediaPlayer.thumbnailImageSrc;
            }
        },

        isThumbnailVisible: {
            get: function () {
                return this._mediaPlayer.isThumbnailVisible;
            }
        },

        mediaMetadata: {
            get: function () {
                return this._mediaPlayer.mediaMetadata;
            }
        },
        
        // Public Methods
        initialize: function () {
            // media player value properties
            this._bindProperty("startTime", this._observableMediaPlayer, this._notifyProperties, ["startTime", "endTime", "currentTime", "elapsedTime", "remainingTime", "totalTime"]);
            this._bindProperty("isStartTimeOffset", this._observableMediaPlayer, this._notifyProperties, ["startTime", "endTime", "currentTime", "elapsedTime", "remainingTime", "maxTime"]);
            this._bindProperty("endTime", this._observableMediaPlayer, this._notifyProperties, ["endTime", "elapsedTime", "remainingTime", "totalTime", "maxTime"]);
            this._bindProperty("liveTime", this._observableMediaPlayer, this._notifyProperties, ["maxTime"]);
            this._bindProperty("virtualTime", this._observableMediaPlayer, this._notifyProperties, ["currentTime", "elapsedTime", "remainingTime"]);
            this._bindProperty("buffered", this._observableMediaPlayer, this._notifyProperties, ["bufferedPercentage"]);
            this._bindProperty("duration", this._observableMediaPlayer, this._notifyProperties, ["bufferedPercentage"]);
            this._bindProperty("skipBackInterval", this._observableMediaPlayer, this._notifyProperties, ["elapsedTimeText"]);
            this._bindProperty("skipAheadInterval", this._observableMediaPlayer, this._notifyProperties, ["remainingTimeText", "totalTimeText"]);
            this._bindProperty("volume", this._observableMediaPlayer, this._notifyProperties, ["volume"]);
            this._bindProperty("muted", this._observableMediaPlayer, this._notifyProperties, ["volumeMuteIcon", "volumeMuteLabel", "volumeMuteTooltip", "volumeIcon", "muteIcon", "muteLabel", "muteTooltip"]);
            this._bindProperty("isFullScreen", this._observableMediaPlayer, this._notifyProperties, ["fullScreenIcon", "fullScreenLabel", "fullScreenTooltip"]);
            this._bindProperty("signalStrength", this._observableMediaPlayer, this._notifyProperties, ["signalStrength", "signalStrengthTooltip"]);
            this._bindProperty("mediaQuality", this._observableMediaPlayer, this._notifyProperties, ["mediaQuality", "mediaQualityTooltip"]);
            this._bindProperty("visualMarkers", this._observableMediaPlayer, this._notifyProperties, ["visualMarkers"]);
            this._bindProperty("thumbnailImageSrc", this._observableMediaPlayer, this._notifyProperties, ["thumbnailImageSrc"]);
            this._bindProperty("isThumbnailVisible", this._observableMediaPlayer, this._notifyProperties, ["isThumbnailVisible"]);
            this._bindProperty("mediaMetadata", this._observableMediaPlayer, this._notifyProperties, ["mediaMetadata"]);

            // media player interaction properties
            this._bindProperty("isPlayPauseAllowed", this._observableMediaPlayer, this._notifyProperties, ["isPlayPauseDisabled"]);
            this._bindProperty("isPlayPauseEnabled", this._observableMediaPlayer, this._notifyProperties, ["isPlayPauseDisabled"]);
            this._bindProperty("isPlayPauseVisible", this._observableMediaPlayer, this._notifyProperties, ["isPlayPauseHidden"]);
            this._bindProperty("isPlayResumeAllowed", this._observableMediaPlayer, this._notifyProperties, ["isPlayResumeDisabled", "playPauseIcon", "playPauseLabel", "playPauseTooltip"]);
            this._bindProperty("isPlayResumeEnabled", this._observableMediaPlayer, this._notifyProperties, ["isPlayResumeDisabled"]);
            this._bindProperty("isPlayResumeVisible", this._observableMediaPlayer, this._notifyProperties, ["isPlayResumeHidden"]);
            this._bindProperty("isPauseAllowed", this._observableMediaPlayer, this._notifyProperties, ["isPauseDisabled"]);
            this._bindProperty("isPauseEnabled", this._observableMediaPlayer, this._notifyProperties, ["isPauseDisabled"]);
            this._bindProperty("isPauseVisible", this._observableMediaPlayer, this._notifyProperties, ["isPauseHidden"]);
            this._bindProperty("isReplayAllowed", this._observableMediaPlayer, this._notifyProperties, ["isReplayDisabled"]);
            this._bindProperty("isReplayEnabled", this._observableMediaPlayer, this._notifyProperties, ["isReplayDisabled"]);
            this._bindProperty("isReplayVisible", this._observableMediaPlayer, this._notifyProperties, ["isReplayHidden"]);
            this._bindProperty("isRewindAllowed", this._observableMediaPlayer, this._notifyProperties, ["isRewindDisabled"]);
            this._bindProperty("isRewindEnabled", this._observableMediaPlayer, this._notifyProperties, ["isRewindDisabled"]);
            this._bindProperty("isRewindVisible", this._observableMediaPlayer, this._notifyProperties, ["isRewindHidden"]);
            this._bindProperty("isFastForwardAllowed", this._observableMediaPlayer, this._notifyProperties, ["isFastForwardDisabled"]);
            this._bindProperty("isFastForwardEnabled", this._observableMediaPlayer, this._notifyProperties, ["isFastForwardDisabled"]);
            this._bindProperty("isFastForwardVisible", this._observableMediaPlayer, this._notifyProperties, ["isFastForwardHidden"]);
            this._bindProperty("isSlowMotionAllowed", this._observableMediaPlayer, this._notifyProperties, ["isSlowMotionDisabled"]);
            this._bindProperty("isSlowMotionEnabled", this._observableMediaPlayer, this._notifyProperties, ["isSlowMotionDisabled"]);
            this._bindProperty("isSlowMotionVisible", this._observableMediaPlayer, this._notifyProperties, ["isSlowMotionHidden"]);
            this._bindProperty("isSkipPreviousAllowed", this._observableMediaPlayer, this._notifyProperties, ["isSkipPreviousDisabled"]);
            this._bindProperty("isSkipPreviousEnabled", this._observableMediaPlayer, this._notifyProperties, ["isSkipPreviousDisabled"]);
            this._bindProperty("isSkipPreviousVisible", this._observableMediaPlayer, this._notifyProperties, ["isSkipPreviousHidden"]);
            this._bindProperty("isSkipNextAllowed", this._observableMediaPlayer, this._notifyProperties, ["isSkipNextDisabled"]);
            this._bindProperty("isSkipNextEnabled", this._observableMediaPlayer, this._notifyProperties, ["isSkipNextDisabled"]);
            this._bindProperty("isSkipNextVisible", this._observableMediaPlayer, this._notifyProperties, ["isSkipNextHidden"]);
            this._bindProperty("isSkipBackAllowed", this._observableMediaPlayer, this._notifyProperties, ["isSkipBackDisabled"]);
            this._bindProperty("isSkipBackEnabled", this._observableMediaPlayer, this._notifyProperties, ["isSkipBackDisabled"]);
            this._bindProperty("isSkipBackVisible", this._observableMediaPlayer, this._notifyProperties, ["isSkipBackHidden"]);
            this._bindProperty("isSkipAheadAllowed", this._observableMediaPlayer, this._notifyProperties, ["isSkipAheadDisabled"]);
            this._bindProperty("isSkipAheadEnabled", this._observableMediaPlayer, this._notifyProperties, ["isSkipAheadDisabled"]);
            this._bindProperty("isSkipAheadVisible", this._observableMediaPlayer, this._notifyProperties, ["isSkipAheadHidden"]);
            this._bindProperty("isElapsedTimeAllowed", this._observableMediaPlayer, this._notifyProperties, ["isElapsedTimeDisabled"]);
            this._bindProperty("isElapsedTimeEnabled", this._observableMediaPlayer, this._notifyProperties, ["isElapsedTimeDisabled"]);
            this._bindProperty("isElapsedTimeVisible", this._observableMediaPlayer, this._notifyProperties, ["isElapsedTimeHidden"]);
            this._bindProperty("isRemainingTimeAllowed", this._observableMediaPlayer, this._notifyProperties, ["isRemainingTimeDisabled"]);
            this._bindProperty("isRemainingTimeEnabled", this._observableMediaPlayer, this._notifyProperties, ["isRemainingTimeDisabled"]);
            this._bindProperty("isRemainingTimeVisible", this._observableMediaPlayer, this._notifyProperties, ["isRemainingTimeHidden"]);
            this._bindProperty("isTotalTimeAllowed", this._observableMediaPlayer, this._notifyProperties, ["isTotalTimeDisabled"]);
            this._bindProperty("isTotalTimeEnabled", this._observableMediaPlayer, this._notifyProperties, ["isTotalTimeDisabled"]);
            this._bindProperty("isTotalTimeVisible", this._observableMediaPlayer, this._notifyProperties, ["isTotalTimeHidden"]);
            this._bindProperty("isTimelineAllowed", this._observableMediaPlayer, this._notifyProperties, ["isTimelineDisabled"]);
            this._bindProperty("isTimelineEnabled", this._observableMediaPlayer, this._notifyProperties, ["isTimelineDisabled"]);
            this._bindProperty("isTimelineVisible", this._observableMediaPlayer, this._notifyProperties, ["isTimelineHidden"]);
            this._bindProperty("isGoLiveAllowed", this._observableMediaPlayer, this._notifyProperties, ["isGoLiveDisabled"]);
            this._bindProperty("isGoLiveEnabled", this._observableMediaPlayer, this._notifyProperties, ["isGoLiveDisabled"]);
            this._bindProperty("isGoLiveVisible", this._observableMediaPlayer, this._notifyProperties, ["isGoLiveHidden"]);
            this._bindProperty("isCaptionsAllowed", this._observableMediaPlayer, this._notifyProperties, ["isCaptionsDisabled"]);
            this._bindProperty("isCaptionsEnabled", this._observableMediaPlayer, this._notifyProperties, ["isCaptionsDisabled"]);
            this._bindProperty("isCaptionsVisible", this._observableMediaPlayer, this._notifyProperties, ["isCaptionsHidden"]);
            this._bindProperty("isAudioAllowed", this._observableMediaPlayer, this._notifyProperties, ["isAudioDisabled"]);
            this._bindProperty("isAudioEnabled", this._observableMediaPlayer, this._notifyProperties, ["isAudioDisabled"]);
            this._bindProperty("isAudioVisible", this._observableMediaPlayer, this._notifyProperties, ["isAudioHidden"]);
            this._bindProperty("isVolumeMuteAllowed", this._observableMediaPlayer, this._notifyProperties, ["isVolumeMuteDisabled"]);
            this._bindProperty("isVolumeMuteEnabled", this._observableMediaPlayer, this._notifyProperties, ["isVolumeMuteDisabled"]);
            this._bindProperty("isVolumeMuteVisible", this._observableMediaPlayer, this._notifyProperties, ["isVolumeMuteHidden"]);
            this._bindProperty("isVolumeAllowed", this._observableMediaPlayer, this._notifyProperties, ["isVolumeDisabled"]);
            this._bindProperty("isVolumeEnabled", this._observableMediaPlayer, this._notifyProperties, ["isVolumeDisabled"]);
            this._bindProperty("isVolumeVisible", this._observableMediaPlayer, this._notifyProperties, ["isVolumeHidden"]);
            this._bindProperty("isMuteAllowed", this._observableMediaPlayer, this._notifyProperties, ["isMuteDisabled"]);
            this._bindProperty("isMuteEnabled", this._observableMediaPlayer, this._notifyProperties, ["isMuteDisabled"]);
            this._bindProperty("isMuteVisible", this._observableMediaPlayer, this._notifyProperties, ["isMuteHidden"]);
            this._bindProperty("isFullScreenAllowed", this._observableMediaPlayer, this._notifyProperties, ["isFullScreenDisabled"]);
            this._bindProperty("isFullScreenEnabled", this._observableMediaPlayer, this._notifyProperties, ["isFullScreenDisabled"]);
            this._bindProperty("isFullScreenVisible", this._observableMediaPlayer, this._notifyProperties, ["isFullScreenHidden"]);
            this._bindProperty("isStopAllowed", this._observableMediaPlayer, this._notifyProperties, ["isStopDisabled"]);
            this._bindProperty("isStopEnabled", this._observableMediaPlayer, this._notifyProperties, ["isStopDisabled"]);
            this._bindProperty("isStopVisible", this._observableMediaPlayer, this._notifyProperties, ["isStopHidden"]);
            this._bindProperty("isInfoAllowed", this._observableMediaPlayer, this._notifyProperties, ["isInfoDisabled"]);
            this._bindProperty("isInfoEnabled", this._observableMediaPlayer, this._notifyProperties, ["isInfoDisabled"]);
            this._bindProperty("isInfoVisible", this._observableMediaPlayer, this._notifyProperties, ["isInfoHidden"]);
            this._bindProperty("isMoreAllowed", this._observableMediaPlayer, this._notifyProperties, ["isMoreDisabled"]);
            this._bindProperty("isMoreEnabled", this._observableMediaPlayer, this._notifyProperties, ["isMoreDisabled"]);
            this._bindProperty("isMoreVisible", this._observableMediaPlayer, this._notifyProperties, ["isMoreHidden"]);
            this._bindProperty("isDisplayModeAllowed", this._observableMediaPlayer, this._notifyProperties, ["isDisplayModeDisabled"]);
            this._bindProperty("isDisplayModeEnabled", this._observableMediaPlayer, this._notifyProperties, ["isDisplayModeDisabled"]);
            this._bindProperty("isDisplayModeVisible", this._observableMediaPlayer, this._notifyProperties, ["isDisplayModeHidden"]);
            this._bindProperty("msZoom", this._observableMediaPlayer, this._notifyProperties, ["displayModeLabel", "displayModeTooltip", "displayModeIcon"]);
            this._bindProperty("isSignalStrengthAllowed", this._observableMediaPlayer, this._notifyProperties, ["isSignalStrengthDisabled"]);
            this._bindProperty("isSignalStrengthEnabled", this._observableMediaPlayer, this._notifyProperties, ["isSignalStrengthDisabled"]);
            this._bindProperty("isSignalStrengthVisible", this._observableMediaPlayer, this._notifyProperties, ["isSignalStrengthHidden"]);
            this._bindProperty("isMediaQualityAllowed", this._observableMediaPlayer, this._notifyProperties, ["isMediaQualityDisabled"]);
            this._bindProperty("isMediaQualityEnabled", this._observableMediaPlayer, this._notifyProperties, ["isMediaQualityDisabled"]);
            this._bindProperty("isMediaQualityVisible", this._observableMediaPlayer, this._notifyProperties, ["isMediaQualityHidden"]);
        },

        uninitialize: function () {
            this._observableViewModel.unbind();
            this._unbindProperties();
            this._unbindEvents();
        },

        playPause: function (e) {
            if (this._mediaPlayer.isPlayResumeAllowed) {
                this._mediaPlayer.playResume();
            } else {
                this._mediaPlayer.pause();
            }
        },

        playResume: function () {
            this._mediaPlayer.playResume();
        },

        pause: function () {
            this._mediaPlayer.pause();
        },

        replay: function () {
            this._mediaPlayer.replay();
        },

        rewind: function () {
            this._mediaPlayer.decreasePlaybackRate();
        },

        fastForward: function () {
            this._mediaPlayer.increasePlaybackRate();
        },

        slowMotion: function () {
            this._mediaPlayer.isSlowMotion = !this._mediaPlayer.isSlowMotion;
        },

        skipPrevious: function () {
            var minTime = this.startTime;
            var previousMarker = null;
            var previousMarkerTime = null;
            for (var i = 0; i < this.visualMarkers.length; i++) {
                var marker = this.visualMarkers[i];
                var markerTime = PlayerFramework.Utilities.convertSecondsToTicks(marker.time);
                if (marker.isSeekable && markerTime < this.currentTime && markerTime > minTime) {
                    if (!previousMarker || previousMarkerTime < markerTime) {
                        previousMarker = marker;
                        previousMarkerTime = markerTime;
                    }
                }
            }
            this._onSkipPrevious(previousMarker);
        },

        skipNext: function () {
            var maxTime = this.maxTime;
            var nextMarker = null;
            var nextMarkerTime = null;
            for (var i = 0; i < this.visualMarkers.length; i++) {
                var marker = this.visualMarkers[i];
                var markerTime = PlayerFramework.Utilities.convertSecondsToTicks(marker.time);
                if (marker.isSeekable && markerTime > this.currentTime && markerTime < maxTime) {
                    if (!nextMarker || nextMarkerTime > markerTime) {
                        nextMarker = marker;
                        nextMarkerTime = markerTime;
                    }
                }
            }
            this._onSkipNext(nextMarker);
        },

        skipBack: function () {
            var minTime = this._mediaPlayer.startTime;
            var time = this._mediaPlayer.skipBackInterval !== null ? Math.max(this._mediaPlayer.virtualTime - this._mediaPlayer.skipBackInterval, minTime) : minTime;

            if (!this.dispatchEvent("skipback", { time: time })) {
                this._mediaPlayer._seek(time);
            }
        },

        skipAhead: function () {
            var maxTime = this._mediaPlayer.liveTime !== null ? this._mediaPlayer.liveTime : this._mediaPlayer.endTime;
            var time = this._mediaPlayer.skipAheadInterval !== null ? Math.min(this._mediaPlayer.virtualTime + this._mediaPlayer.skipAheadInterval, maxTime) : maxTime;

            if (!this.dispatchEvent("skipahead", { time: time })) {
                this._mediaPlayer._seek(time);
            }
        },

        startScrub: function (time) {
            this._mediaPlayer._startScrub(time);
        },

        updateScrub: function (time) {
            this._mediaPlayer._updateScrub(time);
        },

        completeScrub: function (time) {
            this._mediaPlayer._completeScrub(time);
        },

        goLive: function () {
            this._mediaPlayer._seekToLive();
        },

        setVolume: function (volume) {
            this._mediaPlayer.muted = false;
            this._mediaPlayer.volume = volume;
        },

        toggleMuted: function() {
            this._mediaPlayer.muted = !this._mediaPlayer.muted;
        },

        toggleFullScreen: function () {
            this._mediaPlayer.isFullScreen = !this._mediaPlayer.isFullScreen;
        },

        stop: function () {
            this._mediaPlayer.stop();
        },

        info: function () {
            this._mediaPlayer.info();
        },

        more: function () {
            this._mediaPlayer.more();
        },

        toggleDisplayMode: function () {
            this._mediaPlayer.msZoom = !this._mediaPlayer.msZoom;
        },

        captions: function () {
            this._mediaPlayer.captions();
        },

        audio: function () {
            this._mediaPlayer.audio();
        },

        onTimelineSliderStart: function (e) {
            var time = this._getMediaPlayerTime(e.target.winControl.value);
            this.startScrub(time);
        },

        onTimelineSliderUpdate: function (e) {
            var time = this._getMediaPlayerTime(e.target.winControl.value);
            this.updateScrub(time);
        },

        onTimelineSliderComplete: function (e) {
            var time = this._getMediaPlayerTime(e.target.winControl.value);
            this.completeScrub(time);
        },

        onTimelineSliderSkipToMarker: function (e) {
            var marker = e.detail;
            var markerTime = PlayerFramework.Utilities.convertSecondsToTicks(marker.time);
            var time = this._getMediaPlayerTime(markerTime);
            this._mediaPlayer._seek(time);
        },
        
        onVolumeSliderUpdate: function (e) {
            var volume = this._getMediaPlayerVolume(e.target.winControl.value);
            this.setVolume(volume);
        },

        onVolumeMuteClick: function (e) {
            var slider = e.target.nextSibling;

            if (slider.winControl.hidden) {
                this._mediaPlayer.muted = false;
                this._showVolumeMuteSlider(slider);
                this._resetVolumeMuteSliderAutohideTimeout(slider);
            } else if (this._mediaPlayer.muted) {
                this._clearVolumeMuteSliderAutohideTimeout(slider);
                this._hideVolumeMuteSlider(slider);
                this._mediaPlayer.muted = false;
            } else {
                this._clearVolumeMuteSliderAutohideTimeout(slider);
                this._hideVolumeMuteSlider(slider);
                this._mediaPlayer.muted = true;
            }
        },

        onVolumeMuteFocus: function (e) {
            if (!this._mediaPlayer.muted && !WinJS.Utilities.hasClass(e.target, "pf-hide-focus")) {
                var slider = e.target.nextSibling;
                this._showVolumeMuteSlider(slider);
                this._resetVolumeMuteSliderAutohideTimeout(slider);
            }
        },

        onVolumeMuteSliderUpdate: function (e) {
            var slider = e.target;
            var volume = this._getMediaPlayerVolume(slider.winControl.value);
            this.setVolume(volume);
        },

        onVolumeMuteSliderFocusIn: function (e) {
            var slider = e.currentTarget;
            this._clearVolumeMuteSliderAutohideTimeout(slider);
            this._showVolumeMuteSlider(slider);
        },

        onVolumeMuteSliderFocusOut: function (e) {
            var slider = e.currentTarget;
            this._resetVolumeMuteSliderAutohideTimeout(slider);
        },

        onVolumeMuteSliderMSPointerOver: function (e) {
            var slider = e.currentTarget;
            this._clearVolumeMuteSliderAutohideTimeout(slider);
            this._showVolumeMuteSlider(slider);
        },

        onVolumeMuteSliderMSPointerOut: function (e) {
            var slider = e.currentTarget;
            this._resetVolumeMuteSliderAutohideTimeout(slider);
        },

        onVolumeMuteSliderTransitionEnd: function (e) {
            var slider = e.target;
            if (slider.winControl.hidden) {
                slider.style.display = "none";
            }
        },

        // Private Methods
        _showVolumeMuteSlider: function (slider) {
            slider.style.display = "";
            slider.winControl.hidden = false;
        },

        _hideVolumeMuteSlider: function (slider) {
            slider.winControl.hidden = true;
        },

        _clearVolumeMuteSliderAutohideTimeout: function (slider) {
            var data = WinJS.Utilities.data(slider);
            window.clearTimeout(data.autohideTimeoutId);
            delete data.autohideTimeoutId;
        },

        _resetVolumeMuteSliderAutohideTimeout: function (slider) {
            var data = WinJS.Utilities.data(slider);
            window.clearTimeout(data.autohideTimeoutId);
            data.autohideTimeoutId = window.setTimeout(this._onVolumeMuteSliderAutohideTimeout.bind(this, slider), 3000);
        },

        _onVolumeMuteSliderAutohideTimeout: function (slider) {
            var preventAutohide = false;
            var activeElement = document.activeElement;

            if (activeElement && (slider === activeElement || slider.contains(activeElement)) && !WinJS.Utilities.hasClass(activeElement, "pf-hide-focus")) {
                preventAutohide = true;
            }

            if (!preventAutohide) {
                this._clearVolumeMuteSliderAutohideTimeout(slider);
                this._hideVolumeMuteSlider(slider);
            } else {
                this._resetVolumeMuteSliderAutohideTimeout(slider);
            }
        },

        _getMediaPlayerVolume: function (value) {
            return value / 100;
        },

        _getViewModelVolume: function (value) {
            return value * 100;
        },

        _getMediaPlayerTime: function (value) {
            var time = PlayerFramework.Utilities.convertTicksToSeconds(value);
            return this._mediaPlayer.isStartTimeOffset ? time : this._mediaPlayer.startTime + time;
        },

        _getViewModelTime: function (value) {
            var time = this._mediaPlayer.isStartTimeOffset ? value : value - this._mediaPlayer.startTime;
            return PlayerFramework.Utilities.convertSecondsToTicks(time);
        },

        _notifyProperties: function (propertyNames) {
            for (var i = 0; i < propertyNames.length; i++) {
                var propertyName = propertyNames[i];

                // prevents timeline interaction weirdness
                if (propertyName === "currentTime" && (this._mediaPlayer.seeking || this._mediaPlayer.scrubbing)) {
                    continue;
                }

                this._observableViewModel.notify(propertyName, this[propertyName]);
            }
        },
        
        _onSkipPrevious: function (marker) {
            if (marker) {
                var markerTime = PlayerFramework.Utilities.convertSecondsToTicks(marker.time);
                this._mediaPlayer._seek(this._getMediaPlayerTime(markerTime));
            }
            else {
                if (!this.dispatchEvent("skipprevious")) {
                    this._mediaPlayer._seek(this._mediaPlayer.startTime);
                }
            }
        },

        _onSkipNext: function (marker) {
            if (marker) {
                var markerTime = PlayerFramework.Utilities.convertSecondsToTicks(marker.time);
                this._mediaPlayer._seek(this._getMediaPlayerTime(markerTime));
            }
            else {
                if (!this.dispatchEvent("skipnext")) {
                    if (this._mediaPlayer.liveTime !== null) {
                        this._mediaPlayer._seek(this._mediaPlayer.liveTime);
                    } else {
                        this._mediaPlayer._seek(this._mediaPlayer.endTime);
                    }
                }
            }
        }
    });

    // InteractiveViewModel Mixins
    WinJS.Class.mix(InteractiveViewModel, WinJS.Utilities.eventMixin);
    WinJS.Class.mix(InteractiveViewModel, PlayerFramework.Utilities.createEventProperties(events));
    WinJS.Class.mix(InteractiveViewModel, PlayerFramework.Utilities.eventBindingMixin);
    WinJS.Class.mix(InteractiveViewModel, PlayerFramework.Utilities.propertyBindingMixin);

    // InteractiveViewModel Exports
    WinJS.Namespace.define("PlayerFramework", {
        InteractiveViewModel: InteractiveViewModel
    });

})(PlayerFramework);


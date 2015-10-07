(function (PlayerFramework, undefined) {
    "use strict";

    // ControlPlugin Errors
    var invalidConstruction = "Invalid construction: ControlPlugin constructor must be called using the \"new\" operator.";

    // ControlPlugin Class
    var ControlPlugin = WinJS.Class.derive(PlayerFramework.PluginBase, function (options) {
        if (!(this instanceof PlayerFramework.Plugins.ControlPlugin)) {
            throw invalidConstruction;
        }

        this._controlContainerElement = null;
        this._controlPanelElement = null;
        this._flyoutContainerElement = null;
        this._compactThresholdInInches = 5.0;
        this._orientation = "landscape";
        this._isCompact = false;

        PlayerFramework.PluginBase.call(this, options);
    }, {
        // Public Methods
        show: function () {
            this.mediaPlayer.addClass("pf-show-control-container");
        },

        hide: function () {
            this.mediaPlayer.removeClass("pf-show-control-container");
        },

        compactThresholdInInches: {
            get: function () {
                return this._compactThresholdInInches;
            },
            set: function (value) {
                this._compactThresholdInInches = value;
            }
        },

        isCompact: {
            get: function () {
                return this._isCompact;
            },
            set: function (value) {
                if (value !== this._isCompact) {
                    if (this._isCompact) {
                        WinJS.Utilities.removeClass(this._controlPanelElement, "pf-compact");
                    }
                    this._isCompact = value;
                    if (this._isCompact) {
                        WinJS.Utilities.addClass(this._controlPanelElement, "pf-compact");
                    }
                }
            }
        },

        orientation: {
            get: function () {
                return this._orientation;
            },
            set: function (value) {
                if (value !== this._orientation) {
                    if (this._orientation === "portrait") {
                        WinJS.Utilities.removeClass(this._controlPanelElement, "pf-portrait");
                    }
                    this._orientation = value;
                    if (this._orientation === "portrait") {
                        WinJS.Utilities.addClass(this._controlPanelElement, "pf-portrait");
                    }
                }
            }
        },

        // Private Methods
        _setElement: function () {
            this._controlContainerElement = PlayerFramework.Utilities.createElement(this.mediaPlayer.element, ["div", { "class": "pf-control-container" }]);
            this._controlPanelElement = PlayerFramework.Utilities.createElement(this._controlContainerElement, ["div", { "data-win-control": "PlayerFramework.UI.ControlPanel", "data-win-bind": "winControl.isPlayPauseHidden: isPlayPauseHidden; winControl.isPlayResumeHidden: isPlayResumeHidden; winControl.isPauseHidden: isPauseHidden; winControl.isReplayHidden: isReplayHidden; winControl.isRewindHidden: isRewindHidden; winControl.isFastForwardHidden: isFastForwardHidden; winControl.isSlowMotionHidden: isSlowMotionHidden; winControl.isSkipPreviousHidden: isSkipPreviousHidden; winControl.isSkipNextHidden: isSkipNextHidden; winControl.isSkipBackHidden: isSkipBackHidden; winControl.isSkipAheadHidden: isSkipAheadHidden;  winControl.isElapsedTimeHidden: isElapsedTimeHidden; winControl.isRemainingTimeHidden: isRemainingTimeHidden; winControl.isTotalTimeHidden: isTotalTimeHidden; winControl.isTimelineHidden: isTimelineHidden; winControl.isGoLiveHidden: isGoLiveHidden; winControl.isCaptionsHidden: isCaptionsHidden; winControl.isAudioHidden: isAudioHidden; winControl.isVolumeMuteHidden: isVolumeMuteHidden; winControl.isVolumeHidden: isVolumeHidden; winControl.isMuteHidden: isMuteHidden; winControl.isFullScreenHidden: isFullScreenHidden; winControl.isCastHidden: isCastHidden; winControl.isStopHidden: isStopHidden; winControl.isInfoHidden: isInfoHidden; winControl.isMoreHidden: isMoreHidden; winControl.isZoomHidden: isZoomHidden; winControl.isSignalStrengthHidden: isSignalStrengthHidden; winControl.isMediaQualityHidden: isMediaQualityHidden;" }]);

            this._controlContainerElement.winControl = this;

            WinJS.UI.processAll(this._controlContainerElement);
            PlayerFramework.Binding.processAll(this._controlPanelElement, this.mediaPlayer.interactiveViewModel);

            if (!WinJS.Utilities.isPhone) {
                this._flyoutContainerElement = PlayerFramework.Utilities.createElement(document.body, ["div", { "class": "pf-flyout-container" }]);
                this._controlPanelElement.winControl.flyoutContainerElement = this._flyoutContainerElement;
                PlayerFramework.Binding.processAll(this._flyoutContainerElement, this.mediaPlayer.interactiveViewModel);
            }
        },

        _onActivate: function () {
            this._setElement();

            this._bindEvent("interactiveviewmodelchange", this.mediaPlayer, this._onMediaPlayerInteractiveViewModelChange);
            this._bindEvent("interactivestatechange", this.mediaPlayer, this._onMediaPlayerInteractiveStateChange);
            this._bindEvent("transitionend", this._controlPanelElement, this._onControlPanelTransitionEnd);
            if (this.mediaPlayer.element.onresize !== undefined) {
                this._bindEvent("resize", this.mediaPlayer.element, this._onMediaPlayerResize);
            }
            else { // IE11 no longer supports resize event for arbitrary elements. The best we can do is listen to the window resize event.
                this._bindEvent("resize", window, this._onMediaPlayerResize);
            }
            this._onMediaPlayerResize();

            if (this.mediaPlayer.isInteractive) {
                this.show();
                this._controlPanelElement.winControl.hidden = false;
            } else {
                this._controlPanelElement.winControl.hidden = true;
            }

            return true;
        },

        _onDeactivate: function () {
            this.hide();

            this._unbindEvent("interactiveviewmodelchange", this.mediaPlayer, this._onMediaPlayerInteractiveViewModelChange);
            this._unbindEvent("interactivestatechange", this.mediaPlayer, this._onMediaPlayerInteractiveStateChange);
            this._unbindEvent("transitionend", this._controlPanelElement, this._onControlPanelTransitionEnd);
            if (this.mediaPlayer.element.onresize !== undefined) {
                this._unbindEvent("resize", this.mediaPlayer.element, this._onMediaPlayerResize);
            }
            else {
                this._unbindEvent("resize", window, this._onMediaPlayerResize);
            }

            PlayerFramework.Utilities.removeElement(this._controlContainerElement);
            if (this._flyoutContainerElement) {
                PlayerFramework.Utilities.removeElement(this._flyoutContainerElement);
            }

            this._controlContainerElement = null;
            this._controlPanelElement = null;
            this._flyoutContainerElement = null;
        },

        _onMediaPlayerInteractiveViewModelChange: function (e) {
            PlayerFramework.Binding.processAll(this._controlPanelElement, this.mediaPlayer.interactiveViewModel);
        },

        _onMediaPlayerInteractiveStateChange: function (e) {
            if (this.mediaPlayer.isInteractive) {
                this.show();
                this._controlPanelElement.winControl.hidden = false;
            } else {
                this._controlPanelElement.winControl.hidden = true;
            }
        },

        _onControlPanelTransitionEnd: function (e) {
            if (e.target === this._controlPanelElement && this._controlPanelElement.winControl.hidden) {
                this.hide();
            }
        },

        _onMediaPlayerResize: function () {
            var w = this.mediaPlayer.element.scrollWidth;
            var h = this.mediaPlayer.element.scrollHeight;

            this.orientation = (h > w) ? "portrait" : "landscape";
            var physicalSize = this._getPhysicalSize({ width: w, height: h });
            this.isCompact = (physicalSize.width <= this.compactThresholdInInches);
        },

        _getPhysicalSize: function (size) {
            var displayInfo = Windows.Graphics.Display.DisplayInformation.getForCurrentView();
            var scale = displayInfo.resolutionScale / 100;
            var w = size.width * scale / displayInfo.rawDpiX;
            var h = size.height * scale / displayInfo.rawDpiY;
            return { width: w, height: h };
        }

    }, {
        // Static Properties
        isDeclarativeControlContainer: {
            get: function () {
                return true;
            }
        }
    });

    // ControlPlugin Mixins
    WinJS.Class.mix(PlayerFramework.MediaPlayer, {
        controlPlugin: {
            value: null,
            writable: true,
            configurable: true
        }
    });

    // ControlPlugin Exports
    WinJS.Namespace.define("PlayerFramework.Plugins", {
        ControlPlugin: ControlPlugin
    });

})(PlayerFramework);


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

        PlayerFramework.PluginBase.call(this, options);
    }, {
        // Public Methods
        show: function () {
            this.mediaPlayer.addClass("pf-show-control-container");
        },

        hide: function () {
            this.mediaPlayer.removeClass("pf-show-control-container");
        },

        // Private Methods
        _setElement: function () {
            this._controlContainerElement = PlayerFramework.Utilities.createElement(this.mediaPlayer.element, ["div", { "class": "pf-control-container" }]);
            this._controlPanelElement = PlayerFramework.Utilities.createElement(this._controlContainerElement, ["div", { "data-win-control": "PlayerFramework.UI.ControlPanel", "data-win-bind": "winControl.isPlayPauseHidden: isPlayPauseHidden; winControl.isPlayResumeHidden: isPlayResumeHidden; winControl.isPauseHidden: isPauseHidden; winControl.isReplayHidden: isReplayHidden; winControl.isRewindHidden: isRewindHidden; winControl.isFastForwardHidden: isFastForwardHidden; winControl.isSlowMotionHidden: isSlowMotionHidden; winControl.isSkipPreviousHidden: isSkipPreviousHidden; winControl.isSkipNextHidden: isSkipNextHidden; winControl.isSkipBackHidden: isSkipBackHidden; winControl.isSkipAheadHidden: isSkipAheadHidden;  winControl.isElapsedTimeHidden: isElapsedTimeHidden; winControl.isRemainingTimeHidden: isRemainingTimeHidden; winControl.isTotalTimeHidden: isTotalTimeHidden; winControl.isTimelineHidden: isTimelineHidden; winControl.isGoLiveHidden: isGoLiveHidden; winControl.isCaptionsHidden: isCaptionsHidden; winControl.isAudioHidden: isAudioHidden; winControl.isVolumeMuteHidden: isVolumeMuteHidden; winControl.isVolumeHidden: isVolumeHidden; winControl.isMuteHidden: isMuteHidden; winControl.isFullScreenHidden: isFullScreenHidden; winControl.isStopHidden: isStopHidden; winControl.isSignalStrengthHidden: isSignalStrengthHidden; winControl.isMediaQualityHidden: isMediaQualityHidden;" }]);
            
            this._controlContainerElement.winControl = this;

            WinJS.UI.processAll(this._controlContainerElement);
            PlayerFramework.Binding.processAll(this._controlContainerElement, this.mediaPlayer.interactiveViewModel);

            this._flyoutContainerElement = PlayerFramework.Utilities.createElement(document.body, ["div", { "class": "pf-flyout-container" }]);
            this._controlPanelElement.winControl.flyoutContainerElement = this._flyoutContainerElement;
            PlayerFramework.Binding.processAll(this._flyoutContainerElement, this.mediaPlayer.interactiveViewModel);
        },

        _onActivate: function () {
            this._setElement();

            this._bindEvent("interactiveviewmodelchange", this.mediaPlayer, this._onMediaPlayerInteractiveViewModelChange);
            this._bindEvent("interactivestatechange", this.mediaPlayer, this._onMediaPlayerInteractiveStateChange);
            this._bindEvent("transitionend", this._controlPanelElement, this._onControlPanelTransitionEnd);

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

            PlayerFramework.Utilities.removeElement(this._controlContainerElement);
            PlayerFramework.Utilities.removeElement(this._flyoutContainerElement);

            this._controlContainerElement = null;
            this._controlPanelElement = null;
            this._flyoutContainerElement = null;
        },

        _onMediaPlayerInteractiveViewModelChange: function (e) {
            PlayerFramework.Binding.processAll(this._controlContainerElement, this.mediaPlayer.interactiveViewModel);
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


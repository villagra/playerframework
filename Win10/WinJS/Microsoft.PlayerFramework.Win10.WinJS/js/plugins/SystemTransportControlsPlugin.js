(function (PlayerFramework, undefined) {
    "use strict";

    // SystemTransportControlsPlugin Errors
    var invalidConstruction = "Invalid construction: SystemTransportControlsPlugin constructor must be called using the \"new\" operator.";

    // SystemTransportControlsPlugin Class
    var SystemTransportControlsPlugin = WinJS.Class.derive(PlayerFramework.PluginBase, function (options) {
        if (!(this instanceof PlayerFramework.Plugins.SystemTransportControlsPlugin)) {
            throw invalidConstruction;
        }

        this._isNextTrackEnabled = false;
        this._isPreviousTrackEnabled = false;
        this._observableViewModel = null;
        this._observablePlaylistPlugin = null;
        this.systemControls = null;

        PlayerFramework.PluginBase.call(this, options);
    }, {
        // Private Methods
        _onActivate: function () {
            this.systemControls = Windows.Media.SystemMediaTransportControls.getForCurrentView();
            this._bindEvent("buttonpressed", this.systemControls, this._onButtonPressed);
            this._refreshFastForwardState();
            this._refreshRewindState();
            this._refreshStopState();
            this._refreshPlayState();
            this._refreshPauseState();
            this._refreshNextState();
            this._refreshPreviousState();
            this._refreshPlaybackStatus();

            if (this.mediaPlayer.playlistPlugin) {
                this._observablePlaylistPlugin = WinJS.Binding.as(this.mediaPlayer.playlistPlugin);
                this._bindProperty("currentPlaylistItemIndex", this._observablePlaylistPlugin, this._onCurrentPlaylistItemIndexChanged);
            }

            this._wireEvents();
            this._bindEvent("interactiveviewmodelchange", this.mediaPlayer, this._onMediaPlayerInteractiveViewModelChange);

            return true;
        },

        _onDeactivate: function () {
            if (this._observablePlaylistPlugin) {
                this._unbindProperty("currentPlaylistItemIndex", this._observablePlaylistPlugin, this._onCurrentPlaylistItemIndexChanged);
                this._observablePlaylistPlugin = null;
            }

            this.isNextTrackEnabled = false;
            this.isPreviousTrackEnabled = false;

            this._unbindEvent("interactiveviewmodelchange", this.mediaPlayer, this._onMediaPlayerInteractiveViewModelChange);
            this._unwireEvents();

            this._unbindEvent("buttonpressed", this.systemControls, this._onButtonPressed);
            this.systemControls = null;
        },

        _wireEvents: function () {
            if (this.mediaPlayer.interactiveViewModel) {
                this._observableViewModel = WinJS.Binding.as(this.mediaPlayer.interactiveViewModel);
                this._bindProperty("isPlayResumeDisabled", this._observableViewModel, this._onIsPlayResumeDisabledChanged);
                this._bindProperty("isPauseDisabled", this._observableViewModel, this._onIsPauseDisabledChanged);
                this._bindProperty("isStopDisabled", this._observableViewModel, this._onIsStopDisabledChanged);
                this._bindProperty("isFastForwardDisabled", this._observableViewModel, this._onIsFastForwardDisabledChanged);
                this._bindProperty("isRewindDisabled", this._observableViewModel, this._onIsRewindDisabledChanged);
                this._bindProperty("isSkipPreviousDisabled", this._observableViewModel, this._onIsSkipPreviousDisabledChanged);
                this._bindProperty("isSkipNextDisabled", this._observableViewModel, this._onIsSkipNextDisabledChanged);
                this._bindProperty("state", this._observableViewModel, this._onStateChanged);
            }
        },

        _unwireEvents: function () {
            if (this._observableViewModel) {
                this._unbindProperty("state", this._observableViewModel, this._onStateChanged);
                this._unbindProperty("isPlayResumeDisabled", this._observableViewModel, this._onIsPlayResumeDisabledChanged);
                this._unbindProperty("isPauseDisabled", this._observableViewModel, this._onIsPauseDisabledChanged);
                this._unbindProperty("isStopDisabled", this._observableViewModel, this._onIsStopDisabledChanged);
                this._unbindProperty("isFastForwardDisabled", this._observableViewModel, this._onIsFastForwardDisabledChanged);
                this._unbindProperty("isRewindDisabled", this._observableViewModel, this._onIsRewindDisabledChanged);
                this._unbindProperty("isSkipPreviousDisabled", this._observableViewModel, this._onIsSkipPreviousDisabledChanged);
                this._unbindProperty("isSkipNextDisabled", this._observableViewModel, this._onIsSkipNextDisabledChanged);
                this._observableViewModel = null;
            }
        },

        isPreviousTrackEnabled: {
            get: function () {
                return this._isPreviousTrackEnabled;
            },
            set: function (value) {
                if (this._isPreviousTrackEnabled !== value) {
                    this._isPreviousTrackEnabled = value;
                    this.systemControls.isPreviousEnabled = value;
                }
            }
        },

        isNextTrackEnabled: {
            get: function () {
                return this._isNextTrackEnabled;
            },
            set: function (value) {
                if (this._isNextTrackEnabled !== value) {
                    this._isNextTrackEnabled = value;
                    this.systemControls.isNextEnabled = value;
                }
            }
        },

        nextTrackExists: {
            get: function () {
                return this.mediaPlayer && this.mediaPlayer.playlistPlugin && this.mediaPlayer.playlistPlugin.canGoToNextPlaylistItem();
            }
        },

        previousTrackExists: {
            get: function () {
                return this.mediaPlayer && this.mediaPlayer.playlistPlugin && this.mediaPlayer.playlistPlugin.canGoToPreviousPlaylistItem();
            }
        },

        _onStateChanged: function (e) {
            this._refreshPlaybackStatus();
        },

        _refreshPlaybackStatus: function () {
            switch (this.mediaPlayer.interactiveViewModel.state) {
                case PlayerFramework.ViewModelState.unloaded:
                    this.systemControls.playbackStatus = Windows.Media.MediaPlaybackStatus.closed;
                    break;
                case PlayerFramework.ViewModelState.loading:
                    this.systemControls.playbackStatus = Windows.Media.MediaPlaybackStatus.changing;
                    break;
                case PlayerFramework.ViewModelState.paused:
                    this.systemControls.playbackStatus = Windows.Media.MediaPlaybackStatus.paused;
                    break;
                case PlayerFramework.ViewModelState.playing:
                    this.systemControls.playbackStatus = Windows.Media.MediaPlaybackStatus.playing;
                    break;
            }
        },

        _onMediaPlayerStopped: function (e) {
            this.systemControls.playbackStatus = Windows.Media.MediaPlaybackStatus.stopped;
        },

        _refreshTrackButtonStates: function () {
            this.isNextTrackEnabled = this.nextTrackExists;
            this.isPreviousTrackEnabled = this.previousTrackExists;
        },

        _onCurrentPlaylistItemIndexChanged: function (e) {
            this.isNextTrackEnabled = this.nextTrackExists;
            this.isPreviousTrackEnabled = this.previousTrackExists;
        },

        _onIsSkipPreviousDisabledChanged: function (e) {
            this._refreshPreviousState();
        },

        _refreshPreviousState: function () {
            this.isPreviousTrackEnabled = this.mediaPlayer.interactiveViewModel !== null && !this.mediaPlayer.interactiveViewModel.isSkipPreviousDisabled && this.previousTrackExists;
        },

        _onIsSkipNextDisabledChanged: function (e) {
            this._refreshNextState();
        },

        _refreshNextState: function () {
            this.isNextTrackEnabled = this.mediaPlayer.interactiveViewModel !== null && !this.mediaPlayer.interactiveViewModel.isSkipNextDisabled && this.nextTrackExists;
        },

        _onIsPauseDisabledChanged: function (e) {
            this._refreshPauseState();
        },

        _refreshPauseState: function () {
            this.systemControls.isPauseEnabled = this.mediaPlayer.interactiveViewModel !== null;
        },

        _onIsPlayResumeDisabledChanged: function (e) {
            this._refreshPlayState();
        },

        _refreshPlayState: function () {
            this.systemControls.isPlayEnabled = this.mediaPlayer.interactiveViewModel !== null;
        },

        _onIsFastForwardDisabledChanged: function (e) {
            this._refreshFastForwardState();
        },

        _refreshFastForwardState: function () {
            this.systemControls.isFastForwardEnabled = this.mediaPlayer.interactiveViewModel !== null && !this.mediaPlayer.interactiveViewModel.isFastForwardDisabled;
        },

        _onIsRewindDisabledChanged: function (e) {
            this._refreshRewindState();
        },

        _refreshRewindState: function () {
            this.systemControls.isRewindEnabled = this.mediaPlayer.interactiveViewModel !== null && !this.mediaPlayer.interactiveViewModel.isRewindDisabled;
        },

        _onIsStopDisabledChanged: function (e) {
            this._refreshStopState();
        },

        _refreshStopState: function () {
            this.systemControls.isStopEnabled = this.mediaPlayer.interactiveViewModel !== null && !this.mediaPlayer.interactiveViewModel.isStopDisabled;
        },

        _onMediaPlayerInteractiveViewModelChange: function (e) {
            // rewire to the new VM
            this._unwireEvents();

            this._refreshFastForwardState();
            this._refreshRewindState();
            this._refreshStopState();
            this._refreshPlayState();
            this._refreshPauseState();
            this._refreshNextState();
            this._refreshPreviousState();
            this._refreshPlaybackStatus();

            this._wireEvents();
        },

        _onButtonPressed: function (e) {
            switch (e.button) {
                case Windows.Media.SystemMediaTransportControlsButton.pause:
                    if (!this.mediaPlayer.interactiveViewModel.isPauseDisabled) {
                        this.mediaPlayer.interactiveViewModel.pause();
                    }
                    break;
                case Windows.Media.SystemMediaTransportControlsButton.play:
                    if (!this.mediaPlayer.interactiveViewModel.isPlayResumeDisabled) {
                        this.mediaPlayer.interactiveViewModel.playResume();
                    }
                    break;
                case Windows.Media.SystemMediaTransportControlsButton.stop:
                    if (!this.mediaPlayer.interactiveViewModel.isStopDisabled) {
                        this.mediaPlayer.interactiveViewModel.stop();
                    }
                    break;
                case Windows.Media.SystemMediaTransportControlsButton.previous:
                    this.mediaPlayer.playlistPlugin.goToPreviousPlaylistItem();
                    break;
                case Windows.Media.SystemMediaTransportControlsButton.next:
                    this.mediaPlayer.playlistPlugin.goToNextPlaylistItem();
                    break;
                case Windows.Media.SystemMediaTransportControlsButton.rewind:
                    if (!this.mediaPlayer.interactiveViewModel.isRewindDisabled) {
                        this.mediaPlayer.interactiveViewModel.decreasePlaybackRate();
                    }
                    break;
                case Windows.Media.SystemMediaTransportControlsButton.fastForward:
                    if (!this.mediaPlayer.interactiveViewModel.isFastForwardDisabled) {
                        this.mediaPlayer.interactiveViewModel.increasePlaybackRate();
                    }
                    break;

            }
        }
    });

    // SystemTransportControlsPlugin Mixins
    WinJS.Class.mix(PlayerFramework.MediaPlayer, {
        SystemTransportControlsPlugin: {
            value: null,
            writable: true,
            configurable: true
        }
    });

    // SystemTransportControlsPlugin Exports
    WinJS.Namespace.define("PlayerFramework.Plugins", {
        SystemTransportControlsPlugin: SystemTransportControlsPlugin
    });

})(PlayerFramework);


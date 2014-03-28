(function (PlayerFramework, undefined) {
    "use strict";

    // MediaControl Errors
    var invalidConstruction = "Invalid construction: MediaControlPlugin constructor must be called using the \"new\" operator.";
    
    // MediaControl Class
    var MediaControlPlugin = WinJS.Class.derive(PlayerFramework.PluginBase, function (options) {
        if (!(this instanceof PlayerFramework.Plugins.MediaControlPlugin)) {
            throw invalidConstruction;
        }

        this._isNextTrackEnabled = false;
        this._isPreviousTrackEnabled = false;
        this.__observableViewModel = null;
        this.__observablePlaylistPlugin = null;
        this.mediaControl = null;

        PlayerFramework.PluginBase.call(this, options);
    }, {
        // Private Methods
        _onActivate: function () {
            this.mediaControl = Windows.Media.MediaControl;

            this._bindEvent("playpausetogglepressed", this.mediaControl, this._onPlayPauseTogglePressed);
            this._bindEvent("playpressed", this.mediaControl, this._onPlayPressed);
            this._bindEvent("pausepressed", this.mediaControl, this._onPausePressed);
            this._bindEvent("stoppressed", this.mediaControl, this._onStopPressed);
            this._bindEvent("soundlevelchanged", this.mediaControl, this._onSoundLevelChanged);
            this._bindEvent("rewindpressed", this.mediaControl, this._onRewindPressed);
            this._bindEvent("fastforwardpressed", this.mediaControl, this._onFastForwardPressed);
            //this._bindEvent("previoustrackpressed", this.mediaControl, this._onPreviousTrackPressed);
            //this._bindEvent("nexttrackpressed", this.mediaControl, this._onNextTrackPressed);

            this.mediaControl.isPlaying = this.mediaPlayer.interactiveViewModel.isPlayResumeDisabled;
            this._observableViewModel = WinJS.Binding.as(this.mediaPlayer.interactiveViewModel);
            this._bindProperty("isPlayResumeDisabled", this._observableViewModel, this._onIsPlayResumeDisabledChanged);

            this._bindEvent("interactiveviewmodelchange", this.mediaPlayer, this._onMediaPlayerInteractiveViewModelChange);

            this._refreshTrackButtonStates();
            if (this.mediaPlayer.playlistPlugin) {
                this._observablePlaylistPlugin = WinJS.Binding.as(this.mediaPlayer.playlistPlugin);
                this._bindProperty("currentPlaylistItemIndex", this._observablePlaylistPlugin, this._onCurrentPlaylistItemIndexChanged);
            }

            return true;
        },

        _onDeactivate: function () {
            if (this._observablePlaylistPlugin) {
                this._unbindProperty("currentPlaylistItemIndex", this._observablePlaylistPlugin, this._onCurrentPlaylistItemIndexChanged);
                this._observablePlaylistPlugin = null;
            }

            if (this._observableViewModel) {
                this._unbindProperty("isPlayResumeDisabled", this._observableViewModel, this._onIsPlayResumeDisabledChanged);
                this._observableViewModel = null;
            }

            this._unbindEvent("interactiveviewmodelchange", this.mediaPlayer, this._onMediaPlayerInteractiveViewModelChange);

            this._unbindEvent("playpausetogglepressed", this.mediaControl, this._onPlayPauseTogglePressed);
            this._unbindEvent("playpressed", this.mediaControl, this._onPlayPressed);
            this._unbindEvent("pausepressed", this.mediaControl, this._onPausePressed);
            this._unbindEvent("stoppressed", this.mediaControl, this._onStopPressed);
            this._unbindEvent("soundlevelchanged", this.mediaControl, this._onSoundLevelChanged);
            this._unbindEvent("rewindpressed", this.mediaControl, this._onRewindPressed);
            this._unbindEvent("fastforwardpressed", this.mediaControl, this._onFastForwardPressed);
            //this._unbindEvent("previoustrackpressed", this.mediaControl, this._onPreviousTrackPressed);
            //this._unbindEvent("nexttrackpressed", this.mediaControl, this._onNextTrackPressed);

            this.mediaControl = null;
        },

        isPreviousTrackEnabled: {
            get: function () {
                return this._isPreviousTrackEnabled;
            },
            set: function (value) {
                if (this._isPreviousTrackEnabled !== value) {
                    this._isPreviousTrackEnabled = value;
                    if (this._isPreviousTrackEnabled) {
                        this._bindEvent("previoustrackpressed", this.mediaControl, this._onPreviousTrackPressed);
                    }
                    else {
                        this._unbindEvent("previoustrackpressed", this.mediaControl, this._onPreviousTrackPressed);
                    }
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
                    if (this._isNextTrackEnabled) {
                        this._bindEvent("nexttrackpressed", this.mediaControl, this._onNextTrackPressed);
                    }
                    else {
                        this._unbindEvent("nexttrackpressed", this.mediaControl, this._onNextTrackPressed);
                    }
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

        _refreshTrackButtonStates: function () {
            this.isNextTrackEnabled = this.nextTrackExists;
            this.isPreviousTrackEnabled = this.previousTrackExists;
        },

        _onCurrentPlaylistItemIndexChanged: function (e) {
            this._refreshTrackButtonStates();
        },

        _onIsPlayResumeDisabledChanged: function (e) {
            this.mediaControl.isPlaying = this.mediaPlayer.interactiveViewModel.isPlayResumeDisabled;
        },

        _onMediaPlayerInteractiveViewModelChange: function (e) {
            // rewire to the new VM
            if (this._observableViewModel) {
                this._unbindProperty("isPlayResumeDisabled", this._observableViewModel, this._onIsPlayResumeDisabledChanged);
                this._observableViewModel = null;
            }
            if (this.mediaPlayer.interactiveViewModel) {
                this._observableViewModel = WinJS.Binding.as(this.mediaPlayer.interactiveViewModel);
                this._bindProperty("isPlayResumeDisabled", this._observableViewModel, this._onIsPlayResumeDisabledChanged);
            }
        },

        _onPreviousTrackPressed: function (e) {
            this.mediaPlayer.playlistPlugin.goToPreviousPlaylistItem();
        },

        _onNextTrackPressed: function (e) {
            this.mediaPlayer.playlistPlugin.goToNextPlaylistItem();
        },

        _onPlayPauseTogglePressed: function (e) {
            if (!this.mediaPlayer.interactiveViewModel.isPlayPauseDisabled) {
                this.mediaPlayer.interactiveViewModel.playPause();
            }
        },

        _onPlayPressed: function (e) {
            if (!this.mediaPlayer.interactiveViewModel.isPlayResumeDisabled) {
                this.mediaPlayer.interactiveViewModel.playResume();
            }
        },

        _onPausePressed: function (e) {
            if (!this.mediaPlayer.interactiveViewModel.isPauseDisabled) {
                this.mediaPlayer.interactiveViewModel.pause();
            }
        },

        _onStopPressed: function (e) {
            if (!this.mediaPlayer.interactiveViewModel.isStopDisabled) {
                this.mediaPlayer.interactiveViewModel.stop();
            }
        },

        _onSoundLevelChanged: function (e) {
            // do nothing
        },

        _onRewindPressed: function (e) {
            if (!this.mediaPlayer.interactiveViewModel.isRewindDisabled) {
                this.mediaPlayer.interactiveViewModel.decreasePlaybackRate();
            }
        },

        _onFastForwardPressed: function (e) {
            if (!this.mediaPlayer.interactiveViewModel.isFastForwardDisabled) {
                this.mediaPlayer.interactiveViewModel.increasePlaybackRate();
            }
        }
    });

    // MediaControl Mixins
    WinJS.Class.mix(PlayerFramework.MediaPlayer, {
        MediaControlPlugin: {
            value: null,
            writable: true,
            configurable: true
        }
    });

    // MediaControl Exports
    WinJS.Namespace.define("PlayerFramework.Plugins", {
        MediaControlPlugin: MediaControlPlugin
    });

})(PlayerFramework);


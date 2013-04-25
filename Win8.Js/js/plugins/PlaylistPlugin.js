(function (PlayerFramework, undefined) {
    "use strict";

    // PlaylistPlugin Errors
    var invalidConstruction = "Invalid construction: PlaylistPlugin constructor must be called using the \"new\" operator.",
        invalidPlaylistItem = "Invalid playlist item: Playlist must contain item.",
        invalidPlaylistItemIndex = "Invalid playlist item index: Playlist must contain item.";

    // PlaylistPlugin Class
    var PlaylistPlugin = WinJS.Class.derive(PlayerFramework.PluginBase, function (options) {
        if (!(this instanceof PlayerFramework.Plugins.PlaylistPlugin)) {
            throw invalidConstruction;
        }

        this._playlist = [];
        this._autoAdvance = true;
        this._skipBackThreshold = 5;
        this._currentPlaylistItem = null;
        this._currentPlaylistItemIndex = -1;
        this._startupPlaylistItemIndex = 0;
        this._isInitialized = false;

        PlayerFramework.PluginBase.call(this, options);
    }, {
        // Public Properties
        playlist: {
            get: function () {
                return this._playlist;
            },
            set: function (value) {
                var oldValue = this._playlist;
                if (oldValue !== value) {
                    this._playlist = value;
                    this._observablePlugin.notify("playlist", value, oldValue);
                }
            }
        },

        autoAdvance: {
            get: function () {
                return this._autoAdvance;
            },
            set: function (value) {
                var oldValue = this._autoAdvance;
                if (oldValue !== value) {
                    this._autoAdvance = value;
                    this._observablePlugin.notify("autoAdvance", value, oldValue);
                }
            }
        },

        skipBackThreshold: {
            get: function () {
                return this._skipBackThreshold;
            },
            set: function (value) {
                var oldValue = this._skipBackThreshold;
                if (oldValue !== value) {
                    this._skipBackThreshold = value;
                    this._observablePlugin.notify("skipBackThreshold", value, oldValue);
                }
            }
        },

        currentPlaylistItem: {
            get: function () {
                return this._currentPlaylistItem;
            },
            set: function (value) {
                var oldValue = this._currentPlaylistItem;
                if (oldValue !== value) {
                    if ((value && !this.playlist) || (value && this.playlist.indexOf(value) === -1)) {
                        throw invalidPlaylistItem;
                    }

                    if (value) {
                        this._currentPlaylistItem = value;
                        this._observablePlugin.notify("currentPlaylistItem", value, oldValue);
                        this.currentPlaylistItemIndex = this.playlist.indexOf(value);
                        this.mediaPlayer.update(value);
                    } else if (oldValue !== null) {
                        this._currentPlaylistItem = null;
                        this._observablePlugin.notify("currentPlaylistItem", null, oldValue);
                        this.currentPlaylistItemIndex = -1;
                        this.mediaPlayer.update(null);
                    } 
                }
            }
        },

        currentPlaylistItemIndex: {
            get: function () {
                return this._currentPlaylistItemIndex;
            },
            set: function (value) {
                var oldValue = this._currentPlaylistItemIndex;
                if (oldValue !== value) {
                    if (typeof value !== "number" || (value >= 0 && !this.playlist) || (value >= 0 && value >= this.playlist.length)) {
                        throw invalidPlaylistItemIndex;
                    }

                    if (value >= 0) {
                        this._currentPlaylistItemIndex = value;
                        this._observablePlugin.notify("currentPlaylistItemIndex", value, oldValue);
                        this.currentPlaylistItem = this.playlist[value];
                    } else if (oldValue !== -1) {
                        this._currentPlaylistItemIndex = -1;
                        this._observablePlugin.notify("currentPlaylistItemIndex", -1, oldValue);
                        this.currentPlaylistItem = null;
                    }
                }
            }
        },

        startupPlaylistItemIndex: {
            get: function () {
                return this._startupPlaylistItemIndex;
            },
            set: function (value) {
                var oldValue = this._startupPlaylistItemIndex;
                if (oldValue !== value) {
                    this._startupPlaylistItemIndex = value;
                    this._observablePlugin.notify("startupPlaylistItemIndex", value, oldValue);
                }
            }
        },

        // Public Methods
        goToPreviousPlaylistItem: function () {
            if (this.canGoToPreviousPlaylistItem()) {
                this.currentPlaylistItem = this.playlist[this.currentPlaylistItemIndex - 1];
            }
        },

        goToNextPlaylistItem: function () {
            if (this.canGoToNextPlaylistItem()) {
                this.currentPlaylistItem = this.playlist[this.currentPlaylistItemIndex + 1];
            }
        },

        canGoToPreviousPlaylistItem: function () {
            return this.playlist && this.currentPlaylistItemIndex > 0;
        },

        canGoToNextPlaylistItem: function () {
            return this.playlist && this.currentPlaylistItemIndex < this.playlist.length - 1;
        },

        // Private Methods
        _onActivate: function () {
            this._bindEvent("interactiveviewmodelchange", this.mediaPlayer, this._onMediaPlayerInteractiveViewModelChange);
            this._bindEvent("initialized", this.mediaPlayer, this._onMediaPlayerInitialized);
            this._bindEvent("ended", this.mediaPlayer, this._onMediaPlayerEnded);

            this._interactiveViewModel = this.mediaPlayer.interactiveViewModel;

            if (this._interactiveViewModel) {
                this._bindEvent("skipback", this._interactiveViewModel, this._onInteractiveViewModelSkipBack);
                this._bindEvent("skipahead", this._interactiveViewModel, this._onInteractiveViewModelSkipAhead);
                this._bindEvent("skipprevious", this._interactiveViewModel, this._onInteractiveViewModelSkipPrevious);
                this._bindEvent("skipnext", this._interactiveViewModel, this._onInteractiveViewModelSkipNext);
            }

            if (this._isInitialized && this.playlist && this.startupPlaylistItemIndex !== null) {
                this.currentPlaylistItem = this.playlist[this.startupPlaylistItemIndex];
            }

            return true;
        },

        _onDeactivate: function () {
            this.currentPlaylistItem = null;

            this._unbindEvent("interactiveviewmodelchange", this.mediaPlayer, this._onMediaPlayerInteractiveViewModelChange);
            this._unbindEvent("initialized", this.mediaPlayer, this._onMediaPlayerInitialized);
            this._unbindEvent("ended", this.mediaPlayer, this._onMediaPlayerEnded);

            if (this._interactiveViewModel) {
                this._unbindEvent("skipback", this._interactiveViewModel, this._onInteractiveViewModelSkipBack);
                this._unbindEvent("skipahead", this._interactiveViewModel, this._onInteractiveViewModelSkipAhead);
                this._unbindEvent("skipprevious", this._interactiveViewModel, this._onInteractiveViewModelSkipPrevious);
                this._unbindEvent("skipnext", this._interactiveViewModel, this._onInteractiveViewModelSkipNext);
            }
            
            this._interactiveViewModel = null;
        },

        _onMediaPlayerInteractiveViewModelChange: function (e) {
            if (this._interactiveViewModel) {
                this._unbindEvent("skipback", this._interactiveViewModel, this._onInteractiveViewModelSkipBack);
                this._unbindEvent("skipahead", this._interactiveViewModel, this._onInteractiveViewModelSkipAhead);
                this._unbindEvent("skipprevious", this._interactiveViewModel, this._onInteractiveViewModelSkipPrevious);
                this._unbindEvent("skipnext", this._interactiveViewModel, this._onInteractiveViewModelSkipNext);
            }

            this._interactiveViewModel = this.mediaPlayer.interactiveViewModel;

            if (this._interactiveViewModel) {
                this._bindEvent("skipback", this._interactiveViewModel, this._onInteractiveViewModelSkipBack);
                this._bindEvent("skipahead", this._interactiveViewModel, this._onInteractiveViewModelSkipAhead);
                this._bindEvent("skipprevious", this._interactiveViewModel, this._onInteractiveViewModelSkipPrevious);
                this._bindEvent("skipnext", this._interactiveViewModel, this._onInteractiveViewModelSkipNext);
            }
        },

        _onMediaPlayerInitialized: function (e) {
            if (this.playlist && this.startupPlaylistItemIndex !== null) {
                this.currentPlaylistItem = this.playlist[this.startupPlaylistItemIndex];
            }

            this._isInitialized = true;
        },

        _onMediaPlayerEnded: function (e) {
            if (!this.mediaPlayer.loop && this.autoAdvance && this.canGoToNextPlaylistItem()) {
                this.goToNextPlaylistItem();
                e.preventDefault();
            }
        },

        _onInteractiveViewModelSkipBack: function (e) {
            if (e.detail.time === 0 && (!this.skipBackThreshold || this.mediaPlayer.currentTime < this.skipBackThreshold) && this.canGoToPreviousPlaylistItem()) {
                this.goToPreviousPlaylistItem();
                e.preventDefault();
            }
        },

        _onInteractiveViewModelSkipAhead: function (e) {
            if (e.detail.time === this.mediaPlayer.duration && this.canGoToNextPlaylistItem()) {
                this.goToNextPlaylistItem();
                e.preventDefault();
            }
        },

        _onInteractiveViewModelSkipPrevious: function (e) {
            if ((!this.skipBackThreshold || this.mediaPlayer.currentTime < this.skipBackThreshold) && this.canGoToPreviousPlaylistItem()) {
                this.goToPreviousPlaylistItem();
                e.preventDefault();
            }
        },

        _onInteractiveViewModelSkipNext: function (e) {
            if (this.canGoToNextPlaylistItem()) {
                this.goToNextPlaylistItem();
                e.preventDefault();
            }
        }
    });

    // PlaylistPlugin Mixins
    WinJS.Class.mix(PlayerFramework.MediaPlayer, {
        playlistPlugin: {
            value: null,
            writable: true,
            configurable: true
        }
    });

    // PlaylistPlugin Exports
    WinJS.Namespace.define("PlayerFramework.Plugins", {
        PlaylistPlugin: PlaylistPlugin
    });

})(PlayerFramework);


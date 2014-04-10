(function (PlayerFramework, undefined) {
    "use strict";

    // AudioSelectorPlugin Errors
    var invalidConstruction = "Invalid construction: AudioSelectorPlugin constructor must be called using the \"new\" operator.";

    if (WinJS.Utilities.isPhone) {// AudioSelectorPlugin Class
        var AudioSelectorPlugin = WinJS.Class.derive(PlayerFramework.PluginBase, function (options) {
            if (!(this instanceof PlayerFramework.Plugins.AudioSelectorPlugin)) {
                throw invalidConstruction;
            }

            this._menuElement = null;
            this._menuListElement = null;
            this._resumeOnHide = false;

            PlayerFramework.PluginBase.call(this, options);
        }, {
            // Public Methods
            show: function () {
                if (this._menuElement.style.display === "none") {
                    this._menuElement.style.display = "";
                    this.resumeOnHide = !this.mediaPlayer.paused;
                    if (this.resumeOnHide) {
                        this.mediaPlayer.interactiveViewModel.pause();
                    }
                }
            },

            hide: function () {
                if (this._menuElement.style.display !== "none") {
                    this._menuElement.style.display = "none";
                    if (this.resumeOnHide) {
                        this.mediaPlayer.interactiveViewModel.playResume();
                    }
                }
            },

            // Private Methods
            _setElement: function () {
                this._menuElement = PlayerFramework.Utilities.createElement(document.body, ["div", { "class": "pf-audio-menu", "style": "position: absolute; display: none" }]);
                this._menuListElement = PlayerFramework.Utilities.createElement(this._menuElement, ["select", { "class": "pf-audio-menulist" }]);
            },

            _onActivate: function () {
                this._setElement();

                this._bindEvent("audioinvoked", this.mediaPlayer, this._onMediaPlayerAudioInvoked);
                this._menuListElement.addEventListener("change", this._onMenuItemClick.bind(this), false);

                return true;
            },

            _onDeactivate: function () {
                this.hide();

                this._unbindEvent("audioinvoked", this.mediaPlayer, this._onMediaPlayerAudioInvoked);

                PlayerFramework.Utilities.removeElement(this._menuElement);
                this._menuElement = null;

                this._anchor = null;
            },

            _onMediaPlayerAudioInvoked: function (e) {
                if (this._menuElement.style.display === "none") {
                    this._updateList();
                    this.show();
                    this._menuListElement.click();
                } else {
                    this.hide();
                }
            },

            _onMenuItemClick: function (track, e) {
                var that = this;
                var option = this._menuListElement.options[this._menuListElement.selectedIndex];
                var track = option.track;

                window.setImmediate(function () {
                    that.mediaPlayer.currentAudioTrack = track;
                });
                this.hide();
            },

            _updateList: function () {
                this._menuListElement.options.length = 0;

                var tracks = this.mediaPlayer.audioTracks;
                var currentTrack = this.mediaPlayer.currentAudioTrack;
                var commands = [];

                if (tracks) {
                    for (var i = 0; i < tracks.length; i++) {
                        var track = tracks[i];
                        var label = track.label;

                        if (!label && track.language) {
                            label = new Windows.Globalization.Language(track.language).displayName;
                        }

                        if (!label) {
                            label = PlayerFramework.Utilities.getResourceString("AudioTrackLabel_Untitled");
                        }

                        var command = {
                            label: (track === currentTrack) ? PlayerFramework.Utilities.formatResourceString("AudioCommandLabel_Selected", label) : PlayerFramework.Utilities.formatResourceString("AudioCommandLabel_Unselected", label),
                            track: track
                        };
                        commands.push(command);
                    }
                }

                for (var i = 0; i < commands.length; i++) {
                    var command = commands[i];
                    var _menuListItem = PlayerFramework.Utilities.createElement(this._menuListElement, ["option"]);
                    _menuListItem.innerText = command.label;
                    _menuListItem.track = command.track;
                }
            }
        });
    }
    else {
        // AudioSelectorPlugin Class
        var AudioSelectorPlugin = WinJS.Class.derive(PlayerFramework.PluginBase, function (options) {
            if (!(this instanceof PlayerFramework.Plugins.AudioSelectorPlugin)) {
                throw invalidConstruction;
            }

            this._anchor = null;
            this._menuElement = null;
            this._placement = "top";
            this._alignment = "center";

            PlayerFramework.PluginBase.call(this, options);
        }, {
            // Public Properties
            placement: {
                get: function () {
                    return this._placement;
                },
                set: function (value) {
                    var oldValue = this._placement;
                    if (oldValue !== value) {
                        this._placement = value;
                        this._observableMediaPlayer.notify("placement", value, oldValue);
                    }
                }
            },

            alignment: {
                get: function () {
                    return this._alignment;
                },
                set: function (value) {
                    var oldValue = this._alignment;
                    if (oldValue !== value) {
                        this._alignment = value;
                        this._observableMediaPlayer.notify("alignment", value, oldValue);
                    }
                }
            },

            anchor: {
                get: function () {
                    return this._anchor;
                },
                set: function (value) {
                    var oldValue = this._anchor;
                    if (oldValue !== value) {
                        this._anchor = value;
                        this._observableMediaPlayer.notify("anchor", value, oldValue);
                    }
                }
            },

            // Public Methods
            show: function () {
                if (this._menuElement.winControl && this._menuElement.winControl.hidden) {
                    if (!this._anchor) {
                        this._anchor = this.mediaPlayer.element.querySelector(".pf-audioselection-anchor");
                    }
                    this._menuElement.winControl.show(this._anchor, this._placement, this._alignment);
                }
            },

            hide: function () {
                if (this._menuElement.winControl && !this._menuElement.winControl.hidden) {
                    this._menuElement.winControl.hide();
                }
            },

            // Private Methods
            _setElement: function () {
                this._menuElement = PlayerFramework.Utilities.createElement(document.body, ["div", { "class": "pf-audio-menu", "data-win-control": "WinJS.UI.Menu" }]);
                
                WinJS.UI.processAll(this._menuElement);
            },

            _onActivate: function () {
                this._setElement();

                this._bindEvent("audioinvoked", this.mediaPlayer, this._onMediaPlayerAudioInvoked);
                this._bindEvent("beforeshow", this._menuElement, this._onBeforeMenuShow);

                return true;
            },

            _onDeactivate: function () {
                this.hide();

                this._unbindEvent("audioinvoked", this.mediaPlayer, this._onMediaPlayerAudioInvoked);
                this._unbindEvent("beforeshow", this._menuElement, this._onBeforeMenuShow);

                PlayerFramework.Utilities.removeElement(this._menuElement);
                this._menuElement = null;

                this._anchor = null;
            },

            _onMediaPlayerAudioInvoked: function (e) {
                if (this._menuElement.winControl) {
                    if (this._menuElement.winControl.hidden) {
                        this.show();
                    } else {
                        this.hide();
                    }
                }
            },

            _onMenuItemClick: function (track, e) {
                var that = this;
                window.setImmediate(function () {
                    that.mediaPlayer.currentAudioTrack = track;
                });
                this.hide();
            },

            _onBeforeMenuShow: function (e) {
                var flyout = this._menuElement.winControl;
                var tracks = this.mediaPlayer.audioTracks;
                var currentTrack = this.mediaPlayer.currentAudioTrack;
                var commands = [];

                if (tracks) {
                    for (var i = 0; i < tracks.length; i++) {
                        var track = tracks[i];
                        var label = track.label;

                        if (!label && track.language) {
                            label = new Windows.Globalization.Language(track.language).displayName;
                        }

                        if (!label) {
                            label = PlayerFramework.Utilities.getResourceString("AudioTrackLabel_Untitled");
                        }

                        var command = new WinJS.UI.MenuCommand();
                        command.flyout = flyout;
                        command.label = (track === currentTrack) ? PlayerFramework.Utilities.formatResourceString("AudioCommandLabel_Selected", label) : PlayerFramework.Utilities.formatResourceString("AudioCommandLabel_Unselected", label);
                        command.onclick = this._onMenuItemClick.bind(this, track);
                        commands.push(command);
                    }
                }

                flyout.commands = commands;
            }
        });
    }

    // AudioSelectorPlugin Mixins
    WinJS.Class.mix(PlayerFramework.MediaPlayer, {
        AudioSelectorPlugin: {
            value: null,
            writable: true,
            configurable: true
        }
    });

    // AudioSelectorPlugin Exports
    WinJS.Namespace.define("PlayerFramework.Plugins", {
        AudioSelectorPlugin: AudioSelectorPlugin
    });

})(PlayerFramework);


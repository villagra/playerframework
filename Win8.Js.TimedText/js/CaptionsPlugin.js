(function (PlayerFramework, undefined) {
    "use strict";

    // CaptionsPlugin Errors
    var invalidConstruction = "Invalid construction: CaptionsPlugin constructor must be called using the \"new\" operator.",
        invalidCaptionTrack = "Invalid caption track: Caption tracks must contain track.";

    // CaptionsPlugin Class
    var CaptionsPlugin = WinJS.Class.derive(PlayerFramework.PluginBase, function (options) {
        if (!(this instanceof PlayerFramework.Plugins.CaptionsPlugin)) {
            throw invalidConstruction;
        }

        this._captionsContainerElement = null;
        this._pollingIntervalId = null;

        PlayerFramework.PluginBase.call(this, options);
    }, {
        // Public Properties
        displayMode: {
            get: function () {
                return this._displayMode;
            },
            set: function (value) {
                this._displayMode = value;
            }
        },

        pollingInterval: {
            get: function () {
                return this._pollingInterval;
            },
            set: function (value) {
                this._pollingInterval = value;
            }
        },

        currentTrack: {
            get: function () {
                return this.mediaPlayer._currentCaptionTrack;
            },
            set: function (value) {
                var oldValue = this.mediaPlayer._currentCaptionTrack;
                if (oldValue !== value) {
                    // validate new track
                    if (value && this.mediaPlayer.captionTracks.indexOf(value) === -1) {
                        throw invalidCaptionTrack;
                    }

                    // hide old track
                    if (oldValue) {
                        if (oldValue instanceof PlayerFramework.DynamicTextTrack) {
                            this._unbindEvent("payloadaugmented", oldValue, this._onPayloadAugmented);
                        }
                        this._hideTrack(oldValue);
                    }

                    // show new track
                    if (value) {
                        this._showTrack(value);
                        if (value instanceof PlayerFramework.DynamicTextTrack) {
                            this._bindEvent("payloadaugmented", value, this._onPayloadAugmented);
                        }
                        this.show();
                    } else {
                        this.hide();
                    }

                    this.mediaPlayer._currentCaptionTrack = value;
                    this.mediaPlayer._observableMediaPlayer.notify("currentCaptionTrack", value, oldValue);
                    this.mediaPlayer.dispatchEvent("currentcaptiontrackchange");
                }
            }
        },

        // Public Methods
        show: function () {
            this.mediaPlayer.addClass("pf-show-captions-container");
        },

        hide: function () {
            this.mediaPlayer.removeClass("pf-show-captions-container");
        },

        // Private Methods
        _setElement: function () {
            this._captionsContainerElement = PlayerFramework.Utilities.createElement(this.mediaPlayer.element, ["div", { "class": "pf-captions-container" }]);
        },

        _setOptions: function (options) {
            PlayerFramework.Utilities.setOptions(this, options, {
                isEnabled: true,
                displayMode: PlayerFramework.TextTrackDisplayMode.custom,
                pollingInterval: 10
            });
        },

        _showTrack: function (track) {
            /// <summary>
            ///     Shows the specified track.
            /// </summary>
            /// <param name="track" type="TextTrack">
            ///     The track to show.
            /// </param>

            switch (this.displayMode) {
                case PlayerFramework.TextTrackDisplayMode.custom:
                    this._hideNativeCaptions(track);
                    this._showCustomCaptions(track);
                    break;

                case PlayerFramework.TextTrackDisplayMode.native:
                    this._hideCustomCaptions(track);
                    this._showNativeCaptions(track);
                    break;
                        
                case PlayerFramework.TextTrackDisplayMode.all:
                    this._showNativeCaptions(track);
                    this._showCustomCaptions(track);
                    break;

                default:
                    this._hideNativeCaptions(track);
                    this._hideCustomCaptions(track);
                    break;
            }
        },

        _hideTrack: function (track) {
            /// <summary>
            ///     Hides the specified track.
            /// </summary>
            /// <param name="track" type="TextTrack">
            ///     The track to hide.
            /// </param>

            this._hideNativeCaptions(track);
            this._hideCustomCaptions(track);
        },

        _loadTrack: function (track) {
            /// <summary>
            ///     Loads the specified track.
            /// </summary>
            /// <param name="track" type="TextTrack">
            ///     The track to load.
            /// </param>
            /// <returns type="WinJS.Promise">The promise.</returns>

            track.customReadyState = PlayerFramework.TextTrackReadyState.loading;

            var trackElements = this.mediaPlayer.mediaElement.querySelectorAll("track");
            var trackElement = PlayerFramework.Utilities.first(trackElements, function (element) { return (element.track === track); });
            var trackSource = null;
            if (trackElement) {
                var trackSource = trackElement.src;
            }

            if (trackSource) {
                return WinJS.xhr({ url: trackSource }).then(
                    function (result) {
                        this._setTrack(track, result.responseXML);
                    }.bind(this),
                    function () {
                        this._clearTrack(track);
                    }.bind(this)
                );
            }
            else {
                // select the track, but data doesn't exist yet (for in-stream captions)
                return new WinJS.Promise(function () {
                    this._setTrack(track, null);
                }.bind(this));
            }
        },

        _setTrack: function (track, ttmlDocument) {
            if (ttmlDocument) {
                var ttmlParser = new PlayerFramework.TimedText.TtmlParser();
                ttmlParser.parseTtml(ttmlDocument);

                track.customCues = ttmlParser.cues;
            }
            else {
                track.customCues = [];
            }
            track.customReadyState = PlayerFramework.TextTrackReadyState.loaded;
        },

        _clearTrack: function (track) {
            track.customCues = [];
            track.customReadyState = PlayerFramework.TextTrackReadyState.error;
        },

        _onPayloadAugmented: function (e) {
            try {
                var ttmlDocument = new Windows.Data.Xml.Dom.XmlDocument();
                var ttml = String.fromCharCode.apply(String, e.detail.payload);
                ttmlDocument.loadXml(ttml);

                var ttmlParser = new PlayerFramework.TimedText.TtmlParser();
                ttmlParser.parseTtml(ttmlDocument, e.detail.startTime, e.detail.endTime);
                for (var i = 0; i < ttmlParser.cues.length; i++) {
                    var cue = ttmlParser.cues[i];
                    this.currentTrack.customCues.push(cue);
                }
            }
            catch (error) {
                // unable to parse
                // TODO: raise event
            }
        },

        _showNativeCaptions: function (track) {
            /// <summary>
            ///     Shows native captions for the specified track.
            /// </summary>
            /// <param name="track" type="TextTrack">
            ///     The target track.
            /// </param>

            track.mode = PlayerFramework.TextTrackMode.showing;
            if (this.mediaPlayer._dummyTrack) {
                this.mediaPlayer._dummyTrack.mode = PlayerFramework.TextTrackMode.off;
            }
        },

        _hideNativeCaptions: function (track) {
            /// <summary>
            ///     Hides native captions for the specified track.
            /// </summary>
            /// <param name="track" type="TextTrack">
            ///     The target track.
            /// </param>

            if (this.mediaPlayer._dummyTrack) {
                this.mediaPlayer._dummyTrack.mode = PlayerFramework.TextTrackMode.showing;
            }
            track.mode = PlayerFramework.TextTrackMode.off;
        },

        _showCustomCaptions: function (track) {
            /// <summary>
            ///     Shows custom captions for the specified track.
            /// </summary>
            /// <param name="track" type="TextTrack">
            ///     The target track.
            /// </param>

            track.customMode = PlayerFramework.TextTrackMode.showing;

            if (track.customReadyState === PlayerFramework.TextTrackReadyState.loaded) {
                this._updateCustomCues(track);
            } else if (track.customReadyState !== PlayerFramework.TextTrackReadyState.loading) {
                this._loadTrack(track).done(
                    function () {
                        this._updateCustomCues(track);
                    }.bind(this)
                );
            }
        },

        _refreshCustomCaptions: function (track) {
            /// <summary>
            ///     Refreshes custom captions for the specified track.
            /// </summary>
            /// <param name="track" type="TextTrack">
            ///     The target track.
            /// </param>

            if (track.customReadyState === PlayerFramework.TextTrackReadyState.loaded) {
                this._loadTrack(track).done(
                    function () {
                        if (track.customMode === PlayerFramework.TextTrackMode.showing) {
                            this._captionsContainerElement.innerHTML = "";
                            this._updateCustomCues(track);
                        }
                    }.bind(this)
                );
            }
        },

        _hideCustomCaptions: function (track) {
            /// <summary>
            ///     Hides custom captions for the specified track.
            /// </summary>
            /// <param name="track" type="TextTrack">
            ///     The target track.
            /// </param>

            track.customMode = PlayerFramework.TextTrackMode.off;
            this._updateCustomCues(track);
        },

        _updateCustomCues: function (track) {
            /// <summary>
            ///     Updates the custom cues for the specified track.
            /// </summary>
            /// <param name="track" type="TextTrack">
            ///     The target track.
            /// </param>

            var customCues = track.customCues;
            if (customCues) {
                var currentTime = this.mediaPlayer.currentTime;
                for (var i = 0; i < customCues.length; i++) {
                    var customCue = customCues[i];
                    if (track.customMode === PlayerFramework.TextTrackMode.showing && currentTime >= customCue.startTime && currentTime < customCue.endTime) {
                        PlayerFramework.Utilities.appendElement(this._captionsContainerElement, customCue.cue);
                    } else {
                        PlayerFramework.Utilities.removeElement(customCue.cue);
                    }
                }
            }
        },

        _initializePolling: function () {
            window.clearInterval(this._pollingIntervalId);
            this._pollingIntervalId = window.setInterval(this._onPollingInterval.bind(this), this.pollingInterval * 1000);
        },

        _uninitializePolling: function () {
            window.clearInterval(this._pollingIntervalId);
            this._pollingIntervalId = null;
        },

        _onPollingInterval: function () {
            for (var i = 0; i < this.mediaPlayer.captionTracks.length; i++) {
                var track = this.mediaPlayer.captionTracks[i];
                this._refreshCustomCaptions(track);
            }
        },

        _onActivate: function () {
            this._setElement();

            this._bindEvent("timeupdate", this.mediaPlayer, this._onMediaPlayerTimeUpdate);
            this._bindEvent("islivechange", this.mediaPlayer, this._onMediaPlayerIsLiveChange);
            this._bindEvent("currentcaptiontrackchanging", this.mediaPlayer, this._onMediaPlayerCurrentCaptionTrackChanging);
            
            if (this.mediaPlayer.isLive) {
                this._initializePolling();
            }

            return true;
        },

        _onDeactivate: function () {
            this._unbindEvent("timeupdate", this.mediaPlayer, this._onMediaPlayerTimeUpdate);
            this._unbindEvent("islivechange", this.mediaPlayer, this._onMediaPlayerIsLiveChange);
            this._unbindEvent("currentcaptiontrackchanging", this.mediaPlayer, this._onMediaPlayerCurrentCaptionTrackChanging);

            this._uninitializePolling();

            PlayerFramework.Utilities.removeElement(this._captionsContainerElement);
            this._captionsContainerElement = null;
        },

        _onMediaPlayerTimeUpdate: function (e) {
            var currentTrack = this.currentTrack;
            if (currentTrack) {
                this._updateCustomCues(currentTrack);
            }
        },

        _onMediaPlayerIsLiveChange: function (e) {
            if (this.mediaPlayer.isLive) {
                this._initializePolling();
            } else {
                this._uninitializePolling();
            }
        },

        _onMediaPlayerCurrentCaptionTrackChanging: function (e) {
            this.currentTrack = e.detail.track;
            e.preventDefault();
        }
    });

    // CaptionsPlugin Mixins
    WinJS.Class.mix(PlayerFramework.MediaPlayer, {
        captionsPlugin: {
            value: null,
            writable: true,
            configurable: true
        }
    });

    // CaptionsPlugin Exports
    WinJS.Namespace.define("PlayerFramework.Plugins", {
        CaptionsPlugin: CaptionsPlugin
    });

})(PlayerFramework);


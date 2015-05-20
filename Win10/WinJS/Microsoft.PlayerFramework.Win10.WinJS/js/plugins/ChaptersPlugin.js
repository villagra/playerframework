(function (PlayerFramework, undefined) {
    "use strict";

    // ChaptersPlugin Errors
    var invalidConstruction = "Invalid construction: ChaptersPlugin constructor must be called using the \"new\" operator.";

    // ChaptersPlugin Class
    var ChaptersPlugin = WinJS.Class.derive(PlayerFramework.PluginBase, function (options) {
        if (!(this instanceof PlayerFramework.Plugins.ChaptersPlugin)) {
            throw invalidConstruction;
        }

        this._defaultChapterCount = 10;
        this._autoCreateDefaultChapters = false;
        this._autoCreateChaptersFromTextTracks = false;
        this._visualMarkerClass = "pf-marker-chapter";
        this._chapterMarkers = [];

        PlayerFramework.PluginBase.call(this, options);
    }, {
        // Public Properties
        defaultChapterCount: {
            get: function () {
                return this._defaultChapterCount;
            },
            set: function (value) {
                this._defaultChapterCount = value;
            }
        },
        
        autoCreateDefaultChapters: {
            get: function () {
                return this._autoCreateDefaultChapters;
            },
            set: function (value) {
                this._autoCreateDefaultChapters = value;
            }
        },
        
        autoCreateChaptersFromTextTracks: {
            get: function () {
                return this._autoCreateChaptersFromTextTracks;
            },
            set: function (value) {
                this._autoCreateChaptersFromTextTracks = value;
            }
        },
        
        visualMarkerClass: {
            get: function () {
                return this._visualMarkerClass;
            },
            set: function (value) {
                this._visualMarkerClass = value;
            }
        },

        // Private Methods
        _onActivate: function () {
            this._init();
            this._bindEvent("canplay", this.mediaPlayer, this._onMediaPlayerCanPlay);

            return true;
        },

        _onDeactivate: function () {
            this._unbindEvent("canplay", this.mediaPlayer, this._onMediaPlayerCanPlay);
            this._reset();
        },

        _onUpdate: function () {
            this._reset();
            this._init();
        },
        
        _init: function () {
            if (this._autoCreateChaptersFromTextTracks) {
                this._createChaptersFromTextTracks();
            }
        },

        _reset: function () {
            for (var i = 0; i < this._chapterMarkers.length; i++) {
                var marker = this._chapterMarkers[i];
                var index = this.mediaPlayer.visualMarkers.indexOf(marker);
                this.mediaPlayer.visualMarkers.splice(index, 1);
            }
            this._chapterMarkers = [];
        },

        _onMediaPlayerCanPlay: function (e) {
            if (this._autoCreateDefaultChapters) {
                this._createDefaultChapters();
            }
        },
        
        _createChaptersFromTextTracks: function () {
            var textTracks = this.mediaPlayer.textTracks;
            var tracks = this.mediaPlayer.mediaElement.getElementsByTagName("track");
            for (var i = 0; i < tracks.length; i++) {
                if (tracks[i].kind === "chapters") {
                    // Set track to hidden or cue-related events will not fire.
                    tracks[i].mode = tracks[i].HIDDEN;

                    var that = this;
                    this._loadTextTrackCallback = function handleLoadTextTrack() {
                        var textTrackCueList = this.track.cues;
                        var chapterMarkers = [];
                        var textTrackCueListLength = textTrackCueList.length;
                        for (var j = 0; j < textTrackCueListLength; j++) {
                            var marker = {
                                time: textTrackCueList[j].startTime,
                                isSeekable: true,
                                type: "chapter",
                                text: textTrackCueList[j].text,
                                extraClass: that._visualMarkerClass
                            };
                            chapterMarkers.push(marker);
                        }
                        that._addToVisualMarkers(chapterMarkers);
                    };

                    tracks[i].addEventListener("load", this._loadTextTrackCallback, false);
                }
            }
        },

        _createDefaultChapters: function () {
            var chapterLength = this.mediaPlayer.duration / this._defaultChapterCount;

            var chapterMarkers = [];
            for (var i = 0; i <= this._defaultChapterCount; i++) {
                var marker = {
                    time: i * chapterLength,
                    isSeekable: true,
                    type: "chapter",
                    extraClass: this._visualMarkerClass
                };
                chapterMarkers.push(marker);
            }
            this._addToVisualMarkers(chapterMarkers);
        },

        _addToVisualMarkers: function (markers) {
            var allMarkers = [];
            for (var i = 0; i < markers.length; i++) {
                var marker = markers[i];
                this._chapterMarkers.push(marker);
                allMarkers.push(marker);
            }
            for (var i = 0; i < this.mediaPlayer.visualMarkers.length; i++) {
                var marker = this.mediaPlayer.visualMarkers[i];
                allMarkers.push(marker);
            }
            this.mediaPlayer.visualMarkers = allMarkers;
        }
    });

    // ChaptersPlugin Mixins
    WinJS.Class.mix(PlayerFramework.MediaPlayer, {
        ChaptersPlugin: {
            value: null,
            writable: true,
            configurable: true
        }
    });

    // ChaptersPlugin Exports
    WinJS.Namespace.define("PlayerFramework.Plugins", {
        ChaptersPlugin: ChaptersPlugin
    });

})(PlayerFramework);


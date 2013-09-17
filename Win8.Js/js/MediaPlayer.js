(function (PlayerFramework, undefined) {
    "use strict";

    // MediaPlayer Errors
    var invalidConstruction = "Invalid construction: MediaPlayer constructor must be called using the \"new\" operator.",
        invalidElement = "Invalid argument: MediaPlayer expects an element containing an audio or video element as the first argument.",
        invalidPlugin = "Invalid plugin: Plugin must extend PluginBase class.",
        invalidCaptionTrack = "Invalid caption track: Caption tracks must contain track.",
        invalidAudioTrack = "Invalid audio track: Audio tracks must contain track.";

    // MediaPlayer Events
    var events = [
        "advertisingstatechange",
        "canplay",
        "canplaythrough",
        "currentaudiotrackchange",
        "currentaudiotrackchanging",
        "currentcaptiontrackchange",
        "currentcaptiontrackchanging",
        "durationchange",
        "emptied",
        "ended",
        "ending",
        "error",
        "fullscreenchange",
        "initialized",
        "interactivestatechange",
        "interactiveviewmodelchange",
        "islivechange",
        "loadeddata",
        "loadedmetadata",
        "loading",
        "loadstart",
        "MSVideoFormatChanged",
        "MSVideoFrameStepCompleted",
        "MSVideoOptimalLayoutChanged",
        "mutedchange",
        "pause",
        "play",
        "playerstatechange",
        "playing",
        "progress",
        "ratechange",
        "readystatechange",
        "scrub",
        "scrubbed",
        "scrubbing",
        "seek",
        "seeked",
        "seeking",
        "stalled",
        "started",
        "starting",
        "suspend",
        "timeupdate",
        "updated",
        "volumechange",
        "waiting",
        "markerreached",
        "stopped"
    ];

    // MediaPlayer Class
    WinJS.Namespace.define("PlayerFramework", {
        /// <summary>Allows a user to watch and interact with media.</summary>
        /// <htmlSnippet><![CDATA[<div data-win-control="PlayerFramework.MediaPlayer"></div>]]></htmlSnippet>
        /// <event name="advertisingstatechange">Occurs when the advertising state of the player is changed.</event>
        /// <event name="canplay">Occurs when playback is possible, but would require further buffering.</event>
        /// <event name="canplaythrough">Occurs when playback to the end is possible without requiring further buffering.</event>
        /// <event name="currentaudiotrackchange">Occurs when the current audio track has changed.</event>
        /// <event name="currentaudiotrackchanging">Occurs when the current audio track is changing and presents an opportunity for custom behavior. Used by the adaptive plugin.</event>
        /// <event name="currentcaptiontrackchange">Occurs when the current caption track has changed.</event>
        /// <event name="currentcaptiontrackchanging">Occurs when the current caption track is changing and presents an opportunity for custom behavior. Used by the captions plugin.</event>
        /// <event name="durationchange">Occurs when the media duration is updated.</event>
        /// <event name="emptied">Occurs when the media element is reset to its initial state.</event>
        /// <event name="ended">Occurs when the end of playback is reached.</event>
        /// <event name="ending">Occurs before the ended event and presents an opportunity for deferral. Useful for postroll ads.</event>
        /// <event name="error">Occurs when the media element encounters an error.</event>
        /// <event name="fullscreenchange">Occurs when the full screen state of the player is changed.</event>
        /// <event name="initialized">Occurs when the player has finished initializing itself and its plugins.</event>
        /// <event name="interactivestatechange">Occurs when the interactive state of the player is changed.</event>
        /// <event name="interactiveviewmodelchange">Occurs when the view model used to drive interactive components such as the control panel is changed (e.g. when an ad start or ends).</event>
        /// <event name="islivechange">Occurs when the live state of the media source changes.</event>
        /// <event name="loadeddata">Occurs when media data is loaded at the current playback position.</event>
        /// <event name="loadedmetadata">Occurs when the duration and dimensions of the media have been determined.</event>
        /// <event name="loading">Occurs before the media source is set and offers the ability to perform blocking operations.</event>
        /// <event name="loadstart">Occurs after the media source is set and the player begins looking for media data.</event>
        /// <event name="MSVideoFormatChanged">Occurs when the video format changes.</event>
        /// <event name="MSVideoFrameStepCompleted">Occurs when the video frame has been stepped forward or backward one frame.</event>
        /// <event name="MSVideoOptimalLayoutChanged">Occurs when the msIsLayoutOptimalForPlayback state changes.</event>
        /// <event name="mutedchange">Occurs when the muted state of the player changes.</event>
        /// <event name="pause">Occurs when playback is paused.</event>
        /// <event name="play">Occurs when the play method is requested.</event>
        /// <event name="playerstatechange">Occurs when the state of the player has changed.</event>
        /// <event name="playing">Occurs when the media has started playing.</event>
        /// <event name="progress">Occurs when progress is made while downloading media data.</event>
        /// <event name="ratechange">Occurs when the playback rate is increased or decreased.</event>
        /// <event name="readystatechange">Occurs when the ready state has changed.</event>
        /// <event name="scrub">Occurs when a scrub operation is requested.</event>
        /// <event name="scrubbed">Occurs when a scrub operation ends.</event>
        /// <event name="scrubbing">Occurs when the current playback position is moved due to a scrub operation.</event>
        /// <event name="seek">Occurs when a seek operation is requested.</event>
        /// <event name="seeked">Occurs when a seek operation ends.</event>
        /// <event name="seeking">Occurs when the current playback position is moved due to a seek operation.</event>
        /// <event name="stalled">Occurs when the media download has stopped.</event>
        /// <event name="started">Occurs after playback has started.</event>
        /// <event name="starting">Occurs before playback has started and presents an opportunity for deferral or cancellation. Useful for preroll ads.</event>
        /// <event name="suspend">Occurs when the load operation has been intentionally halted.</event>
        /// <event name="timeupdate">Occurs when the current playback position is updated.</event>
        /// <event name="updated">Occurs when the player is updated with a new media source (e.g when the current playlist item is changed).</event>
        /// <event name="volumechange">Occurs when the volume is changed.</event>
        /// <event name="waiting">Occurs when playback stops because the next video frame is unavailable.</event>
        /// <event name="markerreached">Occurs when a marker has been played through.</event>
        /// <event name="stopped">Occurs when stop button was pressed or stop method was invoked.</event>
        MediaPlayer: WinJS.Class.define(function (element, options) {
            /// <summary>Creates a new instance of the MediaPlayer class.</summary>
            /// <param name="element" type="HTMLElement" domElement="true">The element that hosts the MediaPlayer control.</param>
            /// <param name="options" type="Object" optional="true">A JSON object containing the set of options to be applied initially to the MediaPlayer control.</param>
            /// <returns type="PlayerFramework.MediaPlayer">The new MediaPlayer instance.</returns>

            if (!(this instanceof PlayerFramework.MediaPlayer)) {
                throw invalidConstruction;
            }

            if (!element) {
                throw invalidElement;
            }

            this._element = null;
            this._shimElement = null;
            this._mediaElement = null;
            this._mediaExtensionManager = null;
            this._activePromises = [];
            this._plugins = [];
            this._updating = false;
            this._advertisingState = PlayerFramework.AdvertisingState.none;
            this._playerState = PlayerFramework.PlayerState.unloaded;
            this._src = "";
            this._sources = null;
            this._tracks = null;
            this._audioTracks = null;
            this._currentAudioTrack = null;
            this._captionTracks = null;
            this._currentCaptionTrack = null;
            this._dummyTrack = null;
            this._autoload = true;
            this._autoplay = false;
            this._autohide = true;
            this._autohideTime = 3;
            this._autohideTimeoutId = null;
            this._autohideBehavior = PlayerFramework.AutohideBehavior.preventDuringInteractiveHover;
            this._startupTime = null;
            this._startTime = 0;
            this._endTime = null;
            this._liveTime = null;
            this._liveTimeBuffer = 10;
            this._isLive = false;
            this._isCurrentTimeLive = false;
            this._isStartTimeOffset = false;
            this._scrubbing = false;
            this._seekWhileScrubbing = true;
            this._scrubStartTime = null;
            this._scrubPlaybackRate = null;
            this._isPlayPauseEnabled = true;
            this._isPlayPauseVisible = true;
            this._isPlayResumeEnabled = true;
            this._isPlayResumeVisible = false;
            this._isPauseEnabled = true;
            this._isPauseVisible = false;
            this._replayOffset = 5;
            this._isReplayEnabled = true;
            this._isReplayVisible = false;
            this._isRewindEnabled = true;
            this._isRewindVisible = false;
            this._isFastForwardEnabled = true;
            this._isFastForwardVisible = false;
            this._slowMotionPlaybackRate = 0.25;
            this._isSlowMotionEnabled = true;
            this._isSlowMotionVisible = false;
            this._isSkipPreviousEnabled = true;
            this._isSkipPreviousVisible = false;
            this._isSkipNextEnabled = true;
            this._isSkipNextVisible = false;
            this._skipBackInterval = 30;
            this._isSkipBackEnabled = true;
            this._isSkipBackVisible = false;
            this._skipAheadInterval = 30;
            this._isSkipAheadEnabled = true;
            this._isSkipAheadVisible = false;
            this._isElapsedTimeEnabled = true;
            this._isElapsedTimeVisible = true;
            this._isRemainingTimeEnabled = true;
            this._isRemainingTimeVisible = true;
            this._isTotalTimeEnabled = true;
            this._isTotalTimeVisible = false;
            this._isTimelineEnabled = true;
            this._isTimelineVisible = true;
            this._isGoLiveEnabled = true;
            this._isGoLiveVisible = false;
            this._isCaptionsEnabled = true;
            this._isCaptionsVisible = false;
            this._isAudioEnabled = true;
            this._isAudioVisible = false;
            this._isVolumeMuteEnabled = true;
            this._isVolumeMuteVisible = true;
            this._isVolumeEnabled = true;
            this._isVolumeVisible = false;
            this._isMuteEnabled = true;
            this._isMuteVisible = false;
            this._isFullScreen = false;
            this._isFullScreenEnabled = true;
            this._isFullScreenVisible = false;
            this._isStopEnabled = true;
            this._isStopVisible = false;
            this._signalStrength = 0;
            this._isSignalStrengthEnabled = true;
            this._isSignalStrengthVisible = false;
            this._mediaQuality = PlayerFramework.MediaQuality.standardDefinition;
            this._isMediaQualityEnabled = true;
            this._isMediaQualityVisible = false;
            this._isInteractive = false;
            this._isInteractiveHover = false;
            this._interactivePointerArgs = null;
            this._interactiveActivationMode = PlayerFramework.InteractionType.all;
            this._interactiveDeactivationMode = PlayerFramework.InteractionType.soft;
            this._defaultInteractiveViewModel = new PlayerFramework.InteractiveViewModel(this);
            this._interactiveViewModel = this._defaultInteractiveViewModel;
            this._observableMediaPlayer = WinJS.Binding.as(this);
            this._lastTime = null;
            this._testForMediaPack = true;
            this._visualMarkers = [];
            this._markers = [];
            this._lastMarkerCheckTime = -1;
            this._virtualTime = 0;
            this._isTrickPlayEnabled = true;
            this._simulatedPlaybackRate = 1;
            this._simulatedPlaybackRateTimer = null;
            this._isThumbnailVisible = false;
            this._thumbnailImageSrc = null;

            this._setElement(element);
            this._setOptions(options);
            this._initializePlugins();
            this._updateCurrentSource();

            this.isInteractive = true;
            this.dispatchEvent("initialized");
        }, {
            /************************ Public Properties ************************/

            /// <field name="element" type="HTMLElement" domElement="true">Gets the host element for the control.</field>
            element: {
                get: function () {
                    return this._element;
                }
            },

            /// <field name="mediaElement" type="HTMLMediaElement" domElement="true">Gets the media element associated with the player.</field>
            mediaElement: {
                get: function () {
                    return this._mediaElement;
                }
            },

            /// <field name="mediaExtensionManager" type="Windows.Media.MediaExtensionManager">Gets or sets the media extension manager to be used by the player and its plugins. A new instance will be created on first use if one is not already set.</field>
            mediaExtensionManager: {
                get: function () {
                    if (!this._mediaExtensionManager) {
                        this.mediaExtensionManager = new Windows.Media.MediaExtensionManager();
                    }

                    return this._mediaExtensionManager;
                },
                set: function (value) {
                    var oldValue = this._mediaExtensionManager;
                    if (oldValue !== value) {
                        this._mediaExtensionManager = value;
                        this._observableMediaPlayer.notify("mediaExtensionManager", value, oldValue);
                    }
                }
            },

            /// <field name="plugins" type="Array">Gets the plugins associated with the player.</field>
            plugins: {
                get: function () {
                    return this._plugins;
                }
            },

            /// <field name="src" type="String">Gets or sets the URL of the current media source to be considered.</field>
            src: {
                get: function () {
                    return this._src;
                },
                set: function (value) {
                    var oldValue = this._src;
                    if (oldValue !== value) {
                        this._src = value;
                        this._observableMediaPlayer.notify("src", value, oldValue);
                        this._updateCurrentSource();
                    }
                }
            },

            /// <field name="currentSrc" type="String">Gets the URL of the current media source.</field>
            currentSrc: {
                get: function () {
                    return this._mediaElement.currentSrc;
                }
            },

            /// <field name="sources" type="Array">Gets or sets the media sources to be considered.</field>
            sources: {
                get: function () {
                    return this._sources;
                },
                set: function (value) {
                    var oldValue = this._sources;
                    if (oldValue !== value) {
                        var sourceElements = this._mediaElement.querySelectorAll("source");

                        // remove old sources
                        for (var i = 0; i < sourceElements.length; i++) {
                            var sourceElement = sourceElements[i];
                            PlayerFramework.Utilities.removeElement(sourceElement);
                        }

                        // add new sources
                        if (value) {
                            for (var i = 0; i < value.length; i++) {
                                var sourceObj = value[i];
                                PlayerFramework.Utilities.createElement(this._mediaElement, [ "source", sourceObj ]);
                            }
                        }

                        this._sources = value;
                        this._observableMediaPlayer.notify("sources", value, oldValue);
                    }
                }
            },

            /// <field name="tracks" type="Array">Gets or sets the tracks for the player.</field>
            tracks: {
                get: function () {
                    return this._tracks;
                },
                set: function (value) {
                    var oldValue = this._tracks;
                    if (oldValue !== value) {
                        var trackElements = this._mediaElement.querySelectorAll("track");

                        // remove old tracks
                        for (var i = 0; i < trackElements.length; i++) {
                            var trackElement = trackElements[i];
                            PlayerFramework.Utilities.removeElement(trackElement);
                        }

                        // add new tracks
                        if (value) {
                            for (var i = 0; i < value.length; i++) {
                                var trackObj = value[i];
                                PlayerFramework.Utilities.createElement(this._mediaElement, [ "track", trackObj ]);
                            }
                        }

                        // HACK: add dummy track required to show captions without native controls
                        this._dummyTrack = PlayerFramework.Utilities.createElement(this._mediaElement, [ "track", { "default": true } ]).track;

                        this._tracks = value;
                        this._observableMediaPlayer.notify("tracks", value, oldValue);
                    }
                }
            },

            /// <field name="textTracks" type="TextTrackList">Gets the text tracks for the current media source.</field>
            textTracks: {
                get: function () {
                    return this._mediaElement.textTracks;
                }
            },

            /// <field name="captionTracks" type="Array">Gets the caption and subtitle tracks for the current media source.</field>
            captionTracks: {
                get: function () {
                    return this._captionTracks;
                },
                set: function (value) {
                    var oldValue = this._captionTracks;
                    if (oldValue !== value) {
                        this._captionTracks = value;
                        this._observableMediaPlayer.notify("captionTracks", value, oldValue);
                    }
                }
            },

            /// <field name="currentCaptionTrack" type="TextTrack">Gets or sets the current caption/subtitle track.</field>
            currentCaptionTrack: {
                get: function () {
                    return this._currentCaptionTrack;
                },
                set: function (value) {
                    var oldValue = this._currentCaptionTrack;
                    if (oldValue !== value && !this.dispatchEvent("currentcaptiontrackchanging", { track: value })) {
                        // validate new track
                        if (value && this.captionTracks.indexOf(value) === -1) {
                            throw invalidCaptionTrack;
                        }

                        // hide old track
                        if (oldValue) {
                            this._dummyTrack.mode = PlayerFramework.TextTrackMode.showing;
                            oldValue.mode = PlayerFramework.TextTrackMode.off;
                        }

                        // show new track
                        if (value) {
                            value.mode = PlayerFramework.TextTrackMode.showing;
                            this._dummyTrack.mode = PlayerFramework.TextTrackMode.off;
                        }

                        this._currentCaptionTrack = value;
                        this._observableMediaPlayer.notify("currentCaptionTrack", value, oldValue);
                        this.dispatchEvent("currentcaptiontrackchange");
                    }
                }
            },

            /// <field name="audioTracks" type="Array">Gets the audio tracks for the current media source.</field>
            audioTracks: {
                get: function () {
                    return this._audioTracks;
                },
                set: function (value) {
                    var oldValue = this._audioTracks;
                    if (oldValue !== value) {
                        this._audioTracks = value;
                        this._observableMediaPlayer.notify("audioTracks", value, oldValue);
                    }
                }
            },

            /// <field name="currentAudioTrack" type="AudioTrack">Gets or sets the current audio track.</field>
            currentAudioTrack: {
                get: function () {
                    return this._currentAudioTrack;
                },
                set: function (value) {
                    var oldValue = this._currentAudioTrack;
                    if (oldValue !== value && !this.dispatchEvent("currentaudiotrackchanging", { track: value })) {
                        // validate new track
                        if (value && this.audioTracks.indexOf(value) === -1) {
                            throw invalidAudioTrack;
                        }

                        // disable old track
                        if (oldValue) {
                            try {
                                oldValue.enabled = false;
                            } catch (error) {
                                // do nothing
                            }
                        }

                        // enable new track
                        if (value) {
                            value.enabled = true;
                        }

                        this._currentAudioTrack = value;
                        this._observableMediaPlayer.notify("currentAudioTrack", value, oldValue);
                        this.dispatchEvent("currentaudiotrackchange");
                    }
                }
            },

            /// <field name="advertisingState" type="PlayerFramework.AdvertisingState">Gets or sets the current advertising state of the player.</field>
            advertisingState: {
                get: function () {
                    return this._advertisingState;
                },
                set: function (value) {
                    var oldValue = this._advertisingState;
                    if (oldValue !== value) {
                        this._advertisingState = value;
                        this._observableMediaPlayer.notify("advertisingState", value, oldValue);
                        this.dispatchEvent("advertisingstatechange");
                    }
                }
            },

            /// <field name="playerState" type="PlayerFramework.PlayerState">Gets or sets the current state of the player.</field>
            playerState: {
                get: function () {
                    return this._playerState;
                },
                set: function (value) {
                    var oldValue = this._playerState;
                    if (oldValue !== value) {
                        if (value === PlayerFramework.PlayerState.unloaded || value === PlayerFramework.PlayerState.pending || value === PlayerFramework.PlayerState.loaded) {
                            this._cancelActivePromises();
                            this.audioTracks = null;
                            this.currentAudioTrack = null;
                            this.currentCaptionTrack = null;
                            this.startTime = 0;
                            this.endTime = null;
                            this.liveTime = null;
                            this.isLive = false;
                            this.isCurrentTimeLive = false;
                            this.isStartTimeOffset = false;
                            this.signalStrength = 0;
                            this.mediaQuality = PlayerFramework.MediaQuality.standardDefinition;
                            this.interactiveViewModel = this.defaultInteractiveViewModel;
                            this.advertisingState = PlayerFramework.AdvertisingState.none;
                        }

                        this._playerState = value;
                        this._observableMediaPlayer.notify("playerState", value, oldValue);
                        this.dispatchEvent("playerstatechange");
                    }
                }
            },

            /// <field name="autoload" type="Boolean">Gets or sets a value that specifies whether to start loading the current media source automatically.</field>
            autoload: {
                get: function () {
                    return this._autoload;
                },
                set: function (value) {
                    var oldValue = this._autoload;
                    if (oldValue !== value) {
                        this._autoload = value;
                        this._observableMediaPlayer.notify("autoload", value, oldValue);
                    }
                }
            },

            /// <field name="autoplay" type="Boolean">Gets or sets a value that specifies whether to automatically start playing the current media source.</field>
            autoplay: {
                get: function () {
                    return this._autoplay;
                },
                set: function (value) {
                    var oldValue = this._autoplay;
                    if (oldValue !== value) {
                        this._autoplay = value;
                        this._observableMediaPlayer.notify("autoplay", value, oldValue);
                    }
                }
            },

            /// <field name="autohide" type="Boolean">Gets or sets a value that specifies whether interactive elements (e.g. the control panel) will be hidden automatically.</field>
            autohide: {
                get: function () {
                    return this._autohide;
                },
                set: function (value) {
                    var oldValue = this._autohide;
                    if (oldValue !== value) {
                        if (this.isInteractive) {
                            if (value && (this.interactiveDeactivationMode & PlayerFramework.InteractionType.soft) === PlayerFramework.InteractionType.soft) {
                                this._resetAutohideTimeout();
                            } else {
                                this._clearAutohideTimeout();
                            }
                        }

                        this._autohide = value;
                        this._observableMediaPlayer.notify("autohide", value, oldValue);
                    }
                }
            },

            /// <field name="autohideTime" type="Number">Gets or sets the amount of time (in seconds) before interactive elements (e.g. the control panel) will be hidden automatically.</field>
            autohideTime: {
                get: function () {
                    return this._autohideTime;
                },
                set: function (value) {
                    var oldValue = this._autohideTime;
                    if (oldValue !== value) {
                        this._autohideTime = value;
                        this._observableMediaPlayer.notify("autohideTime", value, oldValue);
                    }
                }
            },

            /// <field name="autohideBehavior" type="PlayerFramework.AutohideBehavior">Gets or sets the behavior of the autohide feature.</field>
            autohideBehavior: {
                get: function () {
                    return this._autohideBehavior;
                },
                set: function (value) {
                    var oldValue = this._autohideBehavior;
                    if (oldValue !== value) {
                        this._autohideBehavior = value;
                        this._observableMediaPlayer.notify("autohideBehavior", value, oldValue);
                    }
                }
            },

            /// <field name="controls" type="Boolean">Gets or sets a value that specifies whether to display the native controls for the current media source.</field>
            controls: {
                get: function () {
                    return this._mediaElement.controls;
                },
                set: function (value) {
                    var oldValue = this._mediaElement.controls;
                    if (oldValue !== value) {
                        this._mediaElement.controls = value;
                        this._observableMediaPlayer.notify("controls", value, oldValue);
                    }
                }
            },

            /// <field name="defaultPlaybackRate" type="Number">Gets or sets the playback rate to use when play is resumed.</field>
            defaultPlaybackRate: {
                get: function () {
                    return this._mediaElement.defaultPlaybackRate;
                },
                set: function (value) {
                    var oldValue = this._mediaElement.defaultPlaybackRate;
                    if (oldValue !== value) {
                        this._mediaElement.defaultPlaybackRate = value;
                        this._observableMediaPlayer.notify("defaultPlaybackRate", value, oldValue);
                    }
                }
            },

            /// <field name="playbackRate" type="Number">Gets or sets the playback rate for the current media source.</field>
            playbackRate: {
                get: function () {
                    if (!this.isTrickPlayEnabled) {
                        return this._simulatedPlaybackRate;
                    }
                    else {
                        return this._mediaElement.playbackRate;
                    }
                },
                set: function (value) {
                    var oldValue = this.playbackRate;
                    if (oldValue !== value) {
                        if (!this.isTrickPlayEnabled)
                        {
                            this._simulatedPlaybackRate = value;
                            if (value === 1.0 || value === 0.0)
                            {
                                window.clearInterval(this._simulatedPlaybackRateTimer);
                                this._simulatedPlaybackRateTimer = null;
                                if (oldValue !== 1.0 || oldValue !== 0.0)
                                {
                                    this.currentTime = this.virtualTime; // we're coming out of simulated trick play, sync positions
                                }
                                this._mediaElement.playbackRate = value;
                            }
                            else
                            {
                                if (oldValue === 1.0 || oldValue === 0.0)
                                {
                                    this._mediaElement.playbackRate = 0;
                                    this._simulatedPlaybackRateTimer = window.setInterval(this._onSimulatedPlaybackRateTimerTick.bind(this), 250);
                                }
                                this.dispatchEvent("ratechange"); // manually raise event since we didn't actually set the mediaElement's playback rate
                            }
                        }
                        else
                        {
                            this._mediaElement.playbackRate = value;
                        }
                        this._observableMediaPlayer.notify("playbackRate", value, oldValue);
                    }
                }
            },

            /// <field name="playbackRate" type="Number">Gets or sets the playback rate for the current media source.</field>
            isTrickPlayEnabled: {
                get: function () {
                    return this._isTrickPlayEnabled;
                },
                set: function (value) {
                    var oldValue = this._isTrickPlayEnabled;
                    if (oldValue !== value) {
                        this._isTrickPlayEnabled = value;
                        this._observableMediaPlayer.notify("isTrickPlayEnabled", value, oldValue);
                    }
                }
            },

            /// <field name="slowMotionPlaybackRate" type="Number">Gets or sets the playback rate to use when in slow motion.</field>
            slowMotionPlaybackRate: {
                get: function () {
                    return this._slowMotionPlaybackRate;
                },
                set: function (value) {
                    var oldValue = this._slowMotionPlaybackRate;
                    if (oldValue !== value) {
                        this._slowMotionPlaybackRate = value;
                        this._observableMediaPlayer.notify("slowMotionPlaybackRate", value, oldValue);
                    }
                }
            },

            /// <field name="autobuffer" type="Boolean">Gets or sets a value that indicates whether to automatically start buffering the current media source.</field>
            autobuffer: {
                get: function () {
                    return this._mediaElement.autobuffer;
                },
                set: function (value) {
                    var oldValue = this._mediaElement.autobuffer;
                    if (oldValue !== value) {
                        this._mediaElement.autobuffer = value;
                        this._observableMediaPlayer.notify("autobuffer", value, oldValue);
                    }
                }
            },

            /// <field name="buffered" type="TimeRanges">Gets the buffered time ranges for the current media source.</field>
            buffered: {
                get: function () {
                    try {
                        return this._mediaElement.buffered;
                    }
                    catch (error) { // triggered when Windows Media Feature Pack is not installed on Windows 8 N/KN
                        return null;
                    }
                }
            },

            /// <field name="played" type="TimeRanges">Gets the played time ranges for the current media source.</field>
            played: {
                get: function () {
                    return this._mediaElement.played;
                }
            },

            /// <field name="paused" type="Boolean">Gets a value that specifies whether playback is paused.</field>
            paused: {
                get: function () {
                    return this._mediaElement.paused;
                }
            },

            /// <field name="ended" type="Boolean">Gets a value that specifies whether playback has ended.</field>
            ended: {
                get: function () {
                    return this._mediaElement.ended;
                }
            },

            /// <field name="error" type="MediaError">Gets the current error state of the player.</field>
            error: {
                get: function () {
                    return this._mediaElement.error;
                }
            },

            /// <field name="networkState" type="PlayerFramework.NetworkState">Gets the current network state for the player.</field>
            networkState: {
                get: function () {
                    return this._mediaElement.networkState;
                }
            },

            /// <field name="readyState" type="PlayerFramework.ReadyState">Gets the current readiness state of the player.</field>
            readyState: {
                get: function () {
                    return this._mediaElement.readyState;
                }
            },

            /// <field name="initialTime" type="Number">Gets the earliest possible position (in seconds) that playback can begin.</field>
            initialTime: {
                get: function () {
                    return this._mediaElement.initialTime;
                }
            },

            /// <field name="startupTime" type="Number">Gets or sets the position (in seconds) where playback should start. This is useful for resuming a video where the user left off in a previous session.</field>
            startupTime: {
                get: function () {
                    return this._startupTime;
                },
                set: function (value) {
                    var oldValue = this._startupTime;
                    if (oldValue !== value) {
                        this._startupTime = value;
                        this._observableMediaPlayer.notify("startupTime", value, oldValue);
                    }
                }
            },

            /// <field name="startTime" type="Number">Gets or sets the start time (in seconds) of the current media source. This is useful in live streaming scenarios.</field>
            startTime: {
                get: function () {
                    return this._startTime;
                },
                set: function (value) {
                    var oldValue = this._startTime;
                    if (oldValue !== value && isFinite(value) && !isNaN(value) && value >= 0) {
                        this._startTime = value;
                        this._observableMediaPlayer.notify("startTime", value, oldValue);
                        this.dispatchEvent("durationchange");
                    }
                }
            },

            /// <field name="isStartTimeOffset" type="Boolean">Gets or sets a value that specifies whether the start time is offset.</field>
            isStartTimeOffset: {
                get: function () {
                    return this._isStartTimeOffset;
                },
                set: function (value) {
                    var oldValue = this._isStartTimeOffset;
                    if (oldValue !== value) {
                        this._isStartTimeOffset = value;
                        this._observableMediaPlayer.notify("isStartTimeOffset", value, oldValue);
                    }
                }
            },

            /// <field name="endTime" type="Number">Gets or sets the end time (in seconds) of the current media source. This is useful in live streaming scenarios.</field>
            endTime: {
                get: function () {
                    return this._endTime;
                },
                set: function (value) {
                    var oldValue = this._endTime;
                    if (oldValue !== value && isFinite(value) && !isNaN(value) && value >= 0) {
                        this._endTime = value;
                        this._observableMediaPlayer.notify("endTime", value, oldValue);
                        this.dispatchEvent("durationchange");
                    }
                }
            },

            /// <field name="duration" type="Number">Gets the duration (in seconds) of the current media source.</field>
            duration: {
                get: function () {
                    return this.endTime - this.startTime;
                }
            },

            /// <field name="liveTime" type="Number">Gets or sets the live position (in seconds).</field>
            liveTime: {
                get: function () {
                    return this._liveTime;
                },
                set: function (value) {
                    var oldValue = this._liveTime;
                    if (oldValue !== value) {
                        this._liveTime = value;
                        this._observableMediaPlayer.notify("liveTime", value, oldValue);
                        this._updateIsCurrentTimeLive();
                    }
                }
            },

            /// <field name="liveTimeBuffer" type="Number">Gets or sets the live buffer time (in seconds) for the current playback position to be considered "live".</field>
            liveTimeBuffer: {
                get: function () {
                    return this._liveTimeBuffer;
                },
                set: function (value) {
                    var oldValue = this._liveTimeBuffer;
                    if (oldValue !== value) {
                        this._liveTimeBuffer = value;
                        this._observableMediaPlayer.notify("liveTimeBuffer", value, oldValue);
                        this._updateIsCurrentTimeLive();
                    }
                }
            },

            /// <field name="isLive" type="Boolean">Gets a value that specifies whether the current media source is a live stream.</field>
            isLive: {
                get: function () {
                    return this._isLive;
                },
                set: function (value) {
                    var oldValue = this._isLive;
                    if (oldValue !== value) {
                        this._isLive = value;
                        this._observableMediaPlayer.notify("isLive", value, oldValue);
                        this.dispatchEvent("islivechange");
                    }
                }
            },

            /// <field name="currentTime" type="Number">Gets or sets the current playback position (in seconds).</field>
            currentTime: {
                get: function () {
                    var result = this._mediaElement.currentTime;
                    if (isFinite(result)) {
                        return result;
                    }
                    else {
                        return 0;
                    }
                },
                set: function (value) {
                    if (this._mediaElement.readyState !== PlayerFramework.ReadyState.nothing && isFinite(value) && !isNaN(value) && value >= 0) {
                        // note: the timeupdate event will notify the observable property for us
                        this._virtualTime = value;
                        this._mediaElement.currentTime = value;
                    }
                }
            },

            /// <field name="virtualTime" type="Number">Gets the position that is being seeked to (even before the seek completes). If seekWhileScrubbing = false, also returns the position being scrubbed to for visual feedback (in seconds).</field>
            virtualTime: {
                get: function () {
                    return this._virtualTime;
                }
            },

            /// <field name="isCurrentTimeLive" type="Boolean">Gets a value that specifies whether the current playback position is "live".</field>
            isCurrentTimeLive: {
                get: function () {
                    return this._isCurrentTimeLive;
                },
                set: function (value) {
                    var oldValue = this._isCurrentTimeLive;
                    if (oldValue !== value) {
                        this._isCurrentTimeLive = value;
                        this._observableMediaPlayer.notify("isCurrentTimeLive", value, oldValue);
                    }
                }
            },

            /// <field name="scrubbing" type="Boolean">Gets a value that specifies whether the player is currently moving to a new playback position due to a scrub operation.</field>
            scrubbing: {
                get: function () {
                    return this._scrubbing;
                }
            },

            /// <field name="seekWhileScrubbing" type="Boolean">Gets or sets a value that specifies whether the current video frame should be updated during a scrub operation.</field>
            seekWhileScrubbing: {
                get: function () {
                    return this._seekWhileScrubbing;
                },
                set: function (value) {
                    var oldValue = this._seekWhileScrubbing;
                    if (oldValue !== value) {
                        this._seekWhileScrubbing = value;
                        this._observableMediaPlayer.notify("seekWhileScrubbing", value, oldValue);
                    }
                }
            },

            /// <field name="seekable" type="TimeRanges">Gets the seekable time ranges of the current media source.</field>
            seekable: {
                get: function () {
                    return this._mediaElement.seekable;
                }
            },

            /// <field name="seeking" type="Boolean">Gets a value that specifies whether the player is currently moving to a new playback position due to a seek operation.</field>
            seeking: {
                get: function () {
                    return this._mediaElement.seeking;
                }
            },

            /// <field name="width" type="String">Gets or sets the width of the host element.</field>
            width: {
                get: function () {
                    return this._element.style.width;
                },
                set: function (value) {
                    var oldValue = this._element.style.width;
                    if (oldValue !== value) {
                        if (typeof value === "number") {
                            this._element.style.width = value + "px";
                        } else {
                            this._element.style.width = value;
                        }

                        this._observableMediaPlayer.notify("width", value, oldValue);
                    }
                }
            },

            /// <field name="height" type="String">Gets or sets the height of the host element.</field>
            height: {
                get: function () {
                    return this._element.style.height;
                },
                set: function (value) {
                    var oldValue = this._element.style.height;
                    if (oldValue !== value) {
                        if (typeof value === "number") {
                            this._element.style.height = value + "px";
                        } else {
                            this._element.style.height = value;
                        }

                        this._observableMediaPlayer.notify("height", value, oldValue);
                    }
                }
            },

            /// <field name="videoWidth" type="Number">Gets the intrinsic width of the current video (in pixels).</field>
            videoWidth: {
                get: function () {
                    return this._mediaElement.videoWidth;
                }
            },

            /// <field name="videoHeight" type="Number">Gets the intrinsic height of the current video (in pixels).</field>
            videoHeight: {
                get: function () {
                    return this._mediaElement.videoHeight;
                }
            },

            /// <field name="loop" type="Boolean">Gets or sets a value that specifies whether playback should be restarted after it ends.</field>
            loop: {
                get: function () {
                    return this._mediaElement.loop;
                },
                set: function (value) {
                    var oldValue = this._mediaElement.loop;
                    if (oldValue !== value) {
                        this._mediaElement.loop = value;
                        this._observableMediaPlayer.notify("loop", value, oldValue);
                    }
                }
            },

            /// <field name="poster" type="String">Gets or sets the URL of an image to display while the current media source is loading.</field>
            poster: {
                get: function () {
                    return this._mediaElement.poster;
                },
                set: function (value) {
                    var oldValue = this._mediaElement.poster;
                    if (oldValue !== value) {
                        this._mediaElement.poster = value;
                        this._observableMediaPlayer.notify("poster", value, oldValue);
                    }
                }
            },

            /// <field name="preload" type="String">Gets or sets a hint to how much buffering is advisable for the current media source.</field>
            preload: {
                get: function () {
                    return this._mediaElement.preload;
                },
                set: function (value) {
                    var oldValue = this._mediaElement.preload;
                    if (oldValue !== value) {
                        this._mediaElement.preload = value;
                        this._observableMediaPlayer.notify("preload", value, oldValue);
                    }
                }
            },

            /// <field name="isPlayPauseAllowed" type="Boolean">Gets a value that specifies whether interaction with the play/pause control is allowed based on the current state of the player.</field>
            isPlayPauseAllowed: {
                get: function () {
                    return this.advertisingState !== PlayerFramework.AdvertisingState.loading && this.advertisingState !== PlayerFramework.AdvertisingState.linear && (this.playerState === PlayerFramework.PlayerState.opened || this.playerState === PlayerFramework.PlayerState.started || this.playerState === PlayerFramework.PlayerState.ended);
                }
            },

            /// <field name="isPlayPauseEnabled" type="Boolean">Gets or sets a value that specifies whether the play/pause control is enabled.</field>
            isPlayPauseEnabled: {
                get: function () {
                    return this._isPlayPauseEnabled;
                },
                set: function (value) {
                    var oldValue = this._isPlayPauseEnabled;
                    if (oldValue !== value) {
                        this._isPlayPauseEnabled = value;
                        this._observableMediaPlayer.notify("isPlayPauseEnabled", value, oldValue);
                    }
                }
            },

            /// <field name="isPlayPauseVisible" type="Boolean">Gets or sets a value that specifies whether the play/pause control is visible.</field>
            isPlayPauseVisible: {
                get: function () {
                    return this._isPlayPauseVisible;
                },
                set: function (value) {
                    var oldValue = this._isPlayPauseVisible;
                    if (oldValue !== value) {
                        this._isPlayPauseVisible = value;
                        this._observableMediaPlayer.notify("isPlayPauseVisible", value, oldValue);
                    }
                }
            },

            /// <field name="isPlayResumeAllowed" type="Boolean">Gets a value that specifies whether interaction with the play/resume control is allowed based on the current state of the player.</field>
            isPlayResumeAllowed: {
                get: function () {
                    return this.advertisingState !== PlayerFramework.AdvertisingState.loading && this.advertisingState !== PlayerFramework.AdvertisingState.linear && (this.playerState === PlayerFramework.PlayerState.opened || this.playerState === PlayerFramework.PlayerState.started || this.playerState === PlayerFramework.PlayerState.ended) && (this.paused || this.ended || (this.playbackRate !== this.defaultPlaybackRate && this.playbackRate !== 0));
                }
            },

            /// <field name="isPlayResumeEnabled" type="Boolean">Gets or sets a value that specifies whether the play/resume control is enabled.</field>
            isPlayResumeEnabled: {
                get: function () {
                    return this._isPlayResumeEnabled;
                },
                set: function (value) {
                    var oldValue = this._isPlayResumeEnabled;
                    if (oldValue !== value) {
                        this._isPlayResumeEnabled = value;
                        this._observableMediaPlayer.notify("isPlayResumeEnabled", value, oldValue);
                    }
                }
            },

            /// <field name="isPlayResumeVisible" type="Boolean">Gets or sets a value that specifies whether the play/resume control is visible.</field>
            isPlayResumeVisible: {
                get: function () {
                    return this._isPlayResumeVisible;
                },
                set: function (value) {
                    var oldValue = this._isPlayResumeVisible;
                    if (oldValue !== value) {
                        this._isPlayResumeVisible = value;
                        this._observableMediaPlayer.notify("isPlayResumeVisible", value, oldValue);
                    }
                }
            },

            /// <field name="isPauseAllowed" type="Boolean">Gets a value that specifies whether interaction with the pause control is allowed based on the current state of the player.</field>
            isPauseAllowed: {
                get: function () {
                    return this.advertisingState !== PlayerFramework.AdvertisingState.loading && this.advertisingState !== PlayerFramework.AdvertisingState.linear && (this.playerState === PlayerFramework.PlayerState.opened || this.playerState === PlayerFramework.PlayerState.started || this.playerState === PlayerFramework.PlayerState.ended) && !this.paused;
                }
            },

            /// <field name="isPauseEnabled" type="Boolean">Gets or sets a value that specifies whether the pause control is enabled.</field>
            isPauseEnabled: {
                get: function () {
                    return this._isPauseEnabled;
                },
                set: function (value) {
                    var oldValue = this._isPauseEnabled;
                    if (oldValue !== value) {
                        this._isPauseEnabled = value;
                        this._observableMediaPlayer.notify("isPauseEnabled", value, oldValue);
                    }
                }
            },

            /// <field name="isPauseVisible" type="Boolean">Gets or sets a value that specifies whether the pause control is visible.</field>
            isPauseVisible: {
                get: function () {
                    return this._isPauseVisible;
                },
                set: function (value) {
                    var oldValue = this._isPauseVisible;
                    if (oldValue !== value) {
                        this._isPauseVisible = value;
                        this._observableMediaPlayer.notify("isPauseVisible", value, oldValue);
                    }
                }
            },

            /// <field name="replayOffset" type="Number">Gets or sets the amount of time (in seconds) to offset the current playback position during replay.</field>
            replayOffset: {
                get: function () {
                    return this._replayOffset;
                },
                set: function (value) {
                    var oldValue = this._replayOffset;
                    if (oldValue !== value) {
                        this._replayOffset = value;
                        this._observableMediaPlayer.notify("replayOffset", value, oldValue);
                    }
                }
            },

            /// <field name="isReplayAllowed" type="Boolean">Gets a value that specifies whether interaction with the replay control is allowed based on the current state of the player.</field>
            isReplayAllowed: {
                get: function () {
                    return this.advertisingState !== PlayerFramework.AdvertisingState.loading && this.advertisingState !== PlayerFramework.AdvertisingState.linear && (this.playerState === PlayerFramework.PlayerState.opened || this.playerState === PlayerFramework.PlayerState.started || this.playerState === PlayerFramework.PlayerState.ended);
                }
            },

            /// <field name="isReplayEnabled" type="Boolean">Gets or sets a value that specifies whether the replay control is enabled.</field>
            isReplayEnabled: {
                get: function () {
                    return this._isReplayEnabled;
                },
                set: function (value) {
                    var oldValue = this._isReplayEnabled;
                    if (oldValue !== value) {
                        this._isReplayEnabled = value;
                        this._observableMediaPlayer.notify("isReplayEnabled", value, oldValue);
                    }
                }
            },

            /// <field name="isReplayVisible" type="Boolean">Gets or sets a value that specifies whether the replay control is visible.</field>
            isReplayVisible: {
                get: function () {
                    return this._isReplayVisible;
                },
                set: function (value) {
                    var oldValue = this._isReplayVisible;
                    if (oldValue !== value) {
                        this._isReplayVisible = value;
                        this._observableMediaPlayer.notify("isReplayVisible", value, oldValue);
                    }
                }
            },

            /// <field name="isRewindAllowed" type="Boolean">Gets a value that specifies whether interaction with the rewind control is allowed based on the current state of the player.</field>
            isRewindAllowed: {
                get: function () {
                    return this.advertisingState !== PlayerFramework.AdvertisingState.loading && this.advertisingState !== PlayerFramework.AdvertisingState.linear && (this.playerState === PlayerFramework.PlayerState.opened || this.playerState === PlayerFramework.PlayerState.started || this.playerState === PlayerFramework.PlayerState.ended) && !this.paused && !this.ended && this.playbackRate > -8;
                }
            },

            /// <field name="isRewindEnabled" type="Boolean">Gets or sets a value that specifies whether the rewind control is enabled.</field>
            isRewindEnabled: {
                get: function () {
                    return this._isRewindEnabled;
                },
                set: function (value) {
                    var oldValue = this._isRewindEnabled;
                    if (oldValue !== value) {
                        this._isRewindEnabled = value;
                        this._observableMediaPlayer.notify("isRewindEnabled", value, oldValue);
                    }
                }
            },

            /// <field name="isRewindVisible" type="Boolean">Gets or sets a value that specifies whether the rewind control is visible.</field>
            isRewindVisible: {
                get: function () {
                    return this._isRewindVisible;
                },
                set: function (value) {
                    var oldValue = this._isRewindVisible;
                    if (oldValue !== value) {
                        this._isRewindVisible = value;
                        this._observableMediaPlayer.notify("isRewindVisible", value, oldValue);
                    }
                }
            },

            /// <field name="isFastForwardAllowed" type="Boolean">Gets a value that specifies whether interaction with the fast forward control is allowed based on the current state of the player.</field>
            isFastForwardAllowed: {
                get: function () {
                    return this.advertisingState !== PlayerFramework.AdvertisingState.loading && this.advertisingState !== PlayerFramework.AdvertisingState.linear && (this.playerState === PlayerFramework.PlayerState.opened || this.playerState === PlayerFramework.PlayerState.started || this.playerState === PlayerFramework.PlayerState.ended) && !this.paused && !this.ended && this.playbackRate < 8;
                }
            },

            /// <field name="isFastForwardEnabled" type="Boolean">Gets or sets a value that specifies whether the fast forward control is enabled.</field>
            isFastForwardEnabled: {
                get: function () {
                    return this._isFastForwardEnabled;
                },
                set: function (value) {
                    var oldValue = this._isFastForwardEnabled;
                    if (oldValue !== value) {
                        this._isFastForwardEnabled = value;
                        this._observableMediaPlayer.notify("isFastForwardEnabled", value, oldValue);
                    }
                }
            },

            /// <field name="isFastForwardVisible" type="Boolean">Gets or sets a value that specifies whether the fast forward control is visible.</field>
            isFastForwardVisible: {
                get: function () {
                    return this._isFastForwardVisible;
                },
                set: function (value) {
                    var oldValue = this._isFastForwardVisible;
                    if (oldValue !== value) {
                        this._isFastForwardVisible = value;
                        this._observableMediaPlayer.notify("isFastForwardVisible", value, oldValue);
                    }
                }
            },

            /// <field name="isSlowMotion" type="Boolean">Gets or sets a value that specifies whether the player is playing in slow motion.</field>
            isSlowMotion: {
                get: function () {
                    return this.playbackRate === this.slowMotionPlaybackRate;
                },
                set: function (value) {
                    var oldValue = this.isSlowMotion;
                    if (oldValue !== value) {
                        if (value) {
                            this.playbackRate = this.slowMotionPlaybackRate;
                        } else {
                            this.playbackRate = this.defaultPlaybackRate;
                        }

                        this._observableMediaPlayer.notify("isSlowMotion", value, oldValue);
                    }
                }
            },

            /// <field name="isSlowMotionAllowed" type="Boolean">Gets a value that specifies whether interaction with the slow motion control is allowed based on the current state of the player.</field>
            isSlowMotionAllowed: {
                get: function () {
                    return this.advertisingState !== PlayerFramework.AdvertisingState.loading && this.advertisingState !== PlayerFramework.AdvertisingState.linear && (this.playerState === PlayerFramework.PlayerState.opened || this.playerState === PlayerFramework.PlayerState.started || this.playerState === PlayerFramework.PlayerState.ended) && !this.paused && !this.ended && this.playbackRate !== this.slowMotionPlaybackRate;
                }
            },

            /// <field name="isSlowMotionEnabled" type="Boolean">Gets or sets a value that specifies whether the slow motion control is enabled.</field>
            isSlowMotionEnabled: {
                get: function () {
                    return this._isSlowMotionEnabled;
                },
                set: function (value) {
                    var oldValue = this._isSlowMotionEnabled;
                    if (oldValue !== value) {
                        this._isSlowMotionEnabled = value;
                        this._observableMediaPlayer.notify("isSlowMotionEnabled", value, oldValue);
                    }
                }
            },

            /// <field name="isSlowMotionVisible" type="Boolean">Gets or sets a value that specifies whether the slow motion control is visible.</field>
            isSlowMotionVisible: {
                get: function () {
                    return this._isSlowMotionVisible;
                },
                set: function (value) {
                    var oldValue = this._isSlowMotionVisible;
                    if (oldValue !== value) {
                        this._isSlowMotionVisible = value;
                        this._observableMediaPlayer.notify("isSlowMotionVisible", value, oldValue);
                    }
                }
            },

            /// <field name="isSkipPreviousAllowed" type="Boolean">Gets a value that specifies whether interaction with the skip previous control is allowed based on the current state of the player.</field>
            isSkipPreviousAllowed: {
                get: function () {
                    return this.advertisingState !== PlayerFramework.AdvertisingState.loading && this.advertisingState !== PlayerFramework.AdvertisingState.linear && (this.playerState === PlayerFramework.PlayerState.opened || this.playerState === PlayerFramework.PlayerState.started || this.playerState === PlayerFramework.PlayerState.ended);
                }
            },

            /// <field name="isSkipPreviousEnabled" type="Boolean">Gets or sets a value that specifies whether the skip previous control is enabled.</field>
            isSkipPreviousEnabled: {
                get: function () {
                    return this._isSkipPreviousEnabled;
                },
                set: function (value) {
                    var oldValue = this._isSkipPreviousEnabled;
                    if (oldValue !== value) {
                        this._isSkipPreviousEnabled = value;
                        this._observableMediaPlayer.notify("isSkipPreviousEnabled", value, oldValue);
                    }
                }
            },

            /// <field name="isSkipPreviousVisible" type="Boolean">Gets or sets a value that specifies whether the skip previous control is visible.</field>
            isSkipPreviousVisible: {
                get: function () {
                    return this._isSkipPreviousVisible;
                },
                set: function (value) {
                    var oldValue = this._isSkipPreviousVisible;
                    if (oldValue !== value) {
                        this._isSkipPreviousVisible = value;
                        this._observableMediaPlayer.notify("isSkipPreviousVisible", value, oldValue);
                    }
                }
            },

            /// <field name="isSkipNextAllowed" type="Boolean">Gets a value that specifies whether interaction with the skip next control is allowed based on the current state of the player.</field>
            isSkipNextAllowed: {
                get: function () {
                    return this.advertisingState !== PlayerFramework.AdvertisingState.loading && this.advertisingState !== PlayerFramework.AdvertisingState.linear && (this.playerState === PlayerFramework.PlayerState.opened || this.playerState === PlayerFramework.PlayerState.started || this.playerState === PlayerFramework.PlayerState.ended);
                }
            },

            /// <field name="isSkipNextEnabled" type="Boolean">Gets or sets a value that specifies whether the skip next control is enabled.</field>
            isSkipNextEnabled: {
                get: function () {
                    return this._isSkipNextEnabled;
                },
                set: function (value) {
                    var oldValue = this._isSkipNextEnabled;
                    if (oldValue !== value) {
                        this._isSkipNextEnabled = value;
                        this._observableMediaPlayer.notify("isSkipNextEnabled", value, oldValue);
                    }
                }
            },

            /// <field name="isSkipNextVisible" type="Boolean">Gets or sets a value that specifies whether the skip next control is visible.</field>
            isSkipNextVisible: {
                get: function () {
                    return this._isSkipNextVisible;
                },
                set: function (value) {
                    var oldValue = this._isSkipNextVisible;
                    if (oldValue !== value) {
                        this._isSkipNextVisible = value;
                        this._observableMediaPlayer.notify("isSkipNextVisible", value, oldValue);
                    }
                }
            },

            /// <field name="skipBackInterval" type="Number">Gets or sets the amount of time (in seconds) that the skip back control will seek backward.</field>
            skipBackInterval: {
                get: function () {
                    return this._skipBackInterval;
                },
                set: function (value) {
                    var oldValue = this._skipBackInterval;
                    if (oldValue !== value) {
                        this._skipBackInterval = value;
                        this._observableMediaPlayer.notify("skipBackInterval", value, oldValue);
                    }
                }
            },

            /// <field name="isSkipBackAllowed" type="Boolean">Gets a value that specifies whether interaction with the skip back control is allowed based on the current state of the player.</field>
            isSkipBackAllowed: {
                get: function () {
                    return this.advertisingState !== PlayerFramework.AdvertisingState.loading && this.advertisingState !== PlayerFramework.AdvertisingState.linear && (this.playerState === PlayerFramework.PlayerState.opened || this.playerState === PlayerFramework.PlayerState.started || this.playerState === PlayerFramework.PlayerState.ended);
                }
            },

            /// <field name="isSkipBackEnabled" type="Boolean">Gets or sets a value that specifies whether the skip back control is enabled.</field>
            isSkipBackEnabled: {
                get: function () {
                    return this._isSkipBackEnabled;
                },
                set: function (value) {
                    var oldValue = this._isSkipBackEnabled;
                    if (oldValue !== value) {
                        this._isSkipBackEnabled = value;
                        this._observableMediaPlayer.notify("isSkipBackEnabled", value, oldValue);
                    }
                }
            },

            /// <field name="isSkipBackVisible" type="Boolean">Gets or sets a value that specifies whether the skip back control is visible.</field>
            isSkipBackVisible: {
                get: function () {
                    return this._isSkipBackVisible;
                },
                set: function (value) {
                    var oldValue = this._isSkipBackVisible;
                    if (oldValue !== value) {
                        this._isSkipBackVisible = value;
                        this._observableMediaPlayer.notify("isSkipBackVisible", value, oldValue);
                    }
                }
            },

            /// <field name="skipAheadInterval" type="Number">Gets or sets the amount of time (in seconds) that the skip ahead control will seek forward.</field>
            skipAheadInterval: {
                get: function () {
                    return this._skipAheadInterval;
                },
                set: function (value) {
                    var oldValue = this._skipAheadInterval;
                    if (oldValue !== value) {
                        this._skipAheadInterval = value;
                        this._observableMediaPlayer.notify("skipAheadInterval", value, oldValue);
                    }
                }
            },

            /// <field name="isSkipAheadAllowed" type="Boolean">Gets a value that specifies whether interaction with the skip ahead control is allowed based on the current state of the player.</field>
            isSkipAheadAllowed: {
                get: function () {
                    return this.advertisingState !== PlayerFramework.AdvertisingState.loading && this.advertisingState !== PlayerFramework.AdvertisingState.linear && (this.playerState === PlayerFramework.PlayerState.opened || this.playerState === PlayerFramework.PlayerState.started || this.playerState === PlayerFramework.PlayerState.ended);
                }
            },

            /// <field name="isSkipAheadEnabled" type="Boolean">Gets or sets a value that specifies whether the skip ahead control is enabled.</field>
            isSkipAheadEnabled: {
                get: function () {
                    return this._isSkipAheadEnabled;
                },
                set: function (value) {
                    var oldValue = this._isSkipAheadEnabled;
                    if (oldValue !== value) {
                        this._isSkipAheadEnabled = value;
                        this._observableMediaPlayer.notify("isSkipAheadEnabled", value, oldValue);
                    }
                }
            },

            /// <field name="isSkipAheadVisible" type="Boolean">Gets or sets a value that specifies whether the skip ahead control is visible.</field>
            isSkipAheadVisible: {
                get: function () {
                    return this._isSkipAheadVisible;
                },
                set: function (value) {
                    var oldValue = this._isSkipAheadVisible;
                    if (oldValue !== value) {
                        this._isSkipAheadVisible = value;
                        this._observableMediaPlayer.notify("isSkipAheadVisible", value, oldValue);
                    }
                }
            },

            /// <field name="isElapsedTimeAllowed" type="Boolean">Gets a value that specifies whether interaction with the elapsed time control is allowed based on the current state of the player.</field>
            isElapsedTimeAllowed: {
                get: function () {
                    return this.advertisingState !== PlayerFramework.AdvertisingState.loading && this.advertisingState !== PlayerFramework.AdvertisingState.linear && (this.playerState === PlayerFramework.PlayerState.opened || this.playerState === PlayerFramework.PlayerState.started || this.playerState === PlayerFramework.PlayerState.ended);
                }
            },

            /// <field name="isElapsedTimeEnabled" type="Boolean">Gets or sets a value that specifies whether the elapsed time control is enabled.</field>
            isElapsedTimeEnabled: {
                get: function () {
                    return this._isElapsedTimeEnabled;
                },
                set: function (value) {
                    var oldValue = this._isElapsedTimeEnabled;
                    if (oldValue !== value) {
                        this._isElapsedTimeEnabled = value;
                        this._observableMediaPlayer.notify("isElapsedTimeEnabled", value, oldValue);
                    }
                }
            },

            /// <field name="isElapsedTimeVisible" type="Boolean">Gets or sets a value that specifies whether the elapsed time control is visible.</field>
            isElapsedTimeVisible: {
                get: function () {
                    return this._isElapsedTimeVisible;
                },
                set: function (value) {
                    var oldValue = this._isElapsedTimeVisible;
                    if (oldValue !== value) {
                        this._isElapsedTimeVisible = value;
                        this._observableMediaPlayer.notify("isElapsedTimeVisible", value, oldValue);
                    }
                }
            },

            /// <field name="isRemainingTimeAllowed" type="Boolean">Gets a value that specifies whether interaction with the remaining time control is allowed based on the current state of the player.</field>
            isRemainingTimeAllowed: {
                get: function () {
                    return this.advertisingState !== PlayerFramework.AdvertisingState.loading && this.advertisingState !== PlayerFramework.AdvertisingState.linear && (this.playerState === PlayerFramework.PlayerState.opened || this.playerState === PlayerFramework.PlayerState.started || this.playerState === PlayerFramework.PlayerState.ended);
                }
            },

            /// <field name="isRemainingTimeEnabled" type="Boolean">Gets or sets a value that specifies whether the remaining time control is enabled.</field>
            isRemainingTimeEnabled: {
                get: function () {
                    return this._isRemainingTimeEnabled;
                },
                set: function (value) {
                    var oldValue = this._isRemainingTimeEnabled;
                    if (oldValue !== value) {
                        this._isRemainingTimeEnabled = value;
                        this._observableMediaPlayer.notify("isRemainingTimeEnabled", value, oldValue);
                    }
                }
            },

            /// <field name="isRemainingTimeVisible" type="Boolean">Gets or sets a value that specifies whether the remaining time control is visible.</field>
            isRemainingTimeVisible: {
                get: function () {
                    return this._isRemainingTimeVisible;
                },
                set: function (value) {
                    var oldValue = this._isRemainingTimeVisible;
                    if (oldValue !== value) {
                        this._isRemainingTimeVisible = value;
                        this._observableMediaPlayer.notify("isRemainingTimeVisible", value, oldValue);
                    }
                }
            },

            /// <field name="isTotalTimeAllowed" type="Boolean">Gets a value that specifies whether interaction with the total time control is allowed based on the current state of the player.</field>
            isTotalTimeAllowed: {
                get: function () {
                    return this.advertisingState !== PlayerFramework.AdvertisingState.loading && this.advertisingState !== PlayerFramework.AdvertisingState.linear && (this.playerState === PlayerFramework.PlayerState.opened || this.playerState === PlayerFramework.PlayerState.started || this.playerState === PlayerFramework.PlayerState.ended);
                }
            },

            /// <field name="isTotalTimeEnabled" type="Boolean">Gets or sets a value that specifies whether the total time control is enabled.</field>
            isTotalTimeEnabled: {
                get: function () {
                    return this._isTotalTimeEnabled;
                },
                set: function (value) {
                    var oldValue = this._isTotalTimeEnabled;
                    if (oldValue !== value) {
                        this._isTotalTimeEnabled = value;
                        this._observableMediaPlayer.notify("isTotalTimeEnabled", value, oldValue);
                    }
                }
            },

            /// <field name="isTotalTimeVisible" type="Boolean">Gets or sets a value that specifies whether the total time control is visible.</field>
            isTotalTimeVisible: {
                get: function () {
                    return this._isTotalTimeVisible;
                },
                set: function (value) {
                    var oldValue = this._isTotalTimeVisible;
                    if (oldValue !== value) {
                        this._isTotalTimeVisible = value;
                        this._observableMediaPlayer.notify("isTotalTimeVisible", value, oldValue);
                    }
                }
            },

            /// <field name="isTimelineAllowed" type="Boolean">Gets a value that specifies whether interaction with the timeline control is allowed based on the current state of the player.</field>
            isTimelineAllowed: {
                get: function () {
                    return this.advertisingState !== PlayerFramework.AdvertisingState.loading && this.advertisingState !== PlayerFramework.AdvertisingState.linear && (this.playerState === PlayerFramework.PlayerState.opened || this.playerState === PlayerFramework.PlayerState.started || this.playerState === PlayerFramework.PlayerState.ended);
                }
            },

            /// <field name="isTimelineEnabled" type="Boolean">Gets or sets a value that specifies whether the timeline control is enabled.</field>
            isTimelineEnabled: {
                get: function () {
                    return this._isTimelineEnabled;
                },
                set: function (value) {
                    var oldValue = this._isTimelineEnabled;
                    if (oldValue !== value) {
                        this._isTimelineEnabled = value;
                        this._observableMediaPlayer.notify("isTimelineEnabled", value, oldValue);
                    }
                }
            },

            /// <field name="isTimelineVisible" type="Boolean">Gets or sets a value that specifies whether the timeline control is visible.</field>
            isTimelineVisible: {
                get: function () {
                    return this._isTimelineVisible;
                },
                set: function (value) {
                    var oldValue = this._isTimelineVisible;
                    if (oldValue !== value) {
                        this._isTimelineVisible = value;
                        this._observableMediaPlayer.notify("isTimelineVisible", value, oldValue);
                    }
                }
            },

            /// <field name="isGoLiveAllowed" type="Boolean">Gets a value that specifies whether interaction with the go live control is allowed based on the current state of the player.</field>
            isGoLiveAllowed: {
                get: function () {
                    return this.advertisingState !== PlayerFramework.AdvertisingState.loading && this.advertisingState !== PlayerFramework.AdvertisingState.linear && (this.playerState === PlayerFramework.PlayerState.opened || this.playerState === PlayerFramework.PlayerState.started || this.playerState === PlayerFramework.PlayerState.ended) && this.isLive && !this.isCurrentTimeLive;
                }
            },

            /// <field name="isGoLiveEnabled" type="Boolean">Gets or sets a value that specifies whether the go live control is enabled.</field>
            isGoLiveEnabled: {
                get: function () {
                    return this._isGoLiveEnabled;
                },
                set: function (value) {
                    var oldValue = this._isGoLiveEnabled;
                    if (oldValue !== value) {
                        this._isGoLiveEnabled = value;
                        this._observableMediaPlayer.notify("isGoLiveEnabled", value, oldValue);
                    }
                }
            },

            /// <field name="isGoLiveVisible" type="Boolean">Gets or sets a value that specifies whether the go live control is visible.</field>
            isGoLiveVisible: {
                get: function () {
                    return this._isGoLiveVisible;
                },
                set: function (value) {
                    var oldValue = this._isGoLiveVisible;
                    if (oldValue !== value) {
                        this._isGoLiveVisible = value;
                        this._observableMediaPlayer.notify("isGoLiveVisible", value, oldValue);
                    }
                }
            },

            /// <field name="isCaptionsAllowed" type="Boolean">Gets a value that specifies whether interaction with the captions control is allowed based on the current state of the player.</field>
            isCaptionsAllowed: {
                get: function () {
                    return this.advertisingState !== PlayerFramework.AdvertisingState.loading && this.advertisingState !== PlayerFramework.AdvertisingState.linear && (this.playerState === PlayerFramework.PlayerState.opened || this.playerState === PlayerFramework.PlayerState.started || this.playerState === PlayerFramework.PlayerState.ended) && this.captionTracks.length > 0;
                }
            },

            /// <field name="isCaptionsEnabled" type="Boolean">Gets or sets a value that specifies whether the captions control is enabled.</field>
            isCaptionsEnabled: {
                get: function () {
                    return this._isCaptionsEnabled;
                },
                set: function (value) {
                    var oldValue = this._isCaptionsEnabled;
                    if (oldValue !== value) {
                        this._isCaptionsEnabled = value;
                        this._observableMediaPlayer.notify("isCaptionsEnabled", value, oldValue);
                    }
                }
            },

            /// <field name="isCaptionsVisible" type="Boolean">Gets or sets a value that specifies whether the captions control is visible.</field>
            isCaptionsVisible: {
                get: function () {
                    return this._isCaptionsVisible;
                },
                set: function (value) {
                    var oldValue = this._isCaptionsVisible;
                    if (oldValue !== value) {
                        this._isCaptionsVisible = value;
                        this._observableMediaPlayer.notify("isCaptionsVisible", value, oldValue);
                    }
                }
            },

            /// <field name="isAudioAllowed" type="Boolean">Gets a value that specifies whether interaction with the audio control is allowed based on the current state of the player.</field>
            isAudioAllowed: {
                get: function () {
                    return this.advertisingState !== PlayerFramework.AdvertisingState.loading && this.advertisingState !== PlayerFramework.AdvertisingState.linear && (this.playerState === PlayerFramework.PlayerState.opened || this.playerState === PlayerFramework.PlayerState.started || this.playerState === PlayerFramework.PlayerState.ended) && this.audioTracks.length > 0;
                }
            },

            /// <field name="isAudioEnabled" type="Boolean">Gets or sets a value that specifies whether the audio control is enabled.</field>
            isAudioEnabled: {
                get: function () {
                    return this._isAudioEnabled;
                },
                set: function (value) {
                    var oldValue = this._isAudioEnabled;
                    if (oldValue !== value) {
                        this._isAudioEnabled = value;
                        this._observableMediaPlayer.notify("isAudioEnabled", value, oldValue);
                    }
                }
            },

            /// <field name="isAudioVisible" type="Boolean">Gets or sets a value that specifies whether the audio control is visible.</field>
            isAudioVisible: {
                get: function () {
                    return this._isAudioVisible;
                },
                set: function (value) {
                    var oldValue = this._isAudioVisible;
                    if (oldValue !== value) {
                        this._isAudioVisible = value;
                        this._observableMediaPlayer.notify("isAudioVisible", value, oldValue);
                    }
                }
            },

            /// <field name="volume" type="Number">Gets or sets the volume level (from 0 to 1) for the audio portions of media playback.</field>
            volume: {
                get: function () {
                    return this._mediaElement.volume;
                },
                set: function (value) {
                    if (isFinite(value) && !isNaN(value) && value >= 0 && value <= 1) {
                        // note: the volumechange event should notify the observable property for us
                        this._mediaElement.volume = value;
                        if (this._mediaElement.readyState === PlayerFramework.ReadyState.nothing) {
                            // firing the volume changed event will notify others that the volume has changed in the case that no mediaelement exists. This is useful for ads.
                            this._onMediaElementVolumeChange();
                        }
                    }
                }
            },

            /// <field name="isVolumeMuteAllowed" type="Boolean">Gets a value that specifies whether interaction with the volume/mute control is allowed based on the current state of the player.</field>
            isVolumeMuteAllowed: {
                get: function () {
                    return true;
                }
            },

            /// <field name="isVolumeMuteEnabled" type="Boolean">Gets or sets a value that specifies whether the volume/mute control is enabled.</field>
            isVolumeMuteEnabled: {
                get: function () {
                    return this._isVolumeMuteEnabled;
                },
                set: function (value) {
                    var oldValue = this._isVolumeMuteEnabled;
                    if (oldValue !== value) {
                        this._isVolumeMuteEnabled = value;
                        this._observableMediaPlayer.notify("isVolumeMuteEnabled", value, oldValue);
                    }
                }
            },

            /// <field name="isVolumeMuteVisible" type="Boolean">Gets or sets a value that specifies whether the volume/mute control is visible.</field>
            isVolumeMuteVisible: {
                get: function () {
                    return this._isVolumeMuteVisible;
                },
                set: function (value) {
                    var oldValue = this._isVolumeMuteVisible;
                    if (oldValue !== value) {
                        this._isVolumeMuteVisible = value;
                        this._observableMediaPlayer.notify("isVolumeMuteVisible", value, oldValue);
                    }
                }
            },

            /// <field name="isVolumeAllowed" type="Boolean">Gets a value that specifies whether interaction with the volume control is allowed based on the current state of the player.</field>
            isVolumeAllowed: {
                get: function () {
                    return true;
                }
            },

            /// <field name="isVolumeEnabled" type="Boolean">Gets or sets a value that specifies whether the volume control is enabled.</field>
            isVolumeEnabled: {
                get: function () {
                    return this._isVolumeEnabled;
                },
                set: function (value) {
                    var oldValue = this._isVolumeEnabled;
                    if (oldValue !== value) {
                        this._isVolumeEnabled = value;
                        this._observableMediaPlayer.notify("isVolumeEnabled", value, oldValue);
                    }
                }
            },

            /// <field name="isVolumeVisible" type="Boolean">Gets or sets a value that specifies whether the volume control is visible.</field>
            isVolumeVisible: {
                get: function () {
                    return this._isVolumeVisible;
                },
                set: function (value) {
                    var oldValue = this._isVolumeVisible;
                    if (oldValue !== value) {
                        this._isVolumeVisible = value;
                        this._observableMediaPlayer.notify("isVolumeVisible", value, oldValue);
                    }
                }
            },

            /// <field name="muted" type="Boolean">Gets or sets a value that indicates whether the audio is muted.</field>
            muted: {
                get: function () {
                    return this._mediaElement.muted;
                },
                set: function (value) {
                    var oldValue = this._mediaElement.muted;
                    if (oldValue !== value) {
                        this._mediaElement.muted = value;
                        this._observableMediaPlayer.notify("muted", value, oldValue);
                        this.dispatchEvent("mutedchange");
                    }
                }
            },

            /// <field name="isMuteAllowed" type="Boolean">Gets a value that specifies whether interaction with the mute control is allowed based on the current state of the player.</field>
            isMuteAllowed: {
                get: function () {
                    return true;
                }
            },

            /// <field name="isMuteEnabled" type="Boolean">Gets or sets a value that specifies whether the mute control is enabled.</field>
            isMuteEnabled: {
                get: function () {
                    return this._isMuteEnabled;
                },
                set: function (value) {
                    var oldValue = this._isMuteEnabled;
                    if (oldValue !== value) {
                        this._isMuteEnabled = value;
                        this._observableMediaPlayer.notify("isMuteEnabled", value, oldValue);
                    }
                }
            },

            /// <field name="isMuteVisible" type="Boolean">Gets or sets a value that specifies whether the mute control is visible.</field>
            isMuteVisible: {
                get: function () {
                    return this._isMuteVisible;
                },
                set: function (value) {
                    var oldValue = this._isMuteVisible;
                    if (oldValue !== value) {
                        this._isMuteVisible = value;
                        this._observableMediaPlayer.notify("isMuteVisible", value, oldValue);
                    }
                }
            },

            /// <field name="isFullScreen" type="Boolean">Gets or sets a value that specifies whether the player is in full screen mode.</field>
            isFullScreen: {
                get: function () {
                    return this._isFullScreen;
                },
                set: function (value) {
                    var oldValue = this._isFullScreen;
                    if (oldValue !== value) {
                        if (value) {
                            this._shimElement.style.display = "block";
                            this.addClass("pf-full-screen");
                        } else {
                            this.removeClass("pf-full-screen");
                            this._shimElement.style.display = "none";
                        }

                        if (value && Windows.UI.ViewManagement.ApplicationView.value === Windows.UI.ViewManagement.ApplicationViewState.snapped)
                        {
                            Windows.UI.ViewManagement.ApplicationView.tryUnsnap();
                        }

                        this._isFullScreen = value;
                        this._observableMediaPlayer.notify("isFullScreen", value, oldValue);
                        this.dispatchEvent("fullscreenchange");
                    }
                }
            },

            /// <field name="isFullScreenAllowed" type="Boolean">Gets a value that specifies whether interaction with the full screen control is allowed based on the current state of the player.</field>
            isFullScreenAllowed: {
                get: function () {
                    return true;
                }
            },

            /// <field name="isFullScreenEnabled" type="Boolean">Gets or sets a value that specifies whether the full screen control is enabled.</field>
            isFullScreenEnabled: {
                get: function () {
                    return this._isFullScreenEnabled;
                },
                set: function (value) {
                    var oldValue = this._isFullScreenEnabled;
                    if (oldValue !== value) {
                        this._isFullScreenEnabled = value;
                        this._observableMediaPlayer.notify("isFullScreenEnabled", value, oldValue);
                    }
                }
            },

            /// <field name="isFullScreenVisible" type="Boolean">Gets or sets a value that specifies whether the full screen control is visible.</field>
            isFullScreenVisible: {
                get: function () {
                    return this._isFullScreenVisible;
                },
                set: function (value) {
                    var oldValue = this._isFullScreenVisible;
                    if (oldValue !== value) {
                        this._isFullScreenVisible = value;
                        this._observableMediaPlayer.notify("isFullScreenVisible", value, oldValue);
                    }
                }
            },

            /// <field name="isStopAllowed" type="Boolean">Gets a value that specifies whether interaction with the stop control is allowed based on the current state of the player.</field>
            isStopAllowed: {
                get: function () {
                    return this.playerState !== PlayerFramework.PlayerState.unloaded;
                }
            },

            /// <field name="isStopEnabled" type="Boolean">Gets or sets a value that specifies whether the stop control is enabled.</field>
            isStopEnabled: {
                get: function () {
                    return this._isStopEnabled;
                },
                set: function (value) {
                    var oldValue = this._isStopEnabled;
                    if (oldValue !== value) {
                        this._isStopEnabled = value;
                        this._observableMediaPlayer.notify("isStopEnabled", value, oldValue);
                    }
                }
            },

            /// <field name="isStopVisible" type="Boolean">Gets or sets a value that specifies whether the stop control is visible.</field>
            isStopVisible: {
                get: function () {
                    return this._isStopVisible;
                },
                set: function (value) {
                    var oldValue = this._isStopVisible;
                    if (oldValue !== value) {
                        this._isStopVisible = value;
                        this._observableMediaPlayer.notify("isStopVisible", value, oldValue);
                    }
                }
            },

            /// <field name="signalStrength" type="Number">Gets or sets the signal strength of the current media source. This is useful in adaptive streaming scenarios.</field>
            signalStrength: {
                get: function () {
                    return this._signalStrength;
                },
                set: function (value) {
                    var oldValue = this._signalStrength;
                    if (oldValue !== value) {
                        this._signalStrength = value;
                        this._observableMediaPlayer.notify("signalStrength", value, oldValue);
                    }
                }
            },

            /// <field name="isSignalStrengthAllowed" type="Boolean">Gets a value that specifies whether interaction with the signal strength control is allowed based on the current state of the player.</field>
            isSignalStrengthAllowed: {
                get: function () {
                    return true;
                }
            },

            /// <field name="isSignalStrengthEnabled" type="Boolean">Gets or sets a value that specifies whether the signal strength control is enabled.</field>
            isSignalStrengthEnabled: {
                get: function () {
                    return this._isSignalStrengthEnabled;
                },
                set: function (value) {
                    var oldValue = this._isSignalStrengthEnabled;
                    if (oldValue !== value) {
                        this._isSignalStrengthEnabled = value;
                        this._observableMediaPlayer.notify("isSignalStrengthEnabled", value, oldValue);
                    }
                }
            },

            /// <field name="isSignalStrengthVisible" type="Boolean">Gets or sets a value that specifies whether the signal strength control is visible.</field>
            isSignalStrengthVisible: {
                get: function () {
                    return this._isSignalStrengthVisible;
                },
                set: function (value) {
                    var oldValue = this._isSignalStrengthVisible;
                    if (oldValue !== value) {
                        this._isSignalStrengthVisible = value;
                        this._observableMediaPlayer.notify("isSignalStrengthVisible", value, oldValue);
                    }
                }
            },

            /// <field name="mediaQuality" type="PlayerFramework.MediaQuality">Gets or sets the quality of the current media source.</field>
            mediaQuality: {
                get: function () {
                    return this._mediaQuality;
                },
                set: function (value) {
                    var oldValue = this._mediaQuality;
                    if (oldValue !== value) {
                        this._mediaQuality = value;
                        this._observableMediaPlayer.notify("mediaQuality", value, oldValue);
                    }
                }
            },

            /// <field name="isMediaQualityAllowed" type="Boolean">Gets a value that specifies whether interaction with the media quality control is allowed based on the current state of the player.</field>
            isMediaQualityAllowed: {
                get: function () {
                    return true;
                }
            },

            /// <field name="isMediaQualityEnabled" type="Boolean">Gets or sets a value that specifies whether the media quality control is enabled.</field>
            isMediaQualityEnabled: {
                get: function () {
                    return this._isMediaQualityEnabled;
                },
                set: function (value) {
                    var oldValue = this._isMediaQualityEnabled;
                    if (oldValue !== value) {
                        this._isMediaQualityEnabled = value;
                        this._observableMediaPlayer.notify("isMediaQualityEnabled", value, oldValue);
                    }
                }
            },

            /// <field name="isMediaQualityVisible" type="Boolean">Gets or sets a value that specifies whether the media quality control is visible.</field>
            isMediaQualityVisible: {
                get: function () {
                    return this._isMediaQualityVisible;
                },
                set: function (value) {
                    var oldValue = this._isMediaQualityVisible;
                    if (oldValue !== value) {
                        this._isMediaQualityVisible = value;
                        this._observableMediaPlayer.notify("isMediaQualityVisible", value, oldValue);
                    }
                }
            },

            /// <field name="isInteractive" type="Boolean">Gets or sets a value that specifies whether the player is currently in interactive mode (e.g. showing the control panel).</field>
            isInteractive: {
                get: function () {
                    return this._isInteractive;
                },
                set: function (value) {
                    var oldValue = this._isInteractive;
                    if (oldValue !== value) {
                        if (value && (this.interactiveDeactivationMode & PlayerFramework.InteractionType.soft) === PlayerFramework.InteractionType.soft && this.autohide) {
                            this._resetAutohideTimeout();
                        }

                        this._isInteractive = value;
                        this._observableMediaPlayer.notify("isInteractive", value, oldValue);
                        this.dispatchEvent("interactivestatechange");
                    }
                }
            },

            /// <field name="interactiveActivationMode" type="PlayerFramework.InteractionType">Gets or sets the type of interactions that will cause interactive elements (e.g. the control panel) to be shown.</field>
            interactiveActivationMode: {
                get: function () {
                    return this._interactiveActivationMode;
                },
                set: function (value) {
                    var oldValue = this._interactiveActivationMode;
                    if (oldValue !== value) {
                        this._interactiveActivationMode = value;
                        this._observableMediaPlayer.notify("interactiveActivationMode", value, oldValue);
                    }
                }
            },

            /// <field name="interactiveDeactivationMode" type="PlayerFramework.InteractionType">Gets or sets the type of interactions that will cause interactive elements (e.g. the control panel) to be hidden.</field>
            interactiveDeactivationMode: {
                get: function () {
                    return this._interactiveDeactivationMode;
                },
                set: function (value) {
                    var oldValue = this._interactiveDeactivationMode;
                    if (oldValue !== value) {
                        if (this.isInteractive) {
                            if ((oldValue & PlayerFramework.InteractionType.soft) === PlayerFramework.InteractionType.soft && (value & PlayerFramework.InteractionType.soft) !== PlayerFramework.InteractionType.soft || !this.autohide) {
                                this._clearAutohideTimeout();
                            } else if ((oldValue & PlayerFramework.InteractionType.soft) !== PlayerFramework.InteractionType.soft && (value & PlayerFramework.InteractionType.soft) === PlayerFramework.InteractionType.soft && this.autohide) {
                                this._resetAutohideTimeout();
                            }
                        }

                        this._interactiveDeactivationMode = value;
                        this._observableMediaPlayer.notify("interactiveDeactivationMode", value, oldValue);
                    }
                }
            },

            /// <field name="defaultInteractiveViewModel" type="PlayerFramework.InteractiveViewModel">Gets the view model that will be restored following a temporary change to the current interactive view model (e.g. during an ad).</field>
            defaultInteractiveViewModel: {
                get: function () {
                    return this._defaultInteractiveViewModel;
                }
            },

            /// <field name="interactiveViewModel" type="PlayerFramework.InteractiveViewModel">Gets or sets the view model that interactive elements are bound to (e.g. the control panel).</field>
            interactiveViewModel: {
                get: function () {
                    return this._interactiveViewModel;
                },
                set: function (value) {
                    var oldValue = this._interactiveViewModel;
                    if (oldValue !== value) {
                        if (oldValue) {
                            oldValue.uninitialize();
                        }

                        if (value) {
                            value.initialize();
                        }

                        this._interactiveViewModel = value;
                        this._observableMediaPlayer.notify("interactiveViewModel", value, oldValue);
                        this.dispatchEvent("interactiveviewmodelchange");
                    }
                }
            },

            /// <field name="msIsLayoutOptimalForPlayback" type="Boolean">Gets a value that specifies whether the media can be rendered more efficiently.</field>
            msIsLayoutOptimalForPlayback: {
                get: function () {
                    return this._mediaElement.msIsLayoutOptimalForPlayback;
                }
            },

            /// <field name="msAudioCategory" type="String">Gets or sets a value that specifies the purpose of the media, such as background audio or alerts.</field>
            msAudioCategory: {
                get: function () {
                    return this._mediaElement.msAudioCategory;
                },
                set: function (value) {
                    var oldValue = this._mediaElement.msAudioCategory;
                    if (oldValue !== value) {
                        this._mediaElement.msAudioCategory = value;
                        this._observableMediaPlayer.notify("msAudioCategory", value, oldValue);
                    }
                }
            },

            /// <field name="msAudioDeviceType" type="String">Gets or sets a value that specifies the output device ID that the audio will be sent to.</field>
            msAudioDeviceType: {
                get: function () {
                    return this._mediaElement.msAudioDeviceType;
                },
                set: function (value) {
                    var oldValue = this._mediaElement.msAudioDeviceType;
                    if (oldValue !== value) {
                        this._mediaElement.msAudioDeviceType = value;
                        this._observableMediaPlayer.notify("msAudioDeviceType", value, oldValue);
                    }
                }
            },

            /// <field name="msHorizontalMirror" type="Boolean">Gets or sets a value that specifies whether the media is flipped horizontally.</field>
            msHorizontalMirror: {
                get: function () {
                    return this._mediaElement.msHorizontalMirror;
                },
                set: function (value) {
                    var oldValue = this._mediaElement.msHorizontalMirror;
                    if (oldValue !== value) {
                        this._mediaElement.msHorizontalMirror = value;
                        this._observableMediaPlayer.notify("msHorizontalMirror", value, oldValue);
                    }
                }
            },

            /// <field name="msPlayToDisabled" type="Boolean">Gets or sets a value that specifies whether the DLNA PlayTo device is available.</field>
            msPlayToDisabled: {
                get: function () {
                    return this._mediaElement.msPlayToDisabled;
                },
                set: function (value) {
                    var oldValue = this._mediaElement.msPlayToDisabled;
                    if (oldValue !== value) {
                        this._mediaElement.msPlayToDisabled = value;
                        this._observableMediaPlayer.notify("msPlayToDisabled", value, oldValue);
                    }
                }
            },

            /// <field name="msPlayToPrimary" type="Boolean">Gets or sets the primary DLNA PlayTo device.</field>
            msPlayToPrimary: {
                get: function () {
                    return this._mediaElement.msPlayToPrimary;
                },
                set: function (value) {
                    var oldValue = this._mediaElement.msPlayToPrimary;
                    if (oldValue !== value) {
                        this._mediaElement.msPlayToPrimary = value;
                        this._observableMediaPlayer.notify("msPlayToPrimary", value, oldValue);
                    }
                }
            },

            /// <field name="msPlayToSource" type="Object">Gets the media source for use by the PlayToManager.</field>
            msPlayToSource: {
                get: function () {
                    return this._mediaElement.msPlayToSource;
                }
            },

            /// <field name="msRealTime" type="Boolean">Gets or sets a value that specifies whether or not to enable low-latency playback.</field>
            msRealTime: {
                get: function () {
                    return this._mediaElement.msRealTime;
                },
                set: function (value) {
                    var oldValue = this._mediaElement.msRealTime;
                    if (oldValue !== value) {
                        this._mediaElement.msRealTime = value;
                        this._observableMediaPlayer.notify("msRealTime", value, oldValue);
                    }
                }
            },

            /// <field name="msIsStereo3D" type="Boolean">Gets a value that specifies whether the system considers the media to be stereo 3D.</field>
            msIsStereo3D: {
                get: function () {
                    return this._mediaElement.msIsStereo3D;
                }
            },

            /// <field name="msStereo3DPackingMode" type="String">Gets or sets the frame-packing mode for stereo 3D video content.</field>
            msStereo3DPackingMode: {
                get: function () {
                    return this._mediaElement.msStereo3DPackingMode;
                },
                set: function (value) {
                    var oldValue = this._mediaElement.msStereo3DPackingMode;
                    if (oldValue !== value) {
                        this._mediaElement.msStereo3DPackingMode = value;
                        this._observableMediaPlayer.notify("msStereo3DPackingMode", value, oldValue);
                    }
                }
            },

            /// <field name="msStereo3DRenderMode" type="String">Gets or sets a value that specifies whether the system display is set to stereo display.</field>
            msStereo3DRenderMode: {
                get: function () {
                    return this._mediaElement.msStereo3DRenderMode;
                },
                set: function (value) {
                    var oldValue = this._mediaElement.msStereo3DRenderMode;
                    if (oldValue !== value) {
                        this._mediaElement.msStereo3DRenderMode = value;
                        this._observableMediaPlayer.notify("msStereo3DRenderMode", value, oldValue);
                    }
                }
            },

            /// <field name="msZoom" type="Boolean">Gets or sets a value that specifies whether the video frame is trimmed to fit the display.</field>
            msZoom: {
                get: function () {
                    return this._mediaElement.msZoom;
                },
                set: function (value) {
                    var oldValue = this._mediaElement.msZoom;
                    if (oldValue !== value) {
                        this._mediaElement.msZoom = value;
                        this._observableMediaPlayer.notify("msZoom", value, oldValue);
                    }
                }
            },

            /// <field name="testForMediaPack" type="Boolean">Gets or sets whether a test for the media feature pack should be performed prior to allowing content to be laoded. This is useful to enable if Windows 8 N/KN users will be using this app.</field>
            testForMediaPack: {
                get: function () {
                    return this._testForMediaPack;
                },
                set: function (value) {
                    this._testForMediaPack = value;
                }
            },

            /// <field name="visualMarkers" type="Array">Gets or sets the collection of markers to display in the timeline</field>
            visualMarkers: {
                get: function () {
                    return this._visualMarkers;
                },
                set: function (value) {
                    var oldValue = this._visualMarkers;
                    if (oldValue !== value) {
                        this._visualMarkers = value;
                        this._observableMediaPlayer.notify("visualMarkers", value, oldValue);
                    }
                }
            },

            /// <field name="markers" type="Array">Gets or sets the collection of markers to track. Note: this is different from visualMarkers (which are displayed in the timeline)</field>
            markers: {
                get: function () {
                    return this._markers;
                },
                set: function (value) {
                    var oldValue = this._markers;
                    if (oldValue !== value) {
                        this._markers = value;
                        this._observableMediaPlayer.notify("markers", value, oldValue);
                    }
                }
            },

            /// <field name="isThumbnailVisible" type="Boolean">Gets or sets whether thumbnails should be displayed while scrubbing. Default is false.</field>
            isThumbnailVisible: {
                get: function () {
                    return this._isThumbnailVisible;
                },
                set: function (value) {
                    var oldValue = this._isThumbnailVisible;
                    if (oldValue !== value) {
                        this._isThumbnailVisible = value;
                        this._observableMediaPlayer.notify("isThumbnailVisible", value, oldValue);
                    }
                }
            },

            /// <field name="thumbnailImageSrc" type="String">Gets or sets the thumbnail to show (typically while scrubbing and/or in RW/FF mode).</field>
            thumbnailImageSrc: {
                get: function () {
                    return this._thumbnailImageSrc;
                },
                set: function (value) {
                    var oldValue = this._thumbnailImageSrc;
                    if (oldValue !== value) {
                        this._thumbnailImageSrc = value;
                        this._observableMediaPlayer.notify("thumbnailImageSrc", value, oldValue);
                    }
                }
            },

            /************************ Public Methods ************************/
            canPlayType: function (type) {
                /// <summary>Returns a value that specifies whether the player can play a given media type.</summary>
                /// <param name="type" type="String">The type of media to be played.</param>
                /// <returns type="String">One of the following values: "probably", "maybe", or an empty string if the media cannot be rendered.</returns>

                return this._mediaElement.canPlayType(type);
            },

            retry: function () {
                /// <summary>Reloads the current media source and resumes where playback was left off.</summary>

                this.autoload = true;
                if (this._lastTime) {
                    this.startupTime = this._lastTime;
                }
                this.autoplay = true;
                this.load();
            },

            load: function () {
                /// <summary>Reloads the current media source.</summary>

                if (this._mediaElement.getAttribute("src") === this.src) {
                    this._mediaElement.load();
                } else if (this.playerState !== PlayerFramework.PlayerState.loading) {
                    this.playerState = PlayerFramework.PlayerState.loading;

                    if (this.testForMediaPack) {
                        if (!PlayerFramework.MediaPackHelper.testForMediaPack()) {
                            this._onMediaElementError();
                            return;
                        }
                    }

                    var deferrableOperation = new PlayerFramework.Utilities.DeferrableOperation();
                    deferrableOperation.src = this.src;
                    this.dispatchEvent("loading", deferrableOperation);

                    var promise = deferrableOperation.getPromise().then(
                        function (result) {
                            var canceled = result.some(function (value) { return value === false; });
                            if (!canceled) {
                                this._mediaElement.setAttribute("src", deferrableOperation.src);
                            }
                            else {
                                this._onMediaElementError();
                            }
                        }.bind(this)
                    );

                    promise.done(
                        function () {
                            PlayerFramework.Utilities.remove(this._activePromises, promise);
                        }.bind(this),
                        function () {
                            PlayerFramework.Utilities.remove(this._activePromises, promise);
                        }.bind(this)
                    );

                    this._activePromises.push(promise);
                }
            },

            play: function () {
                /// <summary>Loads and starts playback of the current media source.</summary>

                if (this.playerState === PlayerFramework.PlayerState.started) {
                    this._mediaElement.play();
                } else if (this.playerState !== PlayerFramework.PlayerState.starting) {
                    this.playerState = PlayerFramework.PlayerState.starting;

                    var deferrableOperation = new PlayerFramework.Utilities.DeferrableOperation();
                    this.dispatchEvent("starting", deferrableOperation);

                    var promise = deferrableOperation.getPromise().then(
                        function (result) {
                            var canceled = result.some(function (value) { return value === false; });
                            if (!canceled) {
                                this._mediaElement.play();
                                this.playerState = PlayerFramework.PlayerState.started;
                                this.dispatchEvent("started");
                            }
                        }.bind(this)
                    );

                    promise.done(
                        function () {
                            PlayerFramework.Utilities.remove(this._activePromises, promise);
                        }.bind(this),
                        function () {
                            PlayerFramework.Utilities.remove(this._activePromises, promise);
                        }.bind(this)
                    );

                    this._activePromises.push(promise);
                }
            },

            pause: function () {
                /// <summary>Pauses playback of the current media source.</summary>

                this._mediaElement.pause();
            },

            playResume: function () {
                /// <summary>Resets the playback rate and resumes playing the current media source.</summary>

                this.playbackRate = this.defaultPlaybackRate;
                this.play();
            },

            replay: function () {
                /// <summary>Supports instant replay by applying an offset to the current playback position.</summary>

                this.currentTime = Math.max(this.initialTime, this.currentTime - this.replayOffset);
                this.play();
            },

            decreasePlaybackRate: function () {
                /// <summary>Decreases the current playback rate by a factor of two. After the rate reaches 1 (normal speed), it will flip to -1, and then begins to rewind.</summary>

                var playbackRate = this.playbackRate;

                if (playbackRate <= 1 && playbackRate > -1) {
                    this.playbackRate = -1;
                } else if (playbackRate > 1) {
                    this.playbackRate /= 2;
                } else {
                    this.playbackRate *= 2;
                }
            },

            increasePlaybackRate: function () {
                /// <summary>Increases the current playback rate by a factor of two. After the rate reaches -1, it flips to 1 (normal speed), and then begins to fast forward.</summary>

                var playbackRate = this.playbackRate;

                if (playbackRate >= -1 && playbackRate < 1) {
                    this.playbackRate = 1;
                } else if (playbackRate < -1) {
                    this.playbackRate /= 2;
                } else {
                    this.playbackRate *= 2;
                }
            },

            addClass: function (name) {
                /// <summary>Adds the specified CSS class to the host element.</summary>
                /// <param name="name" type="String">The name of the class to add. Multiple classes can be added using space-delimited names.</param>

                WinJS.Utilities.addClass(this._element, name);
            },

            removeClass: function (name) {
                /// <summary>Removes the specified CSS class from the host element.</summary>
                /// <param name="name" type="String">The name of the class to remove. Multiple classes can be removed using space-delimited names.</param>

                WinJS.Utilities.removeClass(this._element, name);
            },

            focus: function () {
                /// <summary>Gives focus to the host element.</summary>

                this._element.focus();
            },

            update: function (mediaSource) {
                /// <summary>Updates the player and its plugins with the specified media source (e.g. the current playlist item).</summary>
                /// <param name="mediaSource" type="Object" optional="true">A JSON object containing the set of options that represent a media source.</param>

                if (!this._updating) {
                    this._updating = true;

                    // update the player
                    this.playerState = PlayerFramework.PlayerState.unloaded;
                    this._setOptions(mediaSource);

                    // update the plugins
                    for (var i = 0; i < this.plugins.length; i++) {
                        var plugin = this.plugins[i];
                        plugin.update(mediaSource);
                    }

                    // update the source
                    this._updateCurrentSource();

                    this.dispatchEvent("updated");
                    this._updating = false;
                }
            },

            dispose: function () {
                /// <summary>Shuts down and releases all resources.</summary>

                if (this._element) {
                    this._cancelActivePromises();
                    this._clearAutohideTimeout();
                    this._uninitializePlugins();

                    this.interactiveViewModel = null;

                    this._observableMediaPlayer.unbind();
                    this._unbindProperties();
                    this._unbindEvents();
                    this._observableMediaPlayer = null;

                    WinJS.Utilities.data(this._element).mediaPlayer = null;
                    WinJS.Utilities.removeClass(this._element, "pf-container");
                    this._element.winControl = null;

                    PlayerFramework.Utilities.removeElement(this._shimElement);
                    PlayerFramework.Utilities.removeElement(this._mediaElement);

                    this._element.innerHTML = "";
                    this._element = null;
                    this._shimElement = null;

                    this._mediaElement.src = null;
                    this._mediaElement = null;
                    this._mediaExtensionManager = null;
                }
            },

            msFrameStep: function (forward) {
                /// <summary>Steps the video forward or backward by one frame.</summary>
                /// <param name="forward" type="Boolean">If true, the video is stepped forward, otherwise the video is stepped backward.</param>

                this._mediaElement.msFrameStep(forward);
            },

            msClearEffects: function () {
                /// <summary>Clears all effects from the media pipeline.</summary>

                this._mediaElement.msClearEffects();
            },

            msInsertAudioEffect: function (activatableClassId, effectRequired, config) {
                /// <summary>Inserts the specified audio effect into the media pipeline.</summary>
                /// <param name="activatableClassId" type="String">The audio effects class.</param>
                /// <param name="effectRequired" type="Boolean"></param>
                /// <param name="config" type="Object" optional="true"></param>

                this._mediaElement.msInsertAudioEffect(activatableClassId, effectRequired, config);
            },

            msInsertVideoEffect: function (activatableClassId, effectRequired, config) {
                /// <summary>Inserts the specified video effect into the media pipeline.</summary>
                /// <param name="activatableClassId" type="String">The video effects class.</param>
                /// <param name="effectRequired" type="Boolean"></param>
                /// <param name="config" type="Object" optional="true"></param>

                this._mediaElement.msInsertVideoEffect(activatableClassId, effectRequired, config);
            },

            msSetMediaProtectionManager: function (mediaProtectionManager) {
                /// <summary>Sets the media protection manager for a given media pipeline.</summary>
                /// <param name="mediaProtectionManager" type="Windows.Media.Protection.MediaProtectionManager"></param>

                this._mediaElement.msSetMediaProtectionManager(mediaProtectionManager);
            },

            msSetVideoRectangle: function (left, top, right, bottom) {
                /// <summary>Sets the dimensions of a sub-rectangle within a video.</summary>
                /// <param name="left" type="Number">The left position of the rectangle.</param>
                /// <param name="top" type="Number">The top position of the rectangle.</param>
                /// <param name="right" type="Number">The right position of the rectangle.</param>
                /// <param name="bottom" type="Number">The bottom position of the rectangle.</param>

                this._mediaElement.msSetVideoRectangle(left, top, right, bottom);
            },

            stop: function () {
                /// <summary>Stops playback and raises the stopped event.</summary>

                if (this.playerState >= PlayerFramework.PlayerState.loaded) {
                    this._mediaElement.pause();
                    this._mediaElement.currentTime = this.startTime;
                }
                this.dispatchEvent("stopped");
            },

            /************************ Private Methods ************************/

            _setElement: function (element) {
                // host element
                this._element = element;
                this._element.tabIndex = 0;
                this._element.hideFocus = true;
                this._element.winControl = this;
                WinJS.Utilities.data(this._element).mediaPlayer = this;
                WinJS.Utilities.addClass(this._element, "pf-container");

                // media element
                this._mediaElement = this._element.querySelector("video") || this._element.querySelector("audio") || document.createElement("video");
                WinJS.Utilities.addClass(this._mediaElement, "pf-media");
                WinJS.Utilities.addClass(this._element, "pf-media-" + this._mediaElement.tagName.toLowerCase());

                PlayerFramework.Utilities.appendElement(this._element, this._mediaElement);

                // HACK: "iframe shim" fixes issues with full screen overlay
                this._shimElement = document.createElement("iframe");
                this._shimElement.setAttribute("style", "display: none; visibility: hidden; opacity: 0;");
                this._shimElement.setAttribute("role", "presentation");
                this._shimElement.setAttribute("aria-hidden", "true");
                this._shimElement.setAttribute("unselectable", "on");
                WinJS.Utilities.addClass(this._shimElement, "pf-shim");
                PlayerFramework.Utilities.appendElement(document.body, this._shimElement);

                // events
                this._bindEvent("focus", this._element, this._onElementFocus);
                this._bindEvent("keydown", this._element, this._onElementKeyDown);
                this._bindEvent("MSPointerDown", this._element, this._onElementMSPointerDown);
                this._bindEvent("MSPointerMove", this._element, this._onElementMSPointerMove);
                this._bindEvent("MSPointerOver", this._element, this._onElementMSPointerOver);
                this._bindEvent("MSPointerOut", this._element, this._onElementMSPointerOut);

                // media element events
                this._bindEvent("canplay", this._mediaElement, this._onMediaElementCanPlay);
                this._bindEvent("canplaythrough", this._mediaElement, this._onMediaElementCanPlayThrough);
                this._bindEvent("durationchange", this._mediaElement, this._onMediaElementDurationChange);
                this._bindEvent("emptied", this._mediaElement, this._onMediaElementEmptied);
                this._bindEvent("ended", this._mediaElement, this._onMediaElementEnded);
                this._bindEvent("error", this._mediaElement, this._onMediaElementError);
                this._bindEvent("loadeddata", this._mediaElement, this._onMediaElementLoadedData);
                this._bindEvent("loadedmetadata", this._mediaElement, this._onMediaElementLoadedMetadata);
                this._bindEvent("loadstart", this._mediaElement, this._onMediaElementLoadStart);
                this._bindEvent("MSVideoFormatChanged", this._mediaElement, this._onMediaElementMSVideoFormatChanged);
                this._bindEvent("MSVideoFrameStepCompleted", this._mediaElement, this._onMediaElementMSVideoFrameStepCompleted);
                this._bindEvent("MSVideoOptimalLayoutChanged", this._mediaElement, this._onMediaElementMSVideoOptimalLayoutChanged);
                this._bindEvent("pause", this._mediaElement, this._onMediaElementPause);
                this._bindEvent("play", this._mediaElement, this._onMediaElementPlay);
                this._bindEvent("playing", this._mediaElement, this._onMediaElementPlaying);
                this._bindEvent("progress", this._mediaElement, this._onMediaElementProgress);
                this._bindEvent("ratechange", this._mediaElement, this._onMediaElementRateChange);
                this._bindEvent("readystatechange", this._mediaElement, this._onMediaElementReadyStateChange);
                this._bindEvent("seeked", this._mediaElement, this._onMediaElementSeeked);
                this._bindEvent("seeking", this._mediaElement, this._onMediaElementSeeking);
                this._bindEvent("stalled", this._mediaElement, this._onMediaElementStalled);
                this._bindEvent("suspend", this._mediaElement, this._onMediaElementSuspend);
                this._bindEvent("timeupdate", this._mediaElement, this._onMediaElementTimeUpdate);
                this._bindEvent("volumechange", this._mediaElement, this._onMediaElementVolumeChange);
                this._bindEvent("waiting", this._mediaElement, this._onMediaElementWaiting);

                // property notifications
                this._bindEvent("emptied", this._mediaElement, this._notifyProperties, ["currentTime", "virtualTime", "paused", "ended", "buffered"]);
                this._bindEvent("loadstart", this._mediaElement, this._notifyProperties, ["currentTime", "virtualTime", "paused", "ended", "buffered"]);
                this._bindEvent("loadeddata", this._mediaElement, this._notifyProperties, ["currentTime", "virtualTime", "paused", "ended", "buffered"]);
                this._bindEvent("timeupdate", this._mediaElement, this._notifyProperties, ["currentTime", "virtualTime"]);
                this._bindEvent("playing", this._mediaElement, this._notifyProperties, ["paused", "ended"]);
                this._bindEvent("pause", this._mediaElement, this._notifyProperties, ["paused", "ended"]);
                this._bindEvent("ended", this._mediaElement, this._notifyProperties, ["paused", "ended"]);
                this._bindEvent("progress", this._mediaElement, this._notifyProperties, ["buffered"]);
                this._bindEvent("ratechange", this._mediaElement, this._notifyProperties, ["playbackRate"]);
                this._bindEvent("volumechange", this._mediaElement, this._notifyProperties, ["volume", "muted"]);

                // property dependencies
                this._bindProperty("advertisingState", this._observableMediaPlayer, this._notifyProperties, ["isPlayPauseAllowed", "isPlayResumeAllowed", "isPauseAllowed", "isReplayAllowed", "isRewindAllowed", "isFastForwardAllowed", "isSlowMotionAllowed", "isSkipPreviousAllowed", "isSkipNextAllowed", "isSkipBackAllowed", "isSkipAheadAllowed", "isElapsedTimeAllowed", "isRemainingTimeAllowed", "isTotalTimeAllowed", "isTimelineAllowed", "isGoLiveAllowed", "isCaptionsAllowed", "isAudioAllowed"]);
                this._bindProperty("playerState", this._observableMediaPlayer, this._notifyProperties, ["isPlayPauseAllowed", "isPlayResumeAllowed", "isPauseAllowed", "isReplayAllowed", "isRewindAllowed", "isFastForwardAllowed", "isSlowMotionAllowed", "isSkipPreviousAllowed", "isSkipNextAllowed", "isSkipBackAllowed", "isSkipAheadAllowed", "isElapsedTimeAllowed", "isRemainingTimeAllowed", "isTotalTimeAllowed", "isTimelineAllowed", "isGoLiveAllowed", "isCaptionsAllowed", "isAudioAllowed", "isStopAllowed"]);
                this._bindProperty("paused", this._observableMediaPlayer, this._notifyProperties, ["isPlayResumeAllowed", "isPauseAllowed", "isRewindAllowed", "isFastForwardAllowed", "isSlowMotionAllowed"]);
                this._bindProperty("ended", this._observableMediaPlayer, this._notifyProperties, ["isPlayResumeAllowed", "isPauseAllowed", "isRewindAllowed", "isFastForwardAllowed", "isSlowMotionAllowed"]);
                this._bindProperty("playbackRate", this._observableMediaPlayer, this._notifyProperties, ["isPlayResumeAllowed", "isRewindAllowed", "isFastForwardAllowed", "isSlowMotionAllowed", "isSlowMotion"]);
                this._bindProperty("defaultPlaybackRate", this._observableMediaPlayer, this._notifyProperties, ["isPlayResumeAllowed"]);
                this._bindProperty("slowMotionPlaybackRate", this._observableMediaPlayer, this._notifyProperties, ["isSlowMotionAllowed", "isSlowMotion"]);
                this._bindProperty("isLive", this._observableMediaPlayer, this._notifyProperties, ["isGoLiveAllowed"]);
                this._bindProperty("isCurrentTimeLive", this._observableMediaPlayer, this._notifyProperties, ["isGoLiveAllowed"]);
                this._bindProperty("startTime", this._observableMediaPlayer, this._notifyProperties, ["duration"]);
                this._bindProperty("endTime", this._observableMediaPlayer, this._notifyProperties, ["duration"]);
                this._bindProperty("captionTracks", this._observableMediaPlayer, this._notifyProperties, ["isCaptionsAllowed"]);
                this._bindProperty("audioTracks", this._observableMediaPlayer, this._notifyProperties, ["isAudioAllowed"]);

                // initialize view model
                this.interactiveViewModel.initialize();
            },

            _setOptions: function (options) {
                // prevents source updates from loading immediately
                // and allows all plugins to be updated first
                if (options && options.src) {
                    this._src = options.src;
                } else {
                    this._src = "";
                }

                PlayerFramework.Utilities.setOptions(this, options);
            },

            _initializePlugins: function () {
                // instantiate plugins
                for (var pluginKey in PlayerFramework.Plugins) {
                    var pluginType = PlayerFramework.Plugins[pluginKey];
                    if (pluginType && pluginType.prototype instanceof PlayerFramework.PluginBase) {
                        var pluginName = pluginKey[0].toLowerCase() + pluginKey.substring(1);
                        var pluginOptions = this[pluginName] || undefined;
                        var plugin = new pluginType(pluginOptions);
                        this[pluginName] = plugin;
                        this.plugins.push(plugin);
                    } else {
                        throw invalidPlugin;
                    }
                }

                // load plugins
                for (var i = 0; i < this.plugins.length; i++) {
                    var plugin = this.plugins[i];
                    plugin.mediaPlayer = this;
                    plugin.load();
                }
            },

            _uninitializePlugins: function () {
                // unload plugins
                for (var i = 0; i < this.plugins.length; i++) {
                    var plugin = this.plugins[i];
                    plugin.unload();
                    plugin.mediaPlayer = null;
                }

                // reset plugins
                this._plugins = [];
            },

            _cancelActivePromises: function () {
                for (var i = 0; i < this._activePromises.length; i++) {
                    var promise = this._activePromises[i];
                    promise.cancel();
                }

                this._activePromises = [];
            },

            _notifyProperties: function (propertyNames) {
                if (this._observableMediaPlayer) {
                    for (var i = 0; i < propertyNames.length; i++) {
                        var propertyName = propertyNames[i];
                        var propertyValue = this[propertyName];
                        this._observableMediaPlayer.notify(propertyName, propertyValue);
                    }
                }
            },

            _seek: function (time) {
                var previousTime = this._virtualTime;
                this.currentTime = time;
                this.dispatchEvent("seek", { previousTime: previousTime, time: time });
            },

            _seekToLive: function () {
                if (this.liveTime) {
                    this._seek(this.liveTime);
                }
            },

            _startScrub: function (time) {
                if (!this._scrubbing) {
                    this._scrubbing = true;
                    this._scrubStartTime = this._virtualTime;

                    var e = { time: time, canceled: false };
                    this.dispatchEvent("scrub", e);

                    if (!e.canceled) {
                        this._scrubPlaybackRate = this.playbackRate;
                        if (this._mediaElement.tagName !== "AUDIO") {
                            // better UX to not stop playback for audio
                            this.playbackRate = 0;
                        }
                    } else {
                        this._completeScrub(time, true);
                    }
                }
            },

            _updateScrub: function (time) {
                if (this._scrubbing) {
                    var e = { startTime: this._scrubStartTime, time: time, canceled: false };
                    this.dispatchEvent("scrubbing", e);

                    if (!e.canceled) {
                        if (this.seekWhileScrubbing) {
                            this.currentTime = time;
                        }
                        else {
                            this._setVirtualTime(time);
                        }
                    } else {
                        this._completeScrub(time, true);
                    }
                }
            },

            _completeScrub: function (time, canceled) {
                if (this._scrubbing) {
                    this._scrubbing = false;

                    var e = { startTime: this._scrubStartTime, time: time, canceled: !!canceled };
                    this.dispatchEvent("scrubbed", e);

                    if (!e.canceled) {
                        this.currentTime = time;
                        if (this._mediaElement.tagName !== "AUDIO") {
                            this.playbackRate = this._scrubPlaybackRate;
                        }
                    }
                }
            },

            _updateCurrentSource: function () {
                if (this.src && this.autoload) {
                    this.load();
                } else {
                    this.playerState = this.src ? PlayerFramework.PlayerState.pending : PlayerFramework.PlayerState.unloaded;
                    this._mediaElement.removeAttribute("src");
                }
            },

            _updateIsCurrentTimeLive: function () {
                if (this.liveTime !== null) {
                    var liveThreshold = this.isCurrentTimeLive ? this.liveTimeBuffer * 1.1 : this.liveTimeBuffer * 0.9;
                    this.isCurrentTimeLive = this.liveTime - this._virtualTime < liveThreshold;
                } else {
                    this.isCurrentTimeLive = false;
                }
            },

            _updateMarkerState: function () {
                var now = this._virtualTime;
                for (var i = 0; i < this._markers.length; i++) {
                    var marker = this._markers[i];
                    if (marker.time < now && marker.time >= this._lastMarkerCheckTime) {
                        this.dispatchEvent("markerreached", marker);
                    }
                }
                this._lastMarkerCheckTime = now;
            },

            _isFunctionalElement: function (element) {
                var functionalElements = this.element.querySelectorAll(".pf-functional:not(:disabled)");
                return !!PlayerFramework.Utilities.first(functionalElements, function (functionalElement) {
                    return functionalElement === element || functionalElement.contains(element);
                });
            },

            _isInteractiveElement: function (element) {
                var interactiveElements = this.element.querySelectorAll(".pf-interactive");
                return !!PlayerFramework.Utilities.first(interactiveElements, function (interactiveElement) {
                    return interactiveElement === element || interactiveElement.contains(element);
                });
            },

            _clearAutohideTimeout: function () {
                window.clearTimeout(this._autohideTimeoutId);
                this._autohideTimeoutId = null;
            },

            _resetAutohideTimeout: function () {
                window.clearTimeout(this._autohideTimeoutId);
                this._autohideTimeoutId = window.setTimeout(this._onAutohideTimeout.bind(this), this.autohideTime * 1000);
            },

            _onAutohideTimeout: function () {
                var preventAutohide = false;
                var activeElement = document.activeElement;

                if ((this.autohideBehavior & PlayerFramework.AutohideBehavior.allowDuringPlaybackOnly) === PlayerFramework.AutohideBehavior.allowDuringPlaybackOnly && this.interactiveViewModel.paused) {
                    preventAutohide = true;
                } else if ((this.autohideBehavior & PlayerFramework.AutohideBehavior.preventDuringInteractiveHover) === PlayerFramework.AutohideBehavior.preventDuringInteractiveHover && this._isInteractiveHover) {
                    preventAutohide = true;
                } else if (activeElement && this._isInteractiveElement(activeElement) && !WinJS.Utilities.hasClass(activeElement, "pf-hide-focus")) {
                    preventAutohide = true;
                }

                if (!preventAutohide) {
                    this._clearAutohideTimeout();
                    this.isInteractive = false;
                } else {
                    this._resetAutohideTimeout();
                }
            },

            _onUserInteraction: function (interactionType, handled) {
                if (this.isInteractive) {
                    if (!handled && (this.interactiveDeactivationMode & PlayerFramework.InteractionType.hard) === PlayerFramework.InteractionType.hard && (interactionType & PlayerFramework.InteractionType.hard) === PlayerFramework.InteractionType.hard) {
                        this.isInteractive = false;
                    } else if ((this.interactiveDeactivationMode & PlayerFramework.InteractionType.soft) === PlayerFramework.InteractionType.soft && this.autohide) {
                        this._resetAutohideTimeout();
                    }
                } else {
                    if ((this.interactiveActivationMode & interactionType) === interactionType) {
                        this.isInteractive = true;
                    }
                }
            },

            _onElementFocus: function (e) {
                this._onUserInteraction(PlayerFramework.InteractionType.hard, true);
            },

            _onElementKeyDown: function (e) {
                if (e.keyCode === WinJS.Utilities.Key.F11 && this.isFullScreenAllowed && this.isFullScreenEnabled) {
                    this.isFullScreen = !this.isFullScreen;
                } else if (e.keyCode === WinJS.Utilities.Key.escape && this.isFullScreenAllowed && this.isFullScreenEnabled) {
                    this.isFullScreen = false;
                }

                this._onUserInteraction(PlayerFramework.InteractionType.hard, true);
            },

            _onElementMSPointerDown: function (e) {
                // prevent conflict with MSPointerMove event
                this._interactivePointerArgs = e;

                if (this._isFunctionalElement(e.target)) {
                    this._onUserInteraction(PlayerFramework.InteractionType.hard, true);
                } else {
                    this._onUserInteraction(PlayerFramework.InteractionType.hard, false);
                }
            },

            _onElementMSPointerMove: function (e) {
                // prevent conflict with MSPointerDown event
                if (e.pointerType === e.MSPOINTER_TYPE_MOUSE || !this._interactivePointerArgs || e.clientX !== this._interactivePointerArgs.clientX || e.clientY !== this._interactivePointerArgs.clientY) {
                    this._onUserInteraction(PlayerFramework.InteractionType.soft, false);
                }

                this._interactivePointerArgs = e;
            },

            _onElementMSPointerOver: function (e) {
                if (this._isInteractiveElement(e.target)) {
                    this._isInteractiveHover = true;
                }
            },

            _onElementMSPointerOut: function (e) {
                this._isInteractiveHover = false;
            },

            _onMediaElementCanPlay: function (e) {
                this.playerState = PlayerFramework.PlayerState.opened;
                this.dispatchEvent("canplay");
            },

            _onMediaElementCanPlayThrough: function (e) {
                this.dispatchEvent("canplaythrough");

                if (this.autoplay) {
                    this.play();
                }
            },

            _onMediaElementDurationChange: function (e) {
                if (isFinite(this._mediaElement.duration)) {
                    // if duration is infinity, this means initialTime is not necessarily valid either. StartTime must be provided by another means (e.g. adaptive plugin)
                    this.startTime = this._mediaElement.initialTime;
                }
                this.endTime = this._mediaElement.duration;
            },

            _onMediaElementEmptied: function (e) {
                this._lastTime = null;
                this.playerState = this.src ? PlayerFramework.PlayerState.pending : PlayerFramework.PlayerState.unloaded;
                this.dispatchEvent("emptied");
            },

            _onMediaElementEnded: function (e) {
                this.playerState = PlayerFramework.PlayerState.ending;

                var deferrableOperation = new PlayerFramework.Utilities.DeferrableOperation();
                this.dispatchEvent("ending", deferrableOperation);

                var promise = deferrableOperation.getPromise().then(
                    function () {
                        this.playerState = PlayerFramework.PlayerState.ended;
                        this.dispatchEvent("ended");
                    }.bind(this)
                );

                promise.done(
                    function () {
                        PlayerFramework.Utilities.remove(this._activePromises, promise);
                    }.bind(this),
                    function () {
                        PlayerFramework.Utilities.remove(this._activePromises, promise);
                    }.bind(this)
                );

                this._activePromises.push(promise);
            },

            _onMediaElementError: function (e) {
                var e = { error: this.error, canceled: false };
                this.dispatchEvent("error", e);

                if (!e.canceled) {
                    this.playerState = PlayerFramework.PlayerState.failed;
                }
            },

            _onMediaElementLoadedData: function (e) {
                if (this.startupTime) {
                    this.currentTime = this.startupTime;
                }

                this.dispatchEvent("loadeddata");
            },

            _onMediaElementLoadedMetadata: function (e) {
                this.mediaQuality = this.videoHeight >= 720 ? PlayerFramework.MediaQuality.highDefinition : PlayerFramework.MediaQuality.standardDefinition;

                if (!this.audioTracks) {
                    this.audioTracks = PlayerFramework.Utilities.getArray(this._mediaElement.audioTracks);
                }
                this.currentAudioTrack = PlayerFramework.Utilities.first(this.audioTracks, function (track) { return track.enabled; });

                if (!this.captionTracks) {
                    this.captionTracks = Array.prototype.filter.call(this.textTracks, function (track) {
                        return (track.kind === "captions" || track.kind === "subtitles") && track !== this._dummyTrack;
                    }, this);
                }
                this.currentCaptionTrack = PlayerFramework.Utilities.first(this.captionTracks, function (track) { return track.mode === PlayerFramework.TextTrackMode.showing; });

                this.dispatchEvent("loadedmetadata");
            },

            _onMediaElementLoadStart: function (e) {
                this._lastMarkerCheckTime = -1;
                this.playerState = PlayerFramework.PlayerState.loaded;
                this.dispatchEvent("loadstart");
            },

            _onMediaElementMSVideoFormatChanged: function (e) {
                this.dispatchEvent("MSVideoFormatChanged");
            },

            _onMediaElementMSVideoFrameStepCompleted: function (e) {
                this.dispatchEvent("MSVideoFrameStepCompleted");
            },

            _onMediaElementMSVideoOptimalLayoutChanged: function (e) {
                this.dispatchEvent("MSVideoOptimalLayoutChanged");
            },

            _onMediaElementPause: function (e) {
                this.dispatchEvent("pause");
            },

            _onMediaElementPlay: function (e) {
                this.dispatchEvent("play");
            },

            _onMediaElementPlaying: function (e) {
                this.dispatchEvent("playing");
            },

            _onMediaElementProgress: function (e) {
                this.dispatchEvent("progress");
            },

            _onMediaElementRateChange: function (e) {
                this.dispatchEvent("ratechange");
            },

            _onMediaElementReadyStateChange: function (e) {
                this.dispatchEvent("readystatechange");
            },

            _onMediaElementSeeked: function (e) {
                this.dispatchEvent("seeked");
            },

            _onMediaElementSeeking: function (e) {
                this.dispatchEvent("seeking");
            },

            _onMediaElementStalled: function (e) {
                this.dispatchEvent("stalled");
            },

            _onMediaElementSuspend: function (e) {
                this.dispatchEvent("suspend");
            },

            _onMediaElementTimeUpdate: function (e) {
                var time = this.currentTime;
                if ((this.isTrickPlayEnabled || this.playbackRate === 0.0 || this.playbackRate === 1.0) && !this.scrubbing) {
                    this._virtualTime = time;
                }
                this._lastTime = time;
                this._updateIsCurrentTimeLive();
                this._updateMarkerState();
                this.dispatchEvent("timeupdate");
            },

            _onMediaElementVolumeChange: function (e) {
                this.dispatchEvent("volumechange");
            },

            _onMediaElementWaiting: function (e) {
                this.dispatchEvent("waiting");
            },

            _setVirtualTime: function (time) {
                var oldValue = this._virtualTime;
                this._virtualTime = time;
                this._lastTime = this._virtualTime;
                this._updateIsCurrentTimeLive();
                this.dispatchEvent("timeupdate");
                this._observableMediaPlayer.notify("virtualTime", time, oldValue);
            },

            _onSimulatedPlaybackRateTimerTick: function () {
                if (this.playbackRate !== 0.0 && this.playbackRate !== 1.0 && !this.scrubbing) {
                    var newTime = this._virtualTime + this.playbackRate / 4;
                    if (newTime > this.endTime) newTime = this.endTime;
                    if (newTime < this.startTime) newTime = this.startTime;
                    this._setVirtualTime(newTime);
                }
            }
        })
    });

    // MediaPlayer Mixins
    WinJS.Class.mix(PlayerFramework.MediaPlayer, WinJS.Utilities.eventMixin);
    WinJS.Class.mix(PlayerFramework.MediaPlayer, PlayerFramework.Utilities.createEventProperties(events));
    WinJS.Class.mix(PlayerFramework.MediaPlayer, PlayerFramework.Utilities.eventBindingMixin);
    WinJS.Class.mix(PlayerFramework.MediaPlayer, PlayerFramework.Utilities.propertyBindingMixin);

})(PlayerFramework);


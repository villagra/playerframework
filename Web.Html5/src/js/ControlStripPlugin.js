PlayerFramework.Plugins.ControlStripPlugin = PlayerFramework.ControlPlugin.extend(
{
	defaultOptions: function(player)
	{
		return PlayerFramework.mergeOptions(this._super(),
		{
			strings:
			{
				toggleTitle: "Show Controls",
				toggleAltTitle: "Hide Controls",
				playTitle: "Play",
				playAltTitle: "Pause",
				muteTitle: "Mute",
				muteAltTitle: "Unmute",
				fullBrowserTitle: "Full Screen",
				fullBrowserAltTitle: "Exit Full Screen",
				timelineTitle: "Playback: {0}%",
				timelineFillTitle: "Decrement Playback Position",
				timelineEmptyTitle: "Increment Playback Position",
				timeElapsedTitle: "Time Elapsed",
				timeRemainingTitle: "Time Remaining",
				volumeTitle: "Volume: {0}%",
				volumeFillTitle: "Decrement Volume Level",
				volumeEmptyTitle: "Increment Volume Level"
			},
			accessKeys:
			{
				toggleKey: "c",
				playKey: "p",
				muteKey: "m",
				fullBrowserKey: "s"
			}
		});
	},

	init: function(player, options)
	{
		///	<summary>
		///		Initializes the ControlPlugin that creates the default look and behavior for the Player object.
		///	</summary>
		///	<param name="player" type="Object">
		///		The Player object.
		///	</param>
		///	<param name="options" type="Object">
		///		The options for the ControlStripPlugin.
		///	</param>
		///	<returns type="ControlStripPlugin" />

		this._super(player, options);

		this.createControl();

		this.canPlayThrough = false;
		this.playInitiated = false;
		this.isTimelineMouseDown = false;
		this.isVolumeMouseDown = false;
		this.volumeLevelDelta = .1;
		this.currentTimeDelta = 5;
	},

	// Properties
	currentTime: function(value)
	{
		///	<summary>
		///		Gets or sets the current playback position of the media element expressed in seconds.
		///	</summary>
		///	<param name="value" type="Number">
		///		The playback position to seek to.
		///	</param>
		///	<returns type="Number" />
		
		if (value !== undefined)
			this.player.mediaPlugin.currentTime(value);
		
		return this.currentTimeValue;
	},

	// Event Handlers
	onToggleClick: function(e)
	{
		///	<summary>
		///		Called when the the toggle button is clicked and toggles the control strip display.
		///	</summary>

		if (this.element.style.display === "none")
			this.showControlStrip();
		else
			this.hideControlStrip();
	},

	onPlayClick: function(e)
	{
		///	<summary>
		///		Called when the play button is clicked and toggles the media element between play and pause.
		///	</summary>
		
		if (this.player.paused())
			this.player.play();
		else
			this.player.pause();
	},

	onTimelineMouseDown: function(e)
	{
		///	<summary>
		///		Called when the mouse is depressed on the timeline slider.
		///	</summary>
		
		// Prevents text selection.
		document.body.focus();
		document.onselectstart = function () { return false; };
		
		// Store initial measurements while scrubbing - IE reports zero for offset measurements when rendering.
		this.timelineSliderMeasurements = { headWidth: this.timelineHeadElement.offsetWidth, headContainerLeft: PlayerFramework.getTotalOffsetLeft(this.timelineHeadContainerElement), headContainerWidth: this.timelineHeadContainerElement.offsetWidth };
		
		this.isTimelineMouseDown = true;
		this.player.scrubbing = PlayerFramework.proxy(this, this.scrubbing);
		this.player.dispatchEvent({ type: "scrubbing" });

		this.onDocumentMouseMove(e);
	},

	onTimelineFillClick: function(e)
	{
		///	<summary>
		///		Called when filled area of the timeline slider is clicked.
		///	</summary>

		var currentTime = this.player.currentTime();
		if (currentTime - this.currentTimeDelta > 0)
			this.player.currentTime(currentTime - this.currentTimeDelta);
	},

	onTimelineEmptyClick: function(e)
	{
		///	<summary>
		///		Called when empty area of the timeline slider is clicked.
		///	</summary>

		var currentTime = this.player.currentTime();
		if (currentTime + this.currentTimeDelta < this.player.duration())
			this.player.currentTime(currentTime + this.currentTimeDelta);
	},

	onVolumeMouseDown: function(e)
	{
		///	<summary>
		///		Called when the mouse is depressed on the volume slider.
		///	</summary>

		// Prevents text selection.
		document.body.focus();
		document.onselectstart = function () { return false; };
		
		// Store initial measurements while scrubbing - IE reports zero for offset measurements when rendering.
		this.volumeSliderMeasurements = { headWidth: this.volumeHeadElement.offsetWidth, headContainerLeft: PlayerFramework.getTotalOffsetLeft(this.volumeHeadContainerElement), headContainerWidth: this.volumeHeadContainerElement.offsetWidth };
		this.isVolumeMouseDown = true;
		this.onDocumentMouseMove(e);
	},

	onVolumeFillClick: function(e)
	{
		///	<summary>
		///		Called when filled area of the volume slider is clicked.
		///	</summary>

		var volume = this.player.volume();
		if (volume - this.volumeLevelDelta > 0)
			this.player.volume(volume - this.volumeLevelDelta);
	},

	onVolumeEmptyClick: function(e)
	{
		///	<summary>
		///		Called when empty area of the volume slider is clicked.
		///	</summary>

		var volume = this.player.volume();
		if (volume + this.volumeLevelDelta < 1)
			this.player.volume(volume + this.volumeLevelDelta);
	},

	onDocumentMouseMove: function(e)
	{
		///	<summary>
		///		Called when the mouse moves on the document.
		///	</summary>
		
		if (this.isTimelineMouseDown)
		{
			// Adjust the playback position given the mouse position on the timeline slider.
			var timelineFraction = this.getSliderMouseFraction(this.timelineSliderMeasurements, e);
			this.currentTimeValue = timelineFraction * this.player.duration();
			this.player.currentTime(this.currentTimeValue);

			// Adjust the slider manually now because we prevent the slider from being adjusted
			// in response to the "timeupdate" event on the media plugin.
			this.setSliderHeadPosition(this.timelineContainerElement, timelineFraction);
		}
		
		if (this.isVolumeMouseDown)
		{
			// Adjust the volume level given the mouse position on the volume slider slider.
			var volumeFraction = this.getSliderMouseFraction(this.volumeSliderMeasurements, e);
			this.player.volume(volumeFraction);
		}
	},

	onDocumentMouseUp: function(e)
	{
		///	<summary>
		///		Called when the mouse is released on the document.
		///	</summary>

		// Allows text selection.
		document.onselectstart = function () { return true; };

		// Ensure video is paused if scrubbed to the end and released.
		if (this.player.currentTime() === this.player.duration() || (this.player.ended && this.player.ended()))
			this.player.pause();

		this.isTimelineMouseDown = false;
		this.isVolumeMouseDown = false;
		this.player.dispatchEvent({ type: "scrubbed" });
	},

	onDocumentKeyDown: function(e)
	{
		///	<summary>
		///		Called when a key is pressed on the document.
		///	</summary>

		var charCode = PlayerFramework.getCharCode(e);
		
		// exit full screen if escape key is pressed
		if (charCode === 27 && this.player.displayingFullscreen())
			this.exitFullScreen();
	},

	onPlayerMouseOut: function(e)
	{
		///	<summary>
		///		Called when the mouse leaves the player.
		///	</summary>
		
		if (this.player.options.overlayControls)
			this.hideControlStrip();
	},

	onPlayerMouseOver: function(e)
	{
		///	<summary>
		///		Called when the mouse enters the player.
		///	</summary>
		
		if (this.player.options.overlayControls)
			this.showControlStrip();
	},

	onMuteClick: function(e)
	{
		///	<summary>
		///		Called when the mute button is clicked.
		///	</summary>

		this.player.muted(!this.player.muted());
	},

	onFullBrowserClick: function(e)
	{
		///	<summary>
		///		Called when the full browser button is clicked.
		///	</summary>

		if (this.player.displayingFullscreen())
			this.exitFullScreen();
		else
			this.enterFullScreen();

		this.mediaPlugin().onFullScreenChange();
	},

	// ControlPlugin Event Handlers
	onCanPlayThrough: function(e)
	{
		///	<summary>
		///		Called when the media element can play through to the end without having to stop for further buffering.
		///	</summary>

		window.setTimeout(PlayerFramework.proxy(this, function()
		{
			this.canPlayThrough = true;

			if (!this.playInitiated && !(this.player.mediaPlugin instanceof PlayerFramework.StaticContentMediaPlugin))
			{
				if (this.playOverlayControlElement)
					this.playOverlayControlElement.style.display = "block";
				else
					this.showControlStrip();
			}
		}), 500);
	},

	onEnded: function(e)
	{
		///	<summary>
		///		Called when the media playback ends.
		///	</summary>

		if (!this.isTimelineMouseDown)
			this.player.pause();
	},

	onPlay: function(e)
	{
		///	<summary>
		///		Called when the media plays.
		///	</summary>

		if (this.posterElement)
			this.posterElement.style.display = "none";

		if (this.playOverlayControlElement)
			this.playOverlayControlElement.style.display = "none";

		this.playInitiated = true;

		this.playControlElement.className = "pf-pause pf-play-control";
		this.playButtonElement.setAttribute("title", this.options.strings.playAltTitle);
	},

	onPause: function(e)
	{
		///	<summary>
		///		Called when the media pauses.
		///	</summary>

		this.playControlElement.className = "pf-play pf-play-control";
		this.playButtonElement.setAttribute("title", this.options.strings.playTitle);
	},

	onProgress: function(e)
	{
		///	<summary>
		///		Called when the media buffers more data.
		///	</summary>
		
		if (this.player.buffered().length > 0)
			this.timelineLoadElement.style.width = Math.round(100 * (this.player.buffered().end(0) / this.player.duration()), 2) + "%";
	},
	
	onReady: function(e)
	{
		///	<summary>
		///		Called when the player is ready for playback.
		///	</summary>
		
		// Disable volume controls if setting volume is not supported (volume can only be controlled by hardware):
		// http://developer.apple.com/library/safari/#documentation/AudioVideo/Conceptual/Using_HTML5_Audio_Video/Device-SpecificConsiderations/Device-SpecificConsiderations.html#//apple_ref/doc/uid/TP40009523-CH5-SW11)		
		if (!this.mediaPlugin().supportsVolumeSetter())
			this.element.className += " pf-no-volume";

		// Poster Control
		var posterSource = this.player.poster();
		if (posterSource)
		{
			this.posterElement = PlayerFramework.createElement(null,
			[
				"img",
				{
					"class": "pf-poster",
					src: posterSource
				}
			]);
			this.player.containerElement.insertBefore(this.posterElement, this.mediaPlugin().element.nextSibling);
		}

		// Play Overlay Control
		if (this.player.options.overlayPlayButton)
		{
			this.playOverlayControlElement = PlayerFramework.createElement(this.player.containerElement,
			[
				"div",
				{
					"class": "pf-play-overlay-control"
				},
				[
					"button",
					{
						"class": "pf-button",
						type: "button",
						title: this.options.strings.playTitle,
						accessKey: this.options.accessKeys.playKey
					},
					[
						"span"
					]
				]
			]);
		}

		// Initial adjustment of volume control
		this.onVolumeChange();

		this.addEventListeners();
	},

	onSeeked: function(e)
	{
		///	<summary>
		///		Called when the media plugin has completed seeking.
		///	</summary>
		
		// Set currentTime function asynchronously, otherwise currentTime is earlier than the previous time.
		window.setTimeout(PlayerFramework.proxy(this, function()
		{
			this.player.currentTime = PlayerFramework.proxy(this.player.mediaPlugin, this.player.mediaPlugin.currentTime);
		}), 1);
	},
	
	onSeeking: function(e)
	{
		///	<summary>
		///		Called continuously while the media plugin is seeking.
		///	</summary>

		if (this.isTimelineMouseDown)
			this.player.currentTime = PlayerFramework.proxy(this, this.currentTime);
	},

	onTimeUpdate: function(e)
	{
		///	<summary>
		///		Called when the current time of the media is updated.
		///	</summary>
		
		var currentTime = this.player.currentTime();
		var duration = this.player.duration();

		if (!this.isTimelineMouseDown)
		{
			var currentTimePercentage = currentTime !== 0 ? (currentTime / duration) : 0;
			this.setSliderHeadPosition(this.timelineContainerElement, currentTimePercentage);

			var timelineTitle = this.options.strings.timelineTitle.replace("{0}", Math.round(currentTimePercentage * 100));
			this.timelineButtonElement.setAttribute("title", timelineTitle);
		}

		if (this.previousCurrentTime !== Math.round(currentTime))
		{
			this.previousCurrentTime = Math.round(currentTime);
			this.timeElapsedElement.innerText = this.formatTimeString(currentTime);
			this.timeRemainingElement.innerText = this.formatTimeString(duration - currentTime);
		}
	},
	
	onUnloadingMediaPlugin: function(e)
	{
		///	<summary>
		///		Called when the media element unloads.
		///	</summary>
		
		if (this.playOverlayControlElement)
			this.playOverlayControlElement.style.display = "none";
	},

	onVolumeChange: function(e)
	{
		///	<summary>
		///		Called when the volume level of the media changes.
		///	</summary>

		this.setSliderHeadPosition(this.volumeContainerElement, this.player.volume());

		if (this.player.muted())
		{
			this.muteControlElement.className = "pf-mute pf-mute-control";
			this.muteButtonElement.setAttribute("title", this.options.strings.muteAltTitle);
		}
		else
		{
			this.muteControlElement.className = "pf-sound pf-mute-control";
			this.muteButtonElement.setAttribute("title", this.options.strings.muteTitle);
		}

		var volumeTitle = this.options.strings.volumeTitle.replace("{0}", Math.round(this.player.volume() * 100));
		this.volumeButtonElement.setAttribute("title", volumeTitle);
	},

	// Functions
	addEventListeners: function()
	{
		///	<summary>
		///		Adds event listeners to the control's elements.
		///	</summary>
		
		if (this.posterElement)
			PlayerFramework.addEvent(this.posterElement, "click", PlayerFramework.proxy(this, this.onDocumentMouseUp));
		
		if (this.playOverlayControlElement)
			PlayerFramework.addEvent(this.playOverlayControlElement, "click", PlayerFramework.proxy(this, this.onPlayClick));

		PlayerFramework.addEvent(this.toggleControlElement, "click", PlayerFramework.proxy(this, this.onToggleClick));
		PlayerFramework.addEvent(this.playControlElement, "click", PlayerFramework.proxy(this, this.onPlayClick));
		PlayerFramework.addEvent(this.muteControlElement, "click", PlayerFramework.proxy(this, this.onMuteClick));
		PlayerFramework.addEvent(this.fullBrowserControlElement, "click", PlayerFramework.proxy(this, this.onFullBrowserClick));
		PlayerFramework.addEvent(this.timelineContainerElement, "mousedown", PlayerFramework.proxy(this, this.onTimelineMouseDown));
		PlayerFramework.addEvent(this.timelineFillElement, "click", PlayerFramework.proxy(this, this.onTimelineFillClick));
		PlayerFramework.addEvent(this.timelineEmptyElement, "click", PlayerFramework.proxy(this, this.onTimelineEmptyClick));
		PlayerFramework.addEvent(this.volumeContainerElement, "mousedown", PlayerFramework.proxy(this, this.onVolumeMouseDown));
		PlayerFramework.addEvent(this.volumeFillElement, "click", PlayerFramework.proxy(this, this.onVolumeFillClick));
		PlayerFramework.addEvent(this.volumeEmptyElement, "click", PlayerFramework.proxy(this, this.onVolumeEmptyClick));
		PlayerFramework.addEvent(document, "mousemove", PlayerFramework.proxy(this, this.onDocumentMouseMove));
		PlayerFramework.addEvent(document, "mouseup", PlayerFramework.proxy(this, this.onDocumentMouseUp));
		PlayerFramework.addEvent(document, "keydown", PlayerFramework.proxy(this, this.onDocumentKeyDown));

		var mouseOutProxy = PlayerFramework.mouseEventProxy(this.player, "mouseout");
		var mouseOverProxy = PlayerFramework.mouseEventProxy(this.player, "mouseover");
		PlayerFramework.addEvent(this.element, "mouseout", mouseOutProxy ? mouseOutProxy : PlayerFramework.proxy(this, this.onMouseOut));
		PlayerFramework.addEvent(this.element, "mouseover", mouseOverProxy ? mouseOverProxy : PlayerFramework.proxy(this, this.onMouseOver));
	},

	onMouseOut: function () {
	    this.player.dispatchEvent({ type: "mouseout" });
	},

	onMouseOver: function()
	{
	    this.player.dispatchEvent({ type: "mouseover" });
	},

	createControl: function()
	{
		///	<summary>
		///		Creates and appends all markup for the controls to the DOM.
		///	</summary>

		// Control Strip Container
		this.element = PlayerFramework.createElement(this.player.containerElement,
		[
			"div",
			{
				"class": this.player.options.overlayControls ? "pf-controls pf-controls-overlay" : "pf-controls"
			}
		]);

		// Toggle Control
		this.toggleControlElement = PlayerFramework.createElement(this.element,
		[
			"div",
			{
				"class": "pf-toggle-control"
			}
		]);

		this.toggleButtonElement = PlayerFramework.createElement(this.toggleControlElement,
		[
			"button",
			{
				"class": "pf-button",
				type: "button",
				title: this.options.strings.toggleTitle,
				accessKey: this.options.accessKeys.toggleKey
			},
			[
				"span"
			]
		]);

		// Play/Pause Control
		this.playControlElement = PlayerFramework.createElement(this.element,
		[
			"div",
			{
				"class": "pf-play-control pf-play"
			}
		]);

		this.playButtonElement = PlayerFramework.createElement(this.playControlElement,
		[
			"button",
			{
				"class": "pf-button",
				type: "button",
				title: this.options.strings.playTitle,
				accessKey: this.options.accessKeys.playKey
			},
			[
				"span"
			]
		]);

		// Timeline Control
		this.timelineControlElement = PlayerFramework.createElement(this.element,
		[
			"div",
			{
				"class": "pf-timeline-control"
			}
		]);

		this.timelineButtonElement = PlayerFramework.createElement(this.timelineControlElement,
		[
			"button",
			{
				"class": "pf-button",
				type: "button"
			}
		]);

		this.timelineContainerElement = PlayerFramework.createElement(this.timelineControlElement,
		[
			"div",
			{
				"class": "pf-slider-container"
			},
			[
				"span",
				{
					"class": "pf-slider-range"
				}
			]
		]);

		this.timelineLoadElement = PlayerFramework.createElement(this.timelineContainerElement,
		[
			"span",
			{
				"class": "pf-slider-load"
			}
		]);

		this.timelineFillElement = PlayerFramework.createElement(this.timelineContainerElement,
		[
			"span",
			{
				"class": "pf-slider-fill"
			},
			[
				"button",
				{
					"class": "pf-button",
					type: "button",
					title: this.options.strings.timelineFillTitle
				}
			]
		]);

		this.timelineEmptyElement = PlayerFramework.createElement(this.timelineContainerElement,
		[
			"span",
			{
				"class": "pf-slider-empty"
			},
			[
				"button",
				{
					"class": "pf-button",
					type: "button",
					title: this.options.strings.timelineEmptyTitle
				}
			]
		]);

		this.timelineHeadContainerElement = PlayerFramework.createElement(this.timelineContainerElement,
		[
			"span",
			{
				"class": "pf-slider-head-container"
			}
		]);

		this.timelineHeadElement = PlayerFramework.createElement(this.timelineHeadContainerElement,
		[
			"span",
			{
				"class": "pf-slider-head"
			}
		]);

		// Time Elapsed Control
		this.timeElapsedControlElement = PlayerFramework.createElement(this.element,
		[
			"div",
			{
				"class": "pf-time-elapsed-control pf-time-display",
				title: this.options.strings.timeElapsedTitle
			}
		]);

		this.timeElapsedElement = PlayerFramework.createElement(this.timeElapsedControlElement,
		[
			"div",
			"0:00:00"
		]);

		// Time Divider Control
		this.timeDividerControlElement = PlayerFramework.createElement(this.element,
		[
			"div",
			{
				"class": "pf-time-divider-control pf-time-display"
			},
			[
				"div",
				"/"
			]
		]);

		// Time Remaining Control
		this.timeRemainingControlElement = PlayerFramework.createElement(this.element,
		[
			"div",
			{
				"class": "pf-time-remaining-control pf-time-display",
				title: this.options.strings.timeRemainingTitle
			}
		]);

		this.timeRemainingElement = PlayerFramework.createElement(this.timeRemainingControlElement,
		[
			"div",
			"0:00:00"
		]);

		// Mute Control
		this.muteControlElement = PlayerFramework.createElement(this.element,
		[
			"div",
			{
				"class": "pf-mute-control pf-sound"
			}
		]);

		this.muteButtonElement = PlayerFramework.createElement(this.muteControlElement,
		[
			"button",
			{
				"class": "pf-button",
				type: "button",
				title: this.options.strings.muteTitle,
				accessKey: this.options.accessKeys.muteKey
			},
			[
				"span",
				{
					"class": "pf-mute-icon"
				},
				[
					"span",
					{
						"class": "pf-speaker-base"
					}
				],
				[
					"span",
					{
						"class": "pf-speaker"
					}
				],
				[
					"span",
					{
						"class": "pf-sound-waves"
					},
					[
						"span",
						{
							"class": "pf-sound-wave-1 pf-sound-wave"
						}
					],
					[
						"span",
						{
							"class": "pf-sound-wave-2 pf-sound-wave"
						}
					],
					[
						"span",
						{
							"class": "pf-sound-wave-3 pf-sound-wave"
						}
					]
				]
			]
		]);

		// Volume Control
		this.volumeControlElement = PlayerFramework.createElement(this.element,
		[
			"div",
			{
				"class": "pf-volume-control"
			}
		]);

		this.volumeButtonElement = PlayerFramework.createElement(this.volumeControlElement,
		[
			"button",
			{
				"class": "pf-button",
				type: "button"
			}
		]);

		this.volumeContainerElement = PlayerFramework.createElement(this.volumeControlElement,
		[
			"div",
			{
				"class": "pf-slider-container"
			},
			[
				"span",
				{
					"class": "pf-slider-range"
				}
			]
		]);

		this.volumeFillElement = PlayerFramework.createElement(this.volumeContainerElement,
		[
			"span",
			{
				"class": "pf-slider-fill"
			},
			[
				"button",
				{
					"class": "pf-button",
					type: "button",
					title: this.options.strings.volumeFillTitle
				}
			]
		]);

		this.volumeEmptyElement = PlayerFramework.createElement(this.volumeContainerElement,
		[
			"span",
			{
				"class": "pf-slider-empty"
			},
			[
				"button",
				{
					"class": "pf-button",
					type: "button",
					title: this.options.strings.volumeEmptyTitle
				}
			]
		]);

		this.volumeHeadContainerElement = PlayerFramework.createElement(this.volumeContainerElement,
		[
			"span",
			{
				"class": "pf-slider-head-container"
			}
		]);

		this.volumeHeadElement = PlayerFramework.createElement(this.volumeHeadContainerElement,
		[
			"span",
			{
				"class": "pf-slider-head"
			}
		]);

		// Full Browser Control
		this.fullBrowserControlElement = PlayerFramework.createElement(this.element,
		[
			"div",
			{
				"class": "pf-full-browser-control"
			}
		]);

		this.fullBrowserButtonElement = PlayerFramework.createElement(this.fullBrowserControlElement,
		[
			"button",
			{
				"class": "pf-button",
				type: "button",
				title: this.options.strings.fullBrowserTitle,
				accessKey: this.options.accessKeys.fullBrowserKey
			},
			[
				"span",
				{
					"class": "pf-full-browser-box"
				}
			]
		]);

		this.updateLayout();
	},

	showControlStrip: function()
	{
		///	<summary>
		///		Shows the control strip.
		///	</summary>

		if (this.playInitiated || (this.canPlayThrough && !this.player.options.overlayPlayButton))
		{
			this.element.style.display = "block";
			this.toggleButtonElement.setAttribute("title", this.options.strings.toggleAltTitle);
		}
	},

	hideControlStrip: function()
	{
		///	<summary>
		///		Hides the control strip.
		///	</summary>

		if (this.playInitiated)
		{
			this.element.style.display = "none";
			this.toggleButtonElement.setAttribute("title", this.options.strings.toggleTitle);
		}
	},

	enterFullScreen: function()
	{
		///	<summary>
		///		Expands the media element to the full width and height of the browser. 
		///	</summary>

		this.player.containerElement.className = "pf-container pf-full-browser";
		this.elementWidthBeforeFullBrowser = this.mediaPlugin().element.width;
		this.elementHeightBeforeFullBrowser = this.mediaPlugin().element.height;
		this.player.containerElement.style.width = "";
		this.player.containerElement.style.height = "";
		this.fullBrowserButtonElement.setAttribute("title", this.options.strings.fullBrowserAltTitle);
	},

	exitFullScreen: function()
	{
		///	<summary>
		///		Restores the media element to it's original size. 
		///	</summary>

		this.player.containerElement.className = "pf-container";
		this.mediaPlugin().element.width = this.elementWidthBeforeFullBrowser;
		this.mediaPlugin().element.height = this.elementHeightBeforeFullBrowser;
		this.player.containerElement.style.width = this.mediaPlugin().element.width.toString() + "px";
		this.player.containerElement.style.height = this.mediaPlugin().element.height.toString() + "px";
		this.fullBrowserButtonElement.setAttribute("title", this.options.strings.fullBrowserTitle);
	},

	formatTimeString: function(totalSeconds)
	{
		///	<summary>
		///		Formats the seconds in a string using the template "0:00:00".
		///	</summary>
		///	<param name="totalSeconds" type="Number">
		///		The total seconds to format.
		///	</param>
		///	<returns type="String" />

		var dateTime = new Date(0,0,0,0,0,0,0);
		dateTime.setSeconds(totalSeconds);
		
		var hours = PlayerFramework.padString(dateTime.getHours(), 1, "0");
		var minutes = PlayerFramework.padString(dateTime.getMinutes(), 2, "0");
		var seconds = PlayerFramework.padString(dateTime.getSeconds(), 2, "0");

		return hours + ":" + minutes + ":" + seconds;
	},

	getSliderMouseFraction: function(measurements, mouseEvent)
	{
		///	<summary>
		///		Gets the fraction of the mouse in relation to the slider in a range of 0.0 to 1.0.
		///	</summary>
		///	<param name="measurements" type="Object">
		///		Contains the sizing and positioning of the timeline at the time that the mouse was depressed.
		///	</param>
		///	<param name="mouseEvent" type="Object">
		///		The mouse event object.
		///	</param>

		var mouseTimelineOffset = mouseEvent.clientX - (measurements.headWidth / 2) - measurements.headContainerLeft;
		return Math.min(1, Math.max(0, mouseTimelineOffset / measurements.headContainerWidth));
	},

	scrubbing: function()
	{
		///	<summary>
		///		Indicates if the timeline is currently being scrubbed.
		///	</summary>
		///	<returns type="Boolean" />
		
		return this.isTimelineMouseDown;
	},

	setSliderHeadPosition: function(containerElement, percentage)
	{
		///	<summary>
		///		Sets the position of the slider head in relation to the slider.
		///	</summary>
		///	<param name="containerElement" type="Object">
		///		The container element for the slider.
		///	</param>
		///	<param name="percentage" type="Object">
		///		The fraction of the slider head in relation to the slider in a range of 0.0 to 1.0.
		///	</param>

		var percentString = 100 * percentage + "%";
		var percentEmptyString = 100 * (1 - percentage) + "%";
		PlayerFramework.getElementsByClass("pf-slider-fill", containerElement)[0].style.width = percentString;
		PlayerFramework.getElementsByClass("pf-slider-head", containerElement)[0].style.left = percentString;
		PlayerFramework.getElementsByClass("pf-slider-empty", containerElement)[0].style.width = percentEmptyString;
	},

	updateLayout: function()
	{
		///	<summary>
		///		Updates the layout of the control strip to accomodate all displayed controls.
		///	</summary>

		var childNodes = PlayerFramework.convertNodeListToArray(this.element.childNodes);

		var right = 0;
		for(var i = childNodes.length - 1; i >= 0; i--)
		{
			var node = childNodes[i];

			node.style.right = right + "px";
			
			if (PlayerFramework.getComputedStyle(node, "display") != "none")
				right += parseInt(PlayerFramework.getComputedStyle(node, "width"), 10);

			if (node.className == "pf-timeline-control")
				break;
		}

		var left = 0;
		for(var i = 0; i < childNodes.length; i++)
		{
			var node = childNodes[i];

			node.style.left = left + "px";
			
			if (PlayerFramework.getComputedStyle(node, "display") != "none")
				left += parseInt(PlayerFramework.getComputedStyle(node, "width"), 10);

			if (node.className == "pf-timeline-control")
				break;
		}
	}
});

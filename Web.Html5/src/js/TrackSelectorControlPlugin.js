PlayerFramework.Plugins.TrackSelectorControlPlugin = PlayerFramework.ControlPlugin.extend(
{
	defaultOptions: function(player)
	{
		return PlayerFramework.mergeOptions(this._super(),
		{
			strings:
			{
				trackSelectorTitle: "Select Captions/Subtitles",
				trackSelectorLabel: "CC",
				trackOffOptionLabel: "(None)"
			},
			accessKeys:
			{
				trackSelectorKey: "t"
			},
			kind: "captions",
			trackPlugin: PlayerFramework.first(player.modules, function(m)
			{
				return m instanceof PlayerFramework.TrackPlugin && m.options.kind == "captions";
			}),
			controlStripPlugin: PlayerFramework.first(player.modules, function(m)
			{
				return m instanceof PlayerFramework.Plugins.ControlStripPlugin;
			})
		});
	},

	isEnabled: function(player, options)
	{
		return !!options.trackPlugin && !!options.controlStripPlugin;
	},

	init: function(player, options)
	{
		///	<summary>
		///		Initializes the TrackSelectorControlPlugin that provides UI for selecting a text track.
		///	</summary>
		///	<param name="player" type="Object">
		///		The Player object.
		///	</param>
		///	<param name="options" type="Object">
		///		The options for the TrackSelectorControlPlugin.
		///	</param>
		///	<returns type="TrackSelectorControlPlugin" />

		this._super(player, options);

		// Find the required plugins.
		this.trackPlugin = this.options.trackPlugin;
		this.controlStripPlugin = this.options.controlStripPlugin;

		if (!this.trackPlugin)
			throw new Error("TrackPlugin required.");

		if (!this.controlStripPlugin)
			throw new Error("ControlStripPlugin required.");
	},

	// Event Handlers
	onTextTrackAdded: function(e)
	{		
		///	<summary>
		///		Called when a chapter cue is added. Displays the chapter navigation arrows next to the timeline.
		///	</summary>

		if (this.controlStripPlugin.element.className.indexOf("pf-track-selector") === -1)
			this.createControl();
	},

	onPlayerMouseOut: function(e)
	{
		///	<summary>
		///		Called when the mouse leaves the player.
		///	</summary>
		
		if (this.popUpSelectorControl && this.player.options.overlayControls)
			this.popUpSelectorControl.style.display = "none";
	},

	onPlayerMouseOver: function(e)
	{
		///	<summary>
		///		Called when the mouse enters the player.
		///	</summary>

		if (this.popUpSelectorControl && this.player.options.overlayControls)
			this.popUpSelectorControl.style.display = "block";		
	},

	onPopUpSelectorOptionClick: function(e)
	{
		if (!this.trackPlugin.isActiveTextTrack(e.textTrack))
		{
			PlayerFramework.forEach(this.player.textTracks, PlayerFramework.proxy(this, function(t)
			{
				if (t.kind == this.options.kind)
					this.trackPlugin.deactivateTextTrack(t);
			}));

			if (e.textTrack.label != this.options.strings.trackOffOptionLabel)
				this.trackPlugin.activateTextTrack(e.textTrack);
		}

		this.removePopUpSelectorControl();
	},

	onSelectTrackClick: function(e)
	{		
		///	<summary>
		///		Called when the skip forward chapter button is clicked.
		///	</summary>

		if (!this.popUpSelectorControl)
			this.createPopUpSelectorControl();
		else
			this.removePopUpSelectorControl();
	},

	onReady: function(e)
	{		
		///	<summary>
		///		Called when the player is ready for playback.
		///	</summary>

		this.addEventListeners();
	},

	// Functions
	addEventListeners: function()
	{
		///	<summary>
		///		Adds event listeners to the control's elements.
		///	</summary>

		PlayerFramework.addEvent(this.player, "texttrackadded", PlayerFramework.proxy(this, this.onTextTrackAdded));		
	},

	createControl: function()
	{
		///	<summary>
		///		Creates and appends all markup for the controls to the DOM.
		///	</summary>

		this.controlStripPlugin.element.className += " pf-track-selector";

		this.selectTrackControlElement = PlayerFramework.createElement(null,
		[
			"div",
			{
				"class": "pf-track-selector-control"
			},
			[
				"button",
				{
					"class": "pf-button",
					type: "button",
					title: this.options.strings.trackSelectorTitle,
					accessKey: this.options.accessKeys.trackSelectorKey
				},
				[
					"span",
					{
						"class": "pf-track-selector-box"
					},
					this.options.strings.trackSelectorLabel
				]
			]
		]);
		
		PlayerFramework.addEvent(this.selectTrackControlElement, "click", PlayerFramework.proxy(this, this.onSelectTrackClick));

		this.controlStripPlugin.element.insertBefore(this.selectTrackControlElement, this.controlStripPlugin.fullBrowserControlElement);

		this.controlStripPlugin.updateLayout();
	},
	
	createPopUpSelectorControl: function()
	{
		var textTracks = PlayerFramework.filter(this.player.textTracks, PlayerFramework.proxy(this, function(t)
		{
			return t.kind == this.options.kind;
		}));

		var selectorHeight = (textTracks.length + 1) * 22;
		var containerHeight = this.player.containerElement.offsetHeight;

		this.popUpSelectorControl = PlayerFramework.createElement(null,
		[
			"div",
			{
				"class": "pf-popup-track-selector-control",
				style: this.style(
				{
					bottom: this.player.options.overlayControls ? this.controlStripPlugin.element.offsetHeight + "px": "0",
					height: (selectorHeight < containerHeight ? selectorHeight : containerHeight) + "px",
					"overflow-x": "hidden",
					"overflow-y": (selectorHeight < containerHeight ? "hidden" : "scroll")
				})
			}
		]);

		var hasActiveTextTrack = false;
		PlayerFramework.forEach(textTracks, PlayerFramework.proxy(this, function(textTrack)
		{
			var isActiveTextTrack = this.trackPlugin.isActiveTextTrack(textTrack);

			if (isActiveTextTrack)
				hasActiveTextTrack = true;

			this.createPopUpSelectorOptionControl(textTrack, isActiveTextTrack);
		}));

		this.createPopUpSelectorOptionControl({ label: this.options.strings.trackOffOptionLabel }, !hasActiveTextTrack);
		
		PlayerFramework.addEvent(this.popUpSelectorControl, "mouseover", PlayerFramework.mouseEventProxy(this.player, "mouseover"));
		PlayerFramework.addEvent(this.popUpSelectorControl, "mouseout", PlayerFramework.mouseEventProxy(this.player, "mouseout"));

		this.player.containerElement.insertBefore(this.popUpSelectorControl, this.trackPlugin.element.nextSibling);
	},

	createPopUpSelectorOptionControl: function(textTrack, isSelected)
	{
		var popUpSelectorOption = PlayerFramework.createElement(this.popUpSelectorControl,
		[
			"div",
			{
				"class": "pf-popup-track-selector-option" + (isSelected ? " selected" : "")
			},
			[
				"button",
				{
					"class": "pf-button",
					type: "button",
					title: textTrack.label
				},
				textTrack.label
			]
		]);

		PlayerFramework.proxy(this, function(textTrack)
		{
			PlayerFramework.addEvent(popUpSelectorOption, "click", PlayerFramework.proxy(this, function(e)
			{
				this.onPopUpSelectorOptionClick(
				{
					textTrack: textTrack
				});
			}));
		})(textTrack);
	},

	removePopUpSelectorControl: function()
	{
		this.player.containerElement.removeChild(this.popUpSelectorControl);
		this.popUpSelectorControl = null;
	},

	style: function(styles)
	{
		var style = "";
		for(var name in styles)
		{
			style += name + ": " + styles[name] + "; ";
		}

		return style;
	}
});

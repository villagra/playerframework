PlayerFramework.Plugins.TimelineTrackControlPlugin = PlayerFramework.ControlPlugin.extend(
{
	defaultOptions: function(player)
	{
		return PlayerFramework.mergeOptions(this._super(),
		{
			timelineTrackPlugin: PlayerFramework.first(player.modules, function(m)
			{
				return m instanceof PlayerFramework.Plugins.TimelineTrackPlugin;
			}),
			controlStripPlugin: PlayerFramework.first(player.modules, function(m)
			{
				return m instanceof PlayerFramework.Plugins.ControlStripPlugin;
			})
		});
	},

	isEnabled: function(player, options)
	{
		return !!options.timelineTrackPlugin && !!options.controlStripPlugin;
	},

	init: function(player, options)
	{
		///	<summary>
		///		Initializes the TimelineTrackControlPlugin that provides UI for displaying and controlling timeline cues.
		///	</summary>
		///	<param name="player" type="Object">
		///		The Player object.
		///	</param>
		///	<param name="options" type="Object">
		///		The options for the TimelineTrackControlPlugin.
		///	</param>
		///	<returns type="TimelineTrackControlPlugin" />

		this._super(player, options);

		// Find the required plugins.
		this.timelineTrackPlugin = this.options.timelineTrackPlugin;
		this.controlStripPlugin = this.options.controlStripPlugin;

		if (!this.timelineTrackPlugin)
			throw new Error("TimelineTrackPlugin required.");

		if (!this.controlStripPlugin)
			throw new Error("ControlStripPlugin required.");

		this.cuesPositioned = false;
		this.cueElements = [];
	},

	// Event Handlers	
	onTimelineCueAdded: function(e)
	{		
		///	<summary>
		///		Called when a timeline cue is added. Displays the cue control on the timeline.
		///	</summary>
		
		var cueElement = PlayerFramework.createElement(this.timelineCueContainerElement,
		[
			"div",
			{
				"class": "pf-timeline-cue"
			}
		]);

		cueElement.cue = e.cue;

		PlayerFramework.addEvent(cueElement, "click", PlayerFramework.proxy(this, function()
		{
			this.onTimelineCueClicked(e.cue);
		}));

		this.cueElements.push(cueElement);

		if (this.player.duration())
			this.positionCue(cueElement);
	},

	onTimelineCueClicked: function(e)
	{		
		///	<summary>
		///		Called when a timeline cue is clicked. Seeks to the position represented by
		///		the timeline cue.
		///	</summary>
		
		this.player.currentTime(e.startTime);
	},

	onTimelineCueRemoved: function(e)
	{
		///	<summary>
		///		Called when a timeline cue is removed. Removes the cue control from the timeline.
		///	</summary>

		for(var i = 0; i < this.cueElements.length; i++)
		{
			var cueElement = this.cueElements[i];
			if (cueElement.cue == e.cue)
			{
				this.timelineCueContainerElement.removeChild(cueElement);
				this.cueElements.splice(i, 1);
				break;
			}
		}	
	},

	onPlay: function(e)
	{
		///	<summary>
		///		Called when the media plays. Displays and positions all cues. 
		///	</summary>

		if (!this.cuesPositioned)
		{
			this.cuesPositioned = true;
			PlayerFramework.forEach(this.cueElements, PlayerFramework.proxy(this, this.positionCue));
		}
	},

	onReady: function(e)
	{
		///	<summary>
		///		Called when the player is ready for playback. Creates the timeline cue controls and adds event listeners.
		///	</summary>

		this.createControl();
		this.addEventListeners();
	},

	onUnloadingMediaPlugin: function(e)
	{
		///	<summary>
		///		Called when the media element unloads. Removes all cue controls from the timeline.
		///	</summary>

		this.cuesPositioned = false;
		this.cueElements = [];
	},

	// Functions
	addEventListeners: function()
	{
		///	<summary>
		///		Adds event listeners to the control's elements.
		///	</summary>

		PlayerFramework.addEvent(this.player, "timelinecueadded", PlayerFramework.proxy(this, this.onTimelineCueAdded));
		PlayerFramework.addEvent(this.player, "timelinecueremoved", PlayerFramework.proxy(this, this.onTimelineCueRemoved));
		PlayerFramework.addEvent(this.player, "timeupdate", PlayerFramework.proxy(this, this.onTimeUpdate));
	},

	createControl: function()
	{
		///	<summary>
		///		Creates and appends all markup for the controls to the DOM.
		///	</summary>

		this.timelineCueContainerElement = PlayerFramework.createElement(this.controlStripPlugin.timelineControlElement,
		[
			"div",
			{
				"class": "pf-timeline-cues-container"
			}
		]);
	},

	positionCue: function(cueElement)
	{
		///	<summary>
		///		Positions the specified cue element on the timeline.
		///	</summary>
		///	<param name="cueElement" type="Object">
		///		The event object.
		///	</param>
		
		cueElement.style.left = 100 * cueElement.cue.startTime / this.player.duration() + "%";
		cueElement.style.display = "block";
	}
});

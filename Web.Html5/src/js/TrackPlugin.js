PlayerFramework.TrackPlugin = PlayerFramework.ModulePlugin.extend(
{
	defaultOptions: function(player)
	{
		return PlayerFramework.mergeOptions(this._super(),
		{
			displayPreference: PlayerFramework.TextTrack.DisplayPreference.CUSTOM
		});
	},

	init: function(player, options)
	{
		///	<summary>
		///		Initializes the TrackPlugin base.
		///	</summary>
		///	<param name="player" type="Object">
		///		The Player object.
		///	</param>
		///	<param name="options" type="Object">
		///		The options for the TrackPlugin.
		///	</param>
		///	<returns type="TrackPlugin" />

		this._super(player, options);

		this.previousTime = -1;
		this.previousScrubbingValue = false;
		this.lastDirection = 1;
		this.activeTextTrack = null;
		this.isTrackPolyfill = false;

		this.addEventListeners();
	},

	// Event Handlers
	onCueAdded: function(e)
	{
		///	<summary>
		///		When overridden in a derived class, processes an added cue. 
		///	</summary>
	},

	onCueChange: function(e)
	{
		///	<summary>
		///		When overridden in a derived class, processes a changed cue.
		///	</summary>
	},

	onCueLeft: function(e)
	{
		///	<summary>
		///		When overridden in a derived class, processes an newly inactive cue.
		///	</summary>
	},

	onCueReached: function(e)
	{
		///	<summary>
		///		When overridden in a derived class, processes an newly active cue.
		///	</summary>
	},

	onCueRemoved: function(e)
	{
		///	<summary>
		///		When overridden in a derived class, processes a removed cue. 
		///	</summary>
	},

	onCueSkipped: function(e)
	{
		///	<summary>
		///		When overridden in a derived class, processes a skipped cue.
		///	</summary>
	},

	onLoadedMediaPlugin: function(e)
	{
		///	<summary>
		///		Called when the media element unloads.
		///	</summary>
		
		var optionCues = this.player.mediaPlugin.playerOptions[this.options.playerExtensionPropertyName];

		if (optionCues)
		{
			PlayerFramework.forEach(optionCues, PlayerFramework.proxy(this, function(cue)
			{
				this.addCue(cue);
			}));
		}
	},

	onScrubbed: function(e)
	{
		///	<summary>
		///		Called when the timeline is no longer being scrubbed.
		///	</summary>
		
		this.updateCues();
	},
	
	onTimeUpdate: function(e)
	{
		///	<summary>
		///		Called when the current time of the media is updated.
		///	</summary>

		this.updateCues();
	},

	updateCues: function()
	{
		///	<summary>
		///		Determines when to cues have been reached, left, skipped, and changed
		///		depending on if playback is seeking, playing, or if the timeline is being scrubbed.
		///	</summary>

		if (!this.activeTextTrack)
			return;

		var scrubbedTo = this.player.scrubbing() && this.previousScrubbingValue;
		var playedTo = !this.player.scrubbing() && !this.previousScrubbingValue;
		var seekedTo = !this.player.scrubbing() && this.previousScrubbingValue;
		
		var currentTime = this.player.currentTime();
		var previousTime = this.previousTime;
		var cueChange = false;
		var activeCues = [];

		for (var i = 0; i < this.activeTextTrack.cues.length; i++)
		{
			var cue = this.activeTextTrack.cues[i];

			// Cue reached
			if (scrubbedTo && ((previousTime < cue.startTime && currentTime >= cue.startTime) || (previousTime > cue.endTime && currentTime <= cue.endTime)))
			{
				this.onCueReached({ cue: cue, seeked: seekedTo, direction: this.lastDirection });
			}
			else if (!cue.committed && (seekedTo || playedTo) && currentTime >= cue.startTime && currentTime <= cue.endTime)
			{
				cue.committed = true;
				this.onCueReached({ cue: cue, seeked: seekedTo, direction: this.lastDirection });
			}

			// Cue left
			if (scrubbedTo && ((previousTime >= cue.startTime && currentTime < cue.startTime) || (previousTime <= cue.endTime && currentTime > cue.endTime)))
			{
				this.onCueLeft({ cue: cue, seeked: seekedTo, direction: this.lastDirection });
			}
			else if (cue.committed && (seekedTo || playedTo) && (currentTime < cue.startTime || currentTime > cue.endTime))
			{
				cue.committed = false;
				this.onCueLeft({ cue: cue, seeked: seekedTo, direction: this.lastDirection });
			}

			// Cue skipped via seek
			var seekSkipForward = previousTime < cue.startTime && currentTime > cue.endTime;
			var seekSkipBackward = previousTime > cue.endTime && currentTime < cue.startTime;

			if (seekSkipForward || seekSkipBackward)
			{
				var direction = seekSkipForward ? 1 : -1;
				this.onCueSkipped({ cue: cue, direction: direction });
			}

			// Cue skipped via scrub
			var scrubSkipForward = this.scrubStartCurrentTimeValue < cue.startTime && currentTime > cue.endTime;
			var scrubSkipBackward = this.scrubStartCurrentTimeValue > cue.endTime && currentTime < cue.startTime;

			if (!this.scrubStartCurrentTimeValue && this.previousScrubbingValue)
			{
				this.scrubStartCurrentTimeValue = currentTime;
			}
			else if (this.scrubStartCurrentTimeValue && !this.player.scrubbing() && (scrubSkipForward || scrubSkipBackward))
			{
				var direction = scrubSkipForward ? 1 : -1;
				this.onCueSkipped({ cue: cue, direction: direction });
			}

			// Cue enter/exit
			if (currentTime >= cue.startTime && currentTime <= cue.endTime && (previousTime > cue.endTime || previousTime < cue.startTime))
			{
				activeCues.push(cue);
				cue.dispatchEvent({ type: "enter" });
				cueChange = true;
			}
			else if (previousTime >= cue.startTime && previousTime <= cue.endTime && (currentTime > cue.endTime || currentTime < cue.startTime))
			{
				cue.dispatchEvent({ type: "exit" });
				cueChange = true;
			}
		}

		if (cueChange)
		{
			this.activeTextTrack.activeCues = activeCues;
			this.onCueChange();
		}

		if (this.scrubStartCurrentTimeValue && !this.player.scrubbing())
			this.scrubStartCurrentTimeValue = 0;
		
		if (previousTime < currentTime)
			this.lastDirection = 1;
		else if (previousTime > currentTime)
			this.lastDirection = -1;

		this.previousTime = currentTime;
		this.previousScrubbingValue = this.player.scrubbing();
	},

	onUnloadingMediaPlugin: function(e)
	{
		///	<summary>
		///		Called when the media element unloads.
		///	</summary>

		if (!this.activeTextTrack)
			return;

		PlayerFramework.forEach(this.activeTextTrack.cues, PlayerFramework.proxy(this, function(cue)
		{
			this.removeCue(cue);
		}));
	},

	// Functions
	addCue: function(cue)
	{
		///	<summary>
		///		Adds the specified cue to the ordered cue array and calls the addCue
		///		function of the subclass.
		///	</summary>
		///	<param name="cue" type="Object">
		///		The cue to be added.
		///	</param>
		
		if (!this.activeTextTrack)
			return;

		this.activeTextTrack.addCue(cue);
		this.onCueAdded({ cue: cue });
	},

	addEventListeners: function()
	{
		///	<summary>
		///		Adds event listeners to the control's elements.
		///	</summary>
		
		this.player.addEventListener("loadedmediaplugin", PlayerFramework.proxy(this, this.onLoadedMediaPlugin), false);
		this.player.addEventListener("scrubbed", PlayerFramework.proxy(this, this.onScrubbed), false);
		this.player.addEventListener("timeupdate", PlayerFramework.proxy(this, this.onTimeUpdate), false);
		this.player.addEventListener("unloadingmediaplugin", PlayerFramework.proxy(this, this.onUnloadingMediaPlugin), false);
	},

	activateTextTrack: function(textTrack)
	{
		///	<summary>
		///		Handles downloading of a text track if not already downloaded
		///		and then sets the text track as the active text track.
		///	</summary>
		///	<param name="textTrack" type="Object">
		///		The text track to activate.
		///	</param>

		if (!textTrack.xml && textTrack.src)
		{
			textTrack.readyState = PlayerFramework.TextTrack.ReadyState.LOADING;

			PlayerFramework.xhr({ url: textTrack.src }, PlayerFramework.proxy(this, function(result)
			{
				textTrack.xml = result.responseXML;
				this.processTextTrackSource(textTrack);
		
				textTrack.readyState = PlayerFramework.TextTrack.ReadyState.LOADED;
				textTrack.dispatchEvent({ type: "load" });

				this.showTextTrack(textTrack);
			}), PlayerFramework.proxy(this, PlayerFramework.proxy(this, function(result)
			{
				textTrack.readyState = PlayerFramework.TextTrack.ReadyState.ERROR;
				this.player.dispatchEvent({ type: "error" });
			})));
		}
		else
		{
			this.showTextTrack(textTrack);
		}
	},

	showTextTrack: function(textTrack)
	{
		///	<summary>
		///		Shows the text track according to the display preference.
		///	</summary>
		///	<param name="textTrack" type="Object">
		///		The text track to show.
		///	</param>

		switch (this.options.displayPreference)
		{
			// NATIVE
			case PlayerFramework.TextTrack.DisplayPreference.NATIVE:
				
				textTrack.mode = PlayerFramework.TextTrack.Mode.OFF;

				if (textTrack.track)
					textTrack.track.mode = PlayerFramework.TextTrack.Mode.SHOWING;

				break;

			// CUSTOM
			case PlayerFramework.TextTrack.DisplayPreference.CUSTOM:

				textTrack.mode = PlayerFramework.TextTrack.Mode.SHOWING;

				if (textTrack.track)
					textTrack.track.mode = PlayerFramework.TextTrack.Mode.OFF;

				break;

			// ALL
			case PlayerFramework.TextTrack.DisplayPreference.ALL:
						
				textTrack.mode = PlayerFramework.TextTrack.Mode.SHOWING;

				if (textTrack.track)
					textTrack.track.mode = PlayerFramework.TextTrack.Mode.SHOWING;
						
				break;
			
			// NONE
			default:

				textTrack.mode = PlayerFramework.TextTrack.Mode.OFF;

				if (textTrack.track)
					textTrack.track.mode = PlayerFramework.TextTrack.Mode.OFF;

				break;
		}

		this.activeTextTrack = textTrack;
		this.previousTime = -1;

		this.updateCues();
	},

	deactivateTextTrack: function(textTrack)
	{
		///	<summary>
		///		Deactivates the text track.
		///	</summary>
		///	<param name="textTrack" type="Object">
		///		The text track to deactivate.
		///	</param>

		textTrack.mode = PlayerFramework.TextTrack.Mode.OFF;

		if (textTrack.track)
			textTrack.track.mode = PlayerFramework.TextTrack.Mode.OFF;

		this.activeTextTrack = null;
		this.clearCaptionContainer();
	},

	isActiveTextTrack: function(textTrack)
	{
		///	<summary>
		///		Determines if the specified text track is the currently active text track.
		///	</summary>
		///	<param name="textTrack" type="Object">
		///		The text track to check if it is active.
		///	</param>

		return this.activeTextTrack == textTrack;
	},

	processTextTrackSource: function(textTrack)
	{
		///	<summary>
		///		Processes a downloaded text track using a TTML parser.
		///	</summary>
		///	<param name="textTrack" type="Object">
		///		The text track to process.
		///	</param>
	},

	removeCue: function(cue)
	{
		///	<summary>
		///		Removes the specified cue to the ordered cue array and calls the removeCue
		///		function of the subclass.
		///	</summary>
		///	<param name="cue" type="Object">
		///		The cue to be added.
		///	</param>

		if (!this.activeTextTrack)
			return;

		this.activeTextTrack.removeCue(cue);
		this.onCueRemoved({ cue: cue });
	}
});

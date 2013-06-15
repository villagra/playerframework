PlayerFramework.Plugins.TrackDataProviderPlugin = PlayerFramework.ModulePlugin.extend(
{
	init: function(player, options)
	{
		///	<summary>
		///		Initializes the TrackDataProviderPlugin that is a polyfill for the W3C <track> implementation.
		///	</summary>
		///	<param name="player" type="Object">
		///		The Player object.
		///	</param>
		///	<param name="options" type="Object">
		///		The options for the CueDataProviderPlugin.
		///	</param>
		///	<returns type="CueDataProviderPlugin" />

		this._super(player, options);

		this.addEventListeners();

		this.isTrackSupported = !!document.createElement("track").track;

		this.player.textTracks = [];

		this.player.addTextTrack = PlayerFramework.proxy(this, this.addTextTrack);
	},

	// Event Handlers
	onLoadedMediaPlugin: function(e)
	{
		///	<summary>
		///		Called when the media element unloads.
		///	</summary>

		// Create or clear textTracks array
		this.player.textTracks = [];

		// Find all track existing elements (both created from options or pre-existing)
		var trackElements = PlayerFramework.convertNodeListToArray(this.player.mediaPlugin.element.getElementsByTagName("track"));
		
		// Extend all track elements with HTML5 Track API and download the referenced source files
		PlayerFramework.forEach(trackElements, PlayerFramework.proxy(this, this.extendTrackElement));

		var textTracks = this.player.mediaPlugin.playerOptions.textTracks;

		// Create tracks that were specified in the options
		if (textTracks)
		{
			for (var i = 0; i < textTracks.length; i++)
			{
				var textTrackOptions = textTracks[i];

				if (this.player.mediaPlugin.options.supportsTrackElements)
				{
					var trackElement = PlayerFramework.createElement(this.player.mediaPlugin.element,
					[
						"track",
						textTrackOptions
					]);
					
					this.extendTrackElement(trackElement);
				}
				else
				{
					var textTrack = this.player.addTextTrack(textTrackOptions.kind, textTrackOptions.label, textTrackOptions.srclang);
					textTrack.src = textTrackOptions.src;
					
					if (textTrackOptions["default"] != undefined && textTrackOptions.src)
						this.activateTextTrack(textTrack);
				}
			}
		}
	},

	// Functions	
	addEventListeners: function()
	{
		///	<summary>
		///		Adds event listeners to the control's elements.
		///	</summary>
		
		this.player.addEventListener("loadedmediaplugin", PlayerFramework.proxy(this, this.onLoadedMediaPlugin), false);
	},

	addTextTrack: function(kind, label, language, inStream)
	{
		///	<summary>
		///		Entry point for added a text track as specified by the W3C.
		///	</summary>
		///	<param name="kind" type="Object">
		///		The category the given track falls into. 
		///	</param>
		///	<param name="label" type="Object">
		///		The label of the track, if known, or the empty string otherwise.
		///	</param>
		///	<param name="language" type="Object">
		///		The language of the given track, if known, or the empty string otherwise.
		///	</param>

		var textTrack = new PlayerFramework.TextTrack();
		textTrack.kind = kind;
		textTrack.label = label;
		textTrack.language = language;
		textTrack.inStream = inStream;

		this.player.textTracks.push(textTrack);

		this.player.dispatchEvent(
		{
			type: "texttrackadded"
		});

		return textTrack;
	},
	
	activateTextTrack: function(textTrack)
	{
		///	<summary>
		///		Delegates activation of a text track to the TrackPlugin with the matching "kind".
		///	</summary>
		///	<param name="textTrack" type="Object">
		///		The text track to activate.
		///	</param>

		var trackPlugin = PlayerFramework.first(this.player.modules, PlayerFramework.proxy(this, function(m)
		{
			return m instanceof PlayerFramework.TrackPlugin && m.options.kind === textTrack.kind;
		}));

		if (trackPlugin)
			trackPlugin.activateTextTrack(textTrack);
	},

	extendTrackElement: function(trackElement)
	{
		///	<summary>
		///		Extends a track element to match the W3C specification.
		///	</summary>
		///	<param name="trackElement" type="Object">
		///		The text track to extend.
		///	</param>

		trackElement.isDefault = trackElement.getAttribute("default") != undefined;
		trackElement.kind = trackElement.getAttribute("kind");
		trackElement.label = trackElement.getAttribute("label");
		trackElement.srclang = trackElement.getAttribute("srclang");
		trackElement.src = trackElement.getAttribute("src");

		var textTrack = this.player.addTextTrack(trackElement.kind, trackElement.label, trackElement.srclang);
		textTrack.src = trackElement.src;
		
		if (this.isTrackSupported && trackElement.track)
		{
			textTrack.track = trackElement.track;
			textTrack.track.mode = PlayerFramework.TextTrack.Mode.OFF;
		}

		trackElement.track = textTrack;

		if (trackElement.isDefault && trackElement.src)
			this.activateTextTrack(textTrack);
	}
});

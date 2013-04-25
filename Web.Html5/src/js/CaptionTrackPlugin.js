PlayerFramework.Plugins.CaptionTrackPlugin = PlayerFramework.TrackPlugin.extend(
{
	defaultOptions: function(player)
	{
		return PlayerFramework.mergeOptions(this._super(),
		{
			playerExtensionPropertyName: "captions",
			kind: "captions"
		});
	},

	init: function(player, options)
	{
		///	<summary>
		///		Initializes the CaptionTrackPlugin that provides an API for adding, removing, and synchronizing captions.
		///		Parses and displays captions over the media.
		///	</summary>
		///	<param name="player" type="Object">
		///		The Player object.
		///	</param>
		///	<param name="options" type="Object">
		///		The options for the CaptionTrackPlugin.
		///	</param>
		///	<returns type="CaptionTrackPlugin" />

		this._super(player, options);

		this.player.addEventListener("ready", PlayerFramework.proxy(this, this.onReady), false);
	},

	// Event Handlers
	onCueAdded: function(e)
	{
		///	<summary>
		///		Processes an added cue.
		///	</summary>

		e.type = "captioncueadded";
		this.player.dispatchEvent(e);
	},

	onCueChange: function()
	{
		///	<summary>
		///		Processes a changed cue.
		///	</summary>
		
		if (this.activeTextTrack.mode === PlayerFramework.TextTrack.Mode.SHOWING)
		{
			this.clearCaptionContainer();
			
			for (var i = 0; i < this.activeTextTrack.activeCues.length; i++)
			{
				var cueElement = this.activeTextTrack.activeCues[i].getCueAsHTML().cloneNode(true);
				this.element.appendChild(cueElement);
			}
		}

		this.player.dispatchEvent(
		{
			type: "captioncuechange"
		});
	},

	onCueLeft: function(e)
	{
		///	<summary>
		///		Processes an newly inactive cue.
		///	</summary>

		e.type = "captioncueleft";
		this.player.dispatchEvent(e);
	},

	onCueReached: function(e)
	{
		///	<summary>
		///		Processes an newly active cue.
		///	</summary>

		e.type = "captioncuereached";
		this.player.dispatchEvent(e);
	},

	onCueRemoved: function(e)
	{
		///	<summary>
		///		Processes a removed cue.
		///	</summary>

		e.type = "captioncueremoved";
		this.player.dispatchEvent(e);
	},

	onCueSkipped: function(e)
	{
		///	<summary>
		///		Processes a skipped cue.
		///	</summary>

		e.type = "captioncueskipped";
		this.player.dispatchEvent(e);
	},

	onReady: function(e)
	{
		///	<summary>
		///		Called when the player is ready for playback.
		///	</summary>

		this.element = PlayerFramework.createElement(null,
		[
			"div",
			{
				"class": "pf-caption-cues-container"
			}
		]);

		this.player.containerElement.insertBefore(this.element, this.player.mediaPlugin.element.nextSibling);
		PlayerFramework.addEvent(this.element, "mouseover", PlayerFramework.mouseEventProxy(this.player, "mouseover"));
		PlayerFramework.addEvent(this.element, "mouseout", PlayerFramework.mouseEventProxy(this.player, "mouseout"));
	},

	// Functions
	clearCaptionContainer: function()
	{
		///	<summary>
		///		Clears the area containing the caption HTML.
		///	</summary>

		this.element.innerHTML = "";
	},

	processTextTrackSource: function(textTrack)
	{
		///	<summary>
		///		Processes a downloaded text track using a TTML parser.
		///	</summary>
		///	<param name="textTrack" type="Object">
		///		The text track to process.
		///	</param>

		var ttmlParser = new PlayerFramework.TtmlParser();
		var ttml = ttmlParser.parseTtml(textTrack.xml);

		textTrack.cues = new PlayerFramework.TextTrackCueList(
		{
			track: textTrack,
			list: ttml.captions
		});
	}
});

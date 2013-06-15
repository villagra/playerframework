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
	},

	addEventListeners: function () {
	    ///	<summary>
	    ///		Adds event listeners to the control's elements.
	    ///	</summary>
	    this._super();

	    this.player.addEventListener("ready", PlayerFramework.proxy(this, this.onReady), false);
	    this.player.addEventListener("datareceived", PlayerFramework.proxy(this, this.onDataReceived), false);
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

	onDataReceived: function (e)
	{
	    ///	<summary>
	    ///		Processes in-stream caption data
	    ///	</summary>

	    var startTime = e.dataReceived.timestamp * 1000;
	    var endTime = e.dataReceived.duration * 1000 + startTime;
	    var xml = PlayerFramework.parseXml(e.dataReceived.data);

	    this.processTextTrackSource(this.activeTextTrack, xml, startTime, endTime);
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
	activateTextTrack: function (textTrack) {

	    if (textTrack.inStream) {

	        this.player.mediaPlugin.activateTrack(textTrack);

	        textTrack.readyState = PlayerFramework.TextTrack.ReadyState.LOADED;
	        textTrack.dispatchEvent({ type: "load" });

	        this.showTextTrack(textTrack);

	    } else {

	        this._super(textTrack);
	    }
	},

	clearCaptionContainer: function ()
	{
		///	<summary>
		///		Clears the area containing the caption HTML.
		///	</summary>

		this.element.innerHTML = "";
	},

	processTextTrackSource: function(textTrack, ttml, startTime, endTime)
	{
		///	<summary>
		///		Processes a downloaded text track using a TTML parser.
		///	</summary>
		///	<param name="textTrack" type="Object">
		///		The text track to process.
	    ///	</param>
	    ///	<param name="ttml" type="Object">
	    ///		TTML data to process.
	    ///	</param>
	    ///	<param name="startTime" type="Number">
	    ///		The start time in seconds of the provided TTML. Optional.
	    ///	</param>
	    ///	<param name="endTime" type="Number">
	    ///		The end time in seconds of the provided TTML. Optional.
	    ///	</param>

	    var ttmlParser = new PlayerFramework.TtmlParser();

	    ttmlParser.parseTtml(ttml, startTime, endTime);

	    if (!textTrack.cues) {
	        textTrack.cues = new PlayerFramework.TextTrackCueList();
	    }

	    for (var i = 0; i < ttmlParser.cues.length; i++) {

	        var parserCue = ttmlParser.cues[i];

	        var textTrackCue = new PlayerFramework.TextTrackCue(
            {
                track: textTrack,
                id: parserCue.cue.id,
                startTime: parserCue.startTime,
                endTime: parserCue.endTime,
                pauseOnExit: false,
                text: parserCue.cue.innerHTML,
                markup: parserCue.cue,
                uri: parserCue.uri
            });

	        textTrack.cues.push(textTrackCue);
	    }
	}
});

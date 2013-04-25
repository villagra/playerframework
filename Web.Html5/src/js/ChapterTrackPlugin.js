PlayerFramework.Plugins.ChapterTrackPlugin = PlayerFramework.TrackPlugin.extend(
{
	defaultOptions: function(player)
	{
		return PlayerFramework.mergeOptions(this._super(),
		{
			playerExtensionPropertyName: "chapters",
			kind: "chapters"
		});
	},

	init: function(player, options)
	{
		///	<summary>
		///		Initializes the ChapterTrackPlugin that provides an API for adding, removing, and navigating chapters.
		///	</summary>
		///	<param name="player" type="Object">
		///		The Player object.
		///	</param>
		///	<param name="options" type="Object">
		///		The options for the ChapterTrackPlugin.
		///	</param>
		///	<returns type="ChapterTrackPlugin" />
		
		this._super(player, options);

		this.activeTextTrack = this.player.addTextTrack(this.options.kind);
	},

	// Event Handlers
	onCueAdded: function(e)
	{
		///	<summary>
		///		Processes an added cue.
		///	</summary>
		///	<param name="cue" type="Object">
		///		The added cue.
		///	</param>
		
		e.type = "chaptercueadded";
		this.player.dispatchEvent(e);
	},

	onCueChange: function()
	{
		///	<summary>
		///		Processes a changed cue.
		///	</summary>

		this.player.dispatchEvent(
		{
			type: "chaptercuechange"
		});
	},

	onCueLeft: function(e)
	{
		///	<summary>
		///		Processes an newly inactive cue.
		///	</summary>

		e.type = "chaptercueleft";
		this.player.dispatchEvent(e);
	},

	onCueReached: function(e)
	{
		///	<summary>
		///		Processes a newly active cue.
		///	</summary>

		e.type = "chaptercuereached";
		this.player.dispatchEvent(e);
	},

	onCueRemoved: function(e)
	{
		///	<summary>
		///		Processes a removed cue.
		///	</summary>

		e.type = "chaptercueremoved";
		this.player.dispatchEvent(e);
	},

	onCueSkipped: function(e)
	{
		///	<summary>
		///		Processes a skipped cue.
		///	</summary>

		e.type = "chaptercueskipped";
		this.player.dispatchEvent(e);
	},

	// Functions
	skipBackChapter: function()
	{
		///	<summary>
		///		Skips back one chapter.
		///	</summary>

		var chapterTime;
		for(var i = this.activeTextTrack.cues.length - 1; i >= 0 ; i--)
		{
			var cue = this.activeTextTrack.cues[i];
			
			if (cue.startTime < (this.player.currentTime() - 1))
			{
				chapterTime = cue.startTime;
				break;
			}
		}

		if (!chapterTime)
			chapterTime = 0;

		this.player.currentTime(chapterTime);
	},

	skipForwardChapter: function()
	{
		///	<summary>
		///		Skips forward one chapter.
		///	</summary>

		var chapterTime;
		for(var i = 0; i < this.activeTextTrack.cues.length; i++)
		{
			var cue = this.activeTextTrack.cues[i];
			
			if (cue.startTime > this.player.currentTime())
			{
				chapterTime = cue.startTime;
				break;
			}
		}

		if (!chapterTime)
			chapterTime = this.player.duration();

		this.player.currentTime(chapterTime);
	}
});

PlayerFramework.Plugins.TimelineTrackPlugin = PlayerFramework.TrackPlugin.extend(
{
	defaultOptions: function(player)
	{
		return PlayerFramework.mergeOptions(this._super(),
		{
			playerExtensionPropertyName: "timelineCues",
			kind: "metadata"
		});
	},

	init: function(player, options)
	{
		///	<summary>
		///		Initializes the TimelineTrackPlugin that provides an API for adding, removing, and processing timeline cues.
		///	</summary>
		///	<param name="player" type="Object">
		///		The Player object.
		///	</param>
		///	<param name="options" type="Object">
		///		The options for the TimelineTrackPlugin.
		///	</param>
		///	<returns type="TimelineTrackPlugin" />

		this._super(player, options);

		this.activeTextTrack = this.player.addTextTrack(this.options.kind);
	},

	// Event Handlers
	onCueAdded: function(e)
	{
		///	<summary>
		///		Processes an added cue.
		///	</summary>

		e.type = "timelinecueadded";
		this.player.dispatchEvent(e);
	},

	onCueChange: function()
	{
		///	<summary>
		///		Processes a changed cue.
		///	</summary>

		this.player.dispatchEvent(
		{
			type: "timelinecuechange"
		});
	},

	onCueLeft: function(e)
	{
		///	<summary>
		///		Processes an newly inactive cue.
		///	</summary>

		e.type = "timelinecueleft";
		this.player.dispatchEvent(e);
	},

	onCueReached: function(e)
	{
		///	<summary>
		///		Processes a newly active cue.
		///	</summary>

		e.type = "timelinecuereached";
		this.player.dispatchEvent(e);
	},

	onCueRemoved: function(e)
	{
		///	<summary>
		///		Processes a removed cue.
		///	</summary>

		e.type = "timelinecueremoved";
		this.player.dispatchEvent(e);
	},

	onCueSkipped: function(e)
	{
		///	<summary>
		///		Processes a skipped cue.
		///	</summary>

		e.type = "timelinecueskipped";
		this.player.dispatchEvent(e);
	}
});

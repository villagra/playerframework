PlayerFramework.TextTrack = PlayerFramework.Object.extend(
{
	init: function(options)
	{
		///	<summary>
		///		Initializes a TextTrack that represents a set of cues.
		///	</summary>
		///	<param name="options" type="Object">
		///		The options for the TextTrackCueList.
		///	</param>

		this._super(options);
		
		this.kind = "";
		this.label = "";
		this.language = "";

		this.readyState = PlayerFramework.TextTrack.ReadyState.NONE;
		this.mode = PlayerFramework.TextTrack.Mode.HIDDEN;

		this.cues = new PlayerFramework.TextTrackCueList();
		this.activeCues = new PlayerFramework.TextTrackCueList();
		this.xml = null; // Implementation - not per the W3C specification.

		this.onload = function () { };
		this.onerror = function () { };
		this.oncuechange = null;
	},

	addCue: function(cue)
	{
		///	<summary>
		///		Adds the given cue to the text track's list of cues.
		///	</summary>

		this.cues.push(cue);
		
		this.cues.sort(function(a, b)
		{
			return a.startTime > b.startTime ? 1 : -1;
		});
	},

	removeCue: function(cue)
	{
		///	<summary>
		///		Removes the given cue from the text track's list of cues.
		///	</summary>

		for(var i = 0; i < this.cues.length; i++)
		{
			if (this.cues[i].id == cue.id)
			{
				this.cues.splice(i, 1);
				break;
			}
		}
	}
});

PlayerFramework.TextTrack.ReadyState =
{
	///	<summary>
	///		A JSON object used to store the values of the text track's ready state.
	///	</summary>

	NONE: 0,
	LOADING: 1,
	LOADED: 2,
	ERROR: 3
};

PlayerFramework.TextTrack.Mode =
{
	///	<summary>
	///		A JSON object used to store the values of the text track's mode.
	///	</summary>

	OFF: 0,
	HIDDEN: 1,
	SHOWING: 2
};

PlayerFramework.TextTrack.DisplayPreference =
{
	///	<summary>
	///		A JSON object used to store the values of the text track's display preference.
	///	</summary>

	NONE: 0,
	CUSTOM: 1,
	NATIVE: 2,
	ALL: 3
};

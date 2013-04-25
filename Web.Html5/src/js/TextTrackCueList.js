PlayerFramework.TextTrackCueList = function(options)
{
	///	<summary>
	///		Initializes a TextTrackCueList that represents a dynamically updating list of text track cues in a given order.
	///	</summary>
	///	<param name="options" type="Object">
	///		The options for the TextTrackCueList.
	///	</param>

	var list = [];

	if (options && options.list)
	{
		PlayerFramework.forEach(options.list, PlayerFramework.proxy(this, function(item)
		{
			list.push(new PlayerFramework.TextTrackCue(
			{
				track: options.track,
				id: item.id,
				startTime: item.start / 1000,
				endTime: item.end / 1000,
				pauseOnExit: false,
				text: item.caption.innerHTML,
				markup: item.caption,
				uri: item.uri
			}));
		}));
	}

	list.getCueById = function (id)
	{
		///	<summary>
		///		Returns the first text track cue (in text track cue order) with text track cue identifier id.
		///	</summary>.

		if (id === "")
			return null;

		var foundCue = PlayerFramework.first(list, function(cue)
		{
			return cue.id === id;
		});

		return foundCue;
	};

	return list;
};

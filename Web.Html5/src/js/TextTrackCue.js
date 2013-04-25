PlayerFramework.TextTrackCue = PlayerFramework.Object.extend(
{
	init: function(options)
	{
		///	<summary>
		///		Initializes a TextTrackCue that is the unit of time-sensitive data in a text track,
		///		corresponding for instance for subtitles and captions to the text that appears
		///		at a particular time and disappears at another time.
		///	</summary>
		///	<param name="options" type="Object">
		///		The options for the TextTrackCue.
		///	</param>

		this._super(options);

		this.track = this.options.track;
		this.id = this.options.id;
		this.startTime = this.options.startTime;
		this.endTime = this.options.endTime;
		this.pauseOnExit = this.options.pauseOnExit;

		this.onenter = null;
		this.onexit = null;
	},
	
	getCueAsSource: function ()
	{
		///	<summary>
		///		Returns the text track cue text in raw unparsed form.
		///	</summary>
	
		return this.options.text;
	},

	getCueAsHTML: function ()
	{
		///	<summary>
		///		Returns the text track cue text as a DocumentFragment of HTML elements and other DOM nodes.
		///	</summary>

		return this.options.markup;
	}
});

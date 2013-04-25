PlayerFramework.StaticContentMediaPlugin = PlayerFramework.MediaPlugin.extend(
{
	defaultOptions: function(player, playerOptions)
	{
		return PlayerFramework.mergeOptions(this._super(),
		{
			"class": "pf-static"
		});
	},

	init: function(player, options, playerOptions)
	{
		///	<summary>
		///		Initializes the MediaPlugin that provides static content. This is often used as the last fallback in the chain.
		///	</summary>
		///	<param name="player" type="Object">
		///		The Player object.
		///	</param>
		///	<param name="options" type="Object">
		///		The options for the StaticContentMediaPlugin.
		///	</param>
		///	<param name="playerOptions" type="Object">
		///		The merged player options for the current media source.
		///	</param>
		///	<returns type="StaticContentMediaPlugin" />

		this._super(player, options, playerOptions);

		if (this.options.element)
		{
			if (!this.options.element.tagName)
				throw new TypeError("options.element not a DOM element");

			this.element = this.options.element;
		}
	},

	// MediaPlugin Functions
	checkSupport: function(callback)
	{
		///	<summary>
		///		Determines support for the media element.
		///	</summary>
		///	<param name="callback" type="Function">
		///		The function to call after support has been determined.
		///	</param>

		callback(true);
	}
});

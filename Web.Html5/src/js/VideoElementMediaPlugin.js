PlayerFramework.Plugins.VideoElementMediaPlugin = PlayerFramework.Plugins.VideoElementMediaPluginBase.extend(
{
	init: function(player, options, playerOptions)
	{
		///	<summary>
		///		Initializes the VideoMediaPlugin that injects and wraps the HTML5 video element.
		///	</summary>
		///	<param name="player" type="Object">
		///		The Player object.
		///	</param>
		///	<param name="options" type="Object">
		///		The options for the VideoElementMediaPlugin.
		///	</param>
		///	<param name="playerOptions" type="Object">
		///		The merged player options for the current media source.
		///	</param>
		///	<returns type="VideoElementMediaPlugin" />

		this._super(player, options, playerOptions);

		this.render();
	},

	render: function()
	{
		///	<summary>
		///		Creates and sets the MediaPlugin's element given the plugin and player options
		///		and a specific template.
		///	</summary>

		this.element = PlayerFramework.createElement(null,
		[
			"video",
			{
				"class": this.options["class"],
				width: this.playerOptions.width,
				height: this.playerOptions.height,
				controls: "controls", /* Controls must be turned on initially for compatibility with some browsers. */
				poster: this.playerOptions.poster || null,
				autoplay: this.playerOptions.autoplay || null
			}
		]);

		for (var i = 0; i < this.playerOptions.sources.length; i++)
		{
			var source = this.playerOptions.sources[i];
			if (this.canPlayType(source.type))
			{
				PlayerFramework.createElement(this.element,
				[
					"source",
					source
				]);
			}
		}
	}
});

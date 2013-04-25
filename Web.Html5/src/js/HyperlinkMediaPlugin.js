PlayerFramework.Plugins.HyperlinkMediaPlugin = PlayerFramework.StaticContentMediaPlugin.extend(
{
	defaultOptions: function(player, playerOptions)
	{
		return PlayerFramework.mergeOptions(this._super(),
		{
			strings:
			{
				downloadsLabel: "Downloads:"
			},
			"class": "pf-hyperlinks",
			unsupportedTypes: [ /text\/xml/i ]
		});
	},

	init: function(player, options, playerOptions)
	{
		///	<summary>
		///		Initializes the MediaPlugin that provides hyperlinks for downloading the media to play locally.
		///	</summary>
		///	<param name="player" type="Object">
		///		The Player object.
		///	</param>
		///	<param name="options" type="Object">
		///		The options for the HyperlinkMediaPlugin.
		///	</param>
		///	<param name="playerOptions" type="Object">
		///		The merged player options for the current media source.
		///	</param>
		///	<returns type="HyperlinkMediaPlugin" />

		this._super(player, options, playerOptions);

		this.player.containerElement.style["background-color"] = "#fff";

		this.render();

		this.player.dispatchEvent({ type: "canplaythrough" });
	},

	render: function()
	{
		///	<summary>
		///		Creates and sets the MediaPlugin's element given the plugin and player options
		///		and a specific template.
		///	</summary>
		 
		var sources = this.playerOptions.sources;

		this.element = PlayerFramework.createElement(null,
		[
			"div",
			{
				"class": this.options["class"],
				width: this.playerOptions.width,
				height: this.playerOptions.height,
				controls: "controls", /* Controls must be turned on initially for compatibility with some browsers. */
				poster: this.playerOptions.poster
			},
			[
				"div",
				this.options.strings.downloadsLabel
			]
		]);

		for (var i = 0; i < sources.length; i++)
		{
			var isSourceSupported = true;

			for (var j = 0; j < this.options.unsupportedTypes.length; j++)
			{
				if (this.options.unsupportedTypes[j].test(sources[i].type))
					isSourceSupported = false;
			}

			if (isSourceSupported)
			{
				var sourceUri = sources[i].src;

				PlayerFramework.createElement(this.element,
				[
					"div",
					[
						"a",
						{
							href: sourceUri,
							title: sourceUri
						},
						sourceUri
					]
				]);
			}
		}
	}
});

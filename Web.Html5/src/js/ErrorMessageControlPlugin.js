PlayerFramework.Plugins.ErrorMessageControlPlugin = PlayerFramework.ControlPlugin.extend(
{
	defaultOptions: function(player)
	{
		return PlayerFramework.mergeOptions(this._super(),
		{
			strings:
			{
				defaultMessage: "An error occurred while attempting to play the video."
			}
		});
	},

	init: function(player, options)
	{
		///	<summary>
		///		Initializes the ControlPlugin that displays a message when the media element encounters an error.
		///	</summary>
		///	<param name="player" type="Object">
		///		The Player object.
		///	</param>
		///	<param name="options" type="Object">
		///		The options for the ErrorMessageControlPlugin.
		///	</param>
		///	<returns type="ErrorMessageControlPlugin" />

		this._super(player, options);
	},

	// ControlPlugin Event Handlers
	onError: function(e)
	{
		///	<summary>
		///		Called when the media element encounters an error.
		///	</summary>

		//console.log("error = " + this.player.error().code);

		if (!this.element)
		{
			if (this.mediaPlugin() && this.player.error() && this.player.error().code)
				this.show(this.options.strings.defaultMessage);
			else
				this.show(this.options.strings.defaultMessage);
		}
	},

	onNetworkStateChange: function(e)
	{
		///	<summary>
		///		Called when the media element's network state changes.
		///	</summary>

		//console.log("network state = " + this.player.networkState());

		if (!this.element)
		{
			if (this.player.networkState() === PlayerFramework.VideoMediaPlugin.NetworkState.NETWORK_NO_SOURCE && window.navigator.appName !== "Microsoft Internet Explorer")
				this.show(this.options.strings.defaultMessage);
		}
	},

	// Functions
	show: function(text)
	{
		///	<summary>
		///		Creates a message control with the specified text and adds it to the DOM.
		///	</summary>
		///	<param name="text" type="String">
		///		The text to display.
		///	</param>

		// Error message control
		this.element = PlayerFramework.createElement(this.player.containerElement,
		[
			"div",
			{
				"class": "pf-error-message-control"
			},
			[
				"div",
				{
					"class": "pf-error-message-container"
				},
				text
			]
		]);
	}
});

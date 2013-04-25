PlayerFramework.Plugins.LogWriterPlugin = PlayerFramework.ModulePlugin.extend(
{
	init: function(player, options)
	{
		///	<summary>
		///		Initializes a LogWriterPlugin base. Listens for the "log" event dispatched from the Player object and stores log messages in a history array.
		///	</summary>
		///	<param name="player" type="Object">
		///		The Player object.
		///	</param>
		///	<param name="options" type="Object">
		///		The options for the LogWriterPlugin.
		///	</param>
		///	<returns type="LogWriterPlugin" />

		this._super(player, options);
		
		// Store log messages in an array for reference.
		this.history = this.history || [];
		
		// Use this syntax to log: this.player.dispatchEvent({ type: "log", text: text });
		player.addEventListener("log", PlayerFramework.proxy(this, function (e)
		{
			this.log(e.text);
		}), false);
	},

	log: function()
	{
		///	<summary>
		///		When overridden in a derived class, logs a message to a log target. Also stores log messages in the history array.
		///	</summary>
		///	<param name="arguments" type="Object">
		///		The arguments variable is used to retrieve the values to be logged.
		///	</param>

		this.history.push(arguments);
	}
});

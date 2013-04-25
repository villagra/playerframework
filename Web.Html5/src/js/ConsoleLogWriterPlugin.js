PlayerFramework.Plugins.ConsoleLogWriterPlugin = PlayerFramework.LogWriterPlugin.extend(
{
	init: function(player, options)
	{
		///	<summary>
		///		Initializes a LogWriterPlugin plugin that writes log messages to the console.
		///		Base on: http://paulirish.com/2009/log-a-lightweight-wrapper-for-consolelog/
		///	</summary>
		///	<param name="player" type="Object">
		///		The Player object.
		///	</param>
		///	<param name="options" type="Object">
		///		The options for the ConsoleLogWriterPlugin.
		///	</param>
		///	<returns type="ConsoleLogWriterPlugin" />

		this._super(player, options);
	},

	log: function()
	{
		///	<summary>
		///		Logs a message to the console. Called from the LogWriterPlugin base after a "log"
		///		event is dispatched from the Player object. 
		///	</summary>
		///	<param name="arguments" type="Object">
		///		The arguments variable is used to retrieve the values to be logged.
		///	</param>

		this._super();

		if(window.console)
			console.log(Array.prototype.slice.call(arguments));
	}
});

PlayerFramework.Plugins.DocumentLogWriterPlugin = PlayerFramework.LogWriterPlugin.extend(
{
	init: function(player, options)
	{
		///	<summary>
		///		Initializes a LogWriterPlugin plugin that writes log messages to the document.
		///	</summary>
		///	<param name="player" type="Object">
		///		The Player object.
		///	</param>
		///	<param name="options" type="Object">
		///		The options for the DocumentLogWriterPlugin.
		///	</param>
		///	<returns type="DocumentLogWriterPlugin" />

		this._super(player, options);
	},

	log: function()
	{
		///	<summary>
		///		Appends a log message to the document. Called from the LogWriterPlugin base after a "log"
		///		event is dispatched from the Player object. 
		///	</summary>
		///	<param name="arguments" type="Object">
		///		The arguments variable is used to retrieve the values to be logged.
		///	</param>

		this._super();

		PlayerFramework.createElement(document.body,
		[
			"div",
			new String(Array.prototype.slice.call(arguments))
		]);
	}
});

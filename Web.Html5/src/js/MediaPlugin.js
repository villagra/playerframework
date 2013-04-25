PlayerFramework.MediaPlugin = PlayerFramework.Plugin.extend(
{
	init: function(player, options, playerOptions)
	{
		///	<summary>
		///		Initializes the MediaPlugin base. Used as a base class for plugins that display media content in any form.
		///	</summary>
		///	<param name="player" type="Object">
		///		The Player object.
		///	</param>
		///	<param name="options" type="Object">
		///		The options for the MediaPlugin.
		///	</param>
		///	<param name="playerOptions" type="Object">
		///		The merged player options for the current media source.
		///	</param>
		///	<returns type="MediaPlugin" />

		this._super(player, options);

		this.playerOptions = playerOptions;
		this.element = null;
	},

	// Functions
	checkSupport: function(callback)
	{
		///	<summary>
		///		When overridden in a derived class, determines support for the media element.
		///	</summary>
		///	<param name="callback" type="Function">
		///		The function to call after support has been determined.
		///	</param>

		throw new Error("Not implemented.");
	}
});

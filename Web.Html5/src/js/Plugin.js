PlayerFramework.Plugin = PlayerFramework.Object.extend(
{
	defaultOptions: function(player)
	{
		return {};
	},

	isEnabled: function(player, options)
	{
		return true;
	},

	init: function(player, options)
	{
		///	<summary>
		///		The plugin base from which all plugins should be derived.
		///		Stores a reference to the Player object for manipulation by the plugin.
		///	</summary>
		///	<param name="player" type="Object">
		///		The Player object.
		///	</param>

		this._super(options);

		this.player = player;
	}
});

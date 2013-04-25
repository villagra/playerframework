PlayerFramework.ModulePlugin = PlayerFramework.Plugin.extend(
{
	init: function(player, options)
	{
		///	<summary>
		///		The plugin base for singleton plugins that should only have one instance per
		///		Player object.
		///	</summary>
		///	<param name="player" type="Object">
		///		The Player object.
		///	</param>

		this._super(player, options);
		
		if (this.options.playerExtensionPropertyName)
			this.player[this.options.playerExtensionPropertyName] = this;
	}
});

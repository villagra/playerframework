PlayerFramework.Plugins.PlaylistPlugin = PlayerFramework.ModulePlugin.extend(
{
	defaultOptions: function(player)
	{
		return PlayerFramework.mergeOptions(this._super(),
		{
			playlistItemEndBehavior: PlayerFramework.Plugins.PlaylistPlugin.PlaylistItemEndBehavior.ADVANCE_AND_PLAY,
			initialPlaylistItemIndex: 0,
			playerExtensionPropertyName: "playlist"
		});
	},

	isEnabled: function(player, options)
	{
		return !!player.options.playlist;
	},

	init: function(player, options)
	{
		///	<summary>
		///		Initializes the PlaylistPlugin that provides an API for playlist management.
		///	</summary>
		///	<param name="player" type="Object">
		///		The Player object.
		///	</param>
		///	<param name="options" type="Object">
		///		The options for the PlaylistPlugin.
		///	</param>
		///	<returns type="PlaylistPlugin" />

		this._super(player, options);
		
		this.playlistItems = [];

		this.loadPlaylistItems();
		
		this.player.addEventListener("ended", PlayerFramework.proxy(this, this.onEnded), false);
	},

	// Properties
	currentPlaylistItem: function()
	{
		///	<summary>
		///		Returns the playlist item representing the currently loaded media.
		///	</summary>
		///	<returns type="Object" />
		
		if (this.currentPlaylistItemIndex + 1 > this.playlistItems.length)
			throw new Error("Invalid playlist item index.");
		
		return this.playlistItems[this.currentPlaylistItemIndex];
	},

	// Event Handlers
	onEnded: function(e)
	{
		///	<summary>
		///		Called when the media playback ends.
		///	</summary>
		
		if (this.options.playlistItemEndBehavior == PlayerFramework.Plugins.PlaylistPlugin.PlaylistItemEndBehavior.NONE
			|| this.currentPlaylistItemIndex == this.playlistItems.length - 1)
			return;

		this.currentPlaylistItemIndex++;
		
		// Call asynchronously to allow other event handlers to fire first.
		window.setTimeout(PlayerFramework.proxy(this, function()
		{
			switch (this.options.playlistItemEndBehavior)
			{
				case PlayerFramework.Plugins.PlaylistPlugin.PlaylistItemEndBehavior.ADVANCE_ONLY:
					this.changeMediaPlugin(
					{
						poster: null,
						autoplay: null
					});
					break;

				case PlayerFramework.Plugins.PlaylistPlugin.PlaylistItemEndBehavior.ADVANCE_AND_PLAY:
					this.changeMediaPlugin(
					{
						poster: null,
						autoplay: "autoplay"
					});
					break;
			}
		}), 1);
	},

	// Functions
	addPlaylistItem: function(playlistItem, index)
	{
		///	<summary>
		///		Adds the playlist item to the array of playlist items.
		///	</summary>
		///	<param name="playlistItem" type="Object">
		///		The playlist item to add.
		///	</param>
		///	<param name="index" type="Number">
		///		Optional index at which to insert the playlist item.
		///	</param>

		this.playlistItems.splice(index != undefined ? index : this.playlistItems.length, 0, playlistItem);
		this.player.dispatchEvent({ type: "playlistitemadded" });
	},

	changeMediaPlugin: function(options)
	{
		///	<summary>
		///		Sets the media plugin given the current playlist item and the specified options.
		///	</summary>
		///	<param name="options" type="Object">
		///		Options specified by the playlist plugin that determine the behavior for changing the playlist item.
		///	</param>
		
		this.player.mediaPlugin.onUnloadingMediaPlugin();
		this.player.mediaPlugin.element.parentNode.removeChild(this.player.mediaPlugin.element);

		var currentPlaylistItemOptions = this.currentPlaylistItem();
		PlayerFramework.merge(currentPlaylistItemOptions, options);
		this.player.setMediaPlugin(currentPlaylistItemOptions);
	},

	loadPlaylistItems: function()
	{
		///	<summary>
		///		Loads the playlist items from the playlist array specified in the options.
		///	</summary>

		if (!this.player.options.playlist || !(this.player.options.playlist instanceof Array))
			throw new Error("Invalid playlist.");

		this.playlistItems = this.player.options.playlist;

		this.setPlaylistItemOptions();
	},

	nextPlaylistItem: function()
	{
		///	<summary>
		///		Sets the playlist item to the next playlist item in the array.
		///	</summary>

		this.setPlaylistItem(this.currentPlaylistItemIndex + 1);
	},

	previousPlaylistItem: function()
	{
		///	<summary>
		///		Sets the playlist item to the previous playlist item in the array.
		///	</summary>

		this.setPlaylistItem(this.currentPlaylistItemIndex - 1);
	},

	removePlaylistItem: function(index)
	{
		///	<summary>
		///		Removes the playlist item located at the specified index.
		///	</summary>
		///	<param name="index" type="Object">
		///		The index of the playlist item to remove.
		///	</param>

		this.playlistItems.splice(index, 1);
		this.player.dispatchEvent({ type: "playlistitemremoved" });
	},
	
	setPlaylistItem: function(index)
	{
		///	<summary>
		///		Sets the current playlist item to the playlist item located at the specified index.
		///	</summary>
		///	<param name="index" type="Object">
		///		The index of the playlist item to use as the current playlist item.
		///	</param>

		if (index < 0
			|| index > this.playlistItems.length - 1)
			return;
		
		this.currentPlaylistItemIndex = index;

		this.changeMediaPlugin(
		{
			poster: null,
			autoplay: "autoplay"
		});
	},

	setPlaylistItemOptions: function()
	{
		///	<summary>
		///		Merges the player's options to the options of the initial playlist item.
		///	</summary>

		this.currentPlaylistItemIndex = this.options.initialPlaylistItemIndex;
		this.player.setMediaPlugin(this.currentPlaylistItem());
	}
});

PlayerFramework.Plugins.PlaylistPlugin.PlaylistItemEndBehavior =
{
	///	<summary>
	///		A JSON object used to store the values of the playlist plugin's possible behaviors
	///		once playback has ended for a playlist item.
	///	</summary>

	NONE: 0,
	ADVANCE_ONLY: 1,
	ADVANCE_AND_PLAY: 2
};

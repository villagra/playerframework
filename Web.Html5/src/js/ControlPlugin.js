PlayerFramework.ControlPlugin = PlayerFramework.ModulePlugin.extend(
{
	init: function(player, options)
	{
		///	<summary>
		///		Initializes the ControlPlugin base and listens for events needed to change the state of the controls.
		///	</summary>
		///	<param name="player" type="Object">
		///		The Player object.
		///	</param>
		///	<param name="options" type="Object">
		///		The options for the ControlPlugin.
		///	</param>
		///	<returns type="ControlPlugin" />
		
		this._super(player, options);

		this.player.addEventListener("canplaythrough", PlayerFramework.proxy(this, this.onCanPlayThrough), false);
		this.player.addEventListener("ended", PlayerFramework.proxy(this, this.onEnded), false);
		this.player.addEventListener("error", PlayerFramework.proxy(this, this.onError), false);
		this.player.addEventListener("fullscreenchange", PlayerFramework.proxy(this, this.onFullScreenChange), false);
		this.player.addEventListener("loadedmediaplugin", PlayerFramework.proxy(this, this.onLoadedMediaPlugin), false);
		this.player.addEventListener("mouseout", PlayerFramework.proxy(this, this.onPlayerMouseOut), false);
		this.player.addEventListener("mouseover", PlayerFramework.proxy(this, this.onPlayerMouseOver), false);
		this.player.addEventListener("networkstatechange", PlayerFramework.proxy(this, this.onNetworkStateChange), false);
		this.player.addEventListener("pause", PlayerFramework.proxy(this, this.onPause), false);
		this.player.addEventListener("play", PlayerFramework.proxy(this, this.onPlay), false);
		this.player.addEventListener("progress", PlayerFramework.proxy(this, this.onProgress), false);
		this.player.addEventListener("ready", PlayerFramework.proxy(this, this.onReady), false);
		this.player.addEventListener("seeked", PlayerFramework.proxy(this, this.onSeeked), false);
		this.player.addEventListener("seeking", PlayerFramework.proxy(this, this.onSeeking), false);
		this.player.addEventListener("timeupdate", PlayerFramework.proxy(this, this.onTimeUpdate), false);
		this.player.addEventListener("unloadingmediaplugin", PlayerFramework.proxy(this, this.onUnloadingMediaPlugin), false);
		this.player.addEventListener("volumechange", PlayerFramework.proxy(this, this.onVolumeChange), false);
	},

	// Properties
	mediaPlugin: function()
	{
		///	<summary>
		///		Returns the player's media plugin for convenience.
		///	</summary>

		return this.player.mediaPlugin;
	},

	// Event Handlers
	onCanPlayThrough: function(e)
	{
		///	<summary>
		///		Called when the media element can play through to the end without having to stop for further buffering.
		///	</summary>
	},

	onEnded: function(e)
	{
		///	<summary>
		///		Called when the media playback ends.
		///	</summary>
	},

	onError: function(e)
	{
		///	<summary>
		///		Called when the media element encounters an error.
		///	</summary>
	},

	onFullScreenChange: function(e)
	{
		///	<summary>
		///		Called when the media element changes to and from full screen.
		///	</summary>
	},

	onLoadedMediaPlugin: function(e)
	{
		///	<summary>
		///		Called after the media plugin is loaded.
		///	</summary>
	},

	onPlayerMouseOut: function(e)
	{
		///	<summary>
		///		Called when the mouse leaves the player.
		///	</summary>
	},

	onPlayerMouseOver: function(e)
	{
		///	<summary>
		///		Called when the mouse enters the player.
		///	</summary>
	},

	onNetworkStateChange: function(e)
	{
		///	<summary>
		///		Called when the media element's network state changes.
		///	</summary>
	},

	onPause: function(e)
	{
		///	<summary>
		///		Called when the media pauses.
		///	</summary>
	},

	onPlay: function(e)
	{
		///	<summary>
		///		Called when the media plays.
		///	</summary>
	},

	onProgress: function(e)
	{
		///	<summary>
		///		Called when the media buffers more data.
		///	</summary>
	},

	onReady: function(e)
	{
		///	<summary>
		///		Called when the player is ready for playback.
		///	</summary>
	},

	onSeeked: function(e)
	{
		///	<summary>
		///		
		///	</summary>
	},
	
	onSeeking: function(e)
	{
		///	<summary>
		///		
		///	</summary>
	},
	
	onTimeUpdate: function(e)
	{
		///	<summary>
		///		Called when the current time of the media is updated.
		///	</summary>
	},

	onUnloadingMediaPlugin: function(e)
	{
		///	<summary>
		///		Called when the media element unloads.
		///	</summary>
	},

	onVolumeChange: function(e)
	{
		///	<summary>
		///		Called when the volume level of the media changes.
		///	</summary>
	}
});

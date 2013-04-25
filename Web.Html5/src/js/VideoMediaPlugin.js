PlayerFramework.VideoMediaPlugin = PlayerFramework.MediaPlugin.extend(
{
	init: function(player, options, playerOptions)
	{
		///	<summary>
		///		Initializes the VideoMediaPlugin base.
		///	</summary>
		///	<param name="player" type="Object">
		///		The Player object.
		///	</param>
		///	<param name="options" type="Object">
		///		The options for the VideoMediaPlugin.
		///	</param>
		///	<param name="playerOptions" type="Object">
		///		The merged player options for the current media source.
		///	</param>
		///	<returns type="VideoMediaPlugin" />

		this._super(player, options, playerOptions);

		this.canPlayEvent = { type: "canplay" };
		this.canPlayThroughEvent = { type: "canplaythrough" };
		this.endedEvent = { type: "ended" };
		this.errorEvent = { type: "error" };
		this.fullScreenChangeEvent = { type: "fullscreenchange" };
		this.loadedDataEvent = { type: "loadeddata" };
		this.loadedMetadataEvent = { type: "loadedmetadata" };
		this.loadedMediaPluginEvent = { type: "loadedmediaplugin" };
		this.mouseOutEvent = { type: "mouseout" };
		this.mouseOverEvent = { type: "mouseover" };
		this.networkStateChangeEvent = { type: "networkstatechange" };
		this.pauseEvent = { type: "pause" };				
		this.playEvent = { type: "play" };
		this.progressEvent = { type: "progress" };
		this.seekedEvent = { type: "seeked" };
		this.seekingEvent = { type: "seeking" };
		this.timeUpdateEvent = { type: "timeupdate" };
		this.unloadingMediaPluginEvent = { type: "unloadingmediaplugin" };
		this.volumeChangeEvent = { type: "volumechange" };

		// Extend player
		// Properties
		player.buffered = PlayerFramework.proxy(this, this.buffered);
		player.canPlayType = PlayerFramework.proxy(this, this.canPlayType);
		player.controls = PlayerFramework.proxy(this, this.controls);
		player.currentTime = PlayerFramework.proxy(this, this.currentTime);
		player.displayingFullscreen = PlayerFramework.proxy(this, this.displayingFullscreen);
		player.duration = PlayerFramework.proxy(this, this.duration);
		player.error = PlayerFramework.proxy(this, this.error);
		player.muted = PlayerFramework.proxy(this, this.muted);
		player.networkState = PlayerFramework.proxy(this, this.networkState);
		player.paused = PlayerFramework.proxy(this, this.paused);
		player.poster = PlayerFramework.proxy(this, this.poster);
		player.preload = PlayerFramework.proxy(this, this.preload);
		player.readyState = PlayerFramework.proxy(this, this.readyState);
		player.scrubbing = PlayerFramework.proxy(this, this.seeking);
		player.seeking = PlayerFramework.proxy(this, this.seeking);
		player.volume = PlayerFramework.proxy(this, this.volume);

		// Functions
		player.pause = PlayerFramework.proxy(this, this.pause);
		player.play = PlayerFramework.proxy(this, this.play);
	},

	// Properties
	buffered: function()
	{
		///	<summary>
		///		When overridden in a derived class, gets a TimeRanges object that represents the ranges of the media resource.
		///	</summary>
		///	<returns type="TimeRanges" />

		throw new Error("Not implemented.");
	},

	controls: function(value)
	{
		///	<summary>
		///		When overridden in a derived class, gets or sets a boolean indicating whether the media element displays a control strip.
		///	</summary>
		///	<param name="value" type="Boolean" />
		///	<returns type="Boolean" />

		throw new Error("Not implemented.");
	},

	currentTime: function(value)
	{
		///	<summary>
		///		When overridden in a derived class, gets or sets the current playback position of the media element expressed in seconds.
		///	</summary>
		///	<param name="value" type="Number">
		///		The playback position to seek to.
		///	</param>
		///	<returns type="Number" />

		throw new Error("Not implemented.");
	},

	displayingFullscreen: function()
	{
		///	<summary>
		///		Gets a boolean value indicating whether the media element is displaying full screen.
		///	</summary>
		///	<returns type="Boolean" />

		if (!this.element || !this.element.parentNode)
			return false;

		return this.element.parentNode.className === "pf-container pf-full-browser";
	},

	duration: function()
	{
		///	<summary>
		///		When overridden in a derived class, gets the length of the media loaded in the media element expressed in seconds.
		///	</summary>
		///	<returns type="Number" />

		throw new Error("Not implemented.");
	},

	error: function()
	{
		///	<summary>
		///		Gets the last error the media element encountered.
		///	</summary>
		///	<returns type="Number" />

		throw new Error("Not implemented.");
	},

	muted: function(value)
	{
		///	<summary>
		///		When overridden in a derived class, gets or sets a boolean value indicating whether the media element is muted.
		///	</summary>
		///	<param name="value" type="Boolean" />
		///	<returns type="Boolean" />

		throw new Error("Not implemented.");
	},

	paused: function()
	{
		///	<summary>
		///		When overridden in a derived class, gets a boolean value indicating whether the media element is paused.
		///	</summary>
		///	<returns type="Boolean" />

		throw new Error("Not implemented.");
	},
	
	networkState: function()
	{
		///	<summary>
		///		Returns a value that expresses the current state of the element with respect to
		///		loading a resource over the network, from
		///		PlayerFramework.VideoMediaPlugin.NetworkState.
		///	</summary>
		///	<returns type="Number" />

		throw new Error("Not implemented.");
	},

	poster: function()
	{
		///	<summary>
		///		When overridden in a derived class, gets the URL of the poster image to display before initiating playback.
		///	</summary>
		///	<returns type="Boolean" />

		throw new Error("Not implemented.");
	},

	preload: function()
	{
		///	<summary>
		///		Gets the preload state of "none", "metadata", or "auto".
		///	</summary>
		///	<returns type="String" />

		throw new Error("Not implemented.");
	},

	readyState: function()
	{
		///	<summary>
		///		When overridden in a derived class, returns a value that expresses the current state
		///		of the element with respect to rendering the current playback position, from
		///		PlayerFramework.VideoMediaPlugin.ReadyState.
		///	</summary>
		///	<returns type="Number" />

		throw new Error("Not implemented.");
	},

	scrubbing: function()
	{
		///	<summary>

		///	</summary>
		///	<returns type="Boolean" />

		throw new Error("Not implemented.");
	},

	seeking: function()
	{
		///	<summary>

		///	</summary>
		///	<returns type="Boolean" />

		throw new Error("Not implemented.");
	},

	supportsVolumeSetter: function()
	{
		///	<summary>
		///		When overridden in a derived class, gets a boolean value indicating whether the media element supports setting the volume.
		///	</summary>
		///	<returns type="Boolean" />

		throw new Error("Not implemented.");
	},

	volume: function(value)
	{
		///	<summary>
		///		When overridden in a derived class, gets or sets the volume level in a range of 0.0 to 1.0.
		///	</summary>
		///	<param name="value" type="Number" />
		///	<returns type="Number" />

		throw new Error("Not implemented.");
	},

	// Functions
	canPlayType: function()
	{
		///	<summary>
		///		When overridden in a derived class, gets a boolean indicating whether the media element can play content of the specified type.
		///	</summary>
		///	<param name="type" type="String" />
		///	<returns type="Boolean" />

		throw new Error("Not implemented.");
	},

	pause: function()
	{
		///	<summary>
		///		When overridden in a derived class, pauses the media.
		///	</summary>

		throw new Error("Not implemented.");
	},

	play: function()
	{
		///	<summary>
		///		When overridden in a derived class, plays the media.
		///	</summary>

		throw new Error("Not implemented.");
	},

	setup: function()
	{
		///	<summary>
		///		When overridden in a derived class, completes remaining setup operations after the media plugin is selected as the supported media plugin.
		///	</summary>

		throw new Error("Not implemented.");
	},

	// Event Handlers
	onCanPlay: function()
	{
		///	<summary>
		///		Dispatches the "canplay" event.
		///	</summary>
		
		this.player.dispatchEvent(this.canPlayEvent);
	},

	onCanPlayThrough: function()
	{
		///	<summary>
		///		Dispatches the "canplaythrough" event.
		///	</summary>
		
		this.player.dispatchEvent(this.canPlayThroughEvent);
	},

	onEnded: function()
	{
		///	<summary>
		///		Dispatches the "ended" event on behalf of the Player object.
		///	</summary>

		this.player.dispatchEvent(this.endedEvent);
	},

	onError: function()
	{
		///	<summary>
		///		Dispatches the "error" event on behalf of the Player object.
		///	</summary>

		this.player.dispatchEvent(this.errorEvent);
	},

	onFullScreenChange: function()
	{
		///	<summary>
		///		Dispatches the "fullscreenchange" event on behalf of the Player object.
		///	</summary>

		this.player.dispatchEvent(this.fullScreenChangeEvent);
	},

	onLoadedData: function()
	{
		///	<summary>
		///		Dispatches the "loadeddata" event.
		///	</summary>
		
		this.player.dispatchEvent(this.loadedDataEvent);
	},

	onLoadedMetadata: function()
	{
		///	<summary>
		///		Dispatches the "loadedmetadata" event.
		///	</summary>
		
		this.player.dispatchEvent(this.loadedMetadataEvent);
	},

	onLoadedMediaPlugin: function()
	{
		///	<summary>
		///		Dispatches the "loadedMediaPlugin" event on behalf of the Player object.
		///	</summary>

		this.player.dispatchEvent(this.loadedMediaPluginEvent);
	},

	onNetworkStateChange: function()
	{
		///	<summary>
		///		Dispatches the "networkstatechange" event on behalf of the Player object.
		///	</summary>
		
		this.player.dispatchEvent(this.networkStateChangeEvent);
	},

	onMouseOut: function()
	{
		///	<summary>
		///		Dispatches the "mouseout" event on behalf of the Player object.
		///	</summary>

		this.player.dispatchEvent(this.mouseOutEvent);
	},

	onMouseOver: function()
	{
		///	<summary>
		///		Dispatches the "mouseover" event on behalf of the Player object.
		///	</summary>

		this.player.dispatchEvent(this.mouseOverEvent);
	},

	onPause: function()
	{
		///	<summary>
		///		Dispatches the "pause" event on behalf of the Player object.
		///	</summary>

		this.player.dispatchEvent(this.pauseEvent);
	},

	onPlay: function()
	{	
		///	<summary>
		///		Dispatches the "play" event on behalf of the Player object.
		///	</summary>
		
		this.player.dispatchEvent(this.playEvent);
	},

	onProgress: function()
	{
		///	<summary>
		///		Dispatches the "progress" event on behalf of the Player object.
		///	</summary>

		this.player.dispatchEvent(this.progressEvent);
	},

	onSeeked: function(e)
	{
		///	<summary>
		///		Called when the media plugin has completed seeking.
		///	</summary>

		this.player.dispatchEvent(this.seekedEvent);
	},
	
	onSeeking: function(e)
	{
		///	<summary>
		///		Called continuously while the media plugin is seeking.
		///	</summary>

		this.player.dispatchEvent(this.seekingEvent);
	},

	onTimeUpdate: function()
	{
		///	<summary>
		///		Dispatches the "timeupdate" event on behalf of the Player object.
		///	</summary>
		
		this.player.dispatchEvent(this.timeUpdateEvent);
	},

	onUnloadingMediaPlugin: function()
	{
		///	<summary>
		///		Dispatches the "unloadingmediaplugin" event.
		///	</summary>

		this.player.dispatchEvent(this.unloadingMediaPluginEvent);
	},

	onVolumeChange: function()
	{
		///	<summary>
		///		Dispatches the "volumechange" event on behalf of the Player object.
		///	</summary>

		this.player.dispatchEvent(this.volumeChangeEvent);
	}
});

PlayerFramework.VideoMediaPlugin.ReadyState =
{
	///	<summary>
	///		A JSON object used to store the values of the media plugin's ready state.
	///	</summary>

	HAVE_NOTHING: 0,
	HAVE_METADATA: 1,
	HAVE_CURRENT_DATA: 2,
	HAVE_FUTURE_DATA: 3,
	HAVE_ENOUGH_DATA: 4
};

PlayerFramework.VideoMediaPlugin.NetworkState =
{
	///	<summary>
	///		A JSON object used to store the values of the media plugin's network state.
	///	</summary>

	NETWORK_EMPTY: 0,
	NETWORK_IDLE: 1,
	NETWORK_LOADING: 2,
	NETWORK_NO_SOURCE: 3
};

PlayerFramework.VideoMediaPlugin.MediaError =
{
	///	<summary>
	///		A JSON object used to store the values of the media plugin's media error states.
	///	</summary>

	MEDIA_ERR_ABORTED: 1,
	MEDIA_ERR_NETWORK: 2,
	MEDIA_ERR_DECODE: 3,
	MEDIA_ERR_SRC_NOT_SUPPORTED: 4
};

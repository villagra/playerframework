PlayerFramework.Plugins.SilverlightMediaPluginBase = PlayerFramework.VideoMediaPlugin.extend(
{
	defaultOptions: function(player, playerOptions)
	{
		return PlayerFramework.mergeOptions(this._super(),
		{
			"class": "pf-silverlight",
			controls: false,
			supportsTrackElements: false
		});
	},

	init: function(player, options, playerOptions)
	{
		///	<summary>
		///		Initializes the VideoMediaPlugin that wraps the Silverlight player.
		///	</summary>
		///	<param name="player" type="Object">
		///		The Player object.
		///	</param>
		///	<param name="options" type="Object">
		///		The options for the SilverlightMediaPluginBase.
		///	</param>
		///	<param name="playerOptions" type="Object">
		///		The merged player options for the current media source.
		///	</param>
		///	<returns type="SilverlightMediaPluginBase" />
		
		this._super(player, options, playerOptions);

		if (this.options.element)
		{
			if (this.options.element.tagName && this.options.element.tagName.toLowerCase() !== "object")
				throw new TypeError("options.element not a video tag.");

			this.setElement(this.options.element);
		}

		this.seekingValue = false;
	},

	// VideoMediaPlugin Properties
	controls: function(value)
	{
		///	<summary>
		///		Gets or sets a boolean indicating whether the media element displays a control strip.
		///	</summary>
		///	<param name="value" type="Boolean" />
		///	<returns type="Boolean" />

		if (value !== undefined)
			this.silverlightPlayer.IsControlStripVisible = value;

		return this.silverlightPlayer.IsControlStripVisible;
	},

	currentTime: function(value)
	{
		///	<summary>
		///		Gets or sets the current playback position of the media element expressed in seconds.
		///	</summary>
		///	<param name="value" type="Number">
		///		The playback position to seek to.
		///	</param>
		///	<returns type="Number" />

		try
		{
			if (value)
				this.silverlightPlayer.SeekToPosition(value);
		
			return this.silverlightPlayer.PlaybackPositionSeconds;
		}
		catch(e)
		{
			return 0;
		}
	},

	duration: function()
	{
		///	<summary>
		///		Gets the length of the media loaded in the media element expressed in seconds.
		///	</summary>
		///	<returns type="Number" />

		try
		{
			return this.silverlightPlayer.EndPositionSeconds;
		}
		catch(e)
		{
			return 0;
		}
	},

	error: function()
	{
		///	<summary>
		///		Gets the last error the media element encountered.
		///	</summary>
		///	<returns type="Number" />

		return this.lastError;
	},

	muted: function(value)
	{
		///	<summary>
		///		Gets or sets a boolean value indicating whether the media element is muted.
		///	</summary>
		///	<param name="value" type="Boolean" />
		///	<returns type="Boolean" />

		if (value !== undefined)
		{
			this.silverlightPlayer.IsMuted = value;

			// Silverlight player will not dispatch the "volumechanged" when the IsMuted property changes.
			// The media plugin interface must dispatch the "volumechanged" to indicate that the media element is muted.
			this.silverlightPlayer.SetVolume(this.silverlightPlayer.GetVolume() + .00001);
			this.silverlightPlayer.SetVolume(this.silverlightPlayer.GetVolume() - .00001);
		}
		
		return this.silverlightPlayer.IsMuted;
	},

	networkState: function()
	{
		return PlayerFramework.VideoMediaPlugin.NetworkState.NETWORK_IDLE;
	},

	paused: function()
	{
		///	<summary>
		///		Gets a boolean value indicating whether the media element is paused.
		///	</summary>
		///	<returns type="Boolean" />

		return this.silverlightPlayer.PlayState === PlayerFramework.Plugins.SilverlightMediaPluginBase.PlayState.Paused;
	},

	poster: function()
	{
		///	<summary>
		///		Gets the URL of the poster image to display before initiating playback.
		///	</summary>
		///	<returns type="Boolean" />

		var posterAttribute = this.element.getAttribute("data-poster");

		if (posterAttribute)
			return posterAttribute.valueOf();

		return null;
	},

	preload: function()
	{
		///	<summary>
		///		Gets the preload state of "none", "metadata", or "auto".
		///	</summary>
		///	<returns type="String" />

		return "auto";
	},

	readyState: function()
	{
		///	<summary>
		///		Returns a value that expresses the current state of the element with respect to
		///		rendering the current playback position, from
		///		PlayerFramework.VideoMediaPlugin.ReadyState.
		///	</summary>
		///	<returns type="Number" />

		return this.silverlightPlayer.PlayState > PlayerFramework.Plugins.SilverlightMediaPluginBase.PlayState.Buffering
				? PlayerFramework.VideoMediaPlugin.ReadyState.HAVE_ENOUGH_DATA
				: PlayerFramework.VideoMediaPlugin.ReadyState.HAVE_NOTHING;
	},

	scrubbing: function()
	{
		///	<summary>

		///	</summary>
		///	<returns type="Boolean" />

		return this.seekingValue;
	},

	seeking: function()
	{
		///	<summary>

		///	</summary>
		///	<returns type="Boolean" />

		return this.seekingValue;
	},

	supportsVolumeSetter: function()
	{
		///	<summary>
		///		Gets a boolean value indicating whether the media element supports setting the volume.
		///	</summary>
		///	<returns type="Boolean" />

		return true;
	},

	volume: function(value)
	{
		///	<summary>
		///		Gets or sets the volume level in a range of 0.0 to 1.0.
		///	</summary>
		///	<param name="value" type="Number" />
		///	<returns type="Number" />

		if (value)
			this.silverlightPlayer.SetVolume(value);

		return this.silverlightPlayer.GetVolume();
	},

	// MediaPlugin Functions
	canPlayType: function(type)
	{
		///	<summary>
		///		Gets a boolean indicating whether the media element can play content of the specified type.
		///	</summary>
		///	<param name="type" type="String" />
		///	<returns type="Boolean" />

		var supportedTypes = [ /video\/mp4/i, /text\/xml/i ];

		for (var i = 0; i < supportedTypes.length; i++)
		{
			if (supportedTypes[i].test(type))
				return true;
		}

		return false;
	},

	checkSupport: function(callback)
	{
		///	<summary>
		///		Determines support for the media element.
		///	</summary>
		///	<param name="callback" type="Function">
		///		The function to call after support has been determined.
		///	</param>
		
		var isSupported = false;

		// Check if the Silverlight plugin is available.
		if (navigator.plugins)
		{
			isSupported = !!navigator.plugins["Silverlight Plug-In"];
		}

		// If using IE, check if ActiveXObjects are available and attempt to create a Silverlight plugin.
		if (!isSupported && window.ActiveXObject)
		{
			try
			{
				var slControl = new ActiveXObject("AgControl.AgControl");
				isSupported = true;
			}
			catch (e)
			{
			}
		}
		
		if (this.options.initParams && !this.options.initParams.mediaurl)
			isSupported = false;

		if (isSupported)
		{
			this.checkSupportCallback = callback;
			this.onLoadOrError();
		}
		else
		{
			callback(false);
		}
	},

	pause: function()
	{
		///	<summary>
		///		Pauses the media.
		///	</summary>

		try
		{
			this.silverlightPlayer.Pause();
		}
		catch(e)
		{
		}
	},
	
	play: function()
	{
		///	<summary>
		///		Plays the media.
		///	</summary>

		this.silverlightPlayer.Play();
	},

	setup: function()
	{
		///	<summary>
		///		Completes remaining setup operations after the media plugin is selected as the supported media plugin.
		///	</summary>
	},

	// Functions
	addEventListeners: function()
	{
		///	<summary>
		///		Adds event listeners to the media element.
		///	</summary>

		// Using addEventListener directly instead of PlayerFramework.addEvent because addEventListener will be supported by Silverlight player.
		this.silverlightPlayer.addEventListener("ApplicationExit", PlayerFramework.proxy(this, this.onUnloadingMediaPlugin), false);
		this.silverlightPlayer.addEventListener("MediaEnded", PlayerFramework.proxy(this, this.onEnded), false);
		this.silverlightPlayer.addEventListener("MediaFailed", PlayerFramework.proxy(this, this.onMediaFailed), false);
		this.silverlightPlayer.addEventListener("PlayStateChanged", PlayerFramework.proxy(this, this.onPlayStateChanged), false);
		this.silverlightPlayer.addEventListener("PlaybackPositionChanged", PlayerFramework.proxy(this, this.onTimeUpdate), false);
		this.silverlightPlayer.addEventListener("VolumeLevelChanged", PlayerFramework.proxy(this, this.onVolumeChange), false);
		this.silverlightPlayer.addEventListener("SeekCompleted", PlayerFramework.proxy(this, this.onSeekCompleted), false);
		PlayerFramework.addEvent(this.element, "mouseout", PlayerFramework.proxy(this, this.onMouseOut));
		PlayerFramework.addEvent(this.element, "mouseover", PlayerFramework.proxy(this, this.onMouseOver));
	},

	setElement: function(element)
	{
		///	<summary>
		///		Handles needed operations after the element becomes available through either the
		///		declarative approach or the injected approach.
		///	</summary>
		///	<param name="callback" type="Function">
		///		The function to call after support has been determined.
		///	</param>

		this.element = element;
		this.element.style["-webkit-transform"] = "translateX(-" + 2000 + "px)";
		
		// Set callback functions on the element that can be called outside of an instance of a media plugin.
		// Used by the global Silverlight event handlers.
		this.element.onLoadCallback = PlayerFramework.proxy(this, this.onElementLoad);
		this.element.onErrorCallback = PlayerFramework.proxy(this, this.onElementError);
	},

	// Event Handlers
	onElementError: function()
	{
		///	<summary>
		///		Error handler callback for this specific instance of the SilverlightMediaPluginBase.
		///	</summary>
		///	<param name="callback" type="Function">
		///		The function to call after support has been determined.
		///	</param>

		this.hasError = true;
		this.onLoadOrError();
	},

	onElementLoad: function(silverlightPlayer)
	{
		///	<summary>
		///		Load handler callback for this specific instance of the SilverlightMediaPlugin.
		///	</summary>
		///	<param name="silverlightPlayer" type="Object">
		///		The JavaScript bridge object for this specific Silverlight player.
		///	</param>

		// Check if the browser is loading the plugin for the first time (some browsers reload the plugin each time it is displayed).
		var isFirstLoad = true;
		if (this.silverlightPlayer)
			isFirstLoad = false;

		this.silverlightPlayer = silverlightPlayer;
		this.isReady = true;
		this.controls(!!this.options.controls);
		this.volume(this.playerOptions.initialVolume);
		
		// Only callback if the plugin is loading for the first time, otherwise reset the timeline.
		if (isFirstLoad)
			this.onLoadOrError();
		else
			this.onTimeUpdate();
			
		if (this.controls())
			this.element.height = 360;

		this.addEventListeners();
		
		this.element.style["-webkit-transform"] = "translateX(0)";

		// Re-add ControlPlugin event listeners when Silverlight reloads.
		if (!isFirstLoad)
			this.onLoadedMediaPlugin();
	},

	onLoadOrError: function()
	{
		///	<summary>
		///		Controls when the checkSupportCallback is called by the checkSupport,
		///		onLoadCallback, and onErrorCallback functions. The checkSupport function must
		///		complete and either the onLoadCallback or the onErrorCallback function must
		///		complete.
		///	</summary>

		if (this.checkSupportCallback)
		{
			if (this.hasError)
				this.checkSupportCallback(false);
			else if (this.isReady)
				this.checkSupportCallback(true);
		}
	},

	onMediaFailed: function(e)
	{	
		///	<summary>
		///		Called if the Silverlight player's media fails to download. Creates MediaError
		///		specifying a network error and calls the base onError handler.
		///	</summary>

		this.lastError =
		{
			code: PlayerFramework.VideoMediaPlugin.MediaError.MEDIA_ERR_NETWORK
		};
		
		this.onError();
	},
	
	onPlayStateChanged: function()
	{	
		///	<summary>
		///		Handler that determines if the Silverlight player is in a Playing or Paused stated and calls the
		///		respective media plugin handler.
		///	</summary>

		try
		{
			var playState = this.silverlightPlayer.PlayState;
			if (playState === PlayerFramework.Plugins.SilverlightMediaPluginBase.PlayState.Paused)
				this.onPause();
			else if (playState === PlayerFramework.Plugins.SilverlightMediaPluginBase.PlayState.Playing)
				this.onPlay();
		
			if (playState === PlayerFramework.Plugins.SilverlightMediaPluginBase.PlayState.Playing	||
				playState === PlayerFramework.Plugins.SilverlightMediaPluginBase.PlayState.Paused	||
				playState === PlayerFramework.Plugins.SilverlightMediaPluginBase.PlayState.Stopped	||
				playState === PlayerFramework.Plugins.SilverlightMediaPluginBase.PlayState.ClipPlaying)
				this.onCanPlayThrough();
		}
		catch(e)
		{
		}
	},

	onSeekCompleted: function()
	{
		///	<summary>
		///		Handler that determines if the Silverlight player is currently seeking
		///		or if it has completed seeking.
		///	</summary>

		var currentSeekTime = Date.now();

		PlayerFramework.proxy(this, function(previousSeekTime)
		{
			window.setTimeout(PlayerFramework.proxy(this, function()
			{
				this.seekingValue = currentSeekTime != previousSeekTime;

				if (this.seekingValue)
					this.onSeeking();
				else
					this.onSeeked();
			}), 100);
		})(currentSeekTime);
	}
});

PlayerFramework.Plugins.SilverlightMediaPluginBase.PlayState =
{
	///	<summary>
	///		A JSON object used to store the values of the Silverlight player's PlayState enum.
	///		Used by the onPlayStateChanged to determine if the Silverlight player is in a Playing
	///		or Paused stated.
	///	</summary>

	Closed: 0,
	Opening: 1,
	Buffering: 2,
	Playing: 3,
	Paused: 4,
	Stopped: 5,
	Individualizing: 6,
	AcquiringLicense: 7,
	ClipPlaying: 100
};

// Global Silverlight Event Handlers (must be global to be referenced by the param tags in the Silverlight object tag)
var onSilverlightError = function(sender, args)
{
	///	<summary>
	///		Handler for the Silverlight onError event that calls the onErrorCallback set by the
	///		SilverlightMediaPluginBase instance related to that particular object element.
	///	</summary>

	// Check error code to avoid issue where Silverlight throws the following error in FireFox 4:
	//		Unhandled Error in Silverlight Application
	//		Code: 2104
	//		Category: InitializeError
	//		Message: Could not download the Silverlight application. Check web server settings 
	if (args.ErrorCode.toString() !== "2104")
	{
		var element = args.getHost();

		if (element.onErrorCallback)
			element.onErrorCallback();
	}
};

var onSilverlightLoad = function(sender, args)
{
	///	<summary>
	///		Handler for the Silverlight onLoad event that gets a reference to the JavaScript bridge
	///		and calls the onLoadCallback set by the SilverlightMediaPluginBase instance related to
	///		that particular object element.
	///	</summary>
	
	var element = sender.getHost();
	var silverlightPlayer;
	
	try
	{
		silverlightPlayer = element.Content.Player;
	}
	catch (e)
	{
	}

	if (silverlightPlayer && element.onLoadCallback)
		element.onLoadCallback(silverlightPlayer);
};

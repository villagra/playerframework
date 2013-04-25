PlayerFramework.Plugins.VideoElementMediaPluginBase = PlayerFramework.VideoMediaPlugin.extend(
{
	defaultOptions: function(player, playerOptions)
	{
		return PlayerFramework.mergeOptions(this._super(),
		{
			"class": "pf-video",
			controls: false,
			supportsTrackElements: true
		});
	},

	init: function(player, options, playerOptions)
	{
		///	<summary>
		///		Initializes the VideoMediaPlugin that wraps the HTML5 video element.
		///	</summary>
		///	<param name="player" type="Object">
		///		The Player object.
		///	</param>
		///	<param name="options" type="Object">
		///		The options for the VideoElementMediaPluginBase.
		///	</param>
		///	<param name="playerOptions" type="Object">
		///		The merged player options for the current media source.
		///	</param>
		///	<returns type="VideoElementMediaPluginBase" />

		this._super(player, options, playerOptions);
		
		if (this.options.element)
		{
			if (this.options.element.tagName && this.options.element.tagName.toLowerCase() !== "video")
				throw new TypeError("options.element not a video tag.");

			this.element = this.options.element;
		}
	},

	// VideoMediaPlugin Properties
	buffered: function()
	{
		///	<summary>
		///		Gets a TimeRanges object that represents the ranges of the media resource.
		///	</summary>
		///	<returns type="TimeRanges" />

		return this.element.buffered;
	},

	controls: function(value)
	{
		///	<summary>
		///		Gets or sets a boolean indicating whether the media element displays a control strip.
		///	</summary>
		///	<param name="value" type="Boolean" />
		///	<returns type="Boolean" />

		if (value !== undefined)
			this.element.controls = value;

		return this.element.controls;
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

		if (value !== undefined)
			this.element.currentTime = value;
		
		return this.element.currentTime;
	},

	duration: function()
	{
		///	<summary>
		///		Gets the length of the media loaded in the media element expressed in seconds.
		///	</summary>
		///	<returns type="Number" />

		return this.element.duration || 0;
	},

	error: function()
	{
		///	<summary>
		///		Gets the last error the media element encountered.
		///	</summary>
		///	<returns type="Number" />

		return this.element.error;
	},

	ended: function()
	{
		///	<summary>
		///		Gets a boolean value indicating whether the media element has ended.
		///	</summary>
		///	<returns type="Boolean" />

		return this.element.ended;
	},

	muted: function(value)
	{
		///	<summary>
		///		Gets or sets a boolean value indicating whether the media element is muted.
		///	</summary>
		///	<param name="value" type="Boolean" />
		///	<returns type="Boolean" />

		if (value !== undefined)
			this.element.muted = value;
		
		return this.element.muted;
	},

	networkState: function()
	{
		///	<summary>
		///		Returns a value that expresses the current state of the element with respect to
		///		loading a resource over the network, from
		///		PlayerFramework.VideoMediaPlugin.NetworkState.
		///	</summary>
		///	<returns type="Number" />

		var networkState;

		switch (this.element.networkState)
		{
			case HTMLMediaElement.NETWORK_EMPTY:
				networkState = PlayerFramework.VideoMediaPlugin.NetworkState.NETWORK_EMPTY;
				break;

			case HTMLMediaElement.NETWORK_IDLE:
				networkState = PlayerFramework.VideoMediaPlugin.NetworkState.NETWORK_IDLE;
				break;

			case HTMLMediaElement.NETWORK_LOADING:
				networkState = PlayerFramework.VideoMediaPlugin.NetworkState.NETWORK_LOADING;
				break;

			case HTMLMediaElement.NETWORK_LOADED:
				// Available only in Safari and not in the W3C spec; Mapped to NETWORK_IDLE.
				networkState = PlayerFramework.VideoMediaPlugin.NetworkState.NETWORK_IDLE;
				break;

			case HTMLMediaElement.NETWORK_NO_SOURCE:
				networkState = PlayerFramework.VideoMediaPlugin.NetworkState.NETWORK_NO_SOURCE;
				break;
		}

		return networkState;
	},

	paused: function()
	{
		///	<summary>
		///		Gets a boolean value indicating whether the media element is paused.
		///	</summary>
		///	<returns type="Boolean" />

		return this.element.paused;
	},

	poster: function()
	{
		///	<summary>
		///		Gets the URL of the poster image to display before initiating playback.
		///	</summary>
		///	<returns type="Boolean" />

		return this.element.poster;
	},

	preload: function()
	{
		///	<summary>
		///		Gets the preload state of "none", "metadata", or "auto".
		///	</summary>
		///	<returns type="String" />

		return this.element.preload;
	},

	readyState: function()
	{
		///	<summary>
		///		Returns a value that expresses the current state of the element with respect to
		///		rendering the current playback position, from
		///		PlayerFramework.VideoMediaPlugin.ReadyState.
		///	</summary>
		///	<returns type="Number" />

		var readyState;

		switch (this.element.readyState)
		{
			case HTMLMediaElement.HAVE_NOTHING:
				readyState = PlayerFramework.VideoMediaPlugin.ReadyState.HAVE_NOTHING;
				break;

			case HTMLMediaElement.HAVE_METADATA:
				readyState = PlayerFramework.VideoMediaPlugin.ReadyState.HAVE_METADATA;
				break;

			case HTMLMediaElement.HAVE_CURRENT_DATA:
				readyState = PlayerFramework.VideoMediaPlugin.ReadyState.HAVE_CURRENT_DATA;
				break;

			case HTMLMediaElement.HAVE_FUTURE_DATA:
				readyState = PlayerFramework.VideoMediaPlugin.ReadyState.HAVE_FUTURE_DATA;
				break;

			case HTMLMediaElement.HAVE_ENOUGH_DATA:
				readyState = PlayerFramework.VideoMediaPlugin.ReadyState.HAVE_ENOUGH_DATA;
				break;
		}

		return readyState;
	},

	scrubbing: function()
	{
		///	<summary>

		///	</summary>
		///	<returns type="Boolean" />

		return this.element.seeking;
	},

	seeking: function()
	{
		///	<summary>

		///	</summary>
		///	<returns type="Boolean" />

		return this.element.seeking;
	},

	supportsVolumeSetter: function()
	{
		///	<summary>
		///		Gets a boolean value indicating whether the media element supports setting the volume.
		///	</summary>
		///	<returns type="Boolean" />

		// Try decrementing the volume level. If the volume level remains at 1, then the volume level
		// cannot be adjusted.
		this.element.volume -= .01;
		var canChangeVolume = this.volume() !== 1;
		if (canChangeVolume)
			this.element.volume += .01;
		return canChangeVolume;
	},

	volume: function(value)
	{
		///	<summary>
		///		Gets or sets the volume level in a range of 0.0 to 1.0.
		///	</summary>
		///	<param name="value" type="Number" />
		///	<returns type="Number" />

		if (value)
			this.element.volume = value;

		return this.element.volume;
	},

	// MediaPlugin Functions
	canPlayType: function(type)
	{
		///	<summary>
		///		Gets a boolean indicating whether the media element can play content of the specified type.
		///	</summary>
		///	<param name="type" type="String" />
		///	<returns type="Boolean" />

	    var videoElement = this.element ? this.element : document.createElement("video");

	    if (videoElement.canPlayType)
	        return videoElement.canPlayType(type);

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

		var firstSupportedSource = null;
		
		if (this.element.canPlayType)
		{
			var sourceElementArray = PlayerFramework.convertNodeListToArray(this.element.getElementsByTagName("source"));
			var sources = PlayerFramework.filter(sourceElementArray, PlayerFramework.proxy(this, function(s)
			{
				return s.parentNode === this.element;
			}));

			// Iterate over the video sources and use the video's canPlayType function to determine support.
			firstSupportedSource = PlayerFramework.first(sources, PlayerFramework.proxy(this, function(s)
			{
				return !!this.canPlayType(s.type).replace(/no/, "");
			}));

			// Detect stock Android browser and force .mp4 source if one exists
			// (feature detection is not possible: https://github.com/Modernizr/Modernizr/wiki/Undetectables)
			if (!firstSupportedSource && /Android.*AppleWebKit/i.test(navigator.userAgent))
			{
				firstSupportedSource = PlayerFramework.first(sources, PlayerFramework.proxy(this, function(s)
				{
					return s.type.match(/mp4/);
				}));

				if (firstSupportedSource)
				{
					this.element.src = firstSupportedSource.src;
					this.element.load();
				}
			}
		}

		callback(!!firstSupportedSource);
	},

	pause: function()
	{
		///	<summary>
		///		Pauses the media.
		///	</summary>
		
		this.element.pause();
	},

	play: function()
	{
		///	<summary>
		///		Plays the media.
		///	</summary>
		
		this.element.play();
	},

	setup: function()
	{
		///	<summary>
		///		When overridden in a derived class, completes remaining setup operations after the media plugin is selected as the supported media plugin.
		///	</summary>

		this.controls(!!this.options.controls);
		this.volume(this.playerOptions.initialVolume);

		// Detect iOS by checking if volume can be set using the API.
		// Remove, clone, and re-add video element (iOS seems to prevent controlling the video tag
		// if it isn't displayed or added after initialization).
		if (!this.controls() && !this.supportsVolumeSetter()) 
			this.cloneAndReplaceVideoElement();

		if (this.preload() != "none")
			window.setTimeout(PlayerFramework.proxy(this, function() { this.checkBufferProgress(10, 0, 0); }), 1);
		else
			window.setTimeout(PlayerFramework.proxy(this, this.onCanPlayThrough), 1);

		this.addEventListeners();
		this.checkNetworkState();
	},

	// Functions
	addEventListeners: function()
	{
		///	<summary>
		///		Adds event listeners to the media element.
		///	</summary>
		
		PlayerFramework.addEvent(this.element, "canplay", PlayerFramework.proxy(this, this.onCanPlay));
		PlayerFramework.addEvent(this.element, "canplaythrough", PlayerFramework.proxy(this, this.onCanPlayThrough));
		PlayerFramework.addEvent(this.element, "error", PlayerFramework.proxy(this, this.onError));
		PlayerFramework.addEvent(this.element, "ended", PlayerFramework.proxy(this, this.onEnded));
		PlayerFramework.addEvent(this.element, "loadeddata", PlayerFramework.proxy(this, this.onLoadedData));
		PlayerFramework.addEvent(this.element, "loadedmetadata", PlayerFramework.proxy(this, this.onLoadedMetadata));
		PlayerFramework.addEvent(this.element, "pause", PlayerFramework.proxy(this, this.onPause));
		PlayerFramework.addEvent(this.element, "play", PlayerFramework.proxy(this, this.onPlay));
		PlayerFramework.addEvent(this.element, "progress", PlayerFramework.proxy(this, this.onProgress));
		PlayerFramework.addEvent(this.element, "seeked", PlayerFramework.proxy(this, this.onSeeked));
		PlayerFramework.addEvent(this.element, "seeking", PlayerFramework.proxy(this, this.onSeeking));
		PlayerFramework.addEvent(this.element, "timeupdate", PlayerFramework.proxy(this, this.onTimeUpdate));
		PlayerFramework.addEvent(this.element, "volumechange", PlayerFramework.proxy(this, this.onVolumeChange));
		PlayerFramework.addEvent(this.element, "mouseout", PlayerFramework.proxy(this, this.onMouseOut));
		PlayerFramework.addEvent(this.element, "mouseover", PlayerFramework.proxy(this, this.onMouseOver));
	},

	checkBufferProgress: function(maxBufferPollingIterations, bufferPollingIterations, previousBufferFraction)
	{
		///	<summary>
		///		Workaround for Safari: Waits for the video's ready state to be set to "HAVE_ENOUGH_DATA"
		///		Workaround for Mobile Safari: Polls for changes in the buffer. If there are no
		///		changes in the buffer after maxBufferPollingIterations, then the "canplaythrough"
		///		event is dispatched.
		///	</summary>

		if (this.readyState() === PlayerFramework.VideoMediaPlugin.ReadyState.HAVE_ENOUGH_DATA || this.buffered().length == 0)
		{
			window.setTimeout(PlayerFramework.proxy(this, this.onCanPlayThrough), 1);
			return;
		}
		else if (this.buffered().length > 0)
		{
			var duration = this.duration() > 0 ? this.duration() : 0;
			var bufferFraction = this.duration() > 0 ? this.buffered().end(0) / this.duration() : 0;
			
			if (bufferFraction === previousBufferFraction)
			{
				if (bufferPollingIterations === maxBufferPollingIterations)
				{
					this.onCanPlayThrough();
					return;
				}
				else
				{
					bufferPollingIterations++;
				}
			}
			else
			{
				previousBufferFraction = bufferFraction;
				bufferPollingIterations = 0;
			}
		}
		else if (this.networkState() === PlayerFramework.VideoMediaPlugin.NetworkState.NETWORK_NO_SOURCE || this.error())
		{
			return;
		}

		window.setTimeout(PlayerFramework.proxy(this, function()
		{
			this.checkBufferProgress(maxBufferPollingIterations, bufferPollingIterations, previousBufferFraction);
		}), 100);
	},

	checkNetworkState: function(previousNetworkState)
	{
		///	<summary>
		///		Polls for changes in the media element's network state property.
		///	</summary>

		var currentNetworkState = this.networkState();

		if (currentNetworkState !== previousNetworkState)
		{
			window.setTimeout(PlayerFramework.proxy(this, this.onNetworkStateChange), 1);
			previousNetworkState = currentNetworkState;
		}

		window.setTimeout(PlayerFramework.proxy(this, function()
		{
			this.checkNetworkState(previousNetworkState);
		}), 100);
	},

	cloneAndReplaceVideoElement: function()
	{
		///	<summary>
		///		Clones and replaces the video element. Used in the case of iOS to take control of the video element.
		///	</summary>

		var nextSibling = this.element.nextSibling;
		var parentNode = this.element.parentNode;
		var oldChild = parentNode.removeChild(this.element);
		this.element = oldChild.cloneNode(true);
		// Move off screen until the video starts to hide QuickTime logo.
		this.element.style["-webkit-transform"] = "translateX(-" + 2000 + "px)";
		PlayerFramework.addEvent(this.element, "timeupdate", PlayerFramework.proxy(this, function()
		{
			this.element.style["-webkit-transform"] = "translateX(0)";
		}));
		parentNode.insertBefore(this.element, nextSibling);
	}
});

// Workaround to make video tag styleable in IE<9: http://ejohn.org/blog/html5-shiv
// The script or the following line must appear in the head (before the video tag is parsed by IE) for this to work.
document.createElement("video");

PlayerFramework.Plugins.PlaylistControlPlugin = PlayerFramework.ControlPlugin.extend(
{
	defaultOptions: function(player)
	{
		return PlayerFramework.mergeOptions(this._super(),
		{
			strings:
			{
				toggleLabel: "PLAYLIST",
				toggleTitle: "Show Playlist",
				toggleAltTitle: "Hide Playlist",
				arrowLeftTitle: "Scroll Playlist Left",
				arrowRightTitle: "Scroll Playlist Right"
			},
			accessKeys:
			{
				toggleKey: "l",
				arrowLeftKey: "q",
				arrowRightKey: "w"
			},
			playlistPlugin: PlayerFramework.first(player.modules, function(m)
			{
				return m instanceof PlayerFramework.Plugins.PlaylistPlugin;
			})
		});
	},

	isEnabled: function(player, options)
	{
		return !!options.playlistPlugin;
	},

	init: function(player, options)
	{
		///	<summary>
		///		Initializes the ControlPlugin that provides UI for controlling the playlist.
		///	</summary>
		///	<param name="player" type="Object">
		///		The Player object.
		///	</param>
		///	<param name="options" type="Object">
		///		The options for the PlaylistControlPlugin.
		///	</param>
		///	<returns type="PlaylistControlPlugin" />

		this._super(player, options);

		// Find the required plugins.
		this.playlistPlugin = this.options.playlistPlugin;

		if (!this.playlistPlugin)
			throw new Error("PlaylistPlugin required.");

		this.playlistItemSpacer = 10;
		this.playInitiated = false;
	},

	// Event Handlers
	onLoadedMediaPlugin: function(e)
	{
		///	<summary>
		///		Dispatches the "loadedMediaPlugin" event on behalf of the Player object.
		///	</summary>

		this.setSelectedPlaylistItem();
	},

	onPlayerMouseOut: function(e)
	{
		///	<summary>
		///		Called when the mouse leaves the media element.
		///	</summary>

		if (this.playlistElement && this.playlistToggleElement && this.playInitiated)
		{
			this.playlistToggleElement.style.display = "none";
			
			if (this.playlistElement.style.top === "0px")
				this.playlistElement.style.display = "none";
		}
	},

	onPlayerMouseOver: function(e)
	{
		///	<summary>
		///		Called when the mouse enters the player.
		///	</summary>
		
		if (this.playlistElement && this.playlistToggleElement && this.playInitiated)
		{
			this.playlistToggleElement.style.display = "block";

			if (this.playlistElement.style.top === "0px")
				this.playlistElement.style.display = "block";
		}
	},

	onPlay: function(e)
	{
		///	<summary>
		///		Called when the mouse enters the player.
		///	</summary>
		
		this.playInitiated = true;
	},

	onPlaylistItemAdded: function(e)
	{
		///	<summary>
		///		Called when a playlist item is added.
		///	</summary>
		
		this.playlistItemsElement.innerHTML = "";
		this.createPlaylistItems();
		this.setSelectedPlaylistItem();
	},

	onPlaylistItemRemoved: function(e)
	{
		///	<summary>
		///		Called when a playlist item is removed.
		///	</summary>
		
		this.playlistItemsElement.innerHTML = "";
		this.createPlaylistItems();
		this.setSelectedPlaylistItem();
	},

	onPlaylistItemClick: function(e)
	{
		///	<summary>
		///		Called when a playlist item is clicked.
		///	</summary>

		this.togglePlaylistDisplay(false);
		this.playlistPlugin.setPlaylistItem(e.selectedPlaylistItemIndex);
		this.setSelectedPlaylistItem();
	},

	onPlaylistArrowControlLeftElementClick: function(e)
	{
		///	<summary>
		///		Called when the left playlist navigation arrow is clicked or activated.
		///	</summary>

		if (this.arrowElementMouseDownEventFired)
			this.arrowElementMouseDownEventFired = false;
		else
			this.scrollPlaylist(this.playlistItemWidth + this.playlistItemSpacer);
	},

	onPlaylistArrowControlLeftElementMouseDown: function(e)
	{
		///	<summary>
		///		Called when the left playlist navigation arrow is pressed.
		///	</summary>

		this.arrowElementMouseDown = true;
		this.arrowElementMouseDownEventFired = true;
		this.scrollPlaylist(5);
	},

	onPlaylistArrowControlRightElementClick: function(e)
	{
		///	<summary>
		///		Called when the right playlist navigation arrow is clicked or activated.
		///	</summary>

		if (this.arrowElementMouseDownEventFired)
			this.arrowElementMouseDownEventFired = false;
		else
			this.scrollPlaylist(-(this.playlistItemWidth + this.playlistItemSpacer));
	},

	onPlaylistArrowControlRightElementMouseDown: function(e)
	{
		///	<summary>
		///		Called when the right playlist navigation arrow is pressed.
		///	</summary>

		this.arrowElementMouseDown = true;
		this.arrowElementMouseDownEventFired = true;
		this.scrollPlaylist(-5);	
	},

	onPlaylistArrowElementMouseUp: function(e)
	{
		///	<summary>
		///		Called when the a playlist navigation arrow is released.
		///	</summary>

		this.arrowElementMouseDown = false;
	},

	onPlaylistToggleElementClick: function(e)
	{
		///	<summary>
		///		Called when the the playlist toggle button is clicked.
		///	</summary>

		this.togglePlaylistDisplay(this.playlistElement.style.top !== "0px");
	},

	onReady: function(e)
	{
		///	<summary>
		///		When overridden in a derived class, called when the player is ready for playback.
		///	</summary>

		this.createControl();
		this.addEventListeners();
	},

	// Functions
	addEventListeners: function()
	{
		///	<summary>
		///		Adds event listeners to the control's elements.
		///	</summary>
		
		PlayerFramework.addEvent(this.player, "playlistitemadded", PlayerFramework.proxy(this, this.onPlaylistItemAdded));
		PlayerFramework.addEvent(this.player, "playlistitemremoved", PlayerFramework.proxy(this, this.onPlaylistItemRemoved));
		PlayerFramework.addEvent(this.player, "play", PlayerFramework.proxy(this, this.onPlay));
		PlayerFramework.addEvent(this.playlistElement, "mouseover", PlayerFramework.mouseEventProxy(this.player, "mouseover"));
		PlayerFramework.addEvent(this.playlistElement, "mouseout", PlayerFramework.mouseEventProxy(this.player, "mouseout"));
		PlayerFramework.addEvent(this.playlistToggleElement, "mouseover", PlayerFramework.mouseEventProxy(this.player, "mouseover"));
		PlayerFramework.addEvent(this.playlistToggleElement, "mouseout", PlayerFramework.mouseEventProxy(this.player, "mouseout"));
		PlayerFramework.addEvent(this.playlistToggleElement, "click", PlayerFramework.proxy(this, this.onPlaylistToggleElementClick));
		PlayerFramework.addEvent(this.playlistArrowControlLeftElement, "mousedown", PlayerFramework.proxy(this, this.onPlaylistArrowControlLeftElementMouseDown));
		PlayerFramework.addEvent(this.playlistArrowControlLeftElement, "mouseup", PlayerFramework.proxy(this, this.onPlaylistArrowElementMouseUp));
		PlayerFramework.addEvent(this.playlistArrowControlLeftElement, "click", PlayerFramework.proxy(this, this.onPlaylistArrowControlLeftElementClick));
		PlayerFramework.addEvent(this.playlistArrowControlRightElement, "mousedown", PlayerFramework.proxy(this, this.onPlaylistArrowControlRightElementMouseDown));
		PlayerFramework.addEvent(this.playlistArrowControlRightElement, "mouseup", PlayerFramework.proxy(this, this.onPlaylistArrowElementMouseUp));
		PlayerFramework.addEvent(this.playlistArrowControlRightElement, "click", PlayerFramework.proxy(this, this.onPlaylistArrowControlRightElementClick));
	},

	createControl: function()
	{
		///	<summary>
		///		Creates and appends all markup for the controls to the DOM.
		///	</summary>

		this.playlistToggleElement = PlayerFramework.createElement(this.player.containerElement,
		[
			"div",
			{
				"class": "pf-playlist-toggle"
			}
		]);

		this.playlistToggleButtonElement = PlayerFramework.createElement(this.playlistToggleElement,
		[
			"button",
			{
				"class": "pf-button",
				type: "button",
				title: this.options.strings.toggleTitle,
				accessKey: this.options.accessKeys.toggleKey
			},
			[
				"span",
				{
					"class": "pf-playlist-toggle-text"
				},
				this.options.strings.toggleLabel
			]
		]);

		this.playlistElement = PlayerFramework.createElement(this.player.containerElement,
		[
			"div",
			{
				"class": "pf-playlist",
				style: "display: block;" // Ensure playlist div is displaying to get height below.
			},
			[
				"div",
				{
					"class": "pf-playlist-arrow-control pf-playlist-arrow-control-left"
				},
				[
					"button",
					{
						"class": "pf-button",
						type: "button",
						title: this.options.strings.arrowLeftTitle,
						accessKey: this.options.accessKeys.arrowLeftKey
					},
					[
						"span",
						{
							"class": "pf-playlist-arrow"
						}
					]
				]
			],
			[
				"div",
				{
					"class": "pf-playlist-items-container"
				},
				[
					"div",
					{
						"class": "pf-playlist-items"
					}
				]
			],
			[
				"div",
				{
					"class": "pf-playlist-arrow-control pf-playlist-arrow-control-right"
				},
				[
					"button",
					{
						"class": "pf-button",
						type: "button",
						title: this.options.strings.arrowRightTitle,
						accessKey: this.options.accessKeys.arrowRightKey
					},
					[
						"span",
						{
							"class": "pf-playlist-arrow"
						}
					]
				]
			]
		]);

		this.playlistArrowControlLeftElement = PlayerFramework.getElementsByClass("pf-playlist-arrow-control-left", this.playlistElement)[0];
		this.playlistArrowControlRightElement = PlayerFramework.getElementsByClass("pf-playlist-arrow-control-right", this.playlistElement)[0];
		this.playlistItemsContainerElement = PlayerFramework.getElementsByClass("pf-playlist-items-container", this.playlistElement)[0];
		this.playlistItemsElement = PlayerFramework.getElementsByClass("pf-playlist-items", this.playlistElement)[0];
		this.playlistItemsContainerElementWidth = this.playlistItemsContainerElement.offsetWidth;
		this.playlistElementHeight = this.playlistElement.offsetHeight; // Store for use when toggling playlist display.
		this.playlistItemWidth = this.playlistElement.offsetHeight; // Derived from (4/3) * (3/4) * playlist height (4:3 ratio multiplied by 75% of the playlist height).
		
		this.playlistElement.style.display = "none";

		this.createPlaylistItems();
	},

	createPlaylistItems: function()
	{
		///	<summary>
		///		Creates all playlist items.
		///	</summary>

		for (var i = 0; i < this.playlistPlugin.playlistItems.length; i++)
		{
			this.createPlaylistItemControl(this.playlistPlugin.playlistItems[i], i);
		}
	},

	createPlaylistItemControl: function(playlistItem, playlistItemIndex)
	{
		///	<summary>
		///		Creates a single playlist item control.
		///	</summary>

		var playlistItemElement = PlayerFramework.createElement(this.playlistItemsElement,
		[
			"div",
			{
				"class": "pf-playlist-item",					
				style: "left: " + playlistItemIndex * (this.playlistItemWidth + this.playlistItemSpacer) + "px; width: " + this.playlistItemWidth + "px;"
			}
		]);

		var playlistItemButtonElement = PlayerFramework.createElement(playlistItemElement,
		[
			"button",
			{
				"class": "pf-button",
				type: "button",
				title: playlistItem.title
			}
		]);

		if (playlistItem.poster)
		{
			PlayerFramework.createElement(playlistItemButtonElement,
			[
				"img",
				{
					src: playlistItem.poster,
					style: "width: 100%; height: 100%;"
				}
			]);
		}

		PlayerFramework.proxy(this, function(playlistItemIndex)
		{
			PlayerFramework.addEvent(playlistItemElement, "click", PlayerFramework.proxy(this, function(e)
			{
				this.onPlaylistItemClick(
				{
					selectedPlaylistItemIndex: playlistItemIndex
				});
			}));
		})(playlistItemIndex);
	},

	togglePlaylistDisplay: function(showPlaylist)
	{
		///	<summary>
		///		Toggles the display of the playlist.
		///	</summary>
		///	<param name="showPlaylist" type="Boolean">
		///		Determines whether to show or hide the playlist.
		///	</param>

		if (this.mediaPlugin())
		{
			if (showPlaylist)
			{
				this.playlistElement.style.display = "block";
				this.playlistElement.style.top = "0px";
				this.playlistToggleElement.style.display = "block";
				this.playlistToggleElement.style.top = this.playlistElementHeight + "px";
				this.playlistToggleButtonElement.setAttribute("title", this.options.strings.toggleAltTitle);
			}
			else
			{
				this.playlistElement.style.display = "none";
				this.playlistElement.style.top = -this.playlistElementHeight + "px";
				this.playlistToggleElement.style.display = "none";
				this.playlistToggleElement.style.top = "0px";
				this.playlistToggleButtonElement.setAttribute("title", this.options.strings.toggleTitle);
			}
		}
	},

	setSelectedPlaylistItem: function()
	{
		///	<summary>
		///		Changes the style of the current playlist item control to appear selected and changes
		///		the style of all other playlist item controls to appear deselected.
		///	</summary>

		var playlistItemElements = this.playlistItemsElement.childNodes;
		
		// Reset styles for all playlist items.
		PlayerFramework.forEach(playlistItemElements, function(item)
		{
			item.className = "pf-playlist-item";
		});

		var selectedPlaylistItem = playlistItemElements[this.playlistPlugin.currentPlaylistItemIndex];
		
		// Change style.
		selectedPlaylistItem.className = "pf-playlist-item pf-playlist-item-selected";

		// Scroll into view.
		var leftValue = parseInt(this.playlistItemsElement.style.left || "0px");
		if (leftValue + selectedPlaylistItem.offsetLeft + this.playlistItemWidth > this.playlistItemsContainerElementWidth)
		{
			var offset = this.playlistItemsContainerElementWidth - (this.playlistPlugin.currentPlaylistItemIndex + 1) * (this.playlistItemWidth + this.playlistItemSpacer);
			this.playlistItemsElement.style.left = offset + "px";
		}
	},

	scrollPlaylist: function(delta)
	{	
		///	<summary>
		///		Scrolls the playlist along the x-axis by the given delta.
		///	</summary>
		///	<param name="showPlaylist" type="Boolean">
		///		Determines whether to show or hide the playlist.
		///	</param>

		var leftValue = parseInt(this.playlistItemsElement.style.left || "0px") + delta;
		var maxLeft = (this.playlistPlugin.playlistItems.length - 1) * (this.playlistItemWidth + this.playlistItemSpacer);

		if (leftValue > 0)
			leftValue = 0;
		else if (leftValue < -maxLeft)
			leftValue = -maxLeft;
				
		this.playlistItemsElement.style.left = leftValue + "px";

		// Loop animation while mouse is down.
		window.setTimeout(PlayerFramework.proxy(this, function()
		{
			if (this.arrowElementMouseDown)
				this.scrollPlaylist(delta);
		}), 15);
	}
});

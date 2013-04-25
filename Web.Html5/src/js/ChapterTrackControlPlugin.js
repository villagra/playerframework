PlayerFramework.Plugins.ChapterTrackControlPlugin = PlayerFramework.ControlPlugin.extend(
{
	defaultOptions: function(player)
	{
		return PlayerFramework.mergeOptions(this._super(),
		{
			strings:
			{
				skipBackTitle: "Skip Back Chapter",
				skipForwardTitle: "Skip Forward Chapter"
			},
			accessKeys:
			{
				skipBackKey: "b",
				skipForwardKey: "n"
			},
			chapterTrackPlugin: PlayerFramework.first(player.modules, function(m)
			{
				return m instanceof PlayerFramework.Plugins.ChapterTrackPlugin;
			}),
			controlStripPlugin: PlayerFramework.first(player.modules, function(m)
			{
				return m instanceof PlayerFramework.Plugins.ControlStripPlugin;
			})
		});
	},

	isEnabled: function(player, options)
	{
		return !!options.chapterTrackPlugin && !!options.controlStripPlugin;
	},

	init: function(player, options)
	{
		///	<summary>
		///		Initializes the ChapterTrackControlPlugin that provides UI for controlling chapter navigation.
		///	</summary>
		///	<param name="player" type="Object">
		///		The Player object.
		///	</param>
		///	<param name="options" type="Object">
		///		The options for the ChapterTrackControlPlugin.
		///	</param>
		///	<returns type="ChapterTrackControlPlugin" />
		
		this._super(player, options);

		// Find the required plugins.
		this.chapterTrackPlugin = this.options.chapterTrackPlugin;
		this.controlStripPlugin = this.options.controlStripPlugin;

		if (!this.chapterTrackPlugin)
			throw new Error("ChapterTrackPlugin required.");

		if (!this.controlStripPlugin)
			throw new Error("ControlStripPlugin required.");
	},

	// Event Handlers
	onChapterCueAdded: function(e)
	{		
		///	<summary>
		///		Called when a chapter cue is added. Displays the chapter navigation arrows next to the timeline.
		///	</summary>
		
		if (this.controlStripPlugin.element.className.indexOf("pf-chapters") === -1)
			this.controlStripPlugin.element.className += " pf-chapters";

		this.controlStripPlugin.updateLayout();
	},

	onSkipBackClick: function(e)
	{		
		///	<summary>
		///		Called when the skip back chapter button is clicked.
		///	</summary>

		this.chapterTrackPlugin.skipBackChapter();
	},

	onSkipForwardClick: function(e)
	{		
		///	<summary>
		///		Called when the skip forward chapter button is clicked.
		///	</summary>

		this.chapterTrackPlugin.skipForwardChapter();
	},

	onReady: function(e)
	{		
		///	<summary>
		///		Called when the player is ready for playback.
		///	</summary>

		this.createControl();
		this.addEventListeners();
	},

	onUnloadingMediaPlugin: function(e)
	{
		///	<summary>
		///		Called when the media element unloads.
		///	</summary>

		this.controlStripPlugin.element.className = this.controlStripPlugin.element.className.replace("pf-chapters", "");

		this.controlStripPlugin.updateLayout();
	},

	// Functions
	addEventListeners: function()
	{
		///	<summary>
		///		Adds event listeners to the control's elements.
		///	</summary>
		
		PlayerFramework.addEvent(this.player, "chaptercueadded", PlayerFramework.proxy(this, this.onChapterCueAdded));
		PlayerFramework.addEvent(this.skipBackControlElement, "click", PlayerFramework.proxy(this, this.onSkipBackClick));
		PlayerFramework.addEvent(this.skipForwardControlElement, "click", PlayerFramework.proxy(this, this.onSkipForwardClick));
	},

	createControl: function()
	{
		///	<summary>
		///		Creates and appends all markup for the controls to the DOM.
		///	</summary>

		this.skipBackControlElement = PlayerFramework.createElement(null,
		[
			"div",
			{
				"class": "pf-skip-back-chapter-control"
			},
			[
				"button",
				{
					"class": "pf-button",
					type: "button",
					title: this.options.strings.skipBackTitle,
					accessKey: this.options.accessKeys.skipBackKey
				},
				[
					"span",
					{
						"class": "pf-skip-back-chapter-arrow"
					}
				],
				[
					"span",
					{
						"class": "pf-skip-back-chapter-line"
					}
				]
			]
		]);
		
		this.controlStripPlugin.element.insertBefore(this.skipBackControlElement, this.controlStripPlugin.timelineControlElement);

		this.skipForwardControlElement = PlayerFramework.createElement(null,
		[
			"div",
			{
				"class": "pf-skip-forward-chapter-control"
			},
			[
				"button",
				{
					"class": "pf-button",
					type: "button",
					title: this.options.strings.skipForwardTitle,
					accessKey: this.options.accessKeys.skipForwardKey
				},
				[
					"span",
					{
						"class": "pf-skip-forward-chapter-arrow"
					}
				],
				[
					"span",
					{
						"class": "pf-skip-forward-chapter-line"
					}
				]
			]
		]);
		
		this.controlStripPlugin.element.insertBefore(this.skipForwardControlElement, this.controlStripPlugin.timelineControlElement.nextSibling);

		this.controlStripPlugin.updateLayout();
	}
});

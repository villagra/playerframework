
namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// Helper class that includes string names of template parts for the media player
    /// </summary>
    public static class MediaPlayerTemplateParts
    {
        internal const string MediaElement ="Media";
        internal const string LayoutRootElement = "LayoutRoot";
        /// <summary>
        /// The name of the container most appropriate to place the MediaElement in
        /// </summary>
        public const string MediaContainer = "MediaContainer";
        /// <summary>
        /// The name of the container most appropriate to place an interactive UI before the media loads
        /// </summary>
        public const string LoaderViewContainer = "LoaderViewContainer";
#if SILVERLIGHT
        /// <summary>
        /// The name of the container most appropriate to place a poster image
        /// </summary>
        public const string PosterContainer = "PosterContainer";
#endif
        /// <summary>
        /// The name of the container most appropriate to place closed captioning
        /// </summary>
        public const string CaptionsContainer = "CaptionsContainer";
        /// <summary>
        /// The name of the container most appropriate to place linear advertisements
        /// </summary>
        public const string AdvertisingContainer = "AdvertisingContainer";
        /// <summary>
        /// The name of the container most appropriate to place a buffering UI
        /// </summary>
        public const string BufferingContainer = "BufferingContainer";
        /// <summary>
        /// The name of the container most appropriate to place information about errors that prevent the media from playing
        /// </summary>
        public const string ErrorsContainer = "ErrorsContainer";
        /// <summary>
        /// The name of the container most appropriate to place interactive UI elements such as the control panel
        /// </summary>
        public const string InteractivityContainer = "InteractivityContainer";
        /// <summary>
        /// The name of the container most appropriate to place settings dialogs
        /// </summary>
        public const string SettingsContainer = "SettingsContainer";
        /// <summary>
        /// The name of the control panel element
        /// </summary>
        public const string ControlPanel = "ControlPanel";
    }
}


namespace Microsoft.PlayerFramework.CaptionMarkers
{
    /// <summary>
    /// Represents an active (currently visible) closed caption
    /// </summary>
    public class ActiveCaption
    {
        /// <summary>
        /// Gets or sets the text for the caption
        /// </summary>
        public string Text { get; set; }

		public Windows.UI.Xaml.Media.Brush Background { get; set; }
    }
}

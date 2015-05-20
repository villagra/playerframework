#define CODE_ANALYSIS

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// An interface describing the contract for a MMP: Player Framework plugin.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Correctly named architectural pattern")]
    public interface IPlugin
    {
        /// <summary>
        /// The plugin is loaded. This occurs after MediaPlayer is set.
        /// </summary>
        void Load();

        /// <summary>
        /// The MediaPlayer is updating the source.
        /// </summary>
        /// <param name="mediaSource">The new source and associated properties required to control initial state and desired playback behavior.</param>
        void Update(IMediaSource mediaSource);

        /// <summary>
        /// The plugin is no longer being used and should unload.
        /// </summary>
        void Unload();

        /// <summary>
        /// The MediaPlayer the plugin is attached to.
        /// </summary>
        MediaPlayer MediaPlayer { get; set; }
    }

}

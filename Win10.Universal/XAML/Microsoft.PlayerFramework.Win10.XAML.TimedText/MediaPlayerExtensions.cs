using System.Linq;

namespace Microsoft.PlayerFramework.TimedText
{
    /// <summary>
    /// Helper class to extend the MediaPlayer with methods specific to this optional plugin.
    /// </summary>
    public static class MediaPlayerExtensions
    {
        /// <summary>
        /// Returns the active instance of the CaptionsPlugin
        /// </summary>
        /// <param name="source">The MediaPlayer the plugin is associated with.</param>
        /// <returns>The first associated instance of the CaptionsPlugin</returns>
        public static CaptionsPlugin GetCaptionsPlugin(this MediaPlayer source)
        {
            return source.Plugins.OfType<CaptionsPlugin>().FirstOrDefault();
        }
    }
}

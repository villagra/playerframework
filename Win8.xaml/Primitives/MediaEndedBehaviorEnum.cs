
namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// The behavior to control how the MediaPlayer should react to reaching the end of the media.
    /// </summary>
    public enum MediaEndedBehavior
    {
        /// <summary>
        /// Don't do anything (this will leave the MediaPlayer in a paused state).
        /// </summary>
        Manual,
        /// <summary>
        /// Issue a stop (which will reset the MediaPlayer to allow it to play when the user clicks play again).
        /// </summary>
        Stop,
        /// <summary>
        /// Reset the position to the beginning (which will put the MediaPlayer in a paused state).
        /// </summary>
        Reset
    }
}

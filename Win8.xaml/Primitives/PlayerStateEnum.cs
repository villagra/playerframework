
namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// Enum used to describe the player state. This is is different from the MediaState in that it indicates what stage of loading the media the player is in.
    /// Once the media is loaded, you should use CurrentState to examine the state of the media.
    /// </summary>
    public enum PlayerState
    {
        /// <summary>
        /// The player is unloaded, no source is set.
        /// </summary>
        Unloaded,
        /// <summary>
        /// The source is set but the player is waiting to load the media due to AutoLoad=false
        /// </summary>
        Pending,
        /// <summary>
        /// The source is set and everything is ready to go but there are actions from the MediaLoading event that are still executing.
        /// </summary>
        Loading,
        /// <summary>
        /// The media has been loaded into the player but the media has not been opened yet.
        /// </summary>
        Loaded,
        /// <summary>
        /// The media has been loaded and opened. This happens immediately before MediaOpened fires.
        /// </summary>
        Opened,
        /// <summary>
        /// The media has been told to start but there may be an async operation that needs to be performed first.
        /// </summary>
        Starting,
        /// <summary>
        /// The media has been started and is either playing or paused.
        /// </summary>
        Started,
        /// <summary>
        /// The media has finished playing and is in the process of ending. There may be async operations taking place now such as playing post roll ads.
        /// </summary>
        Ending,
        /// <summary>
        /// The media has failed and has been unloaded. You must set the source again on the player to load the media.
        /// </summary>
        Failed
    }
}

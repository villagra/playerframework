using System;

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// Used to indicate how the AutoHide feature works.
    /// </summary>
    [Flags] 
    public enum AutoHideBehavior
    {
        /// <summary> 
        /// The AutoHide feature has no special behavior.
        /// </summary> 
        None = 0,
        /// <summary> 
        /// AutoHide is allowed during media playback only.
        /// </summary> 
        AllowDuringPlaybackOnly = 1,
        /// <summary> 
        /// AutoHide is prevented when the pointer is over the control panel (and other interactive elements).
        /// </summary> 
        PreventDuringInteractiveHover = 2,
        /// <summary> 
        /// Apply all available AutoHide behaviors.
        /// </summary> 
        All = 3
    }

}

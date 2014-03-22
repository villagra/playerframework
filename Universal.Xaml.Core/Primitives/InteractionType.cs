using System;

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// Used to indicate the how interaction by the user causes the UI to be activated or deactivated.
    /// </summary>
    [Flags]
    public enum InteractionType
    {
        /// <summary>
        /// Represents no interaction.
        /// </summary>
        None = 0,
        /// <summary>
        /// Indicates a "soft" interaction (a mouse movement in case case of interactivate activation or a timer timeout in the case of deactivation).
        /// </summary>
        Soft = 1,
        /// <summary>
        /// Indicates a "hard" interaction such as a tap or click.
        /// </summary>
        Hard = 2,
        /// <summary>
        /// Indicates both "soft" and "hard" interactions.
        /// </summary>
        All = 3,
    }
}

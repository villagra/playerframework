// <copyright file="FontFamily.cs" company="Michael S. Scherotter">
// Copyright (c) 2013 Michael S. Scherotter All Rights Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>synergist@charette.com</email>
// <date>2013-11-11</date>
// <summary>Font Family</summary>

namespace Microsoft.PlayerFramework.CaptionSettings.Model
{
    /// <summary>
    /// Caption font family
    /// </summary>
    public enum FontFamily
    {
        /// <summary>
        /// Use the default font
        /// </summary>
        Default = 0,

        /// <summary>
        /// Mono-space serif (like Courier New)
        /// </summary>
        MonospaceSerif,

        /// <summary>
        /// Proportional serif (like Times New Roman)
        /// </summary>
        ProportionalSerif,

        /// <summary>
        /// Mono-space Sans serif (like Consolas)
        /// </summary>
        MonospaceSansSerif,

        /// <summary>
        /// Proportional Sans Serif (like Arial)
        /// </summary>
        ProportionalSansSerif,

        /// <summary>
        /// Casual (like Comic Sans)
        /// </summary>
        Casual,

        /// <summary>
        /// Cursive (like Segoe Script)
        /// </summary>
        Cursive,

        /// <summary>
        /// Small capitals
        /// </summary>
        Smallcaps
    }
}

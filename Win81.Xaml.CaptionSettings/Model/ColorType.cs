// <copyright file="ColorType.cs" company="Microsoft Corporation">
// Copyright (c) 2013 Microsoft Corporation All Rights Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>mischero@microsoft.com</email>
// <date>2013-10-28</date>
// <summary>Color Type enumeration</summary>

namespace Microsoft.PlayerFramework.CaptionSettings.Model
{
#if WINDOWS_PHONE
#else
    using Windows.UI.Xaml.Controls;
#endif

    /// <summary>
    /// Color Type enumeration
    /// </summary>
    public enum ColorType
    {
        /// <summary>
        /// Default color type
        /// </summary>
        Default,

        /// <summary>
        /// Solid color type
        /// </summary>
        Solid,

        /// <summary>
        /// transparent color type (0 alpha)
        /// </summary>
        Transparent,

        /// <summary>
        /// Semitransparent color type (127 alpha)
        /// </summary>
        Semitransparent
    }
}

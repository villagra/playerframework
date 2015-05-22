// <copyright file="ColorEventArgs.cs" company="Microsoft Corporation">
// Copyright (c) 2013 Microsoft Corporation All Rights Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>mischero@microsoft.com</email>
// <date>2013-11-23</date>
// <summary>Color Event Arguments</summary>

namespace Microsoft.PlayerFramework.CaptionSettings
{
    using System;
    using Microsoft.PlayerFramework.CaptionSettings.Model;
    
    /// <summary>
    /// Color event arguments
    /// </summary>
    public class ColorEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the ColorEventArgs class
        /// </summary>
        /// <param name="color">the color</param>
        internal ColorEventArgs(Color color)
        {
            this.Color = color;
        }

        /// <summary>
        /// Gets the color
        /// </summary>
        public Color Color { get; private set; }
    }
}

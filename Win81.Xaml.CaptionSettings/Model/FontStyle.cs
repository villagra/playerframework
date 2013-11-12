// <copyright file="FontStyle.cs" company="Microsoft Corporation">
// Copyright (c) 2013 Microsoft Corporation All Rights Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>mischero@microsoft.com</email>
// <date>2013-10-28</date>
// <summary>Caption font style</summary>

namespace Microsoft.PlayerFramework.CaptionSettings.Model
{
    /// <summary>
    /// Caption font style
    /// </summary>
    public enum FontStyle
    {
        /// <summary>
        /// default font style
        /// </summary>
        Default,

        /// <summary>
        /// no font style
        /// </summary>
        None,

        /// <summary>
        /// raised edge font style
        /// </summary>
        RaisedEdge,

        /// <summary>
        /// Depressed edge font style
        /// </summary>
        DepressedEdge,

        /// <summary>
        /// Outline font style
        /// </summary>
        Outline,

        /// <summary>
        /// Drop shadow font style
        /// </summary>
        DropShadow
    }
}

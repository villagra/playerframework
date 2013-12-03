// <copyright file="AssemblyResources.cs" company="Microsoft Corporation">
// Copyright (c) 2013 Microsoft Corporation All Rights Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>mischero@microsoft.com</email>
// <date>2013-11-25</date>
// <summary>Assembly Resources</summary>

namespace Microsoft.PlayerFramework.CaptionSettings
{
    using Windows.ApplicationModel.Resources;

    /// <summary>
    /// Windows 8.1 Resource loader access
    /// </summary>
    internal class AssemblyResources
    {
        /// <summary>
        /// Get the resources loader
        /// </summary>
        /// <returns>the resource loader for the current view</returns>
        internal static ResourceLoader Get()
        {
            return ResourceLoader.GetForCurrentView("Microsoft.PlayerFramework.CaptionSettings/Resources");
        }
    }
}

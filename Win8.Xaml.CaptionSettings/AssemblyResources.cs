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
    /// Assembly Resources
    /// </summary>
    internal class AssemblyResources
    {
        /// <summary>
        /// Get a resource from the Caption Settings assembly
        /// </summary>
        /// <returns>the resource loader</returns>
        internal static ResourceLoader Get()
        {
            return new ResourceLoader("Microsoft.PlayerFramework.CaptionSettings/Resources");
        }
    }
}

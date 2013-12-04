// <copyright file="LocalizedStrings.cs" company="Microsoft Corporation">
// Copyright (c) 2013 Microsoft Corporation All Rights Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>mischero@microsoft.com</email>
// <date>2013-12-04</date>
// <summary>Localized strings</summary>

namespace WP8.PlayerFramework.Test
{
    using WP8.PlayerFramework.Test.Resources;

    /// <summary>
    /// Provides access to string resources.
    /// </summary>
    public class LocalizedStrings
    {
        /// <summary>
        /// the localized resources
        /// </summary>
        private static AppResources localizedResources = new AppResources();

        /// <summary>
        /// Gets the localized resources
        /// </summary>
        public AppResources LocalizedResources 
        { 
            get 
            { 
                return localizedResources; 
            } 
        }
    }
}
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
            return ResourceLoader.GetForCurrentView("Microsoft.Win81.PlayerFramework.CaptionSettingsPlugIn.Xaml/Resources");
        }
    }
}

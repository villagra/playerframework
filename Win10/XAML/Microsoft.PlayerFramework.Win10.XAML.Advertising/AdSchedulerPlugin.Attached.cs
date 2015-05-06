using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;

namespace Microsoft.PlayerFramework.Advertising
{
    /// <summary>
    /// Helper class used to attach config data specific to each playlist item.
    /// </summary>
    public static class AdScheduler
    {
        /// <summary>
        /// Identifies the Advertisements attached property.
        /// </summary>
        public static readonly DependencyProperty AdvertisementsProperty = DependencyProperty.RegisterAttached("Advertisements", typeof(AdvertisementsCollection), typeof(AdScheduler), null);

        /// <summary>
        /// Sets the Advertisements attached property value.
        /// </summary>
        /// <param name="obj">An instance of the MediaPlayer or PlaylistItem.</param>
        /// <param name="propertyValue">A value containing the Advertisements to apply to the plugin.</param>
        public static void SetAdvertisements(DependencyObject obj, AdvertisementsCollection propertyValue)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            obj.SetValue(AdvertisementsProperty, propertyValue);
        }

        /// <summary>
        /// Gets the Advertisements attached property value.
        /// </summary>
        /// <param name="obj">An instance of the MediaPlayer or PlaylistItem.</param>
        /// <returns>A value containing the Advertisements to apply to the plugin.</returns>
        public static AdvertisementsCollection GetAdvertisements(DependencyObject obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            return obj.GetValue(AdvertisementsProperty) as AdvertisementsCollection;
        }

    }

    /// <summary>
    /// A collection class containing the Advertisements objects associated with a PlaylistItem
    /// </summary>
    public class AdvertisementsCollection : ObservableCollection<Advertisement>
    { }
}

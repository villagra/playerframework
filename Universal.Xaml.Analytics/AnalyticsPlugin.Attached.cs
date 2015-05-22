using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;
#if SILVERLIGHT
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace Microsoft.PlayerFramework.Analytics
{    
    /// <summary>
    /// Helper class used to attach config data specific to each playlist item.
    /// </summary>
    public static class Analytics
    {
        /// <summary>
        /// Identifies the AdditionalData attached property.
        /// </summary>
        public static readonly DependencyProperty AdditionalDataProperty = DependencyProperty.RegisterAttached("AdditionalData", typeof(IDictionary<string, object>), typeof(Analytics), null);

        /// <summary>
        /// Sets the AdditionalData attached property value.
        /// </summary>
        /// <param name="obj">An instance of the MediaPlayer or PlaylistItem.</param>
        /// <param name="propertyValue">A value containing the AdditionalData to apply to the plugin.</param>
        public static void SetAdditionalData(DependencyObject obj, IDictionary<string, object> propertyValue)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            obj.SetValue(AdditionalDataProperty, propertyValue);
        }

        /// <summary>
        /// Gets the AdditionalData attached property value.
        /// </summary>
        /// <param name="obj">An instance of the MediaPlayer or PlaylistItem.</param>
        /// <returns>A value containing the AdditionalData to apply to the plugin.</returns>
        public static IDictionary<string, object> GetAdditionalData(DependencyObject obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            return obj.GetValue(AdditionalDataProperty) as IDictionary<string, object>;
        }

    }
}

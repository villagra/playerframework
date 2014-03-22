using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if SILVERLIGHT
using System.Windows;
using System.Windows.Input;
#else
using Windows.UI.Xaml;
#endif

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// Provides an attached property for ToggleButtons to provide alternate button content when checked.
    /// </summary>
    public sealed class ToggleButtonExtensions
    {
        #region CheckedContent
        /// <summary>
        /// Identifies the CheckedContent dependency property.
        /// </summary>
        public static DependencyProperty CheckedContentProperty = DependencyProperty.RegisterAttached("CheckedContent", typeof(object), typeof(ToggleButtonExtensions), null);

        /// <summary>
        /// Sets the content for the toggle button when in a checked state.
        /// </summary>
        /// <param name="obj">The toggle button to set the alternate content on.</param>
        /// <param name="propertyValue">The alternate content. Must be type UIElement.</param>
        public static void SetCheckedContent(DependencyObject obj, object propertyValue)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            obj.SetValue(CheckedContentProperty, propertyValue);
        }

        /// <summary>
        /// Gets the content for the toggle button when in a checked state.
        /// </summary>
        /// <param name="obj">The toggle button to get the alternate content on.</param>
        /// <returns>The alternate content. Should be type UIElement.</returns>
        public static object GetCheckedContent(DependencyObject obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            return obj.GetValue(CheckedContentProperty);
        }

        #endregion
    }
}

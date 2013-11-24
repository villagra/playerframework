using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.PlayerFramework.CaptionSettings
{
    using Windows.Foundation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Media;

    /// <summary>
    /// Framework Element extensions
    /// </summary>
    internal static class FrameworkElementExtensions
    {
        /// <summary>
        /// Get the rectangle for this Framework element
        /// </summary>
        /// <param name="element">the Framework element</param>
        /// <returns>a rectangle defining the bounds of the framework element</returns>
        internal static Rect GetElementRect(this FrameworkElement element)
        {
            GeneralTransform buttonTransform = element.TransformToVisual(null);
            Point point = buttonTransform.TransformPoint(new Point());
            return new Rect(point, new Size(element.ActualWidth, element.ActualHeight));
        }
    }
}

using System.Collections.Generic;
using System.Linq;
#if SILVERLIGHT
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endif

namespace Microsoft.PlayerFramework
{
    internal static class VisualTreeExtensions
    {
#if !SILVERLIGHT
        public static bool HasKeyboardFocus(this DependencyObject source)
        {
            return GetDescendants(source).OfType<Control>().Any(c => c.FocusState == FocusState.Keyboard);
        }
#endif

        public static IEnumerable<DependencyObject> GetDescendants(this DependencyObject source)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(source); i++)
            {
                var child = VisualTreeHelper.GetChild(source, i);
                yield return child;
                foreach (var grandChild in GetDescendants(child))
                {
                    yield return grandChild;
                }
            }
        }
    }
}

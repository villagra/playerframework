using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Microsoft.PlayerFramework
{
    internal static class VisualTreeExtensions
    {
        public static bool HasKeyboardFocus(this DependencyObject source)
        {
            return GetDescendants(source).OfType<Control>().Any(c => c.FocusState == FocusState.Keyboard);
        }

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

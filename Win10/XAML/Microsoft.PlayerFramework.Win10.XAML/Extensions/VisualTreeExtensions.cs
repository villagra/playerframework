using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Microsoft.PlayerFramework
{
    public static class VisualTreeExtensions
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

		public static IEnumerable<T> FindVisualChildren<T>(DependencyObject obj)
		where T : DependencyObject
		{
			List<T> descendants = new List<T>();

			int childCount = VisualTreeHelper.GetChildrenCount(obj);

			for (int i = 0; i < childCount; i++)
			{
				DependencyObject child = VisualTreeHelper.GetChild(obj, i);
				if (child != null && child is T)
					descendants.Add((T)child);

				var children = FindVisualChildren<T>(child);
				if (children != null && children.Count() > 0)
					descendants.AddRange(children);
            }

			if (descendants.Count > 0)
				return descendants;
			else
				return null;
		}
	}
}

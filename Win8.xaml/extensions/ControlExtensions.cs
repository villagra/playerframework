using System;
#if SILVERLIGHT
using System.Windows;
using System.Windows.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Threading.Tasks;
#endif

namespace Microsoft.PlayerFramework
{
    internal static class ControlExtensions
    {
        public static void GoToVisualState(this Control control, string state)
        {
            control.GoToVisualState(state, true);
        }

        public static void GoToVisualState(this Control control, string state, bool useTransitions)
        {
            VisualStateManager.GoToState(control, state, useTransitions);
        }

#if !SILVERLIGHT
        public static Task BeginInvoke(this Windows.UI.Core.CoreDispatcher source, Action action)
        {
            return source.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => action()).AsTask();
        }
#endif
    }
}

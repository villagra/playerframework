using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Threading.Tasks;

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

        public static Task BeginInvoke(this Windows.UI.Core.CoreDispatcher source, Action action)
        {
            return source.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => action()).AsTask();
        }
    }
}

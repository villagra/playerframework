using System;
#if SILVERLIGHT
using System.Windows.Controls.Primitives;
using System.Windows.Automation;
#else
using Windows.UI.Xaml.Controls.Primitives;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Automation;
#endif

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// A control to display the signal strength for adaptive streaming.
    /// </summary>
    public class SignalStrength : RangeBase
    {
        /// <summary>
        /// Creates a new instance of SignalStrength
        /// </summary>
        public SignalStrength()
        {
            DefaultStyleKey = typeof(SignalStrength);

            AutomationProperties.SetName(this, MediaPlayer.GetResourceString("SignalStrengthLabel"));
        }

        /// <inheritdoc /> 
#if SILVERLIGHT
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();

            RefreshVisualState(Value);
        }

        /// <inheritdoc /> 
        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);

            RefreshVisualState(newValue);
        }

        private void RefreshVisualState(double newValue)
        {
            if (newValue < 1.0 / 5)
            {
                this.GoToVisualState("None");
            }
            else if (newValue < 2.0 / 5)
            {
                this.GoToVisualState("Low");
            }
            else if (newValue < 3.0 / 5)
            {
                this.GoToVisualState("Medium");
            }
            else if (newValue < 4.0 / 5)
            {
                this.GoToVisualState("High");
            }
            else
            {
                this.GoToVisualState("Full");
            }
        }
    }
}

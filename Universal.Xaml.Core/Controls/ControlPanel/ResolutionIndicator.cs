using System;
#if SILVERLIGHT
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Automation;
#else
using Windows.UI.Xaml.Controls.Primitives;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
#endif

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// A control to display the signal strength for adaptive streaming.
    /// </summary>
    public class ResolutionIndicator : Control
    {
        /// <summary>
        /// Creates a new instance of ResolutionIndicator
        /// </summary>
        public ResolutionIndicator()
        {
            DefaultStyleKey = typeof(ResolutionIndicator);
        }

        /// <inheritdoc /> 
#if SILVERLIGHT
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();

            var HighDefinition = GetTemplateChild("HighDefinition") as TextBlock;
            var StandardDefinition = GetTemplateChild("StandardDefinition") as TextBlock;

            if (HighDefinition != null) HighDefinition.Text = MediaPlayer.GetResourceString("HighDefinitionText");
            if (StandardDefinition != null) StandardDefinition.Text = MediaPlayer.GetResourceString("StandardDefinitionText");

            UpdateQuality(MediaQuality);
        }

        /// <summary>
        /// Identifies the MediaQuality dependency property.
        /// </summary>
        public static readonly DependencyProperty MediaQualityProperty = DependencyProperty.Register("MediaQuality", typeof(MediaQuality), typeof(ResolutionIndicator), new PropertyMetadata(MediaQuality.HighDefinition, (d, e) => ((ResolutionIndicator)d).OnMediaQualityChanged((MediaQuality)e.OldValue, (MediaQuality)e.NewValue)));

        /// <summary>
        /// Provides notification that the MediaQuality has changed.
        /// </summary>
        /// <param name="oldValue">The previous media quality.</param>
        /// <param name="newValue">The new media quality.</param>
        protected virtual void OnMediaQualityChanged(MediaQuality oldValue, MediaQuality newValue)
        {
            UpdateQuality(newValue);
        }

        private void UpdateQuality(MediaQuality quality)
        {
            switch (quality)
            {
                case MediaQuality.StandardDefinition:
                    this.GoToVisualState("SD");
                    AutomationProperties.SetName(this, MediaPlayer.GetResourceString("StandardDefinitionLabel"));
                    break;
                case MediaQuality.HighDefinition:
                    this.GoToVisualState("HD");
                    AutomationProperties.SetName(this, MediaPlayer.GetResourceString("HighDefinitionLabel"));
                    break;
            }
        }

        /// <summary>
        /// Gets or sets the media quality displayed in the control
        /// </summary>
        public MediaQuality MediaQuality
        {
            get { return (MediaQuality)GetValue(MediaQualityProperty); }
            set { SetValue(MediaQualityProperty, value); }
        }
    }
}

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.Foundation;

namespace Microsoft.PlayerFramework.Advertising
{
    /// <summary>
    /// Provides a base class to host an ad player in.
    /// This is essentially a ContentControl with a hyperlink on top.
    /// </summary>
    public class AdHost : ContentControl
    {
        Uri navigateUri;

        /// <summary>
        /// Gets the LayoutRoot container for the ad.
        /// </summary>
        protected Panel LayoutRoot { get; private set; }

        /// <summary>
        /// Gets or sets the initial dimensions for the ad.
        /// </summary>
        protected Size? InitialDimensions { get; set; }

        /// <summary>
        /// Creates a new instance of AdHost.
        /// </summary>
        public AdHost()
        {
            this.DefaultStyleKey = typeof(AdHost);
        }

        /// <summary>
        /// Raised after nativation occurs.
        /// </summary>
        public event RoutedEventHandler Navigated;

        /// <summary>
        /// Gets the Button control.
        /// </summary>
        protected ButtonBase ClickThroughButton { get; private set; }

        /// <summary>
        /// Indicates that the template has been loaded.
        /// </summary>
        protected bool IsTemplateLoaded { get; private set; }

        /// <inheritdoc /> 
        protected override void OnApplyTemplate()
        {
            if (ClickThroughButton != null)
            {
                ClickThroughButton.Click -= ClickThroughButton_Click;
            }

            base.OnApplyTemplate();

            ClickThroughButton = base.GetTemplateChild("ClickThroughButton") as ButtonBase;
            if (ClickThroughButton != null)
            {
                ClickThroughButton.Visibility = navigateUri != null ? Visibility.Visible : Visibility.Collapsed;
                ClickThroughButton.Click += ClickThroughButton_Click;
                if (ClickThroughButton.Content != null)
                {
                    ClickThroughButton.Content = MediaPlayer.GetResourceString("AdLinkLabel");
                }
            }

            var LayoutRoot = GetTemplateChild("LayoutRoot") as Panel;
            if (LayoutRoot != null && InitialDimensions.HasValue)
            {
                LayoutRoot.Width = InitialDimensions.Value.Width;
                LayoutRoot.Height = InitialDimensions.Value.Height;
            }

            IsTemplateLoaded = true;
            AdLinear = adLinear; // force the visual state to get set
        }

        void ClickThroughButton_Click(object sender, RoutedEventArgs e)
        {
            if (Navigated != null) Navigated(this, e);
        }

        /// <summary>
        /// Gets or sets the Uri to navigate to when the hyperlink button is clicked.
        /// </summary>
        public Uri NavigateUri
        {
            get { return navigateUri; }
            set
            {
                navigateUri = value;
                if (ClickThroughButton != null)
                {
                    ClickThroughButton.Visibility = navigateUri != null ? Visibility.Visible : Visibility.Collapsed;
                    var hyperlink = ClickThroughButton as HyperlinkButton;
                    if (hyperlink != null)
                    {
                        hyperlink.NavigateUri = navigateUri;
                    }
                }
            }
        }

        private bool adLinear;
        /// <summary>
        /// Gets or sets if the ad is linear or nonlinear
        /// </summary>
        public bool AdLinear
        {
            get { return adLinear; }
            set
            {
                adLinear = value;
                if (IsTemplateLoaded)
                {
                    if (adLinear)
                    {
                        VisualStateManager.GoToState(this, "Linear", true);
                    }
                    else
                    {
                        VisualStateManager.GoToState(this, "Nonlinear", true);
                    }
                }
            }
        }
    }
}

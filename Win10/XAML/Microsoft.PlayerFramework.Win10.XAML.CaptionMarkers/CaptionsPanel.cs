using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Documents;

namespace Microsoft.PlayerFramework.CaptionMarkers
{
    /// <summary>
    /// Represents a panel control to host closed captions
    /// </summary>
    public class CaptionsPanel : Control
    {
        /// <summary>
        /// Creates a new instances of CaptionsPanel
        /// </summary>
        public CaptionsPanel()
        {
            this.DefaultStyleKey = typeof(CaptionsPanel);
			this.Loaded += CaptionsPanel_Loaded;
		}

		private void ActiveCaptions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (Windows.Media.ClosedCaptioning.ClosedCaptionProperties.BackgroundColor != Windows.Media.ClosedCaptioning.ClosedCaptionColor.Default)
			{
				var bgColor = Windows.Media.ClosedCaptioning.ClosedCaptionProperties.ComputedBackgroundColor;

				switch (Windows.Media.ClosedCaptioning.ClosedCaptionProperties.BackgroundOpacity)
				{
					case Windows.Media.ClosedCaptioning.ClosedCaptionOpacity.OneHundredPercent:
						bgColor.A = 255;
						break;
					case Windows.Media.ClosedCaptioning.ClosedCaptionOpacity.SeventyFivePercent:
						bgColor.A = 192;
						break;
					case Windows.Media.ClosedCaptioning.ClosedCaptionOpacity.TwentyFivePercent:
						bgColor.A = 64;
						break;
					case Windows.Media.ClosedCaptioning.ClosedCaptionOpacity.ZeroPercent:
						bgColor.A = 0;
						break;
				}

				this.Background = new SolidColorBrush(bgColor);
				foreach (ActiveCaption caption in ActiveCaptions)
					caption.Background = this.Background;
			}
			else
			{
				//#CC0F0F0F
				foreach (ActiveCaption caption in ActiveCaptions)
					caption.Background = new SolidColorBrush(Color.FromArgb(204, 15, 15, 15));
			}
		}

		private void CaptionsPanel_Loaded(object sender, RoutedEventArgs e)
		{
			this.ResetStyle();
		}

		public void ResetStyle()
		{
			if (Windows.Media.ClosedCaptioning.ClosedCaptionProperties.FontColor != Windows.Media.ClosedCaptioning.ClosedCaptionColor.Default)
			{
				this.Foreground = new SolidColorBrush(Windows.Media.ClosedCaptioning.ClosedCaptionProperties.ComputedFontColor);
			}
			else
			{
				//#FFEBEBEB
				this.Foreground = new SolidColorBrush(Color.FromArgb(255, 235, 235, 235));
			}

			if (Windows.Media.ClosedCaptioning.ClosedCaptionProperties.FontStyle != Windows.Media.ClosedCaptioning.ClosedCaptionStyle.Default)
			{
				switch (Windows.Media.ClosedCaptioning.ClosedCaptionProperties.FontStyle)
				{
					case Windows.Media.ClosedCaptioning.ClosedCaptionStyle.Casual:
						this.FontFamily = new FontFamily("Segoe Print");
						break;
					case Windows.Media.ClosedCaptioning.ClosedCaptionStyle.Cursive:
						this.FontFamily = new FontFamily("Segoe Script");
						break;
					case Windows.Media.ClosedCaptioning.ClosedCaptionStyle.MonospacedWithoutSerifs:
						this.FontFamily = new FontFamily("Lucida Sans Unicode");
						break;
					case Windows.Media.ClosedCaptioning.ClosedCaptionStyle.MonospacedWithSerifs:
						this.FontFamily = new FontFamily("Courier New");
						break;
					case Windows.Media.ClosedCaptioning.ClosedCaptionStyle.ProportionalWithoutSerifs:
						this.FontFamily = new FontFamily("Arial");
						break;
					case Windows.Media.ClosedCaptioning.ClosedCaptionStyle.ProportionalWithSerifs:
						this.FontFamily = new FontFamily("Times New Roman");
						break;
					case Windows.Media.ClosedCaptioning.ClosedCaptionStyle.SmallCapitals:
						this.FontFamily = new FontFamily("Arial");
						Typography.SetCapitals(this, FontCapitals.SmallCaps);
						break;
					default:
						this.FontFamily = new FontFamily("Arial");
						break;
				}
			}
			else
			{
				this.FontFamily = new FontFamily("Arial");
			}

			if (Windows.Media.ClosedCaptioning.ClosedCaptionProperties.FontSize != Windows.Media.ClosedCaptioning.ClosedCaptionSize.Default)
			{
				switch (Windows.Media.ClosedCaptioning.ClosedCaptionProperties.FontSize)
				{
					case Windows.Media.ClosedCaptioning.ClosedCaptionSize.FiftyPercent:
						this.FontSize = 22;
						break;
					case Windows.Media.ClosedCaptioning.ClosedCaptionSize.OneHundredPercent:
						this.FontSize = 32;
						break;
					case Windows.Media.ClosedCaptioning.ClosedCaptionSize.OneHundredFiftyPercent:
						this.FontSize = 42;
						break;
					case Windows.Media.ClosedCaptioning.ClosedCaptionSize.TwoHundredPercent:
						this.FontSize = 52;
						break;
				}
			}
			else
			{
				this.FontSize = 32;
			}
		}

		#region ActiveCaptions
		/// <summary>
		/// ActiveCaptions DependencyProperty definition.
		/// </summary>
		public static readonly DependencyProperty ActiveCaptionsProperty = DependencyProperty.Register("ActiveCaptions", typeof(ObservableCollection<ActiveCaption>), typeof(CaptionsPanel), null);

        /// <summary>
        /// Gets or sets the active captions to be displayed
        /// </summary>
        public ObservableCollection<ActiveCaption> ActiveCaptions
        {
            get { return (ObservableCollection<ActiveCaption>)GetValue(ActiveCaptionsProperty); }
            set { SetValue(ActiveCaptionsProperty, value); if(this.ActiveCaptions != null)
					this.ActiveCaptions.CollectionChanged += ActiveCaptions_CollectionChanged;
			}
        }

        #endregion

    }
}

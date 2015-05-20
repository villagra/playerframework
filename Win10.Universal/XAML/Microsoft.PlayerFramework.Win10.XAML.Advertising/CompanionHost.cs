using Windows.UI.Xaml.Controls;

namespace Microsoft.PlayerFramework.Advertising
{
    /// <summary>
    /// A control that can be used to host a companion ad. This is just a HyperlinkButton with a custom style.
    /// </summary>
    public class CompanionHost : HyperlinkButton
    {
        /// <summary>
        /// Creates a new instance of CompanionHost.
        /// </summary>
        public CompanionHost()
        {
            this.DefaultStyleKey = typeof(CompanionHost);
        }
    }
}

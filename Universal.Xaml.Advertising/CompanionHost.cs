#if SILVERLIGHT
using System.Windows.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif

namespace Microsoft.PlayerFramework.Advertising
{
    public class CompanionHost : HyperlinkButton
    {
        public CompanionHost()
        {
            this.DefaultStyleKey = typeof(CompanionHost);
        }
    }
}

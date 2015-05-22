using Microsoft.PlayerFramework.Samples.Common;
using Windows.UI.Xaml.Controls;

namespace Microsoft.PlayerFramework.Samples
{
    public class ControllableNavigationHelper : NavigationHelper
    {
        public ControllableNavigationHelper(Page page)
            : base(page)
        { }

        public bool PreventBackNavigation { get; set; }

        public override bool CanGoBack()
        {
            return !PreventBackNavigation && base.CanGoBack();
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.PlayerFramework.Samples.Common;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Microsoft.PlayerFramework.Samples
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class VmapPage : Page
    {
        public VmapPage()
        {
            this.InitializeComponent();
            vmapPlugin.Source = new Uri(@"http://pubads.g.doubleclick.net/gampad/ads?sz=640x480&iu=/40762806/prel-10-ch/prel-10-video/prel-10-cs/prel-10-windows-desktop/prel-10-zattoo&impl=s&gdfp_req=1&env=vp&output=xml_vmap1&unviewed_position_start=1&url=http%3A%2F%2Fzattoo.com%2F&description_url=http%3A%2F%2Fzattoo.com%2F&correlator=2836745693032353146&cust_params=uid%3D325e6f25af594faca72926685b09fa8c%26zuid%3D23212672%26ab_test_groups%3D%26random%3D4911887507115606470%26genresnext%3DCrime%26email_allowed%3D0%26category%3DSeries%26genres%3DCrime%26title%3DSOKO%205113%26variantgroup%3D2%26adcount%3D1%26clanguage%3Dde%26channel%3Dsf-2%26last_watch%3D1065%26appversion%3D4003001%26user_type%3Dzattoo%26titlenext%3DKommissar%20Rex%26adid%3Dnull%26embed%3Dpartner_zapi%26categorynext%3DSeries%26email_valid%3D1%26language%3Dde%26gender%3Dnone%26session_id%3D14A3817AA028C15D-1D4924B99C9C2CCD%26appid%3D162%26forerun%3D25");
        }
    }
}

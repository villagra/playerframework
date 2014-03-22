using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Media.Advertising;
using Windows.Foundation;

namespace Microsoft.PlayerFramework.Units.Advertising.Mockups
{
    public class DelayAdSource : IResolveableAdSource
    {
        public DelayAdSource()
        {
        }

        public DelayAdSource(int delay)
        {
            Delay = delay;
        }

        public int Delay { get; set; }

        public string Key { get; set; }

        public object Payload { get; set; }

        public string Type { get; set; }

        public bool AllowMultipleAds { get; set; }

        public int? MaxRedirectDepth { get; set; }

        public bool IsLoaded { get; private set; }

        IAsyncAction IResolveableAdSource.LoadPayload()
        {
            return AsyncInfo.Run(c => Task.Delay(Delay, c));
        }
    }
}

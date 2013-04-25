using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VideoAdvertising;
using Windows.Foundation;

namespace Microsoft.PlayerFramework.Units.Advertising.Mockups
{
    public class ErrorAdSource : IResolveableAdSource
    {
        public ErrorAdSource()
        {
        }

        public ErrorAdSource(Exception error)
        {
            Error = error;
        }

        public Exception Error { get; set; }

        public string Key { get; set; }

        public object Payload { get; set; }

        public string Type { get; set; }

        public bool AllowMultipleAds { get; set; }

        public int? MaxRedirectDepth { get; set; }

        public bool IsLoaded { get; private set; }

        IAsyncAction IResolveableAdSource.LoadPayload()
        {
            return AsyncInfo.Run(async c => 
                {
                    await Task.Delay(100, c);
                    throw Error;
                });
        }
    }
}

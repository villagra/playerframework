using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VideoAdvertising;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Storage;

namespace Microsoft.PlayerFramework.Units.Advertising.Mockups
{
    public class LocalAdSource : IResolveableAdSource
    {
        public string Filename { get; set; }

        public string Key { get; set; }

        public object Payload { get; set; }

        public string Type { get; set; }

        public bool AllowMultipleAds { get; set; }

        public int? MaxRedirectDepth { get; set; }

        public bool IsLoaded {get; private set;}

        IAsyncAction IResolveableAdSource.LoadPayload()
        {
            return AsyncInfo.Run(c => LoadPayload());
        }

        async Task LoadPayload()
        {
            try
            {
                var vastFile = await Package.Current.InstalledLocation.GetFileAsync(Filename);
                Payload = await vastFile.OpenStreamForReadAsync();
            }
            catch (FileNotFoundException) { /* ignore */ }
        }
    }
}

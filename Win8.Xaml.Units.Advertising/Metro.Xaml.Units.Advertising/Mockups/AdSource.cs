using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Media.Advertising;

namespace Microsoft.PlayerFramework.Units.Advertising.Mockups
{
    public class AdSource : IAdSource
    {
        public string Key { get; set; }

        public object Payload { get; set; }

        public string Type { get; set; }

        public bool AllowMultipleAds { get; set; }

        public int? MaxRedirectDepth { get; set; }
    }
}

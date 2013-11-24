using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;

namespace Microsoft.PlayerFramework.CaptionSettings
{
    internal class AssemblyResources
    {
        /// <summary>
        /// Get a resource from the Caption Settings assembly
        /// </summary>
        /// <returns>the resoure loader</returns>
        internal static ResourceLoader Get()
        {
            return new ResourceLoader("Microsoft.PlayerFramework.CaptionSettings/Resources");
        }
    }
}

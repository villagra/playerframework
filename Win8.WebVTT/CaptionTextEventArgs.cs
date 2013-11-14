using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if WINDOWS_PHONE
using System.Windows.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif

namespace Microsoft.WebVTT
{
#if WINDOWS_PHONE
    public sealed class CaptionTextEventArgs : EventArgs
#else
    public sealed class CaptionTextEventArgs
#endif
    {
        public CaptionTextEventArgs(TextBlock textBlock, TextPosition position)
        {
            this.TextBlock = textBlock;
            this.Position = position;
        }

        public TextBlock TextBlock { get; private set; }

        public TextPosition Position { get; private set; }
    }
}

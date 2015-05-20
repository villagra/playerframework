using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Microsoft.WebVTT
{
    public sealed class CaptionTextEventArgs
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

using System;
using System.Collections.Generic;
#if SILVERLIGHT
using System.Windows.Media;
#else
using Windows.UI;
#endif

namespace Microsoft.Media.TimedText
{
    /// <summary>
    /// Represents an on-screen region where captions will be displayed.
    /// </summary>
    public class CaptionRegion : TimedTextElement
    {
        public CaptionRegion()
        {
            ApplyDefaultStyle();
        }

        public IDictionary<string, TunneledData> TunneledData { get; set; }

        private void ApplyDefaultStyle()
        {
            CaptionElementType = TimedTextElementType.Region;
            Begin = TimeSpan.MinValue;
            End = TimeSpan.MaxValue;
            Style.BackgroundColor = Colors.Black;
            Style.Origin = new Origin
            {
                Left = new Length
                {
                    Value = .1,
                    Unit = LengthUnit.Percent
                },
                Top = new Length
                {
                    Value = .8,
                    Unit = LengthUnit.Percent
                }
            };

            Style.Extent = new Extent
            {
                Height = new Length
                {
                    Value = .1,
                    Unit = LengthUnit.Percent
                },
                Width = new Length
                {
                    Value = .8,
                    Unit = LengthUnit.Percent
                }
            };
        }
    }
}
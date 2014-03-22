using System;
#if SILVERLIGHT
using System.Windows;
#else
using Windows.UI.Text;
#endif

namespace Microsoft.Media.TimedText
{
    public static class FontWeightConverter
    {
        public static FontWeight Convert(Weight weight)
        {
            switch (weight)
            {
                case Weight.Bold: return FontWeights.Bold;
                case Weight.Normal: return FontWeights.Normal;
                default: throw new NotImplementedException();
            }
        }
    }

    public enum Weight
    {
        Bold,
        Normal
    }
}

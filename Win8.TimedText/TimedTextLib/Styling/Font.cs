using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if SILVERLIGHT
using System.Windows;
#else
using Windows.UI.Text;
#endif

namespace TimedText.Styling
{
    /// <summary>
    /// For attribute values that are explicit inherit, insert this value. 
    /// It contains an object which can be used to cache the inherited value.
    /// </summary>
    public class Inherit
    {
        public object Cached
        {
            get;
            set;
        }
    }

    public enum TextDecorationAttributeValue
    {
        None,
        Underline,
        Overline,
        Throughline,
    };

    public enum FontStyleAttributeValue
    {
        Regular,
        Oblique,
        ReverseOblique,
        Italic,
    };

    public enum FontWeightAttributeValue
    {
        Regular,
        Bold,
    };


    public class Font
    {

        #region private variables
        string  m_familyName = "Arial";
        //Style m_style = Style.Regular;
        double m_size = 14.0;
        double m_stretch = 1.0;
        FontStyle m_style;
        FontWeight m_weight;
        //StyleAttribute m_styleAttribute;

        #region font metrics
        //int m_unitsPerEm;
        //int m_ascent;
        //int m_descent;
        //int m_averageWidth;
        //int m_maxWidth;
        //int m_capHeight;
        //int m_stemHeight;
        //int m_charHeight;
        //int m_stemV;
        //int m_leading;
        #endregion
  
        #endregion

        /// <summary>
        /// Is the text rendered left to right?
        /// </summary>
        public bool LeftToRight
        {
            get;
            set;
        }

        public Font(string familyName, double emHeight, FontWeightAttributeValue weight, FontStyleAttributeValue style)
        {
            LeftToRight = true;
            m_familyName = familyName;
            m_size = emHeight;
            switch (style)
            {
                case FontStyleAttributeValue.Italic:
                    m_style = FontStyles.Italic;
                    break;
                case FontStyleAttributeValue.Oblique:
                    m_style = FontStyles.Italic;
                    break;
                case FontStyleAttributeValue.ReverseOblique:
                    m_style = FontStyles.Italic;
                    break;
                default:
                    m_style = FontStyles.Normal;
                    break;
            }
            switch (weight)
            {
                case FontWeightAttributeValue.Bold: m_weight = FontWeights.Bold;
                    break;
                default:
                    m_weight = FontWeights.Normal;
                    break;
            }
            m_stretch = 1.0;
         }

        public string Family
        {
            get
            {
                return m_familyName;
            }
        }

        public double EmHeight
        {
            get
            {
                return m_size;
            }
        }

        public FontStyle Style
        {
            get
            {
                return m_style;
            }
        }

        public FontWeight Weight
        {
            get
            {
                return m_weight;
            }
        }

        
        #region WPF font handling
        //private void Initialize()
        //{
        //}

        public FontStretch Stretch()
        {
            if (m_stretch > 1.0)
            {
                return FontStretches.ExtraCondensed;
            } else if (m_stretch < 1.0)
            {
                return FontStretches.ExtraExpanded;
            }
            else 
            {
                return FontStretches.Normal;
            }
        }
        
        //Media.FontFamily m_family;
        ////FontStretch m_stretch;
        //Media.Typeface m_typeface;
        #endregion
    }
}

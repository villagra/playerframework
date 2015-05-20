using System;
using System.Linq;

namespace Microsoft.Media.TimedText
{  
    /// <summary>
    /// The base class for captioning elements.
    /// </summary>
    public class TimedTextElement : MediaMarker
    {
        private TimedTextStyle _currentStyle;
        private TimedTextStyle _style;

        MediaMarkerCollection<TimedTextAnimation> animations;
        MediaMarkerCollection<TimedTextElement> children;

        /// <summary>
        /// Gets or sets the list of animations to be applied to this element.
        /// </summary>
#if HACK_XAMLTYPEINFO
        public object Animations { get { return animations; } }
        public object Children { get { return children; } }
#else
        public MediaMarkerCollection<TimedTextAnimation> Animations { get { return animations; } }
        public MediaMarkerCollection<TimedTextElement> Children { get { return children; } }
#endif

        public TimedTextElement()
        {
            Type = "captionelement";
            Style = new TimedTextStyle();
            animations = new MediaMarkerCollection<TimedTextAnimation>();
            children = new MediaMarkerCollection<TimedTextElement>();
        }

        /// <summary>
        /// Gets or sets the Style to be applied to this element.
        /// </summary>
        public TimedTextStyle Style
        {
            get { return _style; }

            set
            {
                if (_style != value)
                {
                    _style = value;
                    CurrentStyle = Style;
                    NotifyPropertyChanged("Style");
                }
            }
        }

        /// <summary>
        /// Gets or sets the current style of this element.
        /// </summary>
        public TimedTextStyle CurrentStyle
        {
            get { return _currentStyle; }

            protected set
            {
                if (_currentStyle != value)
                {
                    _currentStyle = value;
                    NotifyPropertyChanged("CurrentStyle");
                }
            }
        }

        /// <summary>
        /// Gets or sets the type of this caption element.
        /// </summary>
        public TimedTextElementType CaptionElementType { get; set; }
        
        public void CalculateCurrentStyle(TimeSpan position)
        {
            var activeAnimations = animations.WhereActiveAtPosition(position);
            if (activeAnimations.Any())
            {
                var animatedStyle = Style.Clone();
                activeAnimations.ForEach(i => i.MergeStyle(animatedStyle));
                CurrentStyle = animatedStyle;
            }

            children.WhereActiveAtPosition(position)
                .ForEach(i => i.CalculateCurrentStyle(position));
        }

        public bool HasAnimations
        {
            get
            {
                return animations.Any() || children.Any(i => i.HasAnimations);
            }
        }

    }
}
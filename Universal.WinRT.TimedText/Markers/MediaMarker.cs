using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;

namespace Microsoft.Media.TimedText
{
    /// <summary>
    /// Represents the base class for markers.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Markers signify a time location in a media file.
    /// Each marker has a Begin and End time. 
    /// </para>
    /// <para>
    /// There are two specific types of marker objects:
    /// <list type="bullet">
    /// <item><see cref="T:Microsoft.SilverlightMediaFramework.Media.CaptionMediaMarker">CaptionMediaMarker</see> objects represent caption text that can be displayed at a specific time in the media.</item>
    /// /// <item><see cref="T:Microsoft.SilverlightMediaFramework.Media.TimelineMediaMarker">TimelineMediaMarker</see> objects can be displayed on the timeline</item>
    /// </list>
    /// </para>
    /// </remarks>
    public partial class MediaMarker : INotifyPropertyChanged
    {
        private TimeSpan _begin;
        private object _content;
        private TimeSpan _end;
        private string _id;
        private string _type;

        public event Action<MediaMarker> PositionChanged;

        public MediaMarker()
        {
            _id = Guid.NewGuid().ToString();
        }

        protected MediaMarker(MediaMarker mediaMarker)
        {
            Begin = mediaMarker.Begin;
            Content = mediaMarker.Content;
            End = mediaMarker.End;
            Id = mediaMarker.Id;
            Type = mediaMarker.Type;
        }

        public void NotifyPositionChanged()
        {
            if (PositionChanged != null)
            {
                PositionChanged(this);
            }
        }

        /// <summary>
        /// Gets or sets a unique identifier for the marker.
        /// </summary>
        /// <remarks>
        /// The Id is used to determine which markers are new each time polling occurs.
        /// </remarks>
        public string Id
        {
            get { return _id; }
            set
            {
                if (value != null)
                {
                    if (_id != value)
                    {
                        _id = value;
                        NotifyPropertyChanged("Id");
                    }
                }
                else
                {
                    throw new ArgumentNullException("Id");
                }
            }
        }

        /// <summary>
        /// Gets or sets the begin position in the media stream of this marker item.
        /// </summary>
        public TimeSpan Begin
        {
            get { return _begin; }

            set
            {
                _begin = value;
                NotifyPropertyChanged("Begin");
                NotifyPropertyChanged("Duration");
            }
        }

        /// <summary>
        /// Gets or sets the end position in the media stream of this marker item.
        /// </summary>
        public TimeSpan End
        {
            get { return _end; }

            set
            {
                _end = value;
                NotifyPropertyChanged("End");
                NotifyPropertyChanged("Duration");
            }
        }

        /// <summary>
        /// Gets or sets the text associated with this marker.
        /// </summary>
        public virtual object Content
        {
            get { return _content; }
            set
            {
                if (_content != value)
                {
                    _content = value;
                    NotifyPropertyChanged("Content");
                }
            }
        }

        /// <summary>
        /// Gets the duration of this marker (calculated from start time to end time).
        /// </summary>
        /// <remarks>
        /// The property value is calculated (it cannot be set) and is provided as a convenience.
        /// </remarks>
        public TimeSpan Duration
        {
            get { return Begin < End ? End.Subtract(Begin) : TimeSpan.Zero; }
        }

        /// <summary>
        /// Gets or sets the type of marker (such as caption or display).
        /// </summary>
        public string Type
        {
            get { return _type; }
            set
            {
                if (_type != value)
                {
                    _type = value;
                    NotifyPropertyChanged("Type");
                }
            }
        }
        
        public bool IsActiveAtPosition(TimeSpan position)
        {
            return IsActiveAtPosition(this, position);
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        /// <summary>
        /// Gets a value indicating whether two markers are references to the same marker.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>true if the markers compared are the same marker; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            var mediaMarker = obj as MediaMarker;
            return base.Equals(obj) 
                    || (mediaMarker != null && mediaMarker.Id != null && mediaMarker.Id == Id && mediaMarker.Begin == Begin && mediaMarker.End == End);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        
        public static bool IsActiveAtPosition(MediaMarker mediaMarker, TimeSpan position)
        {
            return mediaMarker.Begin <= position && position < mediaMarker.End;
        }
        
        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            if (propertyName == null)
                throw new ArgumentNullException("propertyName");

            PropertyChangedEventHandler propertyChangedHandler = PropertyChanged;

            if (propertyChangedHandler != null)
            {
                propertyChangedHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
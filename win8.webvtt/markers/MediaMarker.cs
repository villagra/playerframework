using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;

namespace Microsoft.WebVTT
{
    public sealed class MediaMarker
    {
        public MediaMarker()
        {
            Id = Guid.NewGuid().ToString();
        }

        public MediaMarker(MediaMarker mediaMarker)
        {
            Begin = mediaMarker.Begin;
            Content = mediaMarker.Content;
            End = mediaMarker.End;
            Id = mediaMarker.Id;
            Type = mediaMarker.Type;
        }

        /// <summary>
        /// Gets or sets a unique identifier for the marker.
        /// </summary>
        /// <remarks>
        /// The Id is used to determine which markers are new each time polling occurs.
        /// </remarks>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the begin position in the media stream of this marker item.
        /// </summary>
        public TimeSpan Begin { get; set; }

        /// <summary>
        /// Gets or sets the end position in the media stream of this marker item.
        /// </summary>
        public TimeSpan End { get; set; }

        /// <summary>
        /// Gets or sets the text associated with this marker.
        /// </summary>
        public object Content { get; set; }

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
        public string Type { get; set; }

        public bool IsActiveAtPosition(TimeSpan position)
        {
            return this.Begin <= position && position < this.End;
        }

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
    }
}
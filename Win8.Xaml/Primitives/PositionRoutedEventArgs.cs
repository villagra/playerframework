using System;
#if SILVERLIGHT
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// EventArgs associated with a Position in the media.
    /// </summary>
    public class PositionRoutedEventArgs : RoutedEventArgs
    {
        internal PositionRoutedEventArgs(TimeSpan position)
        {
            this.Position = position;
        }

        /// <summary>
        /// The position associated with the event.
        /// </summary>
        public TimeSpan Position { get; private set; }
    }

    /// <summary>
    /// EventArgs associated with a Position in the media.
    /// </summary>
    public class SeekRoutedEventArgs : PositionRoutedEventArgs
    {
        internal SeekRoutedEventArgs(TimeSpan previousPosition, TimeSpan newPosition) : base(newPosition)
        {
            this.PreviousPosition = previousPosition;
        }

        /// <summary>
        /// The position associated with the event.
        /// </summary>
        public TimeSpan PreviousPosition { get; private set; }

        /// <summary>
        /// Indicates that action should be aborted.
        /// </summary>
        public bool Canceled { get; set; }
    }

    /// <summary>
    /// EventArgs associated with a skip operation.
    /// </summary>
    public class SkipRoutedEventArgs : PositionRoutedEventArgs
    {
        internal SkipRoutedEventArgs(TimeSpan position)
            : base(position)
        {
            Canceled = false;
        }

        internal SkipRoutedEventArgs(TimeSpan position, bool canceled)
            : this(position)
        {
            Canceled = canceled;
        }

        /// <summary>
        /// Indicates that action should be aborted.
        /// </summary>
        public bool Canceled { get; set; }
    }

    /// <summary>
    /// EventArgs associated with a scrubbing operation.
    /// </summary>
    public class ScrubRoutedEventArgs : PositionRoutedEventArgs
    {
        internal ScrubRoutedEventArgs(TimeSpan position)
            : base(position)
        {
            Canceled = false;
        }

        internal ScrubRoutedEventArgs(TimeSpan position, bool canceled)
            : this(position)
        {
            Canceled = canceled;
        }

        /// <summary>
        /// Indicates that action should be aborted.
        /// </summary>
        public bool Canceled { get; set; }
    }

    /// <summary>
    /// EventArgs associated with a scrubbing operation that is in progress.
    /// </summary>
    public class ScrubProgressRoutedEventArgs : ScrubRoutedEventArgs
    {
        internal ScrubProgressRoutedEventArgs(TimeSpan startPosition, TimeSpan newPosition)
            : base(newPosition)
        {
            StartPosition = startPosition;
        }

        internal ScrubProgressRoutedEventArgs(TimeSpan startPosition, TimeSpan newPosition, bool canceled)
            : base(newPosition, canceled)
        {
            StartPosition = startPosition;
        }

        /// <summary>
        /// The position when scrubbing started
        /// </summary>
        public TimeSpan StartPosition { get; private set; }
    }
}

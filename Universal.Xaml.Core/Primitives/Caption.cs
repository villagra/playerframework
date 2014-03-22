using System;
using System.Collections.Generic;
#if SILVERLIGHT
using System.Windows;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
#endif

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// Represents a caption or subtitle track.
    /// </summary>
#if SILVERLIGHT
    public class Caption : DependencyObject
#else
    public class Caption : FrameworkElement
#endif
    {
        /// <summary>
        /// Indicates that the Payload property has changed
        /// </summary>
        public event EventHandler PayloadChanged;

        /// <summary>
        /// Invokes the PayloadChanged event
        /// </summary>
        protected void OnPayloadChanged()
        {
            if (PayloadChanged != null) PayloadChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Indicates that the Payload should be appended
        /// </summary>
        public event EventHandler<PayloadAugmentedEventArgs> PayloadAugmented;

        /// <summary>
        /// Invokes the PayloadChanged event
        /// </summary>
        public void AugmentPayload(object payload, TimeSpan startTime, TimeSpan endTime)
        {
            if (PayloadAugmented != null) PayloadAugmented(this, new PayloadAugmentedEventArgs(payload, startTime, endTime));
        }

        /// <summary>
        /// Id DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty IdProperty = DependencyProperty.Register("Id", typeof(string), typeof(Caption), null);

        /// <summary>
        /// Gets or sets the Id of the caption track.
        /// </summary>
        public string Id
        {
            get { return GetValue(IdProperty) as string; }
            set { SetValue(IdProperty, value); }
        }

        /// <summary>
        /// Description DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(Caption), null);

        /// <summary>
        /// Gets or sets the description of the caption track.
        /// </summary>
        public string Description
        {
            get { return GetValue(DescriptionProperty) as string; }
            set { SetValue(DescriptionProperty, value); }
        }

        /// <summary>
        /// Payload DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty PayloadProperty = DependencyProperty.Register("Payload", typeof(object), typeof(Caption), new PropertyMetadata(null, (d, o) => ((Caption)d).OnPayloadChanged()));

        /// <summary>
        /// Gets or sets the payload of the caption track. This can be any object.
        /// </summary>
        public object Payload
        {
            get { return GetValue(PayloadProperty) as object; }
            set { SetValue(PayloadProperty, value); }
        }

        /// <summary>
        /// Gets or sets the source Uri for the timed text. Useful for Xaml binding
        /// </summary>
        public Uri Source
        {
            get { return Payload as Uri; }
            set { Payload = value; }
        }

        /// <inheritdoc /> 
        public override string ToString()
        {
            return Description;
        }
    }

    /// <summary>
    /// Includes information that allows a caption engine to augment the existing caption information.
    /// Useful for in-stream caption support where caption data comes in chunks.
    /// </summary>
    public sealed class PayloadAugmentedEventArgs : EventArgs
    {
        /// <summary>
        /// Creates a new instance of PayloadAugmentedEventArgs.
        /// </summary>
        /// <param name="payload">The caption payload (typically a bytearry or string).</param>
        /// <param name="startTime">The offset for the caption data. Typically set from the chunk timestamp.</param>
        /// <param name="endTime">The end time of the caption data. Typically set from the chunk starttime + duration (or timestamp of next chunk for sparse text tracks).</param>
        public PayloadAugmentedEventArgs(object payload, TimeSpan startTime, TimeSpan endTime)
        {
            Payload = payload;
            StartTime = startTime;
            EndTime = endTime;
        }

        /// <summary>
        /// The caption payload (typically a bytearry or string).
        /// </summary>
        public object Payload { get; private set; }

        /// <summary>
        /// The offset for the caption data. Typically set from the chunk timestamp.
        /// </summary>
        public TimeSpan StartTime { get; private set; }

        /// <summary>
        /// The end time of the caption data. Typically set from the chunk starttime + duration (or timestamp of next chunk for sparse text tracks).
        /// </summary>
        public TimeSpan EndTime { get; private set; }
    }
}

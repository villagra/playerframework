using System;
using System.Collections.Generic;

namespace Microsoft.VideoAnalytics
{
    /// <summary>
    /// A log generated to indicate current frames per second. This log is generated at regular intervals.
    /// </summary>
    public sealed class FpsLog : ILog
    {
        /// <summary>
        /// Creates a new instance of FpsLog.
        /// </summary>
        /// <param name="renderedFramesPerSecond">The number of rendered frames per second</param>
        /// <param name="droppedFramesPerSecond">The number of dropped frames per second</param>
        public FpsLog(double renderedFramesPerSecond, double droppedFramesPerSecond)
        {
            TimeStamp = DateTimeOffset.Now;
            Type = EventTypes.FramesPerSecond;
            RenderedFramesPerSecond = renderedFramesPerSecond;
            DroppedFramesPerSecond = droppedFramesPerSecond;
            Id = Guid.NewGuid();
            ExtraData = new Dictionary<string, object>();
        }

        /// <inheritdoc /> 
        public IDictionary<string, object> ExtraData { get; private set; }

        /// <inheritdoc /> 
        public Guid Id { get; set; }

        /// <inheritdoc /> 
        public DateTimeOffset TimeStamp { get; set; }

        /// <inheritdoc /> 
        public string Type { get; private set; }

        /// <inheritdoc /> 
        public IDictionary<string, object> GetData()
        {
            var result = this.CreateBasicLogData();
            result.Add("RenderedFramesPerSecond", RenderedFramesPerSecond);
            result.Add("DroppedFramesPerSecond", DroppedFramesPerSecond);
            return result;
        }

        /// <summary>
        /// Gets the number of rendered frames per second
        /// </summary>
        public double RenderedFramesPerSecond { get; private set; }

        /// <summary>
        /// Gets the number of dropped frames per second
        /// </summary>
        public double DroppedFramesPerSecond { get; private set; }
    }
}

using System;

namespace Microsoft.Media.Advertising
{
    /// <summary>
    /// EventArgs used to pass information about an event associated with an ActiveAdUnit.
    /// </summary>
    internal class ActiveAdUnitEventArgs : EventArgs
    {
        internal ActiveAdUnitEventArgs(ActiveAdUnit activeAdUnit)
        {
            ActiveAdUnit = activeAdUnit;
        }

        /// <summary>
        /// The ActiveAdUnit associated with the event.
        /// </summary>
        public ActiveAdUnit ActiveAdUnit { get; private set; }
    }

    /// <summary>
    /// EventArgs used to pass information about a log event associated with an ActiveAdUnit for debugging purposes.
    /// </summary>
    internal class ActiveAdUnitLogEventArgs : ActiveAdUnitEventArgs
    {
        internal ActiveAdUnitLogEventArgs(ActiveAdUnit activeAdUnit, string message)
            : base(activeAdUnit)
        {
            Message = message;
        }

        /// <summary>
        /// The log message.
        /// </summary>
        public string Message { get; private set; }
    }
}

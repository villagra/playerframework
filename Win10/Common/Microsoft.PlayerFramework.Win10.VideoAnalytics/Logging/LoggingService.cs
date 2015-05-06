using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Microsoft.VideoAnalytics
{
    /// <summary>
    /// Provides access to the service that all logs are sent to.
    /// </summary>
    public sealed class LoggingService
    {
        IList<ILoggingSource> loggingSources;
        readonly IList<ILoggingSource> wiredLoggingSources = new List<ILoggingSource>();
        
        static LoggingService current;

        /// <summary>
        /// Gets a singleton instance of teh service.
        /// </summary>
        public static LoggingService Current
        {
            get
            {
                if (current == null)
                {
                    current = new LoggingService();
                }
                return current;
            }
        }

        private LoggingService()
        {
            LoggingSources = new ObservableCollection<ILoggingSource>();
            LoggingTargets = new List<ILoggingTarget>();
        }

        /// <summary>
        /// Gets a collection of objects that want to handle logs as they are generated.
        /// </summary>
        public IList<ILoggingTarget> LoggingTargets { get; private set; }

        /// <summary>
        /// Gets a collection of objects that want to send logs to the targets. 
        /// Note: objects can alternatively just call the Log method directly.
        /// </summary>
        public IList<ILoggingSource> LoggingSources
        {
            get { return loggingSources; }
            set
            {
                if (loggingSources != null)
                {
                    if (loggingSources is INotifyCollectionChanged)
                    {
                        ((INotifyCollectionChanged)loggingSources).CollectionChanged -= Logger_CollectionChanged;
                    }
                    UnwireLoggingSources(loggingSources);
                }
                loggingSources = value;
                if (loggingSources != null)
                {
                    WireLoggingSources(loggingSources);
                    if (loggingSources is INotifyCollectionChanged)
                    {
                        ((INotifyCollectionChanged)loggingSources).CollectionChanged += Logger_CollectionChanged;
                    }
                }
            }
        }

        /// <summary>
        /// To be called when a new log is generated that should be logged. Any object can call this and pass custom logs to it.
        /// </summary>
        /// <param name="log">The log object.</param>
        public void Log(ILog log)
        {
            foreach (var item in LoggingTargets)
            {
                item.LogEntry(log);
            }
        }

        void UnwireLoggingSources(IEnumerable<ILoggingSource> loggingSources)
        {
            foreach (var item in loggingSources)
            {
                item.LogCreated -= item_LogCreated;
                wiredLoggingSources.Remove(item);
            }
        }

        void WireLoggingSources(IEnumerable<ILoggingSource> loggingSources)
        {
            foreach (var item in loggingSources)
            {
                item.LogCreated += item_LogCreated;
                wiredLoggingSources.Add(item);
            }
        }

        void Logger_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                UnwireLoggingSources(wiredLoggingSources.ToList());
            }
            else
            {
                if (e.NewItems != null)
                {
                    WireLoggingSources(e.NewItems.Cast<ILoggingSource>());
                }
                if (e.OldItems != null)
                {
                    UnwireLoggingSources(e.OldItems.Cast<ILoggingSource>());
                }
            }
        }

        void item_LogCreated(object sender, LogEventArgs e)
        {
            Log(e.Log);
        }
    }
}
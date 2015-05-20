using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Media.Analytics
{
    internal class ReportAggregator
    {
        IList<ILog> queue = new List<ILog>();

        public void AddLog(ILog entry)
        {
            lock (queue)
            {
                queue.Add(entry);
            }
        }

        protected IEnumerable<ILog> GetAllEntries()
        {
            lock (queue)
            {
                return queue.ToList();
            }
        }

        protected static IEnumerable<T> GetEntries<T>(IEnumerable<ILog> entries, DateTimeOffset windowStart, DateTimeOffset windowEnd, bool entryRequired, Predicate<T> predicate) where T : ILog
        {
            bool itemFound = false;
            foreach (var entry in entries.OfType<T>().Reverse().SkipWhile(l => l.TimeStamp > windowEnd).Where(l => predicate(l)))
            {
                if (entry.TimeStamp > windowStart)
                {
                    yield return entry;
                    itemFound = true;
                }
                else
                {
                    if (itemFound || !entryRequired) break;
                    yield return entry;
                }
            }
        }

        protected static IEnumerable<Sample<T>> GetSamples<T>(IEnumerable<ILog> entries, DateTimeOffset windowStart, DateTimeOffset windowEnd, bool sampleWhilePaused, Predicate<T> predicate) where T : ILog
        {
            var tailTime = windowEnd;
            var duration = TimeSpan.Zero;
            foreach (var entry in entries.Reverse())
            {
                if (!sampleWhilePaused && entry is IMarkerEntry)
                {
                    var marker = (IMarkerEntry)entry;
                    if (!marker.IsPlaying)
                    {
                        duration = duration.Add(tailTime.Subtract(marker.TimeStamp));
                    }
                    else
                    {
                        tailTime = marker.TimeStamp;
                    }
                }
                else if (entry is T)
                {
                    var typedEntry = (T)entry;
                    if (predicate(typedEntry))
                    {
                        if (entry.TimeStamp > windowStart)
                        {
                            yield return new Sample<T>()
                            {
                                Duration = tailTime.Subtract(entry.TimeStamp).Add(duration),
                                Entry = typedEntry
                            };
                            tailTime = entry.TimeStamp;
                            duration = TimeSpan.Zero;
                        }
                        else
                        {
                            yield return new Sample<T>()
                            {
                                Duration = tailTime.Subtract(windowStart).Add(duration),
                                Entry = typedEntry
                            };
                            break;
                        }
                    }
                }
            }
        }
    }

    internal class Sample<T> where T : ILog
    {
        public T Entry;
        public TimeSpan Duration;
    }
}

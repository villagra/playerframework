using System;
using System.Threading.Tasks;
#if SILVERLIGHT
using System.IO.IsolatedStorage;
#else
using Windows.Storage;
#endif

namespace Microsoft.VideoAnalytics
{
    internal static class InstanceDataClient
    {
        static Guid? instanceIdGuid;
        readonly static object syncLock = new object();

        const string SettingInstanceId = "PlayerFramework.Analtyics.InstanceId";

        static public Task<Guid> GetInstanceId()
        {
            lock (syncLock)
            {
                if (!instanceIdGuid.HasValue)
                {
#if SILVERLIGHT
                    if (IsolatedStorageSettings.ApplicationSettings.Contains(SettingInstanceId))
                    {
                        instanceIdGuid = Guid.Parse(IsolatedStorageSettings.ApplicationSettings[SettingInstanceId] as string);
                    }
                    else
                    {
                        instanceIdGuid = Guid.NewGuid();
                        IsolatedStorageSettings.ApplicationSettings.Add(SettingInstanceId, instanceIdGuid.Value.ToString());
                        IsolatedStorageSettings.ApplicationSettings.Save();
                    }
#else
                    var settingsValues = ApplicationData.Current.LocalSettings.Values;
                    if (settingsValues.ContainsKey(SettingInstanceId))
                    {
                        instanceIdGuid = Guid.Parse(settingsValues[SettingInstanceId] as string);
                    }
                    else
                    {
                        instanceIdGuid = Guid.NewGuid();
                        settingsValues.Add(SettingInstanceId, instanceIdGuid.Value.ToString());
                    }
#endif
                }
            }
#if WINDOWS_PHONE7
            return TaskEx.FromResult(instanceIdGuid.Value);
#else
            return Task.FromResult(instanceIdGuid.Value);
#endif
        }
    }
}

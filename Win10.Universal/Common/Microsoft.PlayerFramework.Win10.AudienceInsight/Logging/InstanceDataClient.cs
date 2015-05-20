using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace Microsoft.Media.AudienceInsight
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
                }
            }
            return Task.FromResult(instanceIdGuid.Value);
        }
    }
}

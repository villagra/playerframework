using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Media.Advertising
{
    internal static class ReflectionHelper
    {
        public static object GetValue(object target, string name)
        {
            if (target == null || string.IsNullOrEmpty(name)) return null;

            int i = name.IndexOf('.');
            string next = (i > 0 && name.Length > i) ? name.Substring(i + 1) : null;
            if (i > 0) name = name.Substring(0, i);
            object value = GetValueInternal(target, name);
            return (next == null) ? value : GetValue(value, next);
        }

        private static object GetValueInternal(object target, string name)
        {
            Type type;
            if (target is Type)
            {
                type = (Type)target;
            }
            else
            {
                type = target.GetType();
            }
            return type.GetRuntimeProperty(name).GetValue(target);
        }
        
        private static Dictionary<Delegate, EventInfo> WiredEvents = new Dictionary<Delegate, EventInfo>();

        public static void AttachEvent(object target, string eventName, Delegate handler)
        {
            Type targetType = (target is Type) ? (Type)target : target.GetType();
            EventInfo e = targetType.GetRuntimeEvent(eventName);
            e.AddEventHandler(target, handler);
            WiredEvents.Add(handler, e);
        }

        public static void DetachEvent(object receiver, Delegate handler)
        {
            if (WiredEvents.ContainsKey(handler))
            {
                EventInfo e = WiredEvents[handler];
                e.RemoveEventHandler(receiver, handler);
            }
        }
    }
}

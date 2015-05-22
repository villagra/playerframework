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
#if SILVERLIGHT
            object value = null;
            MemberInfo[] mi = null;

            if (target is Type)
            {
                mi = ((Type)target).GetMember(name, BindingFlags.Public | BindingFlags.Default | BindingFlags.Static | BindingFlags.IgnoreCase);
                target = null;
            }
            else
            {
                mi = target.GetType().GetMember(name, BindingFlags.Public | BindingFlags.Default | BindingFlags.IgnoreCase | BindingFlags.Instance);
            }

            if (mi != null && mi.Length > 0)
            {
                if (mi[0].MemberType == MemberTypes.Property)
                {
                    value = ((PropertyInfo)mi[0]).GetValue(target, null);

                }
                else if (mi[0].MemberType == MemberTypes.Field)
                {
                    value = ((FieldInfo)mi[0]).GetValue(target);

                }
                else if (mi[0].MemberType == MemberTypes.Method)
                {
                    value = ((MethodInfo)mi[0]).Invoke(target, null);
                }
            }
            return value;
#else
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
#endif
        }
        
        private static Dictionary<Delegate, EventInfo> WiredEvents = new Dictionary<Delegate, EventInfo>();

#if SILVERLIGHT
        public static Delegate AttachEvent(object target, string eventName, object handler, string method)
        {
            Type targetType = (target is Type) ? (Type)target : target.GetType();
            EventInfo e = targetType.GetEvent(eventName, BindingFlags.Public | BindingFlags.Default | BindingFlags.IgnoreCase | BindingFlags.Instance);
            Delegate eh = Delegate.CreateDelegate(e.EventHandlerType, handler, method, true, true);
            e.AddEventHandler(target, eh);
            WiredEvents.Combine(eh, e);
            return eh;
        }
#else
        public static void AttachEvent(object target, string eventName, Delegate handler)
        {
            Type targetType = (target is Type) ? (Type)target : target.GetType();
            EventInfo e = targetType.GetRuntimeEvent(eventName);
            e.AddEventHandler(target, handler);
            WiredEvents.Add(handler, e);
        }
#endif

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

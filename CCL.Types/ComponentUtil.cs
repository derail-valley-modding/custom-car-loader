using System.Reflection;
using UnityEngine;

namespace CCL.Types
{
    public static class ComponentUtil
    {
        // https://discussions.unity.com/t/how-to-get-a-component-from-an-object-and-add-it-to-another-copy-components-at-runtime/80939/4
        public static T CopyComponent<T>(T from, T to)
            where T : Component
        {
            var type = typeof(T);

            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
            PropertyInfo[] pinfos = type.GetProperties(flags);

            foreach (var pinfo in pinfos)
            {
                if (pinfo.CanWrite)
                {
                    try
                    {
                        pinfo.SetValue(to, pinfo.GetValue(from, null), null);
                    }
                    catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
                }
            }

            FieldInfo[] finfos = type.GetFields(flags);
            foreach (var finfo in finfos)
            {
                finfo.SetValue(to, finfo.GetValue(from));
            }

            return to;
        }

        public static bool HasComponent<T>(GameObject go, bool inChildren = true)
            where T : Component
        {
            return (inChildren ? go.GetComponentInChildren<T>() : go.GetComponent<T>()) != null;
        }
    }
}

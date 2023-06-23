using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CCL.Importer
{
    using StartupAction = KeyValuePair<int, Action>;

    [HarmonyPatch(typeof(Bootstrap))]
    public static class StartupInitializer
    {
        private static List<StartupAction> _startupActions;

        static StartupInitializer()
        {
            var actions = new List<StartupAction>();

            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
                {
                    if (method.GetCustomAttribute<AfterStartupAttribute>() is AfterStartupAttribute attr)
                    {
                        CCLPlugin.Log($"Registered startup method {type.Name}.{method.Name}");
                        actions.Add(new StartupAction(attr.Priority, AccessTools.MethodDelegate<Action>(method)));
                    }
                }
            }

            _startupActions = actions.OrderBy(a => a.Key).ToList();
        }

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void HandleStartupComplete(ref IEnumerator __result)
        {
            __result = WrappedEnumerator.OnceCompleted(__result, RunStartupMethods);
        }

        private static void RunStartupMethods()
        {
            foreach (var method in _startupActions)
            {
                method.Value();
            }
        }
    }

    public enum StartupPriority
    {
        Critical = -10000,
        Early = -5000,
        Normal = 0,
        Late = 5000,
        Final = 10000,
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class AfterStartupAttribute : Attribute
    {
        public readonly int Priority;

        public AfterStartupAttribute(int priority)
        {
            Priority = priority;
        }

        public AfterStartupAttribute(StartupPriority priority) : this((int)priority) { }

        public AfterStartupAttribute() : this(StartupPriority.Normal) { }
    }
}
